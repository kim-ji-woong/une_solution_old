using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 자식에 있는 오브젝트들을 결합해서 하나의 메쉬를 생성해서 가진다 (최적화/형태 커스터마이징)
 * 
 * Transform 종류 정보만 가지고 있다 (public)
 */

public class Mark : MonoBehaviour
{
    public Shader markShader;
    public Color color;
    public TransformKind kind;

    private Material mat;

    void Awake()
    {
        mat = new Material(markShader);
        mat.SetColor("_Color", color);

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();

        for(int i=0; i<meshFilters.Length; ++i)
        {
            mrs[i].material = mat;
        }
    }

    public void Active(bool b)
    {
        gameObject.SetActive(b);
    }

    public void Select(bool b)
    {
        if (b)
        {
            mat.SetColor("_Color", new Color(1, 1, 0));
        }
        else
        {
            mat.SetColor("_Color", color);
        }
    }
}
