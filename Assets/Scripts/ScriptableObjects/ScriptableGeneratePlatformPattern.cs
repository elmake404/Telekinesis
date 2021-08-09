using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformID
{
    platform01,
    platform02,
    platform03,
    platform04,
    platform05,
    platform06,
    platform07,
    platform08,
    platform09,
}

[CreateAssetMenu(fileName = "PlatformGeneratorPattern", menuName = "ScriptableObjects/PlatformGeneratorPattern", order = 1)]
public class ScriptableGeneratePlatformPattern : ScriptableObject
{
    public Platform[] spawnQueue;
    

}

[System.Serializable]
public struct Platform
{
    public PlatformType platformType;
    public PlatformID platformID;
}