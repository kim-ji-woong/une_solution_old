using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapController
{
    private static List<GameObject> checkObjs;

    private static float snapLength = 0;

    public static void Init(List<GameObject> objs, float length)
    {
        checkObjs = objs;
        snapLength = length;
    }

    public static float XPos(Vector3 movingPos, SnapObj curMovingObj)
    {
        float[] tempDist = new float[4];
        float[] tempSnapWX = new float[4];

        float closestDist = float.MaxValue;
        float closestWX = float.MaxValue;
        foreach (GameObject obj in checkObjs)
        {
            SnapObj snapObj = obj.GetComponent<SnapObj>();

            if (curMovingObj == snapObj)
                continue;

            tempDist[0] = snapObj.X1 - curMovingObj.X1;
            tempSnapWX[0] = snapObj.X1;
            tempDist[1]= snapObj.X2 - curMovingObj.X1;
            tempSnapWX[1] = snapObj.X2;
            tempDist[2]= snapObj.X1 - curMovingObj.X2;
            tempSnapWX[2] = snapObj.X1;
            tempDist[3]= snapObj.X2 - curMovingObj.X2;
            tempSnapWX[3] = snapObj.X2;

            float closestDistIn = float.MaxValue;
            float closestWXIn = float.MaxValue;
            for (int i=0; i < 4; ++i)
            {
                if(closestDistIn> tempDist[i])
                {
                    closestDistIn = tempDist[i];
                    closestWXIn = tempSnapWX[i];
                }
            }

            if(closestDist > closestDistIn)
            {
                closestDist = closestDistIn;
                closestWX = closestWXIn;
            }
        }

        if(closestDist <= snapLength)
        {
            //if (movingPos.x > closestWX)
            //    return closestWX + curMovingObj.Extent.x;
            //else
            //    return closestWX - curMovingObj.Extent.y;
        }

        Debug.Log(closestDist);

        return movingPos.x;
    }
}
