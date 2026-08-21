using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookatCam : MonoBehaviour {
    public Transform mTarget;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(mTarget);
	}
}
