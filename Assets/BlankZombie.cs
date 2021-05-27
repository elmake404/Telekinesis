using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankZombie : MonoBehaviour
{
    public BodyPartAndPutInObj[] partAndPutInObj;
    public Transform[] bones;
    private bool[] partsIsBroken = new bool[6];

    public void SetPartIsBroken(ZombieBodyPartID partID)
    {
        partsIsBroken[(int)partID] = true;
    }

    public bool PartIsBroken(ZombieBodyPartID partID)
    {
        return partsIsBroken[(int)partID];
    }

    public BodyPartAndPutInObj GetPartAndPutInObj(ZombieBodyPartID partID)
    {
        return partAndPutInObj[(int)partID];
    }

    public Transform[] GetInstantiatedBones()
    {
        Transform instanceBonesHolder = Instantiate(bones[0]);
        Transform[] bonesInBonesHolder = instanceBonesHolder.GetComponentsInChildren<Transform>();
        Transform[] tunedBones = new Transform[bones.Length];

        tunedBones[0] = instanceBonesHolder;
        tunedBones[0].position = bones[0].position;
        tunedBones[0].rotation = bones[0].rotation;

        for (int i = 1; i < tunedBones.Length; i++)
        {
            tunedBones[i] = bonesInBonesHolder[i];
            tunedBones[i].position = bones[i].position;
            tunedBones[i].rotation = bones[i].rotation;
        }
        return tunedBones;
    }

    public ZombieBodyControl[] GetZombieBodyControls(Transform[] bones)
    {
        List<ZombieBodyControl> bodyControls = new List<ZombieBodyControl>(bones[0].GetComponentsInChildren<ZombieBodyControl>());
        bodyControls.Add(bones[0].GetComponent<ZombieBodyControl>());
        return bodyControls.ToArray();
    }

    public void PrepareBonesForSlicedPart(ZombieBodyControl[] bodyControls, ZombieBodyPartID partID)
    {
        for (int i = 0; i < bodyControls.Length; i++)
        {
            if (bodyControls[i].zombieBodyPartID != partID)
            {
                bodyControls[i].DestroyAttachedCharacterJoint();
                bodyControls[i].DestroyAttachedCollider();
                bodyControls[i].DestroyAttachedRigidbody();
            }

            else
            {
                if (bodyControls[i].isBreakingJoint == true)
                {
                    bodyControls[i].DestroyAttachedCharacterJoint();
                }
            }
        }
    }

    public void ReBuildHirachlyBones(Transform rootBone, ZombieBodyControl[] zombieBodyControls, ZombieBodyPartID partID)
    {
        Transform beginJoint;
        for (int i = 0; i < zombieBodyControls.Length; i++)
        {
            if (zombieBodyControls[i].zombieBodyPartID == partID && zombieBodyControls[i].isBreakingJoint)
            {
                beginJoint = zombieBodyControls[i].transform;
                beginJoint.SetParent(null);
                rootBone.SetParent(beginJoint);
            }
        }

        

    }

    public GameObject[] GetInstantiatedParts(ZombieBodyPartID partID)
    {
        return partAndPutInObj[(int)partID].usedBodyParts;
    }

    public void DisableBrokenBodyPart(ZombieBodyPartID partID)
    {
        ZombieBodyControl[] bodyControls = partAndPutInObj[(int)partID].zombieBodyControls;

        for (int i = 0; i < bodyControls.Length; i++)
        {
            if (bodyControls[i].isAttachCharacterJoint == true)
            {
                bodyControls[i].DestroyAttachedCharacterJoint();
            }
            
        }

        for (int i = 0; i < bodyControls.Length; i++)
        {
            bodyControls[i].DestroyAttachedRigidbody();
            bodyControls[i].DestroyAttachedCollider();
            //bodyControls[i].enabled = false;
        }
    }

}