using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MRUK_Control : MonoBehaviour
{
    public void WallManipulation()
    {
        MRUK m_mruk = MRUK.Instance;
        List<MRUKAnchor> m_wall = m_mruk.GetCurrentRoom().WallAnchors;
        MRUKAnchor m_floor = m_mruk.GetCurrentRoom().FloorAnchor;

        int wallLayer = LayerMask.NameToLayer("WallLayer"); // Get the layer index for "WallLayer"
        if (wallLayer == -1)
        {
            Debug.LogError("Layer 'WallLayer' not found. Make sure it is added in the Tags and Layers settings.");
            return;
        }

        foreach (MRUKAnchor wall in m_wall)
        {
            wall.tag = "Wall";
            wall.gameObject.layer = wallLayer; // Set the layer to "WallLayer"

            Debug.Log("<color=yellow>" + wall + "</color>");
            Debug.Log("<color=yellow>" + wall.Anchor + "</color>");
            Debug.Log("<color=yellow>" + wall.tag + "</color>");
            Debug.Log("<color=yellow>" + wall.name + "</color>");
            Debug.Log("<color=yellow>" + wall.transform + "</color>");
            Debug.Log("<color=yellow>" + wall.gameObject + "</color>");
            Debug.Log("<color=yellow>" + wall.AnchorLabels + "</color>");
            Debug.Log("<color=yellow>" + wall.VolumeBounds + "</color>");
        }
    }
}

