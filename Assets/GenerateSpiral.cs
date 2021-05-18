using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSpiral
{

    public static Vector2[] GetSpiralPoints(int numPoints, Vector2 rectSize)
    {
        float rotateSlider = numPoints;
        float screenWifth = rectSize.magnitude;
        int a = 5;
        int b = 0;
        b = Mathf.RoundToInt((screenWifth - a) / 100 / 0.2f / 2);
        float angle = 0f;
        Vector2[] points = new Vector2[numPoints];

        float zRotate = Mathf.Lerp(0, 1000, (100- rotateSlider)/100);

        for (int i = 0; i < numPoints; i++)
        {
            
            angle = 0.2f  *i;
            Vector2 spawnPos = Vector2.zero;

            spawnPos.x = (a + b * angle) * Mathf.Cos(angle);
            spawnPos.y = (a + b * angle) * Mathf.Sin(angle);

            spawnPos = Quaternion.Euler(0f,0f,zRotate) * spawnPos;
            points[i] = spawnPos;
        }

        return points;
    }
}
