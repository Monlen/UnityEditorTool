using System.IO;
using UnityEditor;
using UnityEngine;

public class CodeGenerator : EditorWindow
{
    private string templateCode = "public class [MY_CLASS_NAME]\n{\n\t// placeholder for your code\n}";

    [MenuItem("Tools/Code Generator")]
    public static void Open()
    {
        GetWindow<CodeGenerator>("Code Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate code from Unity UI prefab", EditorStyles.boldLabel);

        var prefab = EditorGUILayout.ObjectField("Prefab", null, typeof(GameObject), false) as GameObject;

        if (GUILayout.Button("Generate Code") && prefab != null)
        {
            GenerateCode(prefab);
        }
    }

    private void GenerateCode(GameObject prefab)
    {
        var codePath = Application.dataPath + "/Scripts/UI/" + prefab.name + ".cs";
        var fileExists = File.Exists(codePath);

        if (fileExists)
        {
            var result = EditorUtility.DisplayDialogComplex(
                "Warning",
                "A script with this name already exists. Do you want to overwrite it?",
                "Yes",
                "No, cancel",
                "");

            if (result == 1)
            {
                return;
            }
        }

        var allObjects = GameObjectUtility.GetObjectsOfType<GameObject>(prefab);

        var generatedCode = templateCode;

        foreach (var obj in allObjects)
        {
            if (obj == prefab)
            {
                continue;
            }

            // replace placeholders with actual code
            var nodeCode = "\tpublic GameObject " + obj.name + ";\n\t";

            generatedCode = generatedCode.Replace("[MY_CODE]", nodeCode + "[MY_CODE]");
        }

        // replace class name placeholder with actual class name
        generatedCode = generatedCode.Replace("[MY_CLASS_NAME]", prefab.name);

        // write generated code to file
        File.WriteAllText(codePath, generatedCode);

        if (fileExists)
        {
            AssetDatabase.Refresh();
        }
    }
}
