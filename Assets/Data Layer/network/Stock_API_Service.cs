using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class StockApiService
{


    // Singleton instance
    public static StockApiService Instance { get; private set; } = new StockApiService();

    private StockApiService() { }

    public IEnumerator SearchStocks(string searchKey, System.Action<List<StockData>> onSuccess, System.Action<string> onError, OperationMode oMode)
    {
        string url;
        API_Call m_Api = GameObject.FindObjectOfType<API_Call>();
        switch (oMode)
        {
            case OperationMode.Current:
                url = $"{ConfigurationManager.Instance.BaseUrl}/api/stocks/?search={searchKey}";
                break;
            case OperationMode.Previous:
                url = m_Api.m_Prev_Url;
                break;
            case OperationMode.Next:
                url = m_Api.m_Next_Url;
                break;
            default:
                url = $"{ConfigurationManager.Instance.BaseUrl}/api/stocks/?search={searchKey}";
                break;
        }
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Authorization", $"Token {ConfigurationManager.Instance.Token}");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(webRequest.error);
            }
            else
            {
                try
                {
                    /*var response = JsonConvert.DeserializeObject<StockDataResponse>(webRequest.downloadHandler.text);
                    onSuccess?.Invoke(response.Data);*/
                    var response = JsonDatahandler.HandleJsonData(webRequest.downloadHandler.text);
                    onSuccess?.Invoke(response.m_Results);
                }
                catch (System.Exception ex)
                {
                    onError?.Invoke(ex.Message);
                }
            }
        }
    }

}
public enum OperationMode
{
    Current,
    Previous,
    Next
}