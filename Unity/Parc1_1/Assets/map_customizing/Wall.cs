using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 이동,회전,스케일 On/Off 기능
 */

public class Wall : MonoBehaviour
{
    public Mark MoveXMark;
    public Mark MoveYMark;
    public Mark MoveBothMark;
    public Mark Scale1Mark;
    public Mark Scale2Mark;
    public Mark RotateMark;
    public Transform mesh;

    private Material mat;

    public void Start()
    {
        mat = mesh.GetComponent<MeshRenderer>().material;

        MoveXMark.Active(false);
        MoveYMark.Active(false);
        MoveBothMark.Active(false);
        Scale1Mark.Active(false);
        Scale2Mark.Active(false);
        RotateMark.Active(false);
    }

    public float Scale
    {
        get
        {
            return mesh.localScale.z;
        }
    }
    public float Rotate
    {
        get
        {
            return Vector3.SignedAngle(Vector3.right, mesh.right, Vector3.up);
        }
    }
    public Vector3 MeshRight
    {
        get
        {
            return mesh.transform.right;
        }
    }

    public void ScaleFromCenter(float scale)
    {

        Vector3 scale1Pos = Scale1Mark.transform.position;
        Vector3 scale2Pos = Scale2Mark.transform.position;

        Vector3 scale1Dir = (scale1Pos - scale2Pos).normalized;
        Vector3 scale2Dir = -scale1Dir;

        Vector3 mid = (scale1Pos + scale2Pos) * 0.5f;

        mesh.localScale = new Vector3(mesh.localScale.x, mesh.localScale.y, scale);

        Scale1Mark.transform.position = mid + scale1Dir * scale * 0.5f;
        Scale2Mark.transform.position = mid + scale2Dir * scale * 0.5f;
    }
    public void UpdateScale()
    {
        Vector3 scale1Pos = Scale1Mark.transform.position;
        Vector3 scale2Pos = Scale2Mark.transform.position;

        Vector3 mid = (scale1Pos + scale2Pos) * 0.5f;

        transform.position = new Vector3(mid.x, transform.position.y, mid.z);

        Vector3 subVec = scale1Pos - scale2Pos;
        mesh.localScale = new Vector3(mesh.localScale.x, mesh.localScale.y, subVec.magnitude);

        Scale1Mark.transform.position = scale1Pos;
        Scale2Mark.transform.position = scale2Pos;
    }

    public void Select(CustomizingStage stage)
    {
        mat.SetFloat("_IndicateColor", 1.0f);

        MoveXMark.Active(false);
        MoveYMark.Active(false);
        MoveBothMark.Active(false);
        Scale1Mark.Active(false);
        Scale2Mark.Active(false);
        RotateMark.Active(false);

        switch (stage)
        {
            case CustomizingStage.None:
            case CustomizingStage.Spawn:
                mat.SetFloat("_IndicateColor", 0.0f);
                break;
            case CustomizingStage.Mode_Move:
                MoveXMark.Active(true);
                MoveYMark.Active(true);
                MoveBothMark.Active(true);
                break;
            case CustomizingStage.Mode_Rotate:
                RotateMark.Active(true);
                break;
            case CustomizingStage.Mode_Scale:
                Scale1Mark.Active(true);
                Scale2Mark.Active(true);
                break;
        }
    }
}
