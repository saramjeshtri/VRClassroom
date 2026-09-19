using UnityEngine;

public class MapImageCycler : MonoBehaviour
{
    private Texture2D[] placeholderTextures;
    private int currentIndex = 0;
    private Renderer mapRenderer;
    private bool triggerWasPressed = false;

    void Start()
    {
        mapRenderer = GetComponent<Renderer>();

        // Create 5 placeholder colored textures
        placeholderTextures = new Texture2D[5];
        Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow, Color.cyan };
        string[] labels = { "Map 1", "Map 2", "Map 3", "Map 4", "Map 5" };

        for (int i = 0; i < 5; i++)
        {
            Texture2D tex = new Texture2D(512, 512);
            Color[] pixels = new Color[512 * 512];
            for (int p = 0; p < pixels.Length; p++)
                pixels[p] = colors[i];
            tex.SetPixels(pixels);
            tex.Apply();
            placeholderTextures[i] = tex;
        }

        // Set first texture
        ApplyTexture(0);
    }

    void ApplyTexture(int index)
    {
        // Material index 1 is Map_Image
        Material[] mats = mapRenderer.materials;
        mats[1].mainTexture = placeholderTextures[index];
        mapRenderer.materials = mats;
    }

    public void OnLaserTrigger()
    {
        currentIndex = (currentIndex + 1) % placeholderTextures.Length;
        ApplyTexture(currentIndex);
        Debug.Log("Map changed to index: " + currentIndex);
    }
}