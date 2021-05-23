using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;

    void Start()
    {
        Transform[] bones = skinnedMesh.bones;
    }

}
