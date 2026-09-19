using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FixMaterials : MonoBehaviour
{
    [MenuItem("Tools/Fix Everything")]
    static void FixEverything()
    {
        Dictionary<string, string> matToTex = new Dictionary<string, string>()
        {
            { "Chair", "Chair_Chair_BaseColor" },
            { "Door_", "Door_Door_BaseColor" },
            { "Floor", "Floor_DefaultMaterial_BaseColor" },
            { "GlassonDoor", "Door_Glass_BaseColor" },
            { "Hanging Lamp", "Neon_Hanging_Lamp_BaseColor" },
            { "Large_Window_handle", "Big_Window_Handle_Large_Window_handle_Base" },
            { "Map Fame", "Map_on_hte_Wall_Map_Fame_BaseColor" },
            { "Map_Image", "Map_on_hte_Wall_Map_Image_BaseColor" },
            { "Metal Lamp holders", "Neon_Metal_Lamp_holders_BaseColor" },
            { "Neon_Glass", "Neon_Neon_Glass_BaseColor" },
            { "Projector", "Projector_Projector_BaseColor" },
            { "Projector Table", "Projector_Table_Projector_Table_BaseColor" },
            { "Switches_Signs", "Switches_Signs_Switches_Signs_BaseColor" },
            { "Table", "Table_Student_table_BaseColor" },
            { "Teacher Chair", "Teachers_chair_Teacher_Chair_BaseColor" },
            { "Teachers Table", "Teacher_table_Teachers_Table_BaseColor" },
            { "Walls", "Walls_Walls_Diffuse" },
            { "Windows Frame", "Window_Frame_Windows_Frame_BaseColor" },
            { "Window_Handle", "Window_Handle_Window_Handle_BaseColor" },
            { "Window_Joints", "Window_joint_Window_Joints_BaseColor" },
            { "Windows Glass", "Windows_Windows_Glass_BaseColor" },
        };

        string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Classroom" });
        int fixedCount = 0;

        foreach (string guid in matGuids)
        {
            string matPath = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null) continue;
            if (!matToTex.ContainsKey(mat.name)) continue;

            // Use URP shader
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader != null) mat.shader = shader;

            // Find and apply texture
            string texName = matToTex[mat.name];
            string[] texGuids = AssetDatabase.FindAssets(texName + " t:Texture2D");
            if (texGuids.Length > 0)
            {
                string texPath = AssetDatabase.GUIDToAssetPath(texGuids[0]);
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
                if (tex != null)
                {
                    mat.SetTexture("_BaseMap", tex);
                    mat.SetColor("_BaseColor", Color.white);
                    fixedCount++;
                }
            }
            else
            {
                Debug.LogWarning("Texture not found: " + texName);
            }

            // Glass transparency
            if (mat.name.Contains("Glass"))
            {
                mat.SetFloat("_Surface", 1);
                mat.SetColor("_BaseColor", new Color(0.8f, 0.9f, 1f, 0.25f));
                mat.renderQueue = 3000;
            }

            EditorUtility.SetDirty(mat);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Fixed " + fixedCount + " materials.");
    }
}