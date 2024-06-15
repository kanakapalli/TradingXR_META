using Meta.WitAi.Utilities;
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
    private float previousScaleX;
    private float previousScaleY;
    private float normalScale = 1.0f;

    private void Start()
    {
        previousScaleX = GetOverallScale().x;
        previousScaleY = GetOverallScale().y;
        previousScale = GetOverallScale().ratio;
    }

    private void Update()
    {
        CheckScale();
    }

    private void CheckScale()
    {
        float currentScaleX = GetOverallScale().x;
        float currentScaleY = GetOverallScale().y;
        float currentScale = GetOverallScale().ratio;

        if (currentScale != previousScale || currentScaleX != previousScaleX || currentScaleY != previousScaleY)
        {
            if (currentScale >= 1.5f || currentScaleX >= 1.5f || currentScaleY >= 1.5f)
            {
                // Scale has increased beyond or equal to 1.5 in any axis
                ChangeURLMode(true);
            }
            else if ((currentScale > 0.4f && currentScale < 1.5f) || (currentScaleX > 0.4f && currentScaleX < 1.5f) || (currentScaleY > 0.4f && currentScaleY < 1.5f))
            {
                // Scale is between 0.4 and 1.5 in any axis
                ChangeURLMode(false);
            }
            else if (currentScale <= 0.4f || currentScaleX <= 0.4f || currentScaleY <= 0.4f)
            {
                // Scale has become less than or equal to 0.4 in any axis
                Destroy(gameObject, 2f);
            }

            previousScale = currentScale;
            previousScaleX = currentScaleX;
            previousScaleY = currentScaleY;
        }
    }

    private ScaleStructure GetOverallScale()
    {
        Vector3 localScale = transform.localScale;
        return new ScaleStructure { x = localScale.x, y = localScale.y, ratio = localScale.x * localScale.y };
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

    struct ScaleStructure
    {
        public float x, y, ratio;
    }
}
