using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class CoreSceneSetup
{
    const string ExplorerPath = "Assets/Art/Explorer.png";
    const string PlatformPath = "Assets/Art/Platform.png";
    const string BackgroundPath = "Assets/Gemini_Generated_Image_fw4mcifw4mcifw4m.jpg";

    static int attempts;

    static CoreSceneSetup()
    {
        EditorApplication.delayCall += Ensure;
    }

    public static void BuildNow()
    {
        attempts = 0;
        AssetDatabase.Refresh();
        Ensure();
    }

    static void Ensure()
    {
        return;

        if (attempts > 30)
            return;

        attempts++;

        if (!PrepareSprites())
        {
            EditorApplication.delayCall += Ensure;
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != "Assets/Scenes/SampleScene.unity")
        {
            scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        }

        if (GameObject.Find("Core Background") != null)
            return;

        Sprite background = LoadSprite(BackgroundPath);
        Sprite explorer = AssetDatabase.LoadAssetAtPath<Sprite>(ExplorerPath);
        Sprite platform = AssetDatabase.LoadAssetAtPath<Sprite>(PlatformPath);
        if (background == null || explorer == null || platform == null)
        {
            EditorApplication.delayCall += Ensure;
            return;
        }

        Build(scene, background, explorer, platform);
    }

    static bool PrepareSprites()
    {
        if (AssetDatabase.LoadAssetAtPath<Texture2D>(ExplorerPath) == null)
            return false;
        if (AssetDatabase.LoadAssetAtPath<Texture2D>(PlatformPath) == null)
            return false;

        bool changed = false;
        changed |= ConfigureSprite(ExplorerPath, 1000f, SpriteAlignment.BottomCenter);
        changed |= ConfigureSprite(PlatformPath, 32f, SpriteAlignment.Center);
        return !changed;
    }

    static bool ConfigureSprite(string path, float pixelsPerUnit, SpriteAlignment alignment)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return false;

        bool dirty = importer.textureType != TextureImporterType.Sprite
            || importer.spritePixelsPerUnit != pixelsPerUnit
            || importer.filterMode != FilterMode.Point
            || !importer.alphaIsTransparency
            || importer.mipmapEnabled;

        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        if (settings.spriteAlignment != (int)alignment || settings.spriteMeshType != SpriteMeshType.FullRect)
            dirty = true;

        if (!dirty)
            return false;

        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = pixelsPerUnit;
        importer.filterMode = FilterMode.Point;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        settings.spriteAlignment = (int)alignment;
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);
        importer.SaveAndReimport();
        return true;
    }

    static Sprite LoadSprite(string path)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (Object asset in assets)
        {
            if (asset is Sprite sprite)
                return sprite;
        }

        return null;
    }

    static void Build(Scene scene, Sprite background, Sprite explorer, Sprite platformSprite)
    {
        GameObject backdrop = new GameObject("Core Background");
        SpriteRenderer backdropRenderer = backdrop.AddComponent<SpriteRenderer>();
        backdropRenderer.sprite = background;
        backdropRenderer.sortingOrder = -20;

        GameObject platforms = new GameObject("Platforms");
        float[] heights = { -12.2f, -10f, -7.8f, -5.6f, -3.4f, -1.2f, 1f, 3.2f, 5.4f, 7.6f, 9.8f, 12f };
        for (int i = 0; i < heights.Length; i++)
        {
            GameObject platform = new GameObject("Platform " + (i + 1));
            platform.transform.SetParent(platforms.transform);
            float x = i % 2 == 0 ? -1.6f : 1.6f;
            if (i == 0)
                x = 0f;
            float width = i == 0 ? 6f : 3.4f;
            platform.transform.position = new Vector3(x, heights[i], 0f);
            platform.transform.localScale = new Vector3(width, 0.45f, 1f);
            SpriteRenderer renderer = platform.AddComponent<SpriteRenderer>();
            renderer.sprite = platformSprite;
            renderer.sortingOrder = 0;
            BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }

        GameObject player = new GameObject("Explorer");
        player.transform.position = new Vector3(0f, -11.55f, 0f);
        SpriteRenderer playerRenderer = player.AddComponent<SpriteRenderer>();
        playerRenderer.sprite = explorer;
        playerRenderer.sortingOrder = 10;
        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.freezeRotation = true;
        body.gravityScale = 2.5f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        BoxCollider2D playerCollider = player.AddComponent<BoxCollider2D>();
        playerCollider.size = new Vector2(0.42f, 1.45f);
        playerCollider.offset = new Vector2(0f, 0.75f);
        player.AddComponent<CoreClimber>();

        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.orthographic = true;
            camera.orthographicSize = 4.32f;
            camera.backgroundColor = new Color(0.08f, 0.03f, 0.02f, 1f);
            camera.transform.position = new Vector3(0f, -9.44f, -10f);
            CoreCameraFollow follow = camera.GetComponent<CoreCameraFollow>();
            if (follow == null)
                follow = camera.gameObject.AddComponent<CoreCameraFollow>();
            follow.target = player.transform;
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Core Ascent: placed the lava background and explorer in SampleScene.");
    }
}
