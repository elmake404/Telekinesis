using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformType
{
    standartPlatformType,
    safePlatformType,
    platform_003
}

public class PlatformsGenerator
{
    private LinkedList<PlatformBehaviour> _listPlatforms;
    private SerializeStructPlatforms _serializeStructPlatforms;
    private ScriptableGeneratePlatformPattern _generatePattern;
    private LevelBehaviour _levelBehaviour;
    private Vector3 _startGeneratePoint = Vector3.zero;
    private Vector3 _vectorTest = new Vector3(0, 0, -10);
    public int NumOfSerializedPlatforms { get { return _serializeStructPlatforms.platforms.Count; } private set { } }
    public int NumOfActivePlatforms { get { return _listPlatforms.Count; } private set { } }
    public int NumOfIterationPattern { get { return _generatePattern.spawnQueue.Length; } private set { } }

    private delegate void DelegateMethodsQueue();
    private event DelegateMethodsQueue OnDelegateMethodsQueue;

    public delegate void DelegateCreateNewPlatform(PlatformBehaviour platformBehaviour);
    public event DelegateCreateNewPlatform OnCreateNewPlatform;

    public delegate void DelegateCreateLastPlatform(PlatformBehaviour platformBehaviour);
    public event DelegateCreateLastPlatform OnCreateLastPlatform;

    public PlatformsGenerator(SerializeStructPlatforms serializeStructPlatforms, LevelBehaviour levelBehaviour, DelegateCreateNewPlatform delegateCreateNewPlatform, DelegateCreateLastPlatform delegateCreateLastPlatform)
    {
        _serializeStructPlatforms = serializeStructPlatforms;
        _generatePattern = serializeStructPlatforms.generatePlatformPattern;
        _listPlatforms = new LinkedList<PlatformBehaviour>();
        _levelBehaviour = levelBehaviour;

        OnDelegateMethodsQueue += CheckTailPlatformInFrustumArea;
        SubscribeCreateNewPlatform(delegateCreateNewPlatform);
        SubscribeCreateLastPlatform(delegateCreateLastPlatform);
        _levelBehaviour.SubscribeUpdaterDelegate(Recalculate);
        InitCreatureOfPlatforms();
    }

    public void SubscribeCreateNewPlatform(DelegateCreateNewPlatform delegateCreateNewPlatform)
    {
        OnCreateNewPlatform += delegateCreateNewPlatform;
    }
    public void UnSubscribeCreateNewPlatform(DelegateCreateNewPlatform delegateCreateNewPlatform)
    {
        OnCreateNewPlatform -= delegateCreateNewPlatform;
    }

    public void SubscribeCreateLastPlatform(DelegateCreateLastPlatform delegateCreateLastPlatform)
    {
        OnCreateLastPlatform += delegateCreateLastPlatform;
    }

    public void UnSubscribeCreateLastPlatform(DelegateCreateLastPlatform delegateCreateLastPlatform)
    {
        OnCreateLastPlatform -= delegateCreateLastPlatform;
    }

    private void InitCreatureOfPlatforms()

    {
        Ray ray = new Ray(Vector3.forward, _vectorTest);
        List<IntersectionPointInPlane> intersectionPointInPlanes = _levelBehaviour.boundsCameraUtility.GetRayIntersectionsWithFrustum(ray);

        if (intersectionPointInPlanes.Count < 2) { Debug.LogError("Camera in wrong place" + " " + intersectionPointInPlanes.Count); }

        int farthestIndexPoint = intersectionPointInPlanes[0].distToOriginal > intersectionPointInPlanes[1].distToOriginal ? 0 : 1;
        Vector3 dirAtFarthestToStartPoint = Vector3.Normalize(_startGeneratePoint - intersectionPointInPlanes[farthestIndexPoint].pointIntersect);
        Vector3 farthestPoint = intersectionPointInPlanes[farthestIndexPoint].pointIntersect;
        Vector3 spawnOffset = _startGeneratePoint;
        
        float maxDistanceAtStart = Vector3.Distance(_startGeneratePoint, farthestPoint);
        float currentLengthQueue = 0f;

        while (currentLengthQueue < maxDistanceAtStart)
        {
            PlatformBehaviour platformBehaviour = ReleaseSelectedPattern(_listPlatforms.Count);
            platformBehaviour.transform.position = spawnOffset;
            Bounds bounds = platformBehaviour.RendererStand.bounds;
            if (bounds.Contains(farthestPoint)) { break; }
            float distance = 0f;
            bounds.IntersectRay(new Ray(farthestPoint, dirAtFarthestToStartPoint), out distance);
            Vector3 intersectPoint = farthestPoint + dirAtFarthestToStartPoint * distance;
            spawnOffset = new Vector3(intersectPoint.x, intersectPoint.y, intersectPoint.z + (bounds.size.z / 2));
            currentLengthQueue = Vector3.Distance(_startGeneratePoint, intersectPoint);
            if (_listPlatforms.Count >= _serializeStructPlatforms.generatePlatformPattern.spawnQueue.Length) { break; }
            //Debug.Log("CurrentLength " + currentLengthQueue + "  MaxDistanceAtStart " + maxDistanceAtStart);
        }

    }

    private void Recalculate()
    {
        OnDelegateMethodsQueue?.Invoke();
    }

    private void CheckTailPlatformInFrustumArea()
    {
        bool isTimeToGenerateNextPlatform = _levelBehaviour.boundsCameraUtility.IsFrustumCrossTowardsFwdDirBoundsEdges(_listPlatforms.tail.platformType.RendererStand.bounds);
        if (_listPlatforms.Count >= _generatePattern.spawnQueue.Length) 
        {
            OnDelegateMethodsQueue -= CheckTailPlatformInFrustumArea;
            //Debug.Log(OnDelegateMethodsQueue);
        }

        if (!isTimeToGenerateNextPlatform)
        {
            float zOffset = _listPlatforms.tail.platformType.RendererStand.bounds.size.z;
            Vector3 spawnPos = _listPlatforms.tail.platformType.transform.position;
            spawnPos.z += zOffset;
            
            ReleaseSelectedPattern(_listPlatforms.Count, spawnPos);
            
        }
    }
    
    private PlatformBehaviour ReleaseSelectedPattern(int indexPattern)
    {
        
        PlatformType selectedPattern = _generatePattern.spawnQueue[indexPattern];
        GameObject instance = GameObject.Instantiate(_serializeStructPlatforms.GetSerializePlatformUnit(selectedPattern).gameObject);
        PlatformBehaviour behaviour = instance.GetComponent<PlatformBehaviour>();
        behaviour.InitionalAssign(_levelBehaviour);
        _listPlatforms.AddNode(behaviour);
        OnCreateNewPlatform?.Invoke(behaviour);

        if (_generatePattern.spawnQueue.Length == indexPattern + 1)
        {
            OnCreateLastPlatform?.Invoke(behaviour);
        }
        return behaviour;
    }

    private void ReleaseSelectedPattern(int indexPattern, Vector3 spawnPoint)
    {
        ReleaseSelectedPattern(indexPattern).transform.position = spawnPoint;
    }
}


[System.Serializable]
public struct SerializeStructPlatforms
{
    public List<SerializePlatformUnit> platforms;
    public ScriptableGeneratePlatformPattern generatePlatformPattern;


    public bool IsFindTypeAvaileble(PlatformType type)
    {
        bool result = false;
        for (int i = 0; i < platforms.Count; i++)
        {
            if (platforms[i].platformType == type)
            {
                result = true;
                break;
            }
        }
        return result;
    }
    public SerializePlatformUnit GetSerializePlatformUnit(PlatformType type)
    {
        SerializePlatformUnit platformUnit = new SerializePlatformUnit();

        for (int i = 0; i < platforms.Count; i++)
        {
            if (platforms[i].platformType == type)
            {
                platformUnit = platforms[i];
                break;
            }
        }
        return platformUnit;
    }
}

[System.Serializable]
public struct SerializePlatformUnit
{
    public GameObject gameObject;
    public PlatformBehaviour platformBehaviour;
    public PlatformType platformType;
}