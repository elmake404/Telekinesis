using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuglasKeper
{
    public static List<Vector2> GetSimpleSpline(List<Vector2> points, float eplision)
    {
        List<Vector2> resultList = new List<Vector2>();
        bool[] keepPoints = new bool[points.Count];
        for (int i = 0; i < keepPoints.Length; i++)
        {
            keepPoints[i] = true;
        }

        Stack<StartEndIndexes> indexesStack = new Stack<StartEndIndexes>();
        StartEndIndexes initIndexes = new StartEndIndexes(0, points.Count - 1);
        indexesStack.Push(initIndexes);

        while (indexesStack.Count != 0)
        {
            StartEndIndexes currentIndexes = indexesStack.Pop();
            int startIndex = currentIndexes.startIndex;
            int endIndex = currentIndexes.endIndex;

            float dMax = 0f;
            int index = startIndex;

            FarthestPoint farthestPoint = GetFarthestPoint(points, startIndex, endIndex, eplision);
            //Debug.Log("Calculate farthest point " + farthestPoint.maxDistance);
            dMax = farthestPoint.maxDistance;
            index = farthestPoint.pointIndex;

            if (dMax >= eplision)
            {
                StartEndIndexes firstPartIndexes = new StartEndIndexes(startIndex, index);
                indexesStack.Push(firstPartIndexes);
                StartEndIndexes secondPartIndexes = new StartEndIndexes(index, endIndex);
                indexesStack.Push(secondPartIndexes);
                //Debug.Log("Push New Indexes");
            }
            else
            {
                for (int i = startIndex + 1; i <= endIndex - 1; i++)
                {
                    keepPoints[i] = false;
                }
            }
        }

        for (int i = 0; i < points.Count; i++)
        {
            if (keepPoints[i])
            {
                resultList.Add(points[i]);
            }
        }

        return resultList;
    }


    public static FarthestPoint GetFarthestPoint(List<Vector2> points, int startIndex, int endIndex, float eplision)
    {
        
        float maxDistance = 0f;
        int index = -1;

        //Debug.Log(startIndex + " " + " " + endIndex);

        Vector2 firstPoint = points[startIndex];
        Vector2 endPoint = points[endIndex];
        Debug.DrawLine(firstPoint, endPoint, Color.blue, Mathf.Infinity);
        Vector2 normal = (endPoint - firstPoint).normalized;
        normal = new Vector2(normal.y, -normal.x);
        //Debug.DrawLine(firstPoint, firstPoint + 500f * normal, Color.blue, Mathf.Infinity);

        for (int i = startIndex + 1; i <= endIndex - 1; i++)
        {
            //Debug.Log("Calculated distance ");
            Vector2 pointInLine = GetPointLineIntersection(firstPoint, endPoint, points[i], normal);
            float distance = Vector2.Distance(points[i], pointInLine);

            

            if (distance > maxDistance)
            {
                maxDistance = distance;
                index = i;
            }
            Debug.DrawLine(points[i], pointInLine, Color.green, Mathf.Infinity);
        }
        return new FarthestPoint(maxDistance, index);
    }

    public static Vector2 GetPointLineIntersection(Vector2 startLine, Vector2 endLine, Vector2 pointAtRay, Vector2 rayDirection)
    {
        float x1 = startLine.x;
        float y1 = startLine.y;
        float x2 = endLine.x;
        float y2 = endLine.y;

        float x3 = pointAtRay.x;
        float y3 = pointAtRay.y;
        float x4 = (pointAtRay + rayDirection).x;
        float y4 = (pointAtRay + rayDirection).y;

        float part1 = (x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4);
        float part2 = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

        float t = part1 / part2;

        float xPoint = x1 + t * (x2 - x1);
        float yPoint = y1 + t * (y2 - y1);

        return new Vector2(xPoint, yPoint);
    }

}


public struct FarthestPoint
{
    public float maxDistance;
    public int pointIndex;

    public FarthestPoint(float maxDistance, int pointIndex)
    {
        this.maxDistance = maxDistance;
        this.pointIndex = pointIndex;
    }
}

public struct StartEndIndexes
{
    public int startIndex;
    public int endIndex;

    public StartEndIndexes(int startIndex, int endIndex)
    {
        this.startIndex = startIndex;
        this.endIndex = endIndex;
    }
}
