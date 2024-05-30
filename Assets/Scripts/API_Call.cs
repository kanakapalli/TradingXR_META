using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Meta.Voice.Samples.Dictation;
using Unity.Jobs;

[System.Serializable]
public class API_Call : MonoBehaviour
{
    [SerializeField] internal GameObject m_Stock_Button;
    [SerializeField] Transform m_Stock_Button_Parent;
    [SerializeField] internal Transform m_Stock_Target_Point;
    [SerializeField] internal GameObject m_Stock_Prefab;
    [SerializeField] InputField m_InputField;
    [SerializeField] internal string m_Overview_Base_Url;
    [SerializeField] internal string m_Detail_Base_Url;
    [SerializeField] internal string m_News_Base_Url;
    [SerializeField] internal string m_Exchange;

    [SerializeField] internal GameObject m_VR_Keyboard;
    [SerializeField] internal Vector3 m_Hide_Vector = new Vector3(.001f, .001f, .001f);
    [SerializeField] internal Vector3 m_Show_Vector = new Vector3(1f, 1f, 1f);
    private List<GameObject> m_List = new List<GameObject>();

    [Header("Optimization Technique Used (Object Pooling)")]
    [SerializeField] internal List<StockStruct> m_StockDataList = new List<StockStruct>();
    [SerializeField] internal List<GameObject> m_Instantiated_PrefabList = new List<GameObject>();

    [SerializeField] internal ScrollRect m_StockScrollRect;

    [SerializeField] internal int m_TrackSize;
    [SerializeField] internal int m_LeftSize;
    [SerializeField] internal int m_ReconstructionIndex;
    [SerializeField] internal int m_ObjectIndex;

    [SerializeField] internal bool m_Ended;
    [SerializeField] internal bool m_InitStarted;
    [SerializeField] internal bool m_InitEnded;

    [SerializeField] internal string m_Prev_Url;
    [SerializeField] internal string m_Next_Url;

    private void Awake()
    {
         StartCoroutine(Testing());

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

    private IEnumerator Testing()
    {
        OnEnterPress(); // For Testing
        yield return new WaitForSeconds(2f);
        StartCoroutine(Next_Reconstruction()); // For Testing
        yield return new WaitForSeconds(2f);
        StartCoroutine(Next_Reconstruction()); // For Testing
        yield return new WaitForSeconds(2f);
        StartCoroutine(Next_Reconstruction()); // For Testing
    }

    private void InitOptimization()
    {
        m_TrackSize = m_StockDataList.Count / 5;
        m_LeftSize = m_StockDataList.Count % 5;

        m_StockScrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void InstantiatePrefab()
    {
        StockStruct stock = m_StockDataList[m_ObjectIndex];

        var m_ins_prefab = Instantiate(m_Stock_Button, m_Stock_Button_Parent);
        m_ins_prefab.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = stock.Name;
        m_ins_prefab.GetComponent<Vuplex_Data>().InitValues(stock);
        m_Instantiated_PrefabList.Add(m_ins_prefab);
    }

    private void OnScrollValueChanged(Vector2 scrollPosition)
    {
        CheckBeginEnd();
    }

    private void CheckBeginEnd()
    {
        // Check if the scroll view has reached the right end
        if (m_StockScrollRect.horizontalNormalizedPosition >= 1f && !m_Ended)
        {
            m_Ended = true;
            StartCoroutine(Next_Reconstruction());
            Debug.Log("<color=red>Reached End of Scrolling (Right)</color>");
        }
        // Check if the scroll view has reached the left end
        else if (m_StockScrollRect.horizontalNormalizedPosition <= 0f && m_Ended)
        {
            m_Ended = false;
            StartCoroutine(Previous_Reconstruction());
            Debug.Log("<color=green>Reached Beginning of Scrolling (Left)</color>");
        }
    }

    private void Reconstruction()
    {
        foreach (var stock in m_StockDataList)
        {
            InstantiatePrefab();
        }
    }

    private IEnumerator Next_Reconstruction()
    {
        /*        if (m_ReconstructionIndex < m_TrackSize)
        {
            //DestroyList();
            yield return new WaitForSeconds(.001f);
            m_ReconstructionIndex++;
            for (int i = 0; i < 5; i++)
            {
                InstantiatePrefab();
                m_ObjectIndex++;
            }
            yield return new WaitForSeconds(.002f);
            m_Ended = false;
        }
        else if (m_LeftSize > 0)
        {
            //DestroyList();
            yield return new WaitForSeconds(.001f);
            for (int i = 0; i < m_LeftSize; i++)
            {
                InstantiatePrefab();
                m_ObjectIndex++;
            }
            yield return new WaitForSeconds(.002f);
            m_Ended = false;
        }*/

        NextStockList();
        yield return new WaitForSeconds(.002f);
        m_Ended = false;
    }

    private IEnumerator Previous_Reconstruction()
    {
        /*if (m_Stack_DataList.Count == 0)
        {
            yield return null;
        }
        else if (m_Stack_DataList.Count > 0)
        {
            if (m_ReconstructionIndex > 0)
            {
                //DestroyList();
                yield return new WaitForSeconds(.1f);
                m_ReconstructionIndex++;
                for (int i = 0; i < 5; i++)
                {
                    InstantiatePrefab();
                    m_ObjectIndex--;
                    m_Stack_DataList.Remove(m_I_Track_List[m_ObjectIndex]);
                }
                yield return new WaitForSeconds(.2f);
            }
            else if (m_LeftSize > 0)
            {
                //DestroyList();
                yield return new WaitForSeconds(.1f);
                for (int i = 0; i < m_LeftSize; i++)
                {
                    InstantiatePrefab();
                    m_Stack_DataList.Add(m_I_Track_List[m_ObjectIndex]);
                    m_ObjectIndex++;
                }
                yield return new WaitForSeconds(.2f);
            }
            else
            {
                yield return null;
            }
        }*/
        yield return new WaitForSeconds(.002f);
        m_Ended = true;
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
        #region Default Method For Fetching Data
        //int i = 0;

        /*m_StockDataList.Clear();
        yield return new WaitForSeconds(.2f);*/

        foreach (var stock in stockData)
        {
            //Print the values of the stock name andd symbol and sale
            Debug.Log($"Name: {stock.Name} Stock: {stock.Symbol} - Last Sale: {stock.Last_Sale}");

            //Instantiating the button gameobject and parenting the content of the scroll rect
            //var m_Button = Instantiate(m_Stock_Button, m_Stock_Button_Parent);

            //Also assiging the values
            //m_Button.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = stock.Name;

            //Getting The Urls
            string m_overview_url = string.Concat(m_Overview_Base_Url, stock.Symbol, "&exchange=", m_Exchange);
            string m_detail_url = string.Concat(m_Detail_Base_Url, stock.Symbol, "&exchange=", m_Exchange);

            Debug.Log("<color=cyan>" + m_overview_url + " ============ " + m_detail_url + " =========== " + "</color>");

            StockStruct m_stock = new StockStruct
            {
                Overview_URL = m_overview_url,
                Detail_URL = m_detail_url,
                Name = stock.Name,
                Symbol = stock.Symbol,
                Price = stock.Last_Sale,
                Percent = stock.Percent_Change
            };

            //Pushing the data to the m_StockDatalist
            m_StockDataList.Add(m_stock);

            //m_Button.GetComponent<Vuplex_Data>().InitValues(m_stock);
            //m_List.Add(m_Button);

            /*if (i >= 5)
            {
                yield return new WaitForSeconds(0.8f);
                i = 0;
            }
            i++;*/
        }
        yield return new WaitForSeconds(.001f);
        InitOptimization();
        yield return new WaitForSeconds(.001f);
        Reconstruction();
        #endregion
    }

    private void OnError(string error)
    {
        Debug.LogError("Failed to get stock data: " + error);

    }

    public void OnEnterPress()
    {
        DestroyClearAllList();
        StartCoroutine(StockApiService.Instance.SearchStocks(m_InputField.text, OnDataReceived, OnError, OperationMode.Current));
        m_InputField.text = "";
        m_VR_Keyboard.transform.localScale = m_Hide_Vector;
        m_VR_Keyboard.transform.position = Vector3.zero;
    }

    public void NextStockList()
    {
        StartCoroutine(StockApiService.Instance.SearchStocks(m_InputField.text, OnDataReceived, OnError, OperationMode.Next));
    }

    public void PrevStockList()
    {
        StartCoroutine(StockApiService.Instance.SearchStocks(m_InputField.text, OnDataReceived, OnError, OperationMode.Previous));
    }

    private void DestroyClearList()
    {
        m_StockDataList.Clear();

        //Set Default Value
        m_TrackSize = 0;
        m_LeftSize = 0;
        m_ReconstructionIndex = 0;
        m_ObjectIndex = 0;

        m_Ended = false;
    }

    private void DestroyClearAllList()
    {
        foreach (var item in m_List)
        {
            Destroy(item);
        }

        foreach (var item in m_Instantiated_PrefabList)
        {
            Destroy(item);
        }

        m_List.Clear();
        m_Instantiated_PrefabList.Clear();
        m_StockDataList.Clear();


        //Set Default Value
        m_TrackSize = 0;
        m_LeftSize = 0;
        m_ReconstructionIndex = 0;
        m_ObjectIndex = 0;

        m_Ended = false;
    }
}
