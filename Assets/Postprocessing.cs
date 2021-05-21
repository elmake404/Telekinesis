using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Postprocessing : MonoBehaviour
{
	[SerializeField]
	private Material postprocessMaterial;
	public Camera attachedCamera;
	public Material WriteObject;
	private int selectionBuffer = Shader.PropertyToID("_SelectionBuffer");
	private RenderTexture renderTexture;
	private Renderer[] renderers;

    private void OnEnable()
    {
		SetRenderers();
	}

    private void Start()
    {
		InitRenderTexture();
		
	}

	public void EnableEffect()
	{
		enabled = true;
	}

	public void DisableEffect()
	{
		enabled = false;
	}

    public void SetRenderers()
	{
		renderers = TemporaryRendererContainer.instance.temporaryRenderers.ToArray();
	}

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		var commandBuffer = new CommandBuffer();
		
		commandBuffer.GetTemporaryRT(selectionBuffer, renderTexture.descriptor);
		commandBuffer.SetRenderTarget(selectionBuffer);
		commandBuffer.ClearRenderTarget(true, true, Color.clear);

		for (int i = 0; i < renderers.Length; i++)
		{
			int numOfMaterials = renderers[i].materials.Length;

			for (int k = 0; k < numOfMaterials; k++)
			{
				commandBuffer.DrawRenderer(renderers[i], WriteObject, k);
			}
		}

		commandBuffer.Blit(source, destination, postprocessMaterial);
		commandBuffer.ReleaseTemporaryRT(selectionBuffer);

		Graphics.ExecuteCommandBuffer(commandBuffer);
		commandBuffer.Dispose();
	}

	private void InitRenderTexture()
	{
		if (renderTexture != null) { return; }
		renderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
		renderTexture.Create();
	}


}