using UnityEngine;
using UnityEditor;
using System.IO;

public class RoughnessMetalMerger : EditorWindow
{
    Texture2D metalnessTex;
    Texture2D roughnessTex;

    [MenuItem("Tools/Merge Roughness + Metalness")]
    public static void ShowWindow()
    {
        GetWindow<RoughnessMetalMerger>("Merge Roughness + Metalness");
    }

    void OnGUI()
    {
        GUILayout.Label("Input Textures", EditorStyles.boldLabel);

        metalnessTex = (Texture2D)EditorGUILayout.ObjectField("Metalness (RGB)", metalnessTex, typeof(Texture2D), false);
        roughnessTex = (Texture2D)EditorGUILayout.ObjectField("Roughness (Smoothness = 1 - R)", roughnessTex, typeof(Texture2D), false);

        if (GUILayout.Button("Merge and Save"))
        {
            if (metalnessTex == null || roughnessTex == null)
            {
                EditorUtility.DisplayDialog("Error", "両方のテクスチャを設定してください。", "OK");
                return;
            }

            string path = EditorUtility.SaveFilePanel("保存先を選択", "Assets", "MetallicSmoothness", "png");
            if (string.IsNullOrEmpty(path)) return;

            MergeTextures(metalnessTex, roughnessTex, path);
        }
    }

    void MergeTextures(Texture2D metalTex, Texture2D roughTex, string path)
    {
        int width = metalTex.width;
        int height = metalTex.height;

        if (roughTex.width != width || roughTex.height != height)
        {
            EditorUtility.DisplayDialog("サイズ不一致", "MetalnessとRoughnessのテクスチャサイズが一致していません。", "OK");
            return;
        }

        Texture2D resultTex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] metalPixels = metalTex.GetPixels();
        Color[] roughPixels = roughTex.GetPixels();
        Color[] resultPixels = new Color[metalPixels.Length];

        for (int i = 0; i < metalPixels.Length; i++)
        {
            float metal = metalPixels[i].r;
            float rough = roughPixels[i].r;
            float smooth = 1.0f - rough;

            resultPixels[i] = new Color(metal, metal, metal, smooth);
        }

        resultTex.SetPixels(resultPixels);
        resultTex.Apply();

        byte[] pngData = resultTex.EncodeToPNG();
        if (pngData != null)
        {
            File.WriteAllBytes(path, pngData);
            Debug.Log("保存しました: " + path);

            // Unity プロジェクトパスなら自動インポート
            if (path.StartsWith(Application.dataPath))
            {
                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
            }
        }
    }
}
