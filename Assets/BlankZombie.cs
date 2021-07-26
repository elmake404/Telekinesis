using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlankZombie : MonoBehaviour
{
    public BodyPartAndPutInObj[] partAndPutInObj;
    public Transform[] bones;
    private bool[] partsIsBroken = new bool[6];
    private int[] bonesID;
    private Coroutine ReductionBreakForce;

    private void Start()
    {
        InitBonesID();
    }

    private void InitBonesID()
    {
        bonesID = new int[bones.Length];

        for (int i = 0; i < bonesID.Length; i++)
        {
            bonesID[i] = bones[i].GetInstanceID();
        }
    }

    public void StartDelayReductionBreakForce()
    {
        ReductionBreakForce = StartCoroutine(DelayReductionBreakForce());
    }

    public ZombieBodyPartID[] GetAllNotYetDeatroyedPartIDs()
    {
        List<ZombieBodyPartID> notDstroyed = new List<ZombieBodyPartID>();
        for (int i = 0; i < partsIsBroken.Length; i++)
        {
            if (partsIsBroken[i] == true)
            {
                continue;
            }
            else
            {
                notDstroyed.Add((ZombieBodyPartID)i);
            }
        }
        return notDstroyed.ToArray();
    }

    private IEnumerator DelayReductionBreakForce()
    {
        for (float p = 0f; p < 15f; p += 0.5f)
        {
            for (int i = 0; i < partAndPutInObj.Length; i++)
            {
                if (partsIsBroken[i] == true)
                {
                    continue;
                }
                else
                {
                    for (int z = 0; z < partAndPutInObj[i].zombieBodyControls.Length; z++)
                    {
                        if (partAndPutInObj[i].zombieBodyControls[z].isBreakingJoint == true)
                        {
                            CharacterJoint characterJoint = partAndPutInObj[i].zombieBodyControls[z].GetAttachedCahracterJoint();
                            characterJoint.breakForce = Mathf.Lerp(1000f, 300f, p / 15f);
                        }
                    }
                    
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    

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
        Transform instanceBonesHolder = Instantiate(bones[0], transform);
        
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
        //tunedBones[0].position = bones[0].position;
        tunedBones[0].SetParent(null);
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
            bodyControls[i].gameObject.layer = 0;
            Destroy(bodyControls[i]);
        }
    }

    public void MakeIgnoreCollisionsWithOtherColliders(Collider[] otherColliders)
    {
        for (int i = 0; i < partAndPutInObj.Length; i++)
        {
            if (partsIsBroken[i] == true)
            {
                continue;
            }
            else
            {
                ZombieBodyControl[] zombieBodyControls = partAndPutInObj[i].zombieBodyControls;
                for (int k = 0; k < zombieBodyControls.Length; k++)
                {
                    for (int p = 0; p < otherColliders.Length; p++)
                    {
                        Physics.IgnoreCollision(zombieBodyControls[k].attachedCollider, otherColliders[p]);
                    }
                }
            }
        }
    }

    public Collider[] DestroyUnusualCollidersAndReturnUsed(ZombieBodyPartID usingPartID, Transform[] instancedBones)
    {
        List<Collider> usingCollider = new List<Collider>();

        for (int i = 0; i < partAndPutInObj.Length; i++)
        {
            if (i == (int)usingPartID)
            {
                int[] indexes = GetBonesIndexesFromHirachly(partAndPutInObj[i]);

                for (int k = 0; k < indexes.Length; k++)
                {
                    usingCollider.Add(instancedBones[indexes[k]].GetComponent<Collider>());
                }
            }
            else
            {
                int[] indexes = GetBonesIndexesFromHirachly(partAndPutInObj[i]);

                for (int k = 0; k < indexes.Length; k++)
                {
                    Destroy(instancedBones[indexes[k]].GetComponent<Collider>());
                }
            }
        }
        return usingCollider.ToArray();
    }

    public void ReBuildHirachlyBones(Transform platformTransform, Transform rootBone, ZombieBodyControl[] zombieBodyControls, ZombieBodyPartID partID, out Transform beginJoint)
    {
        beginJoint = rootBone;
        for (int i = 0; i < zombieBodyControls.Length; i++)
        {
            if (zombieBodyControls[i].zombieBodyPartID == partID && zombieBodyControls[i].isBreakingJoint)
            {
                beginJoint = zombieBodyControls[i].transform;
                beginJoint.SetParent(platformTransform);
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
        int[] instanceID = new int[part.zombieBodyControls.Length];
        int[] indexes = new int[part.zombieBodyControls.Length];

        for (int i = 0; i < instanceID.Length; i++)
        {
            instanceID[i] = part.zombieBodyControls[i].transform.GetInstanceID();
        }

        for (int i = 0; i < bonesID.Length; i++)
        {
            for (int k = 0; k < instanceID.Length; k++)
            {
                if (bonesID[i] == instanceID[k])
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

    public void AddExpectedObjectToAllBody(int id)
    {
        for (int i = 0; i < partAndPutInObj.Length; i++)
        {
            for (int k = 0; k < partAndPutInObj[i].zombieBodyControls.Length; k++)
            {
                partAndPutInObj[i].zombieBodyControls[k].expectedCollisionObjectID = id;
            }
        }
    }
    
}