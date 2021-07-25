using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedList<T> : IEnumerable<T>
{
    public LinkedListNode<T> head;
    public LinkedListNode<T> tail;
    public int Count { get { return count; } private set { } }
    public bool IsEmpty { get { return count == 0; } private set { } }

    private int count;

    public LinkedList()
    {
        
    }

    public LinkedList(List<T> inputData) : this()
    {
        for (int i = 0; i < inputData.Count; i++)
        {
            AddNode(inputData[i]);
        }
    }

    public void AddNode(T data)
    {
        LinkedListNode<T> node = new LinkedListNode<T>(data);
        if (head == null)
        {
            head = node;
        }
        else
        {
            tail.NextNode = node;
        }
        tail = node;
        count++;
    }

    public bool RemoveNode(T data)
    {
        LinkedListNode<T> current = head;
        LinkedListNode<T> previous = null;

        while (current != null)
        {
            if (current.platformType.Equals(data))
            {
                if (previous != null)
                {
                    previous.NextNode = current.NextNode;

                    if (current.NextNode == null)
                    {
                        tail = previous;
                    }
                }
                else
                {
                    head = head.NextNode;

                    if (head == null)
                    {
                        tail = null;
                    }
                }
                count--;
                return true;    
            }

            previous = current;
            current = current.NextNode;
        }
        return false;
    }

    public void Clear()
    {
        head = null;
        tail = null;
        count = 0;
    }

    /*public bool Contains()
    {
        
    }*/

    public IEnumerator<T> GetEnumerator()
    {
        LinkedListNode<T> current = head;
        while (current != null)
        {
            yield return current.platformType;
            current = current.NextNode;

        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)this).GetEnumerator();
    }
}
