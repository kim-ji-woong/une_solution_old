using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform follow;

    private RectTransform rTransform;

    // Start is called before the first frame update
    void Start()
    {
        rTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
         

        rTransform.position = Camera.main.WorldToScreenPoint(follow.position);
    }
}
