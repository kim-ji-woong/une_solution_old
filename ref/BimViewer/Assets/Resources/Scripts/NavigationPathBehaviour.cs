using System.Collections;
using System.Collections.Generic;
using VolumetricLines;
using UnityEngine;

public class NavigationPathBehaviour : MonoBehaviour {
    [SerializeField]
    VolumetricMultiLineBehavior lineBehaviour = null;

    List<Vector3> originalVertices = new List<Vector3>();
    float elapsedTime = 0;

    int currentIndex = 0;

    bool isLineAnimation = false;

    bool isJustAnimationStarted = false;

    float speed = 3;

    public bool IsLineAnimation
    {
        get
        {
            return isLineAnimation;
        }
    }

    // Use this for initialization
    void Start () {
		
	}

    void FixedUpdate()
    {
        if(isJustAnimationStarted)
        {
            lineBehaviour.LineWidth = 5.0f;
            isJustAnimationStarted = false;
        }

        if(isLineAnimation)
        {
            Vector3 start = originalVertices[currentIndex];
            Vector3 end = originalVertices[currentIndex + 1];

            Vector3 lastPos = Vector3.Lerp(start, end, elapsedTime);

            //recreate line

            elapsedTime += Time.deltaTime * speed;

            Vector3[] newVertices = new Vector3[currentIndex + 2];

            for(int i=0;i<currentIndex+1;i++)
            {
                newVertices[i] = originalVertices[i];
            }

            newVertices[currentIndex + 1] = lastPos;

            lineBehaviour.UpdateLineVertices(newVertices);

            if ((lastPos - end).magnitude < 0.01f)
            {
                currentIndex++;
                elapsedTime = 0.0f;

                if (currentIndex == originalVertices.Count-1)
                {
                    isLineAnimation = false;
                    originalVertices.Clear();
                    currentIndex = 0;                    
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
		if(null != lineBehaviour)
        {

        }
	}

    public void StartAnimation()
    {
        if (lineBehaviour.LineVertices.Length < 2)
            return;

        
        //get vertices
        foreach (Vector3 v in lineBehaviour.LineVertices)
        {
            originalVertices.Add(new Vector3(v.x,v.y,v.z));  //deep copy
        }
        
        isLineAnimation = true;
        isJustAnimationStarted = true;
    }

    public void TerminateAnimation()
    {
        isLineAnimation = false;

        lineBehaviour.UpdateLineVertices(originalVertices.ToArray());

        originalVertices.Clear();
        currentIndex = 0;
    }
}
