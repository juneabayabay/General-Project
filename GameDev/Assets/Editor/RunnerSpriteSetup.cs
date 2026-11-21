using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class RunnerSpriteSetup
{
    static RunnerSpriteSetup()
    {
        EditorApplication.delayCall += Ensure;
    }

    static void Ensure()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Resources/Runner", "Assets/Resources/Idle", "Assets/Resources/Jump" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                continue;

            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            bool ready = importer.textureType == TextureImporterType.Sprite
                && importer.spriteImportMode == SpriteImportMode.Single
                && importer.spritePixelsPerUnit == 280f
                && settings.spriteAlignment == (int)SpriteAlignment.BottomCenter
                && settings.spriteMeshType == SpriteMeshType.FullRect
                && importer.filterMode == FilterMode.Point
                && importer.alphaIsTransparency
                && !importer.mipmapEnabled;
            if (ready)
                continue;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 280f;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Point;
            settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
            settings.spritePivot = new Vector2(0.5f, 0f);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }
    }
}
