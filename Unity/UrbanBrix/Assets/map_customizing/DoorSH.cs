using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSH : MonoBehaviour
{
    public Mark MoveXMark;
    public Mark MoveYMark;
    public Mark MoveBothMark;
    public Transform mesh;

    private Wall m_parentWall = null;
    public Wall ParentWall
    {
        get { return m_parentWall; }
        set { m_parentWall = value; }
    }

    public float Scale
    {
        get { return 8; }
        //set
        //{
        //    mesh.localScale = new Vector3(mesh.localScale.x, mesh.localScale.y, value);
        //}
    }

    // Start is called before the first frame update
    void Start()
    {
        //mat = mesh.GetComponent<MeshRenderer>().material;

        MoveXMark.Active(false);
        MoveYMark.Active(false);
        MoveBothMark.Active(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select(CustomizingStage stage)
    {
        switch (stage)
        {
            case CustomizingStage.Spawn:
            case CustomizingStage.Mode_Move:
                MoveXMark.Active(true);
                MoveYMark.Active(true);
                MoveBothMark.Active(true);
                break;
            default:
                MoveXMark.Active(false);
                MoveYMark.Active(false);
                MoveBothMark.Active(false);
                break;
        }
    }

    public Vector3 GetDoorPoint(bool referenceAxis)
    {
        Vector3 center = transform.localPosition;
        float angle = transform.localEulerAngles.y * Mathf.Deg2Rad;

        float cos = Mathf.Cos(-angle);
        float sin = Mathf.Sin(-angle);

        float halfScale = Scale / 2;

        float x2 = center.x + halfScale * sin;
        float z2 = center.z - halfScale * cos;

        Vector3 p2 = new Vector3(x2, center.y, z2);

        if (referenceAxis == false)
            return p2;

        return center * 2 - p2;
    }

    public void SetWall(Wall wall)
    {
        if (this.ParentWall == wall)
            return;

        if (this.ParentWall != null)
        {
            this.ParentWall.RemoveDoor(this);
            this.ParentWall = null;
        }

        if (wall != null)
            wall.AddDoor(this);
        this.ParentWall = wall;
    }
}
