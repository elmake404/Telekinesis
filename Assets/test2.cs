using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;

    void Start()
    {
        Transform[] bones = skinnedMesh.bones;
        Debug.Log("Root bone: " + skinnedMesh.rootBone.name);
        for (int i = 0; i < bones.Length; i++)
        {
            Debug.Log(i + "   " + bones[i].gameObject.name);
        }
    }


}
