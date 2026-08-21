using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapObj : MonoBehaviour
{
    [SerializeField] private Collider detectCollider;

    private Bounds bounds;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        bounds = detectCollider.bounds;
    }

    public Vector2 Extent
    {
        get
        {
            return new Vector2(bounds.extents.x, bounds.extents.z);
        }
    }

    public float X1
    {
        get
        {
            return (bounds.center.x - bounds.extents.x);
        }
    }
    public float X2
    {
        get
        {
            return (bounds.center.x + bounds.extents.x);
        }
    }
    public float Z1
    {
        get
        {
            return (bounds.center.z - bounds.extents.z);
        }
    }
    public float Z2
    {
        get
        {
            return (bounds.center.z + bounds.extents.z);
        }
    }
}
