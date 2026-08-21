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
    //public Mark RotateMark;
    public Mark Rotate1Mark;
    public Mark Rotate2Mark;
    public Transform mesh;

    public Material mat;

    public void Start()
    {
        mat = mesh.GetComponent<MeshRenderer>().material;

        MoveXMark.Active(false);
        MoveYMark.Active(false);
        MoveBothMark.Active(false);
        Scale1Mark.Active(false);
        Scale2Mark.Active(false);
        //RotateMark.Active(false);
        Rotate1Mark.Active(false);
        Rotate2Mark.Active(false);
    }
    
    public float Scale
    {
        get
        {
            return mesh.localScale.z;
        }
        set
        {
            
            mesh.localScale = new Vector3(mesh.localScale.x, mesh.localScale.y, value);
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
        set
        {
            mesh.transform.right = value;
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
        /*
        Vector3 scale1Pos = Scale1Mark.transform.position;
        Vector3 scale2Pos = Scale2Mark.transform.position;

        Vector3 mid = (scale1Pos + scale2Pos) * 0.5f;

        transform.position = new Vector3(mid.x, transform.position.y, mid.z);

        Vector3 subVec = scale1Pos - scale2Pos;
        mesh.localScale = new Vector3(mesh.localScale.x, mesh.localScale.y, subVec.magnitude);

        Scale1Mark.transform.position = scale1Pos;
        Scale2Mark.transform.position = scale2Pos;

        Rotate1Mark.transform.position = scale1Pos;
        Rotate2Mark.transform.position = scale2Pos;
        */

        Vector3 leftPT = GetWallPoint(true);
        Vector3 rightPT = GetWallPoint(false);
        Scale1Mark.transform.position = new Vector3(leftPT.x, Scale1Mark.transform.position.y, leftPT.z);
        Scale2Mark.transform.position = new Vector3(rightPT.x, Scale2Mark.transform.position.y, rightPT.z);
        Rotate1Mark.transform.position = new Vector3(leftPT.x, Rotate1Mark.transform.position.y, leftPT.z);
        Rotate2Mark.transform.position = new Vector3(rightPT.x, Rotate2Mark.transform.position.y, rightPT.z);
    }

    /// <summary>
    /// 중심축 좌표 가져오기
    /// </summary>
    /// <param name="referenceAxis">true면 왼쪽</param>
    /// <returns>중심축 좌표</returns>
    public Vector3 GetWallPoint(bool referenceAxis)
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

    public Vector3 GetWallPoint(bool referenceAxis, Vector3 vector)
    {
        Vector3 center = vector;
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

    public void Select(CustomizingStage stage)
    {
        mat.SetFloat("_IndicateColor", 1.0f);

        MoveXMark.Active(false);
        MoveYMark.Active(false);
        MoveBothMark.Active(false);
        Scale1Mark.Active(false);
        Scale2Mark.Active(false);
        //RotateMark.Active(false);
        Rotate1Mark.Active(false);
        Rotate2Mark.Active(false);

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
                //RotateMark.Active(true);
                Rotate1Mark.Active(true);
                Rotate2Mark.Active(true);
                break;
            case CustomizingStage.Mode_Scale:
                Scale1Mark.Active(true);
                Scale2Mark.Active(true);
                break;
        }
    }
}
