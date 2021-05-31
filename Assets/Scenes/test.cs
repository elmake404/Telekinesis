using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum states
{
    a,
    b,
    c,
    d
}
    public class test : MonoBehaviour
    {
        
    private void Start()
    {
        states state = states.d;

        switch (state)
        {
            case states.a:
            case states.b:
            case states.c:
                Debug.Log("C");
                break;
            case states.d:
                Debug.Log("D");
                break;
        }
    }
}

