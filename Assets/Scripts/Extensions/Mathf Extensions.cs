using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfExtensions
{
    public static float ClampAngle(this float angle, float min, float max)
    {
        // method 2

        angle = Mathf.Repeat(angle, 360);
        min = Mathf.Repeat(min, 360);
        max = Mathf.Repeat(max, 360);
        bool inverse = false;
        var tmin = min;
        var tangle = angle;
        if (min > 180)
        {
            inverse = !inverse;
            tmin -= 180;
        }
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        var result = !inverse ? tangle > tmin : tangle < tmin;
        if (!result)
            angle = min;

        inverse = false;
        tangle = angle;
        var tmax = max;
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        if (max > 180)
        {
            inverse = !inverse;
            tmax -= 180;
        }

        result = !inverse ? tangle < tmax : tangle > tmax;
        if (!result)
            angle = max;
        return angle;

        // method 1

        //if (angle > 180) angle = 360 - angle;
        //Debug.Log(angle + " | " + min + " | " + max);
        //angle = Mathf.Clamp(angle, min, max);
        //Debug.Log(angle + " | " + min + " | " + max);
        //if (angle < 0) angle = 360 + angle;
        //Debug.Log(angle + " | " + min + " | " + max);

        //Debug.Log("--------------------------------");

        // return angle;
    }
}
