using UnityEngine;

public class S_A_SkinnedOutfitColorChange : MonoBehaviour
{
    [Tooltip("Assign your outfit SkinnedMeshRenderer here")]
    public SkinnedMeshRenderer outfitRenderer;

    [Tooltip("Which material index to recolor (0 = first material)")]
    public int materialIndex = 0;

    [Tooltip("Color to apply")]
    public Color newColor = Color.red;

    private Material[] mats;

    void Awake()
    {
        if (outfitRenderer == null)
        {
            Debug.LogError("No SkinnedMeshRenderer assigned!");
            return;
        }

        mats = outfitRenderer.materials;
        if (materialIndex < 0 || materialIndex >= mats.Length)
        {
            Debug.LogError("Material index out of range!");
            return;
        }

        ChangeOutfitColor(materialIndex, newColor);
    }

    public void ChangeOutfitColor(int matIndex, Color color) {
        newColor = color;
        materialIndex = matIndex;

        Material mat = mats[materialIndex];

        // Try to find a color property the shader supports
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", newColor);
        else if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", newColor);
        else if (mat.HasProperty("_Tint"))
            mat.SetColor("_Tint", newColor);
        else
            Debug.LogWarning($"{mat.name} shader has no recognized color property (_BaseColor/_Color/_Tint).");

        // Reassign materials array to apply changes
        outfitRenderer.materials = mats;
    }
}
