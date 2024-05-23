using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Optimization : MonoBehaviour
{
    [SerializeField] internal Transform m_Content;
    [SerializeField] internal ScrollRect m_ScrollRect;
    [SerializeField] internal GameObject m_Prefab;

    [SerializeField] internal List<GameObject> m_Ins_PrefabList;
    [SerializeField] internal List<int> m_Stack_DataList;
    [SerializeField] internal List<int> m_I_Track_List;

    [SerializeField] int m_TrackSize;
    [SerializeField] int m_LeftSize;
    [SerializeField] int m_ReconstructionIndex;
    [SerializeField] int m_ObjectIndex;

    private bool m_Ended = false;
    private bool m_InitEnded = false;
    private bool m_InitStarted = false;

    private void Start()
    {
        m_TrackSize = Mathf.FloorToInt(m_I_Track_List.Count / 5);
        m_LeftSize = Mathf.FloorToInt(m_I_Track_List.Count % 5);

        m_ScrollRect.onValueChanged.AddListener(OnScrollValueChanged);

        StartCoroutine(Next_Reconstruction());
    }

    /*    private void Update()
        {
            CheckBeginEnd();
        }*/

    private void InstantiatePrefab()
    {
        var m_ins_prefab = Instantiate(m_Prefab);
        m_ins_prefab.transform.SetParent(m_Content.transform);
        m_ins_prefab.transform.GetChild(0).GetComponent<TMP_Text>().text = m_I_Track_List[m_ObjectIndex].ToString();
        m_Ins_PrefabList.Add(m_ins_prefab);
    }

    private void OnScrollValueChanged(Vector2 scrollPosition)
    {
        CheckBeginEnd();
    }

    private void CheckBeginEnd()
    {
        // Check if the scroll view has reached the right end
        if (m_ScrollRect.horizontalNormalizedPosition >= 1f && !m_Ended)
        {
            m_Ended = true;
            /*if (!m_InitEnded)
            {
                m_InitEnded = true;
            }*/
            //else
            //{
            StartCoroutine(Next_Reconstruction());
            Debug.Log("<color=red>Reached End of Scrolling (Right)</color>");
            //}
        }
        // Check if the scroll view has reached the left end
        else if (m_ScrollRect.horizontalNormalizedPosition <= 0f && m_Ended)
        {
            m_Ended = false;
            /*if (!m_InitStarted)
            {
                m_InitStarted = true;
            }*/
            StartCoroutine(Previous_Reconstruction());
            Debug.Log("<color=green>Reached Beginning of Scrolling (Left)</color>");
        }
    }

    private void DestroyList()
    {
        foreach (var x in m_Ins_PrefabList)
        {
            Destroy(x.gameObject);
        }
        m_Ins_PrefabList.Clear();
    }

    private IEnumerator Next_Reconstruction()
    {
        if (m_ReconstructionIndex < m_TrackSize)
        {
            //DestroyList();
            yield return new WaitForSeconds(.1f);
            m_ReconstructionIndex++;
            for (int i = 0; i < 5; i++)
            {
                InstantiatePrefab();
                m_Stack_DataList.Add(m_I_Track_List[m_ObjectIndex]);
                m_ObjectIndex++;
            }
            yield return new WaitForSeconds(.2f);
            m_Ended = false;
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
            m_Ended = false;
        }
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
        yield return new WaitForSeconds(.2f);
        m_Ended = true;
    }
}
