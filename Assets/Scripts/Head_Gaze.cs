using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeadGaze : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float maxRayDistance = 10f;
    [SerializeField] private LayerMask targetLayer = ~0;
    [SerializeField] private GameObject cursorPrefab;

    [SerializeField] private bool m_HeadGazeActivate = false;

    [SerializeField] private Material m_Default_Material;
    [SerializeField] private Material m_Custom_Material;

    [SerializeField] private SkinnedMeshRenderer m_Left_Renderer;
    [SerializeField] private SkinnedMeshRenderer m_Right_Renderer;

    private GameObject cursorInstance;

    private List<GameObject> tabFocused = new List<GameObject>();

    private void Update()
    {
        if (m_HeadGazeActivate)
        {
            ApplyMaterial(m_Left_Renderer, m_Custom_Material);
            ApplyMaterial(m_Right_Renderer, m_Custom_Material);

            //MoveCamera();
            //DeleteObject();
            UpdateCursor();
        }
        else
        {
            ApplyMaterial(m_Left_Renderer, m_Default_Material);
            ApplyMaterial(m_Right_Renderer, m_Default_Material);
        }
    }

    private void ApplyMaterial(SkinnedMeshRenderer renderer, Material material)
    {
        Material[] materials = renderer.materials;
        /*if (materials.Length > 1) // Ensure the array has the correct length
        {
            materials[1] = material;
            renderer.materials = materials; // Assign the updated materials array back
        }
        else
        {
            Debug.LogWarning("The materials array does not have the expected length.");
        }*/
        materials[0] = material;
        renderer.materials = materials; // Assign the updated materials array back
    }

    private void MoveCamera()
    {
        // Move the camera based on mouse input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * mouseX, Space.Self);
        transform.Rotate(Vector3.left * mouseY, Space.World);
    }

    private void DeleteObject()
    {
        // Cast a ray from the camera
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, targetLayer))
        {
            // Check if the hit object has the DeletableObject component
            DeletableObject deletableObject = hit.collider.GetComponent<DeletableObject>();
            if (deletableObject != null && Input.GetMouseButtonDown(0))
            {
                // Delete the object if it's on the correct layer
                if (deletableObject.CanBeDeleted())
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    private void UpdateCursor()
    {
        // Cast a ray from the camera
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, targetLayer))
        {
            // Check if the hit object has the DeletableObject component
            DeletableObject deletableObject = hit.collider.GetComponent<DeletableObject>();
            if (deletableObject != null && deletableObject.CanBeDeleted())
            {
                Transform m_collided = hit.collider.gameObject.transform;
                GameObject tab = m_collided.GetChild(m_collided.childCount - 1).gameObject; // Detects the last child

                if (!tabFocused.Contains(tab))
                {
                    TurnOffBorderTargets();
                    tab.SetActive(true);
                    tabFocused.Add(tab);
                }
            }
            else
            {
                TurnOffBorderTargets();
            }
        }
        else
        {
            TurnOffBorderTargets();
        }
    }

    public void DeleteTab()
    {
        if (m_HeadGazeActivate)
        {
            // Cast a ray from the camera
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRayDistance, targetLayer))
            {
                // Check if the hit object has the DeletableObject component
                DeletableObject deletableObject = hit.collider.GetComponent<DeletableObject>();
                if (deletableObject != null && deletableObject.CanBeDeleted())
                {
                    // Delete the object
                    Destroy(hit.collider.gameObject);
                    TurnOffBorderTargets();
                }
            }
        }
    }

    public void ToggleHeadGaze()
    {
        m_HeadGazeActivate = !m_HeadGazeActivate;
        TurnOffBorderTargets();
    }

    private void TurnOffBorderTargets()
    {
        foreach (GameObject obj in tabFocused)
        {
            obj.SetActive(false);
        }
        tabFocused.Clear();
    }
}
