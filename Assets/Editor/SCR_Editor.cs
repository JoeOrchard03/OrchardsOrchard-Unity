using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SCR_ReworkedSaveSystem))]
public class SCR_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        SCR_ReworkedSaveSystem saveSystem = (SCR_ReworkedSaveSystem)target;
        
        GUILayout.Space(10);
        GUILayout.Label("Testing", EditorStyles.boldLabel);

        if (GUILayout.Button("Clear Save Data"))
        {
            if (EditorUtility.DisplayDialog(
                    "Confirm Clear Save",
                    "Are you sure yo want to delete the save data?",
                    "Yes",
                    "No"))
            {
                saveSystem.ClearSaveData();
            }
        }
    }
}
