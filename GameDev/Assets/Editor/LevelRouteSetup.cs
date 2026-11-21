using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class LevelRouteSetup
{
    const float LayerWidth = 15.36f;
    const string ScenePath = "Assets/Scenes/SampleScene.unity";

    static readonly string[] Titles =
    {
        "Level 1 - Core",
        "Level 2 - Mantle",
        "Level 3 - Earth",
        "Level 4 - Atmosphere",
        "Level 5 - Outer Space"
    };

    static readonly string[] Backgrounds =
    {
        "Assets/core2.jpg",
        "Assets/mantle.jpg",
        "Assets/earth.jpg",
        "Assets/atmospher.jpg",
        "Assets/outerSpace.jpg"
    };

    static readonly string[] ObjectivePaths =
    {
        "Assets/objective1.png",
        "Assets/objective2.png",
        "Assets/object3.png",
        "Assets/object4.png",
        "Assets/objective5.png"
    };

    static int attempts;

    static LevelRouteSetup()
    {
        EditorApplication.delayCall += Ensure;
    }

    static void Ensure()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.playModeStateChanged -= OnPlayMode;
            EditorApplication.playModeStateChanged += OnPlayMode;
            return;
        }

        if (attempts > 12)
            return;

        attempts++;
        if (!SpritesReady(Backgrounds) || !SpritesReady(ObjectivePaths))
        {
            EditorApplication.delayCall += Ensure;
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath)
            scene = EditorSceneManager.OpenScene(ScenePath);

        if (HasFiveStages())
            return;

        Build(scene);
    }

    static void OnPlayMode(PlayModeStateChange change)
    {
        if (change != PlayModeStateChange.EnteredEditMode)
            return;

        attempts = 0;
        EditorApplication.delayCall += Ensure;
    }

    static bool HasFiveStages()
    {
        if (GameObject.Find("Core Background") != null)
            return false;

        for (int i = 0; i < Titles.Length; i++)
        {
            GameObject level = GameObject.Find(Titles[i]);
            if (level == null)
                return false;
            SpriteRenderer renderer = level.GetComponent<SpriteRenderer>();
            if (renderer == null || renderer.sprite == null)
                return false;
        }

        GameObject platforms = GameObject.Find("Platforms");
        if (platforms == null || platforms.transform.childCount < 20)
            return false;

        float top = float.MinValue;
        foreach (Transform child in platforms.transform)
            top = Mathf.Max(top, child.position.y);

        GameObject space = GameObject.Find(Titles[4]);
        float spaceTop = space.transform.position.y;
        SpriteRenderer spaceRenderer = space.GetComponent<SpriteRenderer>();
        if (spaceRenderer != null && spaceRenderer.sprite != null)
        {
            float height = spaceRenderer.bounds.size.y;
            spaceTop = spaceRenderer.bounds.max.y;
            height = Mathf.Max(height, 1f);
        }

        return top > spaceTop - 8f;
    }

    static bool SpritesReady(string[] paths)
    {
        foreach (string path in paths)
        {
            if (LoadSprite(path) == null && !MakeSprite(path))
                return false;
            if (LoadSprite(path) == null)
                return false;
        }

        return true;
    }

    static bool MakeSprite(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return false;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100f;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();
        return true;
    }

    static Sprite LoadSprite(string path)
    {
        foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(path))
        {
            if (asset is Sprite sprite)
                return sprite;
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static void Build(Scene scene)
    {
        DestroyIfFound("Core Background");
        DestroyIfFound("Level Route");
        DestroyIfFound("Level Objectives");
        DestroyIfFound("Linked Steps");
        for (int i = 0; i < Titles.Length; i++)
            DestroyIfFound(Titles[i]);
        for (int i = 1; i <= 5; i++)
            DestroyIfFound("Objective " + i);
        DestroyNamed("Level 1 — Core");
        DestroyNamed("Level 2 — Mantle");
        DestroyNamed("Level 3 — Earth's Crust");
        DestroyNamed("Level 4 — Atmosphere");
        DestroyNamed("Level 5 — Outer Space");

        GameObject route = new GameObject("Level Route");
        float cursor = 0f;
        float[] bottoms = new float[Titles.Length];
        float[] tops = new float[Titles.Length];

        for (int i = 0; i < Backgrounds.Length; i++)
        {
            Sprite sprite = LoadSprite(Backgrounds[i]);
            GameObject layer = new GameObject(Titles[i]);
            layer.transform.SetParent(route.transform);
            SpriteRenderer renderer = layer.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = -30 + i;

            float pixelsPerUnit = sprite.pixelsPerUnit > 1f ? sprite.pixelsPerUnit : 100f;
            float nativeWidth = sprite.rect.width / pixelsPerUnit;
            float nativeHeight = sprite.rect.height / pixelsPerUnit;
            float scale = LayerWidth / nativeWidth;
            float height = nativeHeight * scale;
            float pivotY = sprite.pivot.y / pixelsPerUnit;
            layer.transform.localScale = new Vector3(scale, scale, 1f);
            layer.transform.position = new Vector3(0f, cursor + pivotY * scale, 0f);
            bottoms[i] = cursor;
            tops[i] = cursor + height;
            cursor += height;
        }

        Sprite platformSprite = LoadSprite("Assets/Art/Platform.png");
        GameObject platformRoot = GameObject.Find("Platforms");
        if (platformRoot == null)
            platformRoot = new GameObject("Platforms");

        for (int i = platformRoot.transform.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(platformRoot.transform.GetChild(i).gameObject);

        float firstStep = bottoms[0] + 1.15f;
        int step = 0;
        float previousX = 0f;
        if (platformSprite != null)
        {
            for (float y = firstStep; y < cursor - 1.4f;)
            {
                GameObject platform = new GameObject("Step " + (step + 1));
                platform.transform.SetParent(platformRoot.transform);
                bool floor = step == 0;
                float x = floor ? 0f : NextEditorPlatformX(ref previousX);
                if (floor)
                    previousX = 0f;
                float width = floor ? 6.5f : Random.Range(1.8f, 2.6f);
                platform.transform.position = new Vector3(x, y, 0f);
                platform.transform.localScale = new Vector3(width, floor ? 0.32f : 0.24f, 1f);
                SpriteRenderer renderer = platform.AddComponent<SpriteRenderer>();
                renderer.sprite = platformSprite;
                renderer.sortingOrder = 0;
                renderer.color = floor
                    ? new Color(0.82f, 0.95f, 1f, 0.95f)
                    : new Color(0.78f, 0.94f, 1f, 0.88f);
                BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
                collider.size = Vector2.one;
                collider.usedByEffector = true;
                PlatformEffector2D effector = platform.AddComponent<PlatformEffector2D>();
                effector.useOneWay = true;
                effector.surfaceArc = 160f;
                if (!floor)
                    platform.AddComponent<CrumblePlatform>().standTime = 2f;
                if (step % 3 == 2)
                    SpikeHazard.BuildUnder(platform.transform);
                step++;
                y += floor ? 2.1f : Random.Range(2.05f, 2.55f);
            }
        }

        GameObject objectives = new GameObject("Level Objectives");
        for (int level = 0; level < ObjectivePaths.Length; level++)
        {
            Sprite icon = LoadSprite(ObjectivePaths[level]);
            if (icon == null)
                continue;

            Transform best = null;
            foreach (Transform child in platformRoot.transform)
            {
                float y = child.position.y;
                if (y < bottoms[level] + 0.4f || y > tops[level] - 0.4f)
                    continue;
                if (best == null || y > best.position.y)
                    best = child;
            }

            GameObject goal = new GameObject("Objective " + (level + 1));
            goal.transform.SetParent(objectives.transform);
            goal.transform.position = best != null
                ? best.position + new Vector3(0f, 1.05f, 0f)
                : new Vector3(0f, (bottoms[level] + tops[level]) * 0.5f, 0f);
            SpriteRenderer renderer = goal.AddComponent<SpriteRenderer>();
            renderer.sprite = icon;
            renderer.sortingOrder = 8;
            float height = icon.bounds.size.y;
            float scale = height > 0.01f ? 1.25f / height : 1f;
            goal.transform.localScale = new Vector3(scale, scale, 1f);
            ObjectivePickup pickup = goal.AddComponent<ObjectivePickup>();
            pickup.levelNumber = level + 1;
            pickup.isCoin = false;
        }

        GameObject player = GameObject.Find("Explorer");
        if (player != null)
            player.transform.position = new Vector3(0f, firstStep + 0.35f, 0f);

        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.orthographic = true;
            camera.orthographicSize = 4.32f;
            camera.backgroundColor = new Color(0.02f, 0.02f, 0.05f, 1f);
            CoreCameraFollow follow = camera.GetComponent<CoreCameraFollow>();
            if (follow == null)
                follow = camera.gameObject.AddComponent<CoreCameraFollow>();
            follow.target = player != null ? player.transform : null;
            follow.minY = bottoms[0] + camera.orthographicSize;
            follow.maxY = cursor - camera.orthographicSize;
            camera.transform.position = new Vector3(0f, follow.minY, -10f);
        }

        if (GameObject.Find("Level Linker") == null)
        {
            GameObject linker = new GameObject("Level Linker");
            linker.AddComponent<LevelLinker>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Connected 5 stages: Core, Mantle, Earth, Atmosphere, Outer Space.");
    }

    static float NextEditorPlatformX(ref float previousX)
    {
        float direction = Random.value < 0.5f ? -1f : 1f;
        float x = Mathf.Clamp(previousX + direction * Random.Range(1.8f, 4.2f), -4.4f, 4.4f);
        if (Mathf.Abs(x - previousX) < 1.3f)
            x = Mathf.Clamp(-previousX + Random.Range(-1.2f, 1.2f), -4.4f, 4.4f);
        previousX = x;
        return x;
    }

    static void DestroyIfFound(string name)
    {
        GameObject found = GameObject.Find(name);
        if (found != null)
            Object.DestroyImmediate(found);
    }

    static void DestroyNamed(string name)
    {
        GameObject found = GameObject.Find(name);
        if (found != null)
            Object.DestroyImmediate(found);
    }
}
