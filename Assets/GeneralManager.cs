using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralManager : MonoBehaviour
{
    [HideInInspector] public static GeneralManager instance;

    public RopesController ropesController;
    public CanvasManager canvasManager;
    public ZombieConstructor zombieConstructor;

    private void OnEnable()
    {
        instance = this;
    }
}
