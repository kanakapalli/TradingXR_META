using Meta.WitAi.CallbackHandlers;
using Meta.WitAi.TTS.Utilities;
using Oculus.Interaction.Samples.PalmMenu;
using Oculus.Voice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[System.Serializable]
public class Listner : MonoBehaviour
{
    [SerializeField] TTSSpeaker m_TTSSpeaker;
    [SerializeField] API_Call m_API_Call;
    [SerializeField] PalmMenuExampleButtonHandlers m_PalmMenuExampleButtonHandlers;
    [SerializeField] Palm_Menu_Controller m_PalmMenuController;
    [SerializeField] AppVoiceExperience m_AppVoiceExperience;
    [SerializeField] List<Dict> m_Demo_Dict = new List<Dict>();
    [SerializeField] string m_Listener_Full_Response;

    private string gainLose = "https://assisntak.web.app/topgainerlossers";
    private bool _intent_found = false;
    private int _intent_count = 0;

    private void Start()
    {
        
    }

    public void GetStocksIntent(string[] values)
    {
        //Stock Searching e.g How is apple doing today -> values[0]
        string _intent = values[0];
        string _stock_name = values[1];

        Debug.Log("<color=blue>" + _stock_name + "</color>");
        Debug.Log("<color=blue>" + _intent + "</color>");
        _intent_found = true;

        var m_Object = Instantiate(m_API_Call.m_Stock_Prefab, m_API_Call.m_Stock_Target_Point.position, Quaternion.identity);
        //Assigning Overview and Detail URL

        int _res = GetIndex(_stock_name);
        Dict _dict;
        if (_res == -1)
        {
            return;
        }
        else
        {
            _dict = m_Demo_Dict[_res];
            TurnOffIntent();
        }

        string m_overview_url = string.Concat(m_API_Call.m_Overview_Base_Url, _dict._symbol, "&exchange=", _dict._exchange);
        string m_detail_url = string.Concat(m_API_Call.m_Detail_Base_Url, _dict._symbol, "&exchange=", _dict._exchange);

        m_Object.GetComponent<Vuplex_Tab>().m_Overview_Url = m_overview_url;
        m_Object.GetComponent<Vuplex_Tab>().m_Detail_Url = m_detail_url;

        //Refreshing By Assigning The URL
        m_Object.GetComponent<Vuplex_Tab>().m_CanvasWebView.InitialUrl = m_overview_url;
        m_TTSSpeaker.Speak("Here is the stock of " + _stock_name);

        //m_Voice_Animation.SetActive(false);
    }

    public void GetStocksNewsIntent(string[] values)
    {
        //Webview Spawn with news url using value 1 -> stock name
        string _intent = values[0];
        string _stock_name = values[1];

        Debug.Log("<color=blue>" + _stock_name + "</color>");
        Debug.Log("<color=blue>" + _intent + "</color>");
        _intent_found = true;

        int _res = GetIndex(_stock_name);
        Dict _dict;
        if (_res == -1)
        {
            return;
        }
        else
        {
            _dict = m_Demo_Dict[_res];
            TurnOffIntent();
        }

        var m_Object = Instantiate(m_API_Call.m_Stock_Prefab, m_API_Call.m_Stock_Target_Point.position, Quaternion.identity);
        string m_news_url = string.Concat(m_API_Call.m_News_Base_Url, _dict._symbol, "&exchange=", _dict._exchange);
        m_Object.GetComponent<Vuplex_Tab>().m_CanvasWebView.InitialUrl = m_news_url;
        m_Object.GetComponent<Vuplex_Tab>().m_CanvasWebView.WebView.LoadUrl(m_news_url);
        m_Object.GetComponent<Vuplex_Tab>().m_CanvasWebView.WebView.Reload();

        m_TTSSpeaker.Speak("Here is the stock of " + _stock_name);

        //m_Voice_Animation.SetActive(false);
    }

    public void GetNewsIntent(string[] values)
    {
        //Spawning tab with top gainers and loosers
        string _intent = values[0];
        string _stock_name = values[1];

        Debug.Log("<color=blue>" + _stock_name + "</color>");
        Debug.Log("<color=blue>" + _intent + "</color>");
        _intent_found = true;

        int _res = GetIndex(_stock_name);
        Dict _dict;
        if (_res == -1)
        {
            return;
        }
        else
        {
            _dict = m_Demo_Dict[_res];
        }

        var m_Object = Instantiate(m_API_Call.m_Stock_Prefab, m_API_Call.m_Stock_Target_Point.position, Quaternion.identity);
        m_Object.GetComponent<Vuplex_Tab>().m_CanvasWebView.InitialUrl = gainLose;
        m_Object.GetComponent<Vuplex_Tab>().m_CanvasWebView.WebView.LoadUrl(gainLose);
        m_Object.GetComponent<Vuplex_Tab>().m_CanvasWebView.WebView.Reload();

        m_TTSSpeaker.Speak("Here is the stock of " + _stock_name);

        //m_Voice_Animation.SetActive(false);
        TurnOffIntent();
    }

    public void transitionPArt(string values)
    {
        Debug.Log("transitionPArt");
        Debug.Log(values);


    }

    public void transitionFull(string values)

    {
        Debug.Log("<color=red>TransitionFull</color>");

        Debug.Log(values);


    }

    private int GetIndex(string _name)
    {
        for (int i = 0; i < m_Demo_Dict.Count; i++)
        {
            if (m_Demo_Dict[i]._name.ToLower() == _name.ToLower())
            {
                return i;
            }
        }
        return -1;
    }

    private void TurnOffIntent()
    {
        m_PalmMenuExampleButtonHandlers.ToggleRotationEnabled();
        m_PalmMenuController.DeactivateAllScreens();
        m_AppVoiceExperience.Deactivate();
    }

    public void ConfirmIntent()
    {
        StartCoroutine(CheckIntent());
    }

    private IEnumerator CheckIntent()
    {
        yield return new WaitForSeconds(2f);
        if (_intent_found)
        {
            Debug.Log("<color=green>" + "Intent Found" + "</color>");
        }
        else
        {
            //if (!_intent_found) { yield return null; }
            IntentAction();
            if (_intent_found)
            {
                Debug.Log("<color=red>" + "Intent Not Found Perform Action " + "</color>");
                IntentNotFoundAction();
            }
        }
        yield return new WaitForSeconds(1f);
    }

    private void IntentNotFoundAction()
    {
        LLM m_Response = LLM_Model.Instance.SendRequest(m_Listener_Full_Response);
        m_TTSSpeaker.Speak(m_Response.result);
        Debug.Log("<color=red>" + "Intent Not Found Action Method Triggered " + "</color>");
    }

    private void InitIntent()
    {
        _intent_count = 0;
        _intent_found = false;
    }

    private void IntentAction()
    {
        if (_intent_count < 2)
        {
            _intent_count++;
        }
        else
        {
            _intent_found = true;
        }
    }

    public void Begin() //Is called by event listener when command initates.
    {
        Debug.Log("Begin");
        InitIntent();
    }

    public void End() //Is called by event listener when response completes
    {
        Debug.Log("End");
    }

    public void TranscriptionResult(string m_result)
    {
        m_Listener_Full_Response = m_result;
        Debug.Log("<color=green><b>" + m_result + "</b></color>");
    }

    [System.Serializable]
    struct Dict
    {
        public string _name;
        public string _symbol;
        public string _exchange;
    }
}

