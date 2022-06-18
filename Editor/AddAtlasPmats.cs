using Atlas.MappingComponents;
using UnityEditor;
using UnityEngine;


public class AddAtlasPmats : EditorWindow
{
    
    private static AtlasPMat.MatDefEnum _matDef;

    [MenuItem("Tools/AddAtlasPmats")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AddAtlasPmats));
    }

    public void OnGUI()
    {
        _matDef = (AtlasPMat.MatDefEnum)EditorGUILayout.EnumPopup("Atlas PMat", _matDef);

        if (GUILayout.Button("Add Atlas Pmat to selected environment"))
        {
            AddToSelected();
        }

        if (GUILayout.Button("Remove Atlas PMat from selected enironment"))
        {
            RemoveFromSelected();
        }
    }

    private static void AddToSelected()
    {
        var go = Selection.gameObjects;

        foreach (var g in go)
        {
            AddToGO(g);
        }

        AssetDatabase.SaveAssets();
    }

    private static void RemoveFromSelected()
    {
        var go = Selection.gameObjects;

        foreach (var g in go)
        {
            RemoveFromGO(g);
        }

        AssetDatabase.SaveAssets();
    }

    private static void AddToGO(GameObject g)
    {
        //Make sure the object is static and environment
        if(g.isStatic && g.layer == LayerMask.NameToLayer("Environment"))
        {
            //Make sure the object has colliders
            Collider col = g.GetComponent<Collider>();
            if(col != null && !col.isTrigger)
            {
                AtlasPMat matComp = g.GetComponent<AtlasPMat>();

                if (matComp == null)
                {
                    matComp = g.AddComponent<AtlasPMat>();
                    matComp.AtlasMatDef = _matDef;
                }
            }
        }

        foreach (Transform childT in g.transform)
        {
            AddToGO(childT.gameObject);
        }
    }

    private static void RemoveFromGO(GameObject g)
    {
        //Make sure the object is static and environment
        if (g.isStatic && g.layer == LayerMask.NameToLayer("Environment"))
        {
            //Make sure the object has colliders
            Collider col = g.GetComponent<Collider>();
            if (col != null && !col.isTrigger)
            {
                AtlasPMat matComp = g.GetComponent<AtlasPMat>();

                if (matComp != null)
                {
                    Destroy(matComp);
                }
            }
        }

        foreach (Transform childT in g.transform)
        {
            RemoveFromGO(childT.gameObject);
        }
    }
}