// frustum[0] downLeft
// frustum[1] topLeft
// frustum[2] topRight
// frustum[3] downRight

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FrustumPlaneSide
{
    down,
    top,
    left,
    right
}

public class BoundsCameraUtility
{
    private Camera _renderCamera;
    private Transform _transformCamera;
    private Plane[] _frustumPlanes;
    private LevelBehaviour _levelBehaviour;

    public BoundsCameraUtility(Camera renderCamera, LevelBehaviour levelBehaviour)
    {
        _frustumPlanes = new Plane[4];
        _renderCamera = renderCamera;
        _transformCamera = _renderCamera.transform;
        _levelBehaviour = levelBehaviour;
        UpdateCalculations();
        levelBehaviour.SubscribeUpdaterDelegate(UpdateCalculations);
        
    }

    public void UpdateCalculations()
    {
        Vector3[] frustumCorners = new Vector3[4];
        _renderCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), _renderCamera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        Vector3 downLeftCornerDir = _transformCamera.TransformVector( Vector3.Normalize(frustumCorners[0]));
        Vector3 topLeftCornerDir = _transformCamera.TransformVector( Vector3.Normalize(frustumCorners[1]));
        Vector3 topRighCornerDir = _transformCamera.TransformVector( Vector3.Normalize(frustumCorners[2]));
        Vector3 downRighCornerDir = _transformCamera.TransformVector( Vector3.Normalize(frustumCorners[3]));

        _frustumPlanes[0] = new Plane(_transformCamera.position, _transformCamera.position + downLeftCornerDir, _transformCamera.position + downRighCornerDir);
        _frustumPlanes[1] = new Plane(_transformCamera.position, _transformCamera.position + topRighCornerDir, _transformCamera.position + topLeftCornerDir);
        _frustumPlanes[2] = new Plane(_transformCamera.position, _transformCamera.position + topLeftCornerDir, _transformCamera.position + downLeftCornerDir);
        _frustumPlanes[3] = new Plane(_transformCamera.position, _transformCamera.position + downRighCornerDir, _transformCamera.position + topRighCornerDir);

        for (int i = 0; i < _frustumPlanes.Length; i++)
        {
            Debug.DrawRay(_frustumPlanes[i].ClosestPointOnPlane(Vector3.zero), _frustumPlanes[i].normal, Color.red);

            //Debug.DrawLine(new Vector3(0, 10, 0), _frustumPlanes[i].flipped.normal * _frustumPlanes[i].distance, Color.green, Mathf.Infinity);
        }

    }

    public bool IsPointInsideFrustumCamera(Vector3 point)
    {
        bool result = true;

        for (int i = 0; i < _frustumPlanes.Length; i++)
        {
            if (!_frustumPlanes[i].GetSide(point))
            {
                result = false;
                break;
            }
        }

        return result;
    }

    /*public bool IntersectionFrustumWithBounds(Bounds bounds, Vector3 vector, out float distance)
    {
        
    }*/



    public bool IsPointInsideFrustumCamera(Transform point)
    {
        return IsPointInsideFrustumCamera(point.position);
    }

    public List<IntersectionPointInPlane> GetRayIntersectionsWithFrustum(Ray ray)
    {
        List<IntersectionPointInPlane> intersectionPointInPlanes = new List<IntersectionPointInPlane>();

        for (int i = 0; i < _frustumPlanes.Length; i++)
        {
            float distance = 0.0f; 

            _frustumPlanes[i].Raycast(ray, out distance);
            Vector3 point = ray.origin + ray.direction * distance;

            if(IsPointInsideFrustumCamera(point + _frustumPlanes[i].normal))
            {
                IntersectionPointInPlane intersection = new IntersectionPointInPlane();
                intersection.pointIntersect = point;
                intersection.intersectedFrustumPlaneSide = (FrustumPlaneSide)i;
                intersection.distToOriginal = Vector3.Distance(ray.origin, point);
                intersectionPointInPlanes.Add(intersection);
            }

        }
        return intersectionPointInPlanes;
    }

    public bool IsFrustumCrossTowardsFwdDirBoundsEdges(Bounds bounds)
    {
        bool result = true;
        //Vector3 fwd = Vector3.forward;
        Vector3 boundsCenter = bounds.center;
        float halfBoundsSizeX = bounds.size.x / 2;
        float halfBoundsSizeY = bounds.size.y / 2;
        float halfBoundsSizeZ = bounds.size.z / 2;

        Vector3[] edgePoints = new Vector3[8];
        edgePoints[0] = new Vector3(boundsCenter.x - halfBoundsSizeX, boundsCenter.y - halfBoundsSizeY, boundsCenter.z - halfBoundsSizeZ);  //edge1_point1
        edgePoints[1] = new Vector3(boundsCenter.x - halfBoundsSizeX, boundsCenter.y - halfBoundsSizeY, boundsCenter.z + halfBoundsSizeZ);  //edge1_point2
        edgePoints[2] = new Vector3(boundsCenter.x + halfBoundsSizeX, boundsCenter.y - halfBoundsSizeY, boundsCenter.z - halfBoundsSizeZ);  //...
        edgePoints[3] = new Vector3(boundsCenter.x + halfBoundsSizeX, boundsCenter.y - halfBoundsSizeY, boundsCenter.z + halfBoundsSizeZ);
        edgePoints[4] = new Vector3(boundsCenter.x - halfBoundsSizeX, boundsCenter.y + halfBoundsSizeY, boundsCenter.z - halfBoundsSizeZ);
        edgePoints[5] = new Vector3(boundsCenter.x - halfBoundsSizeX, boundsCenter.y + halfBoundsSizeY, boundsCenter.z + halfBoundsSizeZ);
        edgePoints[6] = new Vector3(boundsCenter.x + halfBoundsSizeX, boundsCenter.y + halfBoundsSizeY, boundsCenter.z - halfBoundsSizeZ);
        edgePoints[7] = new Vector3(boundsCenter.x + halfBoundsSizeX, boundsCenter.y + halfBoundsSizeY, boundsCenter.z + halfBoundsSizeZ);

        /*Vector3[] edgeDirections = new Vector3[4];
        edgeDirections[0] = Vector3.Normalize(edgePoints[1] - edgePoints[0]);
        edgeDirections[1] = Vector3.Normalize(edgePoints[3] - edgePoints[2]);
        edgeDirections[2] = Vector3.Normalize(edgePoints[5] - edgePoints[4]);
        edgeDirections[3] = Vector3.Normalize(edgePoints[7] - edgePoints[6]);*/

        float[] distances = new float[16];

        for (int i = 0; i < _frustumPlanes.Length; i++)
        {

            for (int k = 0; k < 4; k++)
            {
                float dist = 0f;
                _frustumPlanes[i].Raycast(new Ray(edgePoints[i * 2], Vector3.forward), out dist);
                if (dist < 0.0f | Vector3.Dot(_frustumPlanes[i].normal, Vector3.forward) > 0.0f) { continue; }
                distances[i * 4 + k] = dist;
            }
            
        }

        for (int i = 0; i < 16; i++)
        {
            if (distances[i] > bounds.size.z)
            {
                result = false;
                break;
            }
        }

        /*for (int i = 0; i < 16; i++)
        {
            Debug.Log(distances[i]);
            Debug.DrawRay(edgePoints[i % 4 * 2], edgeDirections[i % 4] * distances[i], Color.blue, Mathf.Infinity);
        }*/
        return result;
    }
}

public struct IntersectionPointInPlane
{
    public Vector3 pointIntersect;
    public FrustumPlaneSide intersectedFrustumPlaneSide;
    public float distToOriginal;
}
