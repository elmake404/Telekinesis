using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{
    public Transform gaObject;

    void Start()
    {
        IInitObject initObject = gaObject.GetComponent<IInitObject>();
        initObject.InitComponent();
    }


}
