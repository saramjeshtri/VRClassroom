using UnityEngine;

public class ColorPickerPanel : MonoBehaviour
{
    private string colorName;
    private static ColorPickerPanel currentSelected;
    private Renderer rend;
    private Color baseColor;

    void Start()
    {
        colorName = gameObject.name.Replace("ColorPanel_", "").ToLower();
        rend = GetComponent<Renderer>();

        switch (colorName)
        {
            case "black":  baseColor = Color.black;   break;
            case "red":    baseColor = Color.red;     break;
            case "blue":   baseColor = Color.blue;    break;
            case "green":  baseColor = Color.green;   break;
            case "yellow": baseColor = Color.yellow;  break;
            case "eraser": baseColor = Color.white;   break;
            default:       baseColor = Color.grey;    break;
        }

        rend.material.color = baseColor;
        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", Color.black);
    }

    public void SelectColor()
    {
        WhiteboardDrawer drawer = FindObjectOfType<WhiteboardDrawer>();
        if (drawer == null)
        {
            Debug.LogWarning("WhiteboardDrawer not found!");
            return;
        }

        switch (colorName)
        {
            case "black":
                drawer.drawColor = Color.black;
                drawer.brushSize = 5;
                break;
            case "red":
                drawer.drawColor = Color.red;
                drawer.brushSize = 5;
                break;
            case "blue":
                drawer.drawColor = Color.blue;
                drawer.brushSize = 5;
                break;
            case "green":
                drawer.drawColor = Color.green;
                drawer.brushSize = 5;
                break;
            case "yellow":
                drawer.drawColor = Color.yellow;
                drawer.brushSize = 5;
                break;
            case "eraser":
                drawer.drawColor = Color.white;
                drawer.brushSize = 50; // UPDATED: bigger eraser
                break;
        }

        if (currentSelected != null)
            currentSelected.SetSelected(false);
        currentSelected = this;
        SetSelected(true);

        Debug.Log("Selected: " + colorName);
    }

    public void SetHover(bool on)
    {
        if (currentSelected == this) return;
        transform.localScale = on
            ? new Vector3(0.11f, 0.11f, 0.11f)
            : new Vector3(0.1f, 0.1f, 0.1f);
    }

    void SetSelected(bool on)
    {
        rend.material.SetColor("_EmissionColor", Color.black);
        transform.localScale = on
            ? new Vector3(0.12f, 0.12f, 0.12f)
            : new Vector3(0.1f, 0.1f, 0.1f);
    }
}