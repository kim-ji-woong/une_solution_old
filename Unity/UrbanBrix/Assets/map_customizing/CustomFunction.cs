using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFunction
{
    public static bool IsNumber(string str)
    {
        foreach (var item in str)
        {
            if (item < 28 || item > 57)
            {
                return false;
            }
        }

        return true;
    }
}
