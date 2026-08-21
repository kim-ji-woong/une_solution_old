using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    public Transform follow;

    private RectTransform rTransform;

    // 거리에 따른 UI크기
    private const float MAX_DIST = 100;
    private const float MIN_DIST = 10;
    private const float MIN_SIZE = 0.1f;
    private const float MAX_SIZE = 1;

    void Start()
    {
        rTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        float distFromCam = (Camera.main.transform.position - follow.position).magnitude;
        float distT = (distFromCam - MIN_DIST) / (MAX_DIST - MIN_DIST);

        float t = MAX_SIZE + distT * (MIN_SIZE - MAX_SIZE);

        rTransform.localScale = new Vector3(t, t, t);
        rTransform.position = Camera.main.WorldToScreenPoint(follow.position);
    }
}
