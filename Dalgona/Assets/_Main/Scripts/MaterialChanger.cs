using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    public Renderer targetRenderer;        // Gắn Renderer object bạn muốn đổi texture
    public List<Texture> newTextures;             // Kéo texture mới vào Inspector

    private void Start()
    {
        ChangeTexture();
    }

    [ContextMenu("ChangeTexture")]
    public void ChangeTexture()
    {
        if (targetRenderer == null)
        {
            Debug.LogError("Chưa gán Renderer hoặc Texture mới!");
            return;
        }
        // Đổi texture cho material (channel _MainTex)
        targetRenderer.material.SetTexture("_MainTex", newTextures[Module.EasyRandom(newTextures.Count)]);
    }
}
