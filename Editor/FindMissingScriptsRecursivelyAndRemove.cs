using UnityEditor;
using UnityEngine;


public class FindMissingScriptsRecursivelyAndRemove : EditorWindow
{
    private static int _goCount;
    private static int _componentsCount;
    private static int _missingCount;

    private static bool _bHaveRun;

    [MenuItem("Tools/FindMissingScriptsRecursivelyAndRemove")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FindMissingScriptsRecursivelyAndRemove));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
        {
            FindInSelected();
        }

        if (!_bHaveRun) return;

        EditorGUILayout.TextField(_goCount.ToString() + "GameObjects Selected");
        if (_goCount > 0) EditorGUILayout.TextField(_componentsCount.ToString() + " Components");
        if (_goCount > 0) EditorGUILayout.TextField(_missingCount.ToString() + " Deleted");
    }

    private static void FindInSelected()
    {
        var go = Selection.gameObjects;
        _goCount = 0;
        _componentsCount = 0;
        _missingCount = 0;
        foreach (var g in go)
        {
            FindInGo(g);
        }

        _bHaveRun = true;
        Debug.Log("Searched " + _goCount + " GameObjects, " + _componentsCount + " components, found " + _missingCount + " missing");

        AssetDatabase.SaveAssets();
    }

    private static void FindInGo(GameObject g)
    {
        _goCount++;
        var components = g.GetComponents<Component>();

        var r = 0;

        for (var i = 0; i < components.Length; i++)
        {
            _componentsCount++;
            if (components[i] != null) continue;
            _missingCount++;
            var s = g.name;
            var t = g.transform;
            while (t.parent != null)
            {
                s = t.parent.name + "/" + s;
                t = t.parent;
            }

            Debug.Log(s + " has a missing script at " + i, g);

            var serializedObject = new SerializedObject(g);

            var prop = serializedObject.FindProperty("m_Component");

            prop.DeleteArrayElementAtIndex(i - r);
            r++;

            serializedObject.ApplyModifiedProperties();
        }

        foreach (Transform childT in g.transform)
        {
            FindInGo(childT.gameObject);
        }
    }
}