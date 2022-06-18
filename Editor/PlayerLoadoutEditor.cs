using Gamemodes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PlayerLoadout), true)]
public class PlayerLoadoutEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerLoadout loadout = serializedObject.targetObject as PlayerLoadout;

        int currentCost = 0;

        if(loadout.rightHandTable != null)
        {
            currentCost += loadout.rightHandTable.poolCost;
        }

        foreach(LoadoutPool pool in loadout.quickbeltTables)
        {
            if (pool == null) continue;

            currentCost += pool.poolCost;
        }

        EditorGUILayout.HelpBox("Loadout Cost: " + currentCost, MessageType.Info);
        
    }

}
