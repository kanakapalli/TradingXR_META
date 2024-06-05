using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuplex.WebView;

public class Vuplex_Tab : MonoBehaviour
{
    [SerializeField] internal CanvasWebViewPrefab m_CanvasWebView;

    [SerializeField] internal string m_Overview_Url;
    [SerializeField] internal string m_Detail_Url;

    private float previousScale;
    private float normalScale = 1.0f;

    private void LateUpdate()
    {
        CheckScale();
    }

    private void CheckScale()
    {
        float currentScale = transform.localScale.x;

        if (currentScale != previousScale)
        {
            if (currentScale > previousScale && currentScale >= 1.5f)
            {
                // Scale has increased beyond 2
                ChangeURLMode(true);
            }
            else if (currentScale < previousScale && currentScale > normalScale && currentScale <= 1.2f)
            {
                // Scale has decreased but is still above normal scale
                ChangeURLMode(false);
            }
            else if (currentScale < normalScale && currentScale < previousScale && currentScale <= .4f)
            {
                // Scale has become less than normal scale
                Destroy(gameObject, 2f);
            }

            previousScale = currentScale;
        }
    }

    internal void ChangeURLMode(bool m_config)
    {
        if (m_config)
        {
            StartCoroutine(URL_Refresh(m_Detail_Url));
        }
        else
        {
            StartCoroutine(URL_Refresh(m_Overview_Url));
        }
    }

    private IEnumerator URL_Refresh(string m_url)
    {
        m_CanvasWebView.InitialUrl = m_url;
        m_CanvasWebView.WebView.LoadUrl(m_url);
        yield return new WaitForSeconds(1f);
        m_CanvasWebView.WebView.Reload();
    }
}
