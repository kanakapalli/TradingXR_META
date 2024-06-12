using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StockSearchCanvas : MonoBehaviour
{
    [SerializeField] InputField m_Key_Search;
    [SerializeField] GameObject m_SearchIcon_1;
    [SerializeField] GameObject m_SearchIcon_2;
    [SerializeField] Button m_BackToDefault;
    [SerializeField] GameObject m_SearchVoice;
    [SerializeField] GameObject m_Recent_Text;
    [SerializeField] GameObject m_Stocklist;

    [SerializeField] Vector3 m_Icon_Default;
    [SerializeField] Vector3 m_Icon_Searched;

    public void SearchActivate()
    {
        m_Key_Search.gameObject.SetActive(true);
        m_Stocklist.SetActive(false);
        m_BackToDefault.gameObject.SetActive(false);
        m_SearchVoice.SetActive(true);
        m_Recent_Text.gameObject.SetActive(false);
        m_SearchIcon_1.SetActive(false);
        m_SearchIcon_2.SetActive(true);
    }

    public void SearchDeactivate()
    {
        m_Key_Search.gameObject.SetActive(false);
        m_Stocklist.SetActive(true);
        m_BackToDefault.gameObject.SetActive(true);
        m_SearchVoice.SetActive(false);
        m_Recent_Text.gameObject.SetActive(true);
        m_SearchIcon_1.SetActive(true); 
        m_SearchIcon_2.SetActive(false);
    }
}
