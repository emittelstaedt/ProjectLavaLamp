using UnityEngine;
using UnityEditor;
using System.IO;

[RequireComponent(typeof(Renderer))]
public class ScreenCameraLinker : MonoBehaviour
{
    [Tooltip("The camera to render to the screen.")]
    [SerializeField] new private Camera camera;
    [Tooltip("The screen height in pixels for screen resolution.")]
    [SerializeField] private int pixelHeight = 500;
    [SerializeField] private string materialPath = "Assets/Art/Materials/";
    [SerializeField] private string renderTexturePath = "Assets/Art/Textures/";
    [SerializeField] new private string name;

    [ContextMenu("Link camera to screen")]
    private void LinkCameraToScreen()
    {
        int pixelWidth = (int) (pixelHeight * (transform.lossyScale.x / transform.lossyScale.y));

        Renderer renderer = GetComponent<Renderer>();
        RenderTexture newRenderTexture = new(pixelWidth, pixelHeight, 24);
        Material newMaterial = new(Shader.Find("Universal Render Pipeline/Lit"));

        string fullRenderTexturePath = GetFullPath(renderTexturePath, ".renderTexture");
        string fullMaterialPath = GetFullPath(materialPath, ".mat");

        if (File.Exists(fullRenderTexturePath))
        {
            Debug.LogError(fullRenderTexturePath + " already exists.");
            return;
        }

        if (File.Exists(fullMaterialPath))
        {
            Debug.LogError(fullMaterialPath + " already exists.");
            return;
        }

        AssetDatabase.CreateAsset(newMaterial, fullMaterialPath);
        AssetDatabase.CreateAsset(newRenderTexture, fullRenderTexturePath);
        AssetDatabase.SaveAssets();

        newMaterial.mainTexture = newRenderTexture;
        camera.targetTexture = newRenderTexture;
        renderer.material = newMaterial;

        Debug.Log("New material created at: " + fullMaterialPath);
        Debug.Log("New render texture created at: " + fullRenderTexturePath);
    }

    [ContextMenu("Resync Aspect Ratio")]
    private void SyncRenderTextureRatio()
    {
        string fullRenderTexturePath = GetFullPath(renderTexturePath, ".renderTexture");
        string fullMaterialPath = GetFullPath(materialPath, ".mat");
        
        if (File.Exists(fullRenderTexturePath) && File.Exists(fullMaterialPath))
        {
            AssetDatabase.DeleteAsset(fullRenderTexturePath);
            AssetDatabase.DeleteAsset(fullMaterialPath);
            Debug.Log("Deleting " + fullRenderTexturePath);
            Debug.Log("Deleting " + fullMaterialPath);

            LinkCameraToScreen();
        }
        else
        {
            Debug.LogError("Resync failed. Please manually delete the material in" + fullMaterialPath + ", and render texture in" + renderTexturePath + ", then link the camera to the screen. The files may be missing or renamed.");
        }
    }

    private string GetFullPath(string path, string extension)
    {
        if (path[path.Length - 1] != '/')
        {
            path += '/';
        }

        if (name == null || name == "")
        {
            return path + transform.parent.name + extension;
        }
        else
        {
            return path + name + extension;
        }
    }
}