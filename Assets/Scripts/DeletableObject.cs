using UnityEngine;

public class DeletableObject : MonoBehaviour
{
    [SerializeField] private LayerMask deletableLayer;

    public bool CanBeDeleted()
    {
        // Check if the object is on the deletable layer
        return (deletableLayer.value & (1 << gameObject.layer)) != 0;
    }
}