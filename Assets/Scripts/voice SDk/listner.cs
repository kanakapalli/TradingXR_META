using Meta.WitAi.CallbackHandlers;
using Meta.WitAi.TTS.Utilities;
using Oculus.Interaction.Samples.PalmMenu;
using Oculus.Voice;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Listener : MonoBehaviour
{
    [SerializeField] TTSSpeaker m_TTSSpeaker;
    [SerializeField] API_Call m_API_Call;
    [SerializeField] PalmMenuExampleButtonHandlers m_PalmMenuExampleButtonHandlers;
    [SerializeField] Palm_Menu_Controller m_PalmMenuController;
    [SerializeField] AppVoiceExperience m_AppVoiceExperience;
    [SerializeField] List<Dict> m_Demo_Dict = new List<Dict>();
    [SerializeField] Text m_LLM_Text;
    [SerializeField] string m_Listener_Full_Response;

    private string gainLose = "https://assisntak.web.app/topgainerlossers";
    private bool _intent_found = false;
    private int _intent_count = 0;

    public void GetStocksIntent(string[] values)
    {
        string _intent = values[0];
        string _stock_name = values[1];

        Debug.Log("<color=blue>" + _stock_name + "</color>");
        Debug.Log("<color=blue>" + _intent + "</color>");
        _intent_found = true;

        var m_Object = Instantiate(m_API_Call.m_Stock_Prefab, m_API_Call.m_Stock_Target_Point.position, Quaternion.identity);
        int _res = GetIndex(_stock_name);
        if (_res == -1)
        {
            return;
        }

        var _dict = m_Demo_Dict[_res];
        TurnOffIntent();

        string m_overview_url = $"{m_API_Call.m_Overview_Base_Url}{_dict._symbol}&exchange={_dict._exchange}";
        string m_detail_url = $"{m_API_Call.m_Detail_Base_Url}{_dict._symbol}&exchange={_dict._exchange}";

        var tab = m_Object.GetComponent<Vuplex_Tab>();
        tab.m_Overview_Url = m_overview_url;
        tab.m_Detail_Url = m_detail_url;
        tab.m_CanvasWebView.InitialUrl = m_overview_url;

        m_TTSSpeaker.Speak($"Here is the stock of {_stock_name}");
    }

    public void GetStocksNewsIntent(string[] values)
    {
        string _intent = values[0];
        string _stock_name = values[1];

        Debug.Log("<color=blue>" + _stock_name + "</color>");
        Debug.Log("<color=blue>" + _intent + "</color>");
        _intent_found = true;

        int _res = GetIndex(_stock_name);
        if (_res == -1)
        {
            return;
        }

        var _dict = m_Demo_Dict[_res];
        TurnOffIntent();

        var m_Object = Instantiate(m_API_Call.m_Stock_Prefab, m_API_Call.m_Stock_Target_Point.position, Quaternion.identity);
        string m_news_url = $"{m_API_Call.m_News_Base_Url}{_dict._symbol}&exchange={_dict._exchange}";

        var tab = m_Object.GetComponent<Vuplex_Tab>();
        tab.m_CanvasWebView.InitialUrl = m_news_url;
        tab.m_CanvasWebView.WebView.LoadUrl(m_news_url);
        tab.m_CanvasWebView.WebView.Reload();

        m_TTSSpeaker.Speak($"Here is the stock of {_stock_name}");
    }

    public void GetNewsIntent(string[] values)
    {
        string _intent = values[0];
        string _stock_name = values[1];

        Debug.Log("<color=blue>" + _stock_name + "</color>");
        Debug.Log("<color=blue>" + _intent + "</color>");
        _intent_found = true;

        int _res = GetIndex(_stock_name);
        if (_res == -1)
        {
            return;
        }

        var _dict = m_Demo_Dict[_res];
        TurnOffIntent();

        var m_Object = Instantiate(m_API_Call.m_Stock_Prefab, m_API_Call.m_Stock_Target_Point.position, Quaternion.identity);
        var tab = m_Object.GetComponent<Vuplex_Tab>();
        tab.m_CanvasWebView.InitialUrl = gainLose;
        tab.m_CanvasWebView.WebView.LoadUrl(gainLose);
        tab.m_CanvasWebView.WebView.Reload();

        m_TTSSpeaker.Speak($"Here is the stock of {_stock_name}");
    }

    private int GetIndex(string _name)
    {
        for (int i = 0; i < m_Demo_Dict.Count; i++)
        {
            if (m_Demo_Dict[i]._name.Equals(_name, StringComparison.OrdinalIgnoreCase))
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
        CheckIntentAsync();
    }

    private async void CheckIntentAsync()
    {
        await Task.Delay(2000);
        if (_intent_found)
        {
            Debug.Log("<color=green>Intent Found</color>");
        }
        else
        {
            IntentAction();
            if (_intent_found)
            {
                Debug.Log("<color=red>Intent Not Found Perform Action</color>");
                m_LLM_Text.text = "Thinking...";
                await IntentNotFoundActionAsync();
            }
        }
        await Task.Delay(500);
    }

    private async Task IntentNotFoundActionAsync()
    {
        await Task.Delay(500);
        var m_Response = await LLM_Model.Instance.SendRequestAsync(m_Listener_Full_Response);
        await Task.Delay(500);
        string m_response = m_Response.result;
        m_TTSSpeaker.Speak(m_response);
        m_LLM_Text.text = m_response;
        Debug.Log("<color=red>Intent Not Found Action Method Triggered</color>");
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

    public void Begin()
    {
        Debug.Log("Begin");
        InitIntent();
    }

    public void End()
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