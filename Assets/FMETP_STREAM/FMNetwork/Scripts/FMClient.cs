using System.Collections;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FMSolution.FMNetwork
{
    public class FMClient
    {
        public class FMClientComponent : MonoBehaviour
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            private int udpSendBufferSize = 1024 * 65; //max 65535
            private int udpReceiveBufferSize = 1024 * 1024 * 4; //max 2147483647
#else
            private int udpSendBufferSize = 1024 * 60; //max 65535
            private int udpReceiveBufferSize = 1024 * 512; //max 2147483647
#endif

            [HideInInspector] public FMNetworkManager Manager;

            [HideInInspector] public int ServerListenPort = 3333;
            [HideInInspector] public int ClientListenPort = 3334;

            public bool SupportMulticast = false;
            public string MulticastAddress = "239.255.255.255";

            [HideInInspector] public string ServerIP = "0,0,0,0";
            [HideInInspector] public string ClientIP = "0,0,0,0";

            public bool IsConnected = false;
            private long _foundServer = 0;
            private bool FoundServer
            {
                get { return Interlocked.Read(ref _foundServer) == 1; }
                set { Interlocked.Exchange(ref _foundServer, Convert.ToInt64(value)); }
            }

            public bool AutoNetworkDiscovery = true;
            public bool ForceBroadcast = false;

            private int EnvironmentTickCountDelta(int currentMS, int lastMS)
            {
                int _gap = 0;
                if (currentMS < 0 && lastMS > 0)
                {
                    _gap = Mathf.Abs(currentMS - int.MinValue) + (int.MaxValue - lastMS);
                }
                else
                {
                    _gap = currentMS - lastMS;
                }
                return _gap;
            }

            private int connectionThreshold = 3000;//3sec
            private long _currentSeenTimeMS = 0;
            public int CurrentSeenTimeMS
            {
                get { return Convert.ToInt32(Interlocked.Read(ref _currentSeenTimeMS)); }
                set { Interlocked.Exchange(ref _currentSeenTimeMS, (long)value); }
            }
            private long _lastReceivedTimeMS = 0;
            public int LastReceivedTimeMS
            {
                get { return Convert.ToInt32(Interlocked.Read(ref _lastReceivedTimeMS)); }
                set { Interlocked.Exchange(ref _lastReceivedTimeMS, (long)value); }
            }
            private long _lastSentTimeMS = 0;
            public int LastSentTimeMS
            {
                get { return (int)Interlocked.Read(ref _lastSentTimeMS); }
                set { Interlocked.Exchange(ref _lastSentTimeMS, (long)value); }
            }

            [Header("[Experimental] suggested for mobile")]
            public bool UseMainThreadSender = false;
            private ConcurrentQueue<FMPacket> _appendQueueSendPacket = new ConcurrentQueue<FMPacket>();
            private ConcurrentQueue<FMPacket> _appendQueueReceivedPacket = new ConcurrentQueue<FMPacket>();
            private ConcurrentQueue<byte[]> _appendQueueAck = new ConcurrentQueue<byte[]>();
            private ConcurrentQueue<FMPacket> _appendQueueRetryPacket = new ConcurrentQueue<FMPacket>();
            private ConcurrentQueue<FMPacket> _appendQueueMissingPacket = new ConcurrentQueue<FMPacket>();

            private int _getSyncID = 0;
            private uint syncIDMax = UInt16.MaxValue - 1024;
            public UInt16 getSyncID
            {
                get
                {
                    _getSyncID = Interlocked.Increment(ref _getSyncID);
                    if (_getSyncID >= syncIDMax) _getSyncID = Interlocked.Exchange(ref _getSyncID, 1);
                    return (UInt16)_getSyncID;
                }
            }

            private void EnqueueReceivedPacket(byte[] _receivedData)
            {
                FMPacket _packet = new FMPacket();
                _packet.SendByte = _receivedData;
                _appendQueueReceivedPacket.Enqueue(_packet);
            }

            public void Action_AddPacket(byte[] _byteData, FMSendType _type, FMPacketDataType _dataType, bool _reliable)
            {
                byte[] _meta = new byte[4];
                _meta[0] = (byte)_dataType; //_meta[0] = 0;//raw byte

                if (_type == FMSendType.All) _meta[1] = 0;//all clients
                if (_type == FMSendType.Server) _meta[1] = 1;//all clients
                if (_type == FMSendType.Others) _meta[1] = 2;//skip sender

                byte[] _sendByte = new byte[_byteData.Length + _meta.Length];
                Buffer.BlockCopy(_meta, 0, _sendByte, 0, _meta.Length);
                Buffer.BlockCopy(_byteData, 0, _sendByte, 4, _byteData.Length);

                //if (_appendQueueSendPacket.Count < 60)
                {
                    FMPacket _packet = new FMPacket();
                    _packet.Reliable = _reliable;
                    _packet.SendByte = _sendByte;
                    _packet.SendType = _type;
                    _appendQueueSendPacket.Enqueue(_packet);
                }
            }
            public void Action_AddPacket(string _stringData, FMSendType _type, FMPacketDataType _dataType, bool _reliable)
            {
                byte[] _byteData = Encoding.ASCII.GetBytes(_stringData);

                byte[] _meta = new byte[4];
                _meta[0] = (byte)_dataType; //_meta[0] = 1;//string data

                if (_type == FMSendType.All) _meta[1] = 0;//all clients
                if (_type == FMSendType.Server) _meta[1] = 1;//server
                if (_type == FMSendType.Others) _meta[1] = 2;//skip sender

                byte[] _sendByte = new byte[_byteData.Length + _meta.Length];
                Buffer.BlockCopy(_meta, 0, _sendByte, 0, _meta.Length);
                Buffer.BlockCopy(_byteData, 0, _sendByte, 4, _byteData.Length);

                //if (_appendQueueSendPacket.Count < 60)
                {
                    FMPacket _packet = new FMPacket();
                    _packet.Reliable = _reliable;
                    _packet.SendByte = _sendByte;
                    _packet.SendType = _type;
                    _appendQueueSendPacket.Enqueue(_packet);
                }
            }

            public void Action_AddPacket(byte[] _byteData, string _targetIP, FMPacketDataType _dataType, bool _reliable)
            {
                //if (ServerIP == _targetIP)
                //{
                //    Action_AddPacket(_byteData, FMSendType.Server, _reliable);
                //    return;
                //}

                //Send To Target IP
                byte[] _meta = new byte[4];
                _meta[0] = (byte)_dataType; //_meta[0] = 0;//raw byte
                _meta[1] = 3;//target ip

                byte[] _ip = IPAddress.Parse(_targetIP).GetAddressBytes();
                byte[] _sendByte = new byte[_byteData.Length + _meta.Length + _ip.Length];
                Buffer.BlockCopy(_meta, 0, _sendByte, 0, _meta.Length);
                Buffer.BlockCopy(_ip, 0, _sendByte, 4, _ip.Length);
                Buffer.BlockCopy(_byteData, 0, _sendByte, 8, _byteData.Length);

                //if (_appendQueueSendPacket.Count < 60)
                {
                    FMPacket _packet = new FMPacket();
                    _packet.Reliable = _reliable;
                    _packet.SendByte = _sendByte;
                    _packet.SendType = FMSendType.TargetIP;
                    _packet.TargetIP = _targetIP;
                    _appendQueueSendPacket.Enqueue(_packet);
                }
            }
            public void Action_AddPacket(string _stringData, string _targetIP, FMPacketDataType _dataType, bool _reliable)
            {
                //if (ServerIP == _targetIP)
                //{
                //    Action_AddPacket(_stringData, FMSendType.Server, _reliable);
                //    return;
                //}

                byte[] _byteData = Encoding.ASCII.GetBytes(_stringData);

                byte[] _meta = new byte[4];
                _meta[0] = (byte)_dataType; //_meta[0] = 1;//string data
                _meta[1] = 3;//target ip

                byte[] _ip = IPAddress.Parse(_targetIP).GetAddressBytes();
                byte[] _sendByte = new byte[_byteData.Length + _meta.Length + _ip.Length];
                Buffer.BlockCopy(_meta, 0, _sendByte, 0, _meta.Length);
                Buffer.BlockCopy(_ip, 0, _sendByte, 4, _ip.Length);
                Buffer.BlockCopy(_byteData, 0, _sendByte, 8, _byteData.Length);

                //if (_appendQueueSendPacket.Count < 60)
                {
                    FMPacket _packet = new FMPacket();
                    _packet.Reliable = _reliable;
                    _packet.SendByte = _sendByte;
                    _packet.SendType = FMSendType.TargetIP;
                    _packet.TargetIP = _targetIP;
                    _appendQueueSendPacket.Enqueue(_packet);
                }
            }

            private long _stop = 0;
            private bool stop
            {
                get { return Interlocked.Read(ref _stop) == 1; }
                set { Interlocked.Exchange(ref _stop, Convert.ToInt64(value)); }
            }
            private long _destroy = 0;
            private bool destroy
            {
                get { return Interlocked.Read(ref _destroy) == 1; }
                set { Interlocked.Exchange(ref _destroy, Convert.ToInt64(value)); }
            }

            private void Start() { StartAll(); }
            public void Action_StartClient() { StartCoroutine(NetworkClientStartCOR()); }

            private UdpClient Client;
            private UdpClient ClientListener;
            private IPEndPoint ServerEp;
            private IEnumerator NetworkClientStartCOR()
            {
                CurrentSeenTimeMS = Environment.TickCount;
                if (CurrentSeenTimeMS > 0)
                {
                    LastSentTimeMS = CurrentSeenTimeMS - connectionThreshold;
                    LastReceivedTimeMS = CurrentSeenTimeMS - connectionThreshold;
                }
                else
                {
                    LastSentTimeMS = int.MaxValue - connectionThreshold;
                    LastReceivedTimeMS = int.MaxValue - connectionThreshold;
                }

                stop = false;
                yield return new WaitForSeconds(0.1f);
                yield return null;

                if (UseMainThreadSender)
                {
                    StartCoroutine(MainThreadSenderCOR());
                }
                else
                {
                    //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv Client Sender vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
                    while (Loom.numThreads >= Loom.maxThreads) yield return null;
                    Loom.RunAsync(() =>
                    {
                        //client request
                        while (!stop)
                        {
                            Sender();
                            System.Threading.Thread.Sleep(FoundServer ? 1 : 200);
                        }
                        System.Threading.Thread.Sleep(1);
                    });
                    //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ Client Sender ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                }

                //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv Client Receiver vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
                while (Loom.numThreads >= Loom.maxThreads) yield return null;
                Loom.RunAsync(() =>
                {
                    while (!stop)
                    {
                        try
                        {
                            if (ClientListener == null)
                            {
                                ClientListener = new UdpClient(ClientListenPort);
                                ClientListener.Client.SendBufferSize = udpSendBufferSize;
                                ClientListener.Client.ReceiveBufferSize = udpReceiveBufferSize;
                                ClientListener.Client.ReceiveTimeout = 2000;
                                //ClientListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                                if (SupportMulticast)
                                {
                                    //enable multicast option
                                    ClientListener.MulticastLoopback = true;
                                    ClientListener.JoinMulticastGroup(IPAddress.Parse(MulticastAddress));
                                }

                                ServerEp = new IPEndPoint(IPAddress.Any, ClientListenPort);
                            }

                            while (!stop && ClientListener.Client.Poll(100, SelectMode.SelectRead))
                            {
                                while (!stop && ClientListener.Client.Available > 0)
                                {
                                    byte[] _receivedData = ClientListener.Receive(ref ServerEp);
                                    int _receivedDataLength = _receivedData.Length;
                                    LastReceivedTimeMS = Environment.TickCount;

                                    //=======================Decode Data=======================
                                    if (!FoundServer)
                                    {
                                        //looking for server and handshake
                                        if (AutoNetworkDiscovery)
                                        {
                                            if (_receivedDataLength == 1)
                                            {
                                                //Received Auto Network Discovery signal from Server
                                                if (_receivedData[0] == 93)
                                                {
                                                    ServerIP = ServerEp.Address.ToString();
                                                    FoundServer = true;

                                                    //handshaking signal
                                                    SendHandShaking(new IPEndPoint(IPAddress.Parse(ServerIP), ServerListenPort));
                                                    EnqueueReceivedPacket(_receivedData);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Any response from server will be consider as handshake
                                            if (ServerIP == ServerEp.Address.ToString())
                                            {
                                                FoundServer = true;

                                                //handshaking signal
                                                SendHandShaking(new IPEndPoint(IPAddress.Parse(ServerIP), ServerListenPort));
                                                EnqueueReceivedPacket(_receivedData);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (_receivedDataLength == 1)
                                        {
                                            //Received Close() command from Server
                                            if (_receivedData[0] == 94)
                                            {
                                                destroy = true;
                                                stop = true;
                                            }
                                            else if (_receivedData[0] == 95)
                                            {
                                                //Down server...
                                                FoundServer = false;
                                            }
                                        }
                                    }

                                    UInt16 _verifiedAckID = 0;
                                    if (_receivedDataLength > 4)
                                    {
                                        EnqueueReceivedPacket(_receivedData);

                                        //ack send queue
                                        if (_receivedData[2] != 0 && _receivedData[3] != 0)
                                        {
                                            _appendQueueAck.Enqueue(new byte[] { _receivedData[2], _receivedData[3] });
                                        }
                                    }
                                    else if (_receivedDataLength <= 2)
                                    {
                                        //ack received
                                        if (_appendQueueRetryPacket.Count > 0)
                                        {
                                            if (_receivedData.Length == 2) _verifiedAckID = BitConverter.ToUInt16(_receivedData, 0);

                                            bool _completed = false;
                                            if (_verifiedAckID == 0)
                                            {
                                                //Debug.LogError("confirmed AckID");
                                                _completed = true;
                                            }
                                            while (!_completed)
                                            {
                                                if (_appendQueueRetryPacket.Count <= 0)
                                                {
                                                    //complete when there is no retry packet to check
                                                    _completed = true;
                                                }
                                                else
                                                {
                                                    if (_appendQueueRetryPacket.TryDequeue(out FMPacket retryPacket))
                                                    {
                                                        if (retryPacket.syncID == _verifiedAckID)
                                                        {
                                                            //found matching packet, confirmed
                                                            _completed = true;
                                                        }
                                                        else
                                                        {
                                                            _appendQueueMissingPacket.Enqueue(retryPacket);
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                    //=======================Decode Data=======================
                                }
                            }
                        }
                        catch
                        {
                        //DebugLog("Client Receiver Timeout: " + socketException.ToString());
                        if (ClientListener != null) ClientListener.Close(); ClientListener = null;
                        }
                    //System.Threading.Thread.Sleep(1);
                }
                    System.Threading.Thread.Sleep(1);
                });
                //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ Client Receiver ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

                //try sending handshaking to reach server as fast as possible, after client listener initialised
                SendHandShaking(new IPEndPoint(IPAddress.Parse(Manager.ReadBroadcastAddress), ServerListenPort));

                //processing
                while (!stop)
                {
                    CurrentSeenTimeMS = Environment.TickCount;

                    #region Check Connection Status
                    bool _connected = false;
                    if (FoundServer)
                    {
                        int _connectionThreshold = 10000;//10sec
                        _connected = EnvironmentTickCountDelta(CurrentSeenTimeMS, LastReceivedTimeMS) < _connectionThreshold;
                    }

                    if (IsConnected != _connected)
                    {
                        if (_connected)
                        {
                            Manager.OnFoundServer(ServerIP);
                        }
                        else
                        {
                            Manager.OnLostServer(ServerIP);

                            _appendQueueSendPacket = new ConcurrentQueue<FMPacket>();
                            _appendQueueReceivedPacket = new ConcurrentQueue<FMPacket>();

                            _appendQueueAck = new ConcurrentQueue<byte[]>();
                            _appendQueueRetryPacket = new ConcurrentQueue<FMPacket>();
                            _appendQueueMissingPacket = new ConcurrentQueue<FMPacket>();
                        }
                        IsConnected = _connected;
                    }
                    #endregion

                    while (_appendQueueReceivedPacket.Count > 0)
                    {
                        ReceivedCount = _appendQueueReceivedPacket.Count;
                        if (_appendQueueReceivedPacket.TryDequeue(out FMPacket _packet))
                        {
                            if (Manager != null)
                            {
                                byte[] _receivedData = _packet.SendByte;
                                if (_receivedData.Length > 4)
                                {
                                    byte[] _meta = new byte[] { _receivedData[0], _receivedData[1] };
                                    byte[] _data = new byte[_receivedData.Length - 4];
                                    Buffer.BlockCopy(_receivedData, 4, _data, 0, _data.Length);

                                    //process received data>> byte data: 0, string msg: 1, network object data: 2
                                    switch (_meta[0])
                                    {
                                        case 0: Manager.OnReceivedByteDataEvent.Invoke(_data); break;
                                        case 1: Manager.OnReceivedStringDataEvent.Invoke(Encoding.ASCII.GetString(_data)); break;
                                        case 11:
                                            try
                                            {
                                                FMNetworkFunction _fmfunction = JsonUtility.FromJson<FMNetworkFunction>(Encoding.ASCII.GetString(_data));
                                                if (_fmfunction != null) Manager.OnReceivedNetworkFunction(_fmfunction);
                                            }
                                            catch { }
                                            break;
                                    }
                                }
                                Manager.GetRawReceivedData.Invoke(_receivedData);
                            }
                        }
                    }
                    yield return null;
                }

                while (!destroy) yield return null;
                if (destroy)
                {
                    StopAll();
                    yield return null;
                    Manager.Action_Close();
                }

                yield break;
            }
            public int ReceivedCount = 0;

            private IEnumerator MainThreadSenderCOR()
            {
                //client request
                while (!stop)
                {
                    yield return null;
                    Sender();
                }
            }

            private void SendHandShaking(IPEndPoint ipEndPoint)
            {
                if (Client == null) return;
                try { Client.Send(new byte[] { (byte)FMClientSignal.handshake }, 1, ipEndPoint); }
                catch { if (Client != null) Client.Close(); Client = null; }
            }

            private void Sender()
            {
                try
                {
                    if (Client == null)
                    {
                        Client = new UdpClient();
                        Client.Client.SendBufferSize = udpSendBufferSize;
                        Client.Client.ReceiveBufferSize = udpReceiveBufferSize;
                        Client.Client.SendTimeout = 500;
                        Client.EnableBroadcast = true;
                    }

                    if (FoundServer == false && AutoNetworkDiscovery)
                    {
                        if (EnvironmentTickCountDelta(CurrentSeenTimeMS, LastSentTimeMS) > 2000)
                        {
                            //broadcast
                            SendHandShaking(new IPEndPoint(IPAddress.Parse(Manager.ReadBroadcastAddress), ServerListenPort));
                            LastSentTimeMS = Environment.TickCount;
                        }
                    }
                    else
                    {
                        //send to server ip only
                        if (_appendQueueSendPacket.Count > 0 || _appendQueueAck.Count > 0 || _appendQueueMissingPacket.Count > 0)
                        {
                            bool _sent = false;

                            //send queuedAck
                            int _ackCount = 0;
                            while (_appendQueueAck.Count > 0 && _ackCount < 100)
                            {
                                _ackCount++;
                                if (_appendQueueAck.TryDequeue(out byte[] _ackBytes))
                                {
                                    if (SendPacket(_ackBytes)) _sent = true;
                                }
                            }

                            //limit 30 packet sent in each frame, solved overhead issue on receiver
                            int _sendCount = 0;
                            while (_appendQueueSendPacket.Count > 0 && _sendCount < 100)
                            {
                                _sendCount++;
                                if (_appendQueueSendPacket.TryDequeue(out FMPacket _packet))
                                {
                                    if (SendPacket(_packet)) _sent = true;
                                }
                            }

                            int _missingCount = 0;
                            while (_appendQueueMissingPacket.Count > 0 && _missingCount < 100)
                            {
                                _missingCount++;
                                if (_appendQueueMissingPacket.TryDequeue(out FMPacket _missingPacket))
                                {
                                    _missingPacket.Reliable = true;
                                    SendPacket(_missingPacket);
                                }
                            }
                            sendBufferThreshold = _missingCount > 0 ? sendBufferThresholdMin : sendBufferThresholdMax;

                            if (_sent) LastSentTimeMS = Environment.TickCount;
                        }
                        else
                        {
                            if (EnvironmentTickCountDelta(CurrentSeenTimeMS, LastSentTimeMS) > 2000)
                            {
                                //check connection: minimum 2000ms
                                SendHandShaking(new IPEndPoint(ForceBroadcast ? IPAddress.Parse(Manager.ReadBroadcastAddress) : IPAddress.Parse(ServerIP), ServerListenPort));
                                LastSentTimeMS = Environment.TickCount;
                            }
                        }
                    }
                }
                catch
                {
                    //DebugLog("client sender timeout: " + socketException.ToString());
                    if (Client != null) Client.Close(); Client = null;
                }
            }

            private int sendBufferSize = 0;
            private int sendBufferThreshold = 1024 * 128;
            private int sendBufferThresholdMin = 1024 * 8;
            private int sendBufferThresholdMax = 1024 * 128;

            private bool SendPacket(byte[] _bytes)
            {
                bool _sent = false;
                try
                {
                    Client.Send(_bytes, _bytes.Length, new IPEndPoint(ForceBroadcast ? IPAddress.Parse(Manager.ReadBroadcastAddress) : IPAddress.Parse(ServerIP), ServerListenPort));
                    _sent = true;
                }
                catch
                {
                    if (Client != null) Client.Close(); Client = null;
                }
                return _sent;
            }
            private bool SendPacket(FMPacket _packet)
            {
                bool _sent = false;
                sendBufferSize += _packet.SendByte.Length;
                if (sendBufferSize > sendBufferThreshold)
                {
                    sendBufferSize = 0;
                    GC.Collect();
                    System.Threading.Thread.Sleep(1);
                }

                if (_packet.Reliable)
                {
                    _packet.syncID = getSyncID;
                    Buffer.BlockCopy(BitConverter.GetBytes(_packet.syncID), 0, _packet.SendByte, 2, 2);
                }

                if (!ForceBroadcast)
                {
                    //default mode, non-broadcasting
                    Client.Send(_packet.SendByte, _packet.SendByte.Length, new IPEndPoint(IPAddress.Parse(ServerIP), ServerListenPort));
                    _sent = true;
                }
                else
                {
                    //broadcasting mode for multiple servers..etc
                    if (_packet.SendType == FMSendType.TargetIP)
                    {
                        //ignore broadcast, if you have a target IP
                        Client.Send(_packet.SendByte, _packet.SendByte.Length, new IPEndPoint(IPAddress.Parse(_packet.TargetIP), ServerListenPort));
                        if (_packet.TargetIP == ServerIP) _sent = true;
                    }
                    else
                    {
                        _packet.SendType = FMSendType.Server;
                        _packet.SendByte[1] = 1;//when broadcast mode enabled, force the send type to server(SendByte[1] = 1), then it won't send twice to others

                        Client.Send(_packet.SendByte, _packet.SendByte.Length, new IPEndPoint(IPAddress.Parse(Manager.ReadBroadcastAddress), ServerListenPort));
                        _sent = true;
                    }

                    if (EnvironmentTickCountDelta(CurrentSeenTimeMS, LastSentTimeMS) > 2000)
                    {
                        //check connection: minimum 2000ms
                        SendHandShaking(new IPEndPoint(IPAddress.Parse(Manager.ReadBroadcastAddress), ServerListenPort));
                        _sent = true;
                    }
                }

                //buffer retry... check ack later
                if (_packet.Reliable) _appendQueueRetryPacket.Enqueue(_packet);

                return _sent;
            }

            public bool ShowLog { get { return Manager.ShowLog; } }
            public void DebugLog(string _value) { if (ShowLog) Debug.Log(_value); }

            private void OnApplicationQuit() { StopAll(); }
            private void OnDisable() { StopAll(); }
            private void OnDestroy() { StopAll(); }
            private void OnEnable() { StartAll(0.1f); }

            private bool isPaused = false;
            private bool isPaused_old = false;
            private long _needResetFromPaused = 0;
            private bool needResetFromPaused
            {
                get { return Interlocked.Read(ref _needResetFromPaused) == 1; }
                set { Interlocked.Exchange(ref _needResetFromPaused, Convert.ToInt64(value)); }
            }

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID || WINDOWS_UWP)
            //try fixing Android/Mobile connection issue after a pause...
            //some devices will trigger OnApplicationPause only, when some devices will trigger both...etc
            private void ResetFromPause()
            {
                if (!needResetFromPaused) return;
                needResetFromPaused = false;

                StopAll();
                StartAll(0.1f);
            }
            private void OnApplicationPause(bool pause)
            {
                if (!initialised) return; //ignore it if not initialised yet

                isPaused_old = isPaused;
                isPaused = pause;
                if (isPaused && !isPaused_old) needResetFromPaused = true;
                if (!isPaused && isPaused_old) ResetFromPause();
            }
            private void OnApplicationFocus(bool focus)
            {
                if (!initialised) return; //ignore it if not initialised yet

                isPaused_old = isPaused;
                isPaused = !focus;
                if (isPaused && !isPaused_old) needResetFromPaused = true;
                if (!isPaused && isPaused_old) ResetFromPause();
            }
#endif

            private long _initialised = 0;
            private bool initialised
            {
                get { return Interlocked.Read(ref _initialised) == 1; }
                set { Interlocked.Exchange(ref _initialised, Convert.ToInt64(value)); }
            }

            private IEnumerator StartAllDelayCOR(float _delay = 0.1f)
            {
                yield return new WaitForSecondsRealtime(_delay);
                yield return null;

                StartAll();
            }
            private void StartAll(float _delay = 0f)
            {
                if (_delay > 0f)
                {
                    StartCoroutine(StartAllDelayCOR(_delay));
                    return;
                }

                if (initialised) return;
                initialised = true;

                stop = false;
                destroy = false;
                Action_StartClient();
            }

            private void StopAll()
            {
                initialised = false;

                //skip, if stopped already
                if (stop)
                {
                    StopAllCoroutines();//stop all coroutines, just in case
                    return;
                }

                //try sending disconnect signal 94 as possible, before destroy
                if (IsConnected && FoundServer)
                {
                    //send status "closed" in background before end...
                    SendClientClosedAsync(ServerIP, ServerListenPort);

                    if (Client != null)
                    {
                        try { Client.Close(); }
                        catch (Exception e) { DebugLog(e.Message); }
                        Client = null;
                    }

                    Manager.OnLostServer(ServerIP);
                }

                if (ClientListener != null)
                {
                    try
                    {
                        ClientListener.DropMulticastGroup(IPAddress.Parse(MulticastAddress));
                        ClientListener.Close();
                    }
                    catch (Exception e) { DebugLog(e.Message); }
                    ClientListener = null;
                }

                stop = true;
                IsConnected = false;
                FoundServer = false;
                StopAllCoroutines();

                _appendQueueSendPacket = new ConcurrentQueue<FMPacket>();
                _appendQueueReceivedPacket = new ConcurrentQueue<FMPacket>();
            }

            private async void SendClientClosedAsync(string IP, int Port)
            {
                await Task.Yield();
                UdpClient Client = new UdpClient();
                try
                {
                    Client.Client.SendBufferSize = udpSendBufferSize;
                    Client.Client.ReceiveBufferSize = udpReceiveBufferSize;
                    Client.Client.SendTimeout = 500;
                    Client.EnableBroadcast = true;

                    byte[] _byte = new byte[] { 94 };
                    Client.Send(_byte, _byte.Length, new IPEndPoint(IPAddress.Parse(IP), Port));
                }
                catch { }
            }
        }
    }
}