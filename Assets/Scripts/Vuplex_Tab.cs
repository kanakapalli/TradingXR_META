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
    [SerializeField] internal LayerMask wallLayerMask;
    [SerializeField] internal Transform raycastOrigin;
    [SerializeField] internal float rangeDistance;
    [SerializeField] internal GameObject cursorPrefab; // Add this line

    private float previousScale;
    private float normalScale = 1.0f;
    private GameObject spawnedCursor; // Add this line

    private void Start()
    {
        previousScale = GetOverallScale();
    }

    private void LateUpdate()
    {
        CheckScale();
        CheckCollisionWithWall();
    }

    private void CheckScale()
    {
        float currentScale = GetOverallScale();

        if (currentScale != previousScale)
        {
            if (currentScale >= 1.5f)
            {
                // Scale has increased beyond or equal to 1.5
                ChangeURLMode(true);
            }
            else if (currentScale > 0.4f && currentScale < 1.5f)
            {
                // Scale is between 0.4 and 1.5
                ChangeURLMode(false);
            }
            else if (currentScale <= 0.4f)
            {
                // Scale has become less than or equal to 0.4
                Destroy(gameObject, 2f);
            }

            previousScale = currentScale;
        }
    }

    private float GetOverallScale()
    {
        Vector3 localScale = transform.localScale;
        return localScale.x * localScale.y;
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

    private void CheckCollisionWithWall()
    {
        if (raycastOrigin == null)
        {
            Debug.LogError("Raycast origin is not assigned.");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, -raycastOrigin.forward, out hit, rangeDistance, wallLayerMask))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                AlignWithWall(hit.point, hit.normal);
                SpawnCursor(hit.point); // Add this line
            }
        }
    }

    private void AlignWithWall(Vector3 collisionPoint, Vector3 collisionNormal)
    {
        // Snap to the wall at the collision point and align with the wall's normal
        transform.position = collisionPoint;
        transform.rotation = Quaternion.LookRotation(-collisionNormal, Vector3.up);
    }

    private void SpawnCursor(Vector3 position)
    {
        // Destroy the old cursor if it exists
        if (spawnedCursor != null)
        {
            Destroy(spawnedCursor);
        }
        // Instantiate a new cursor at the given position
        spawnedCursor = Instantiate(cursorPrefab, position, Quaternion.identity);
    }
}
