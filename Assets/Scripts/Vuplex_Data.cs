using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Vuplex_Data : MonoBehaviour
{
    [SerializeField] internal string m_Overview_Url;
    [SerializeField] internal string m_Detail_Url;
    [SerializeField] internal GameObject m_Stock_Prefab;
    [SerializeField] internal TMP_Text m_Stock_Name;
    [SerializeField] internal TMP_Text m_Stock_Symbol;
    [SerializeField] internal TMP_Text m_Stock_Price;
    [SerializeField] internal TMP_Text m_Stock_Percent;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            //Spawning Vuplex View
            var m_Object = Instantiate(m_Stock_Prefab, FindObjectOfType<API_Call>().m_Stock_Target_Point.position, Quaternion.identity);

            //Assigning Overview and Detail URL
            m_Object.GetComponent<Vuplex_Tab>().m_Overview_Url = m_Overview_Url;
            m_Object.GetComponent<Vuplex_Tab>().m_Detail_Url = m_Detail_Url;

            //Refreshing By Assigning The URL
            m_Object.GetComponent<Vuplex_Tab>().ChangeURLMode(false);
        });
    }

    internal void InitValues(StockStruct m_Stock_Data)
    {
        m_Stock_Name.text = string.Concat("Name: ", m_Stock_Data.Name);
        m_Stock_Symbol.text = string.Concat("Symbol: ", m_Stock_Data.Symbol);
        m_Stock_Price.text = string.Concat("Price: ", m_Stock_Data.Price);
        m_Stock_Percent.text = string.Concat("Percent: ", m_Stock_Data.Percent);
        m_Overview_Url = m_Stock_Data.Overview_URL;
        m_Detail_Url = m_Stock_Data.Detail_URL;
    }
}
