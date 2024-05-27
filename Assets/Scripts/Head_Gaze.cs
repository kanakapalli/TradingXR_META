using UnityEngine;

public class HeadGaze : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float maxRayDistance = 10f;
    [SerializeField] private LayerMask targetLayer = ~0;
    [SerializeField] private GameObject cursorPrefab;

    [SerializeField] private bool m_HeadGazeActivate = false;

    private GameObject cursorInstance;

    private void Update()
    {
        if (m_HeadGazeActivate)
        {
            MoveCamera();
            DeleteObject();
            UpdateCursor();
        }
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
                // Instantiate or move the cursor prefab to the hit point
                if (cursorInstance == null)
                {
                    cursorInstance = Instantiate(cursorPrefab, hit.point, Quaternion.identity);
                }
                else
                {
                    cursorInstance.transform.position = hit.point;
                }
            }
            else
            {
                // Destroy the cursor instance
                if (cursorInstance != null)
                {
                    Destroy(cursorInstance);
                    cursorInstance = null;
                }
            }
        }
        else
        {
            // Destroy the cursor instance
            if (cursorInstance != null)
            {
                Destroy(cursorInstance);
                cursorInstance = null;
            }
        }
    }

    public void DeleteTab()
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
            }
        }
    }

    public void ToggleHeadGaze()
    {
        m_HeadGazeActivate = !m_HeadGazeActivate;
    }
}