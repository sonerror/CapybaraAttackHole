/*using UnityEngine;
using UnityEditor;

public class RemoveMissingScripts : EditorWindow
{
    private GameObject parentObject;  // Đối tượng cha
    private GameObject prefabToInstantiate;  // Đối tượng prefab cần tạo mới

    [MenuItem("Tools/Create Children Tool")]
    public static void ShowWindow()
    {
        GetWindow<RemoveMissingScripts>("Create Children Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Children from Prefab", EditorStyles.boldLabel);

        // Nhập đối tượng cha
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);

        // Nhập prefab để tạo mới
        prefabToInstantiate = (GameObject)EditorGUILayout.ObjectField("Prefab to Instantiate", prefabToInstantiate, typeof(GameObject), false);

        if (GUILayout.Button("Create Children"))
        {
            CreateChildren();
        }
    }

    private void CreateChildren()
    {
        if (parentObject == null || prefabToInstantiate == null)
        {
            Debug.LogError("Please assign both Parent Object and Prefab.");
            return;
        }

        // Lặp qua tất cả các đối tượng con của đối tượng cha
        foreach (Transform child in parentObject.transform)
        {
            // Tạo một đối tượng mới từ prefab tại vị trí của đối tượng con
            GameObject newChild = Instantiate(prefabToInstantiate, child.position, child.rotation, parentObject.transform);

            // Xóa đối tượng con
            DestroyImmediate(child.gameObject);
        }

        // Lưu lại thay đổi
        Undo.RegisterCreatedObjectUndo(parentObject, "Create Children");
        EditorUtility.SetDirty(parentObject);
        Debug.Log("Children created and original children removed.");
    }
}
*/