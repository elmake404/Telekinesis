using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryRendererContainer
{
    public static TemporaryRendererContainer instance;
    public List<Renderer> temporaryRenderers = new List<Renderer>();

    public TemporaryRendererContainer()
    {
        instance = this;
    }

    public void AddRendererToPost(Renderer renderer)
    {
        temporaryRenderers.Add(renderer);
        Debug.Log(temporaryRenderers.Count);

    }

    public void DeleteRenderer(int objectIdToDestroy)
    {
        for (int i = 0; i < temporaryRenderers.Count; i++)
        {
            if (temporaryRenderers[i].GetInstanceID() == objectIdToDestroy)
            {
                temporaryRenderers.RemoveAt(i);
                break;
            }
        }
    }

    public void ClearTemporaryRenderers()
    {
        temporaryRenderers.Clear();
    }
}
