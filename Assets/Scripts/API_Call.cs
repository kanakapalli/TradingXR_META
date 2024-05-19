using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class API_Call : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI m_Test_Text;
    [SerializeField] GameObject m_Stock_Button;
    [SerializeField] Transform m_Stock_Button_Parent;
    [SerializeField] internal Transform m_Stock_Target_Point;
    [SerializeField] internal GameObject m_Stock_Prefab;
    [SerializeField] InputField m_InputField;
    [SerializeField] internal string m_Overview_Base_Url;
    [SerializeField] internal string m_Detail_Base_Url;
    [SerializeField] internal string m_News_Base_Url;
    [SerializeField] internal string m_Exchange;

    [SerializeField] internal GameObject m_VR_Keyboard;
    //[SerializeField] internal GameObject m_VR_Virtual_Keyboard;
    [SerializeField] internal Vector3 m_Hide_Vector = new Vector3(.001f, .001f, .001f);
    [SerializeField] internal Vector3 m_Show_Vector = new Vector3(1f, 1f, 1f);

    private List<GameObject> m_List = new List<GameObject>();

    void Start()
    {
        OnEnterPress();

        m_VR_Keyboard.transform.localScale = m_Hide_Vector;

        User sampleUser = new User
        {
            Username = "JohnDoe",
            Email = "johndoe@example.com",
            Office = "None",
            Age = 30,
            Bio = "Just a sample user for testing.",
            LoginCode = "123456",
            ConversionThread = null,
            ProfilePicture = null
        };
        UserManager.SaveUserData(sampleUser);
    }

    private void Update()
    {
        if (m_InputField.isFocused && m_VR_Keyboard.transform.localScale == m_Hide_Vector)
        {
            m_VR_Keyboard.transform.localScale = m_Show_Vector;
            m_VR_Keyboard.transform.position = m_Stock_Target_Point.position;
            m_VR_Keyboard.transform.rotation = Quaternion.identity;
        }
    }

    private void OnDataReceived(List<StockData> stockData)
    {
        User user = UserManager.LoadUserData();
        //m_Test_Text.text = $"Username: {user.Username} - Bio: {user.Bio}";
        DestroyClearList();
        StartCoroutine(GetData(stockData));
    }

    private IEnumerator GetData(List<StockData> stockData)
    {
        int i = 0;

        foreach (var stock in stockData)
        {
            Debug.Log(stock.Name + " " + stock.Symbol);


            Debug.Log($"Stock: {stock.Symbol} - Last Sale: {stock.Last_Sale}");
            var m_Button = Instantiate(m_Stock_Button, m_Stock_Button_Parent);
            m_Button.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = stock.Name;

            //Getting The Urls
            string m_overview_url = string.Concat(m_Overview_Base_Url, stock.Symbol, "&exchange=", m_Exchange);
            string m_detail_url = string.Concat(m_Detail_Base_Url, stock.Symbol, "&exchange=", m_Exchange);

            Debug.Log("<color=blue>" + m_overview_url + " ============ " + m_detail_url + " =========== " + "</color>");

            /*m_Button.GetComponent<Vuplex_Data>().m_Overview_Url = m_overview_url;
            m_Button.GetComponent<Vuplex_Data>().m_Detail_Url = m_detail_url;*/
            m_Button.GetComponent<Vuplex_Data>().InitValues(new StockStruct { 
                                                                    Overview_URL = m_overview_url, 
                                                                    Detail_URL = m_detail_url,
                                                                    Name = stock.Name,
                                                                    Symbol = stock.Symbol,
                                                                    Price = stock.Last_Sale,
                                                                    Percent = stock.Percent_Change
                                                                    });

            m_List.Add(m_Button);

            if (i >= 5)
            {
                yield return new WaitForSeconds(0.8f);
                i = 0;
            }
            i++;
        }
    }

    private void OnError(string error)
    {
        Debug.LogError("Failed to get stock data: " + error);

    }

    public void OnEnterPress()
    {
        StartCoroutine(StockApiService.Instance.SearchStocks(m_InputField.text, OnDataReceived, OnError));
        m_InputField.text = "";
        m_VR_Keyboard.transform.localScale = m_Hide_Vector;
        m_VR_Keyboard.transform.position = Vector3.zero;
    }

    private void DestroyClearList()
    {
        foreach (var item in m_List)
        {
            Destroy(item);
        }

        m_List.Clear();
    }
}
