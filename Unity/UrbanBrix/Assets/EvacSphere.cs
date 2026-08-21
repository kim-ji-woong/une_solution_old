using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;


public class EvacSphere : MonoBehaviour
{
    public bool m_Visible = false;


    public int m_Raious = 1;
     
    public void SetRaious(int nRadius)
    {
        m_Raious = nRadius;

        float y = gameObject.transform.localScale.y;
        gameObject.transform.localScale = new Vector3(nRadius, y, nRadius);
    }


    public int m_nID = 1;

    void Awake()
    {
        MeshCollider colldier = gameObject.GetComponent<MeshCollider>();

        
         
       
        if( colldier != null)
        {
            Destroy(colldier);            
        }
    }
    
    void Start()
    {
    }

    void Update()
    {
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            if (m_Visible == true)
            {
                mr.enabled = true;
            }
            else
            {
                mr.enabled = false;
            }
        }
    }
    
}
