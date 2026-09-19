using UnityEngine;

public class ColorPanelSetup : MonoBehaviour
{
    public GameObject[] panels;

    [ContextMenu("Arrange Cubes")]
    void Start()
    {
        float spacing = 0.15f;
        float y = 1.18f;
        float z = -11.5f;

        float totalWidth = spacing * 5f;
        float startX = 2.8f - (totalWidth / 2f);  // Nudged right from 2.5f

        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null)
            {
                float newX = startX + (i * spacing);
                panels[i].transform.position = new Vector3(newX, y, z);
                panels[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                panels[i].transform.rotation = Quaternion.identity;
                Debug.Log($"Moved {panels[i].name} to X: {newX}");
            }
        }
    }
}