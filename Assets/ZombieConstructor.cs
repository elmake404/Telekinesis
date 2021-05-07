using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieConstructor : MonoBehaviour
{
    public GameObject headsContainer;
    public GameObject bodyContainerl;
    public GameObject weaponContainerl;

    public GameObject GetLinkToRandomHead()
    {
        int numOfChildrens = headsContainer.transform.childCount;
        int randomIndex = Random.Range(0, numOfChildrens);
        return headsContainer.transform.GetChild(randomIndex).gameObject;
    }

    public GameObject GetLinkToRandomBody()
    {
        int numOfChildrens = bodyContainerl.transform.childCount;
        int randomIndex = Random.Range(0, numOfChildrens);
        return bodyContainerl.transform.GetChild(randomIndex).gameObject;
    }

    public GameObject GetLinkToRandomWeapon()
    {
        int numOfChildrens = weaponContainerl.transform.childCount;
        int randomIndex = Random.Range(0, numOfChildrens);
        return weaponContainerl.transform.GetChild(randomIndex).gameObject;
    }
}
