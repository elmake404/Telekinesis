using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRopeControl : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public CreateRope createRope;

    private void FixedUpdate()
    {
        UpdateDrawRope();
    }

    private void UpdateDrawRope()
    {
        List<RopeSection> ropeSections = createRope.GetCreatedRopeSections();
        float count = ropeSections.Count;

        List<Vector3> sectionsPos = new List<Vector3>();
        for (int i = 0; i < ropeSections.Count; i++)
        {
            sectionsPos.Add(ropeSections[i].transform.position);
            if (i == 0) { sectionsPos.Add(ropeSections[i].transform.position); }
            if (i == ropeSections.Count - 1) { sectionsPos.Add(ropeSections[i].transform.position); }
        }

        List<Vector3> simpledPoints = new List<Vector3>();
        for (float i = 0.0f; i < sectionsPos.Count - 3.0f; i += 0.10f)
        {
            simpledPoints.Add(CatmulSpline.GetSplinePoint(i,sectionsPos));
        }

        lineRenderer.positionCount = simpledPoints.Count;
        lineRenderer.SetPositions(simpledPoints.ToArray());
    }

    
}
