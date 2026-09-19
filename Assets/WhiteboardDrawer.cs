using UnityEngine;

public class WhiteboardDrawer : MonoBehaviour
{
    [Header("Drawing Settings")]
    public Color drawColor = Color.black;
    public int brushSize = 5;

    private Texture2D drawTexture;
    private Renderer rend;
    private bool needsApply = false;

    // LINE DRAWING: track last painted UV position
    private Vector2 lastUV = Vector2.zero;
    private bool hasLastUV = false; // true when we are mid-stroke

    void Start()
    {
        rend = GetComponent<Renderer>();

        drawTexture = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);

        Color[] pixels = new Color[2048 * 2048];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;

        drawTexture.SetPixels(pixels);
        drawTexture.Apply();

        rend.material.mainTexture = drawTexture;
    }

    void Update()
    {
        if (needsApply)
        {
            drawTexture.Apply();
            needsApply = false;
        }
    }

    // Called every frame while trigger is held and ray hits board
    public void DrawAtRaycastHit(RaycastHit hit)
    {
        Vector2 currentUV = hit.textureCoord;

        if (hasLastUV)
        {
            // FIXED: interpolate between last UV and current UV to draw a smooth line
            float distance = Vector2.Distance(lastUV, currentUV);
            int steps = Mathf.Max(1, Mathf.CeilToInt(distance * 2048 / brushSize));

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                Vector2 interpolated = Vector2.Lerp(lastUV, currentUV, t);
                PaintAtUV(interpolated.x, interpolated.y);
            }
        }
        else
        {
            // First point of a new stroke
            PaintAtUV(currentUV.x, currentUV.y);
        }

        lastUV = currentUV;
        hasLastUV = true;
    }

    // Called when trigger is released or ray leaves board
    public void StopDrawing()
    {
        // Reset so next stroke starts fresh with no interpolation from old position
        hasLastUV = false;
    }

    public void DrawAtWorldPoint(Vector3 worldPoint)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
        float u = localPoint.x + 0.5f;
        float v = localPoint.z + 0.5f;

        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);

        PaintAtUV(u, v);
    }

    private void PaintAtUV(float u, float v)
    {
        int texX = (int)(u * drawTexture.width);
        int texY = (int)(v * drawTexture.height);
        PaintPixel(texX, texY);
        needsApply = true;
    }

    private void PaintPixel(int texX, int texY)
    {
        for (int x = -brushSize; x <= brushSize; x++)
        {
            for (int y = -brushSize; y <= brushSize; y++)
            {
                if (x * x + y * y <= brushSize * brushSize)
                {
                    int px = Mathf.Clamp(texX + x, 0, drawTexture.width - 1);
                    int py = Mathf.Clamp(texY + y, 0, drawTexture.height - 1);
                    drawTexture.SetPixel(px, py, drawColor);
                }
            }
        }
    }

    public void SetColor(Color newColor)
    {
        drawColor = newColor;
    }

    public void ClearBoard()
    {
        Color[] pixels = new Color[drawTexture.width * drawTexture.height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
        drawTexture.SetPixels(pixels);
        needsApply = true;
        hasLastUV = false;
    }
}