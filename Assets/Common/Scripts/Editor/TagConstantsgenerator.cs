using System.IO;
using UnityEditor;
using UnityEngine;

public class TagConstantsgenerator : AssetPostprocessor
{
    private const string outputPath = "Assets/Common/Scripts/TagsAndLayers/TagsAndLayersIribus.cs"; // Puedes cambiar la ruta

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
        string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets)
        {
            if (asset == "ProjectSettings/TagManager.asset")
            {
                GenerateTagConstants();
                break;
            }
        }
    }
    public static void GenerateTagConstants()
    {
        // Cargar el TagManager.asset
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        // ---------------------------- TAGS ----------------------------

        // Construir el contenido de la clase
        string classContent = "public static class TagsIribus\n{\n";

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            string tag = tagsProp.GetArrayElementAtIndex(i).stringValue;
            if (!string.IsNullOrEmpty(tag))
            {
                string safeName = MakeSafeIdentifier(tag);
                classContent += $"    public const string {safeName} = \"{tag}\";\n";
            }
        }

        classContent += "}\n";

        //---------------------------- LAYERS ---------------------------

        classContent += "public static class LayersIribus\n{\n";
        for (int i = 0; i < layersProp.arraySize; i++)
        {
            string layer = layersProp.GetArrayElementAtIndex(i).stringValue;
            if (!string.IsNullOrEmpty(layer))
            {
                string safeName = MakeSafeIdentifier(layer);
                classContent += $"    public const int {safeName} = {i};\n";
            }
            else
            {
                classContent += $"\n";
            }
        }
        classContent += "}\n";

        //---------------------------------------------------------------

        // Crear carpeta si no existe
        string folder = Path.GetDirectoryName(outputPath);
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        // Guardar el archivo
        File.WriteAllText(outputPath, classContent);
        AssetDatabase.Refresh();

        Debug.Log($"Clase TagsAndLayersIribus generada en: {outputPath}");
    }

    private static string MakeSafeIdentifier(string input)
    {
        // Asegurarse de que el nombre sea válido como identificador de C#
        return input.Replace(" ", "_").Replace("-", "_");
    }
}
