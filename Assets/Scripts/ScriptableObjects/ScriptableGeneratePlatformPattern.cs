using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlatformGeneratorPattern", menuName = "ScriptableObjects/PlatformGeneratorPattern", order = 1)]
public class ScriptableGeneratePlatformPattern : ScriptableObject
{
    public PlatformType[] spawnQueue;


}

