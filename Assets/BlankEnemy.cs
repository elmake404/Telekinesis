using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankEnemy : MonoBehaviour
{
    public Transform headContainer;
    public Transform bodyContainer;
    public Transform weaponContainer;
    public Transform rootBones;
    public Transform[] bones;

    public Transform[] GetBones()
    {
        return bones;
    }
}
