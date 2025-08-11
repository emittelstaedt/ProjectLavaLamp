using System.Collections.Generic;
using UnityEngine;

public enum CorrodeLevel
{
    Clean,
    Corrode,
    UltraCorrode,
}

[RequireComponent(typeof(Renderer))]
public class MaterialCorroder : MonoBehaviour
{
    [SerializeField] private Material cleanMaterial;
    [SerializeField] private Material corrodeMaterial;
    [SerializeField] private Material ultraCorrodeMaterial;
    private Dictionary<CorrodeLevel, Material> materialList;
    private Renderer materialRenderer;
    
    private void Awake()
    {
        materialRenderer = GetComponent<Renderer>();

        materialList = new();
        if (cleanMaterial != null)
        {
            materialList.Add(CorrodeLevel.Clean, cleanMaterial);
        }
        if (corrodeMaterial != null)
        {
            materialList.Add(CorrodeLevel.Corrode, corrodeMaterial);
        }
        if (ultraCorrodeMaterial != null)
        {
            materialList.Add(CorrodeLevel.UltraCorrode, ultraCorrodeMaterial);
        }
    }

    public void SetMaterial(int level)
    {
        CorrodeLevel corrodeLevel = (CorrodeLevel) level;

        if (materialList.TryGetValue(corrodeLevel, out Material material))
        {
            materialRenderer.material = material;
        }
        else if (level > 0)
        {
            SetMaterial(level - 1);
        }
    }
}