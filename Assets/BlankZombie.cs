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

    public void DestroyConfigurableJointsOnBodyPart(ZombieBodyPartID partID)
    {
        Transform rootBone = partAndPutInObj[(int)partID].zombieBodyControls[0].transform;
        List<ConfigurableJoint> configurableJoints = new List<ConfigurableJoint>(rootBone.GetComponentsInChildren<ConfigurableJoint>());
        configurableJoints.Add(rootBone.GetComponent<ConfigurableJoint>());

        for (int i = 0; i < configurableJoints.Count; i++)
        {
            Destroy(configurableJoints[i]);
        }
    }

    public int[] GetBonesIndexesFromHirachly(BodyPartAndPutInObj part)
    {
        Transform[] transforms = bones;
        int[] instanceID = new int[part.zombieBodyControls.Length];
        int[] indexes = new int[part.zombieBodyControls.Length];

        for (int i = 0; i < instanceID.Length; i++)
        {
            instanceID[i] = part.zombieBodyControls[i].transform.GetInstanceID();
        }

        for (int i = 0; i < transforms.Length; i++)
        {
            for (int k = 0; k < instanceID.Length; k++)
            {
                if (transforms[i].GetInstanceID() == instanceID[k])
                {
                    indexes[k] = i;
                }
            }
        }
        return indexes;
    }

    public bool PartIsAttachedToRope(BodyPartAndPutInObj part)
    {
        bool isAttached = false;
        for (int i = 0; i < part.zombieBodyControls.Length; i++)
        {
            if (part.zombieBodyControls[i].isAttachedToRope == true)
            {
                isAttached = true;
                break;
            }
        }

        return isAttached;
    }

    public List<List<ConnectedRope>> GetConnectedsRopesFromBodyPart(BodyPartAndPutInObj part)
    {
        List<List<ConnectedRope>> connectedRopes = new List<List<ConnectedRope>>();
        for (int i = 0; i < part.zombieBodyControls.Length; i++)
        {
            connectedRopes.Add(part.zombieBodyControls[i].GetConnectedRopes());
        }
        return connectedRopes;
    }

    public Transform[] GetBonesFromHirachlyRootBone(Transform rootBone, int[] ids)
    {
        Transform[] transforms = rootBone.GetComponentsInChildren<Transform>();
        Transform[] bones = new Transform[ids.Length];

        for (int i = 0; i < ids.Length; i++)
        {
            bones[i] = transforms[ids[i]];
        }

        return bones;
    }
}