using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMeshedSpiral : Graphic
{
    [Range(0, 100)] public int sliderToggle;
    public float width;
    private float canvasScale;



    protected override void Start()
    {
        if (!Application.isPlaying) { return; }
        canvasScale = GeneralManager.instance.canvas.scaleFactor;
        
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (!Application.isPlaying) { return; }
        
        if (sliderToggle == 0)
        {
            return;
        }
        canvasRenderer.SetMaterial(this.material, null);
        

        MeshSpline meshSpline = MeshedSpline.GetGeneratedMesh(GenerateSpiral.GetSpiralPoints(sliderToggle, rectTransform.rect.size), width, color, canvasScale);
        
        vh.AddUIVertexStream(new List<UIVertex>(meshSpline.uIVertices), new List<int>(meshSpline.tris));
        
    }

    

    public void UpdateMesh()
    {
        SetVerticesDirty();
        
    }

}
