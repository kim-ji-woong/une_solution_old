using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Resources.Scripts;
public class AndroidPoiBehaviour : MonoBehaviour {

   
    bool isSensorActivated = false;   
    
    private SpriteRenderer thisrenderer = null;
    
    public bool IsSensorActivated
    {
        get
        {
            return isSensorActivated;
        }

        set
        {
            isSensorActivated = value;

            if (!isSensorActivated)
            {
                //reset color
                thisrenderer.color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
    }
    
    // Use this for initialization
    void Start()
    {
        thisrenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    float currentColorPosition = 1.0f;

    //double timeElapsed = 0.0f;

    float colorPositionStep = -0.1f;

    void FixedUpdate()
    {
        if (IsSensorActivated)
        {
            currentColorPosition += colorPositionStep;

            if (Mathf.Abs(currentColorPosition) >= 1.0f)
            {
                colorPositionStep = colorPositionStep * -1.0f;
            }

            Color currentColor = new Color(1.0f, currentColorPosition, currentColorPosition);

            thisrenderer.color = currentColor;
        }
    }

   
   

}
