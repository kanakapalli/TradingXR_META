using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyProfile", menuName = "Profile")]
public class My_Profile : ScriptableObject
{
    [SerializeField] internal string m_Token;
    [SerializeField] internal string m_ProfileName;
    [SerializeField] internal int m_Age;
    [SerializeField] internal string m_Email;
    [SerializeField] internal string m_Office;
    [SerializeField] internal string m_Bio;
    [SerializeField] internal string m_Thread;

    public void Clear()
    {
        m_Token = "";
        m_ProfileName = "";
        m_Age = 0;
        m_Email = "";
        m_Office = "";
        m_Bio = "";
        m_Thread = "";
    }
}
