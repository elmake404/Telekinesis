using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianConstructor : MonoBehaviour
{
    public GameObject headsContainer;
    public GameObject bodiesContainer;

    public GameObject GetRandomHead()
    {
        int numOfContains = headsContainer.transform.childCount;
        int randomIndex = Random.Range(0, numOfContains);
        return headsContainer.transform.GetChild(randomIndex).gameObject;
    }

    public GameObject GetRandomBody()
    {
        int numOfContains = bodiesContainer.transform.childCount;
        int randomIndex = Random.Range(0, numOfContains);
        return bodiesContainer.transform.GetChild(randomIndex).gameObject;
    }
}
