using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedListNode<T>
{
    public LinkedListNode(T platformType)
    {
        this.platformType = platformType;
    }

    public T platformType;
    public LinkedListNode<T> NextNode;

    
}

