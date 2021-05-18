using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMeshedSpiral : Graphic
{
    [Range(0, 100)] public int sliderToggle;
    public float width;
    protected override void OnValidate()
    {
        UpdateMesh();
        
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        
        vh.Clear();
        if (sliderToggle == 0)
        {
            return;
        }
        canvasRenderer.SetMaterial(this.material, null);
        

        MeshSpline meshSpline = MeshedSpline.GetGeneratedMesh(GenerateSpiral.GetSpiralPoints(sliderToggle, rectTransform.rect.size), width, color);
        
        vh.AddUIVertexStream(new List<UIVertex>(meshSpline.uIVertices), new List<int>(meshSpline.tris));
        
    }



    private void BuildMesh(VertexHelper vh)
    {

    }

    public void UpdateMesh()
    {
        SetVerticesDirty();
        
    }

}
