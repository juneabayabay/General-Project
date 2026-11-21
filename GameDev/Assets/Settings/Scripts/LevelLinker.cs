using System.Collections.Generic;
using UnityEngine;

public class LevelLinker : MonoBehaviour
{
    public static LevelLinker Instance { get; private set; }

    public float[] BandBottom { get; private set; } = new float[5];
    public float[] BandTop { get; private set; } = new float[5];

    const float TargetWidth = 15.36f;
    const float SeamOverlap = 0.04f;

    static readonly string[] LevelNames =
    {
        "Core",
        "Mantle",
        "Earth",
        "Atmosphere",
        "Outer Space"
    };

    readonly List<Transform> steps = new List<Transform>();

    void Awake()
    {
        Instance = this;
        float worldTop = StackBackgrounds();
        BuildSteps(worldTop);
        PlaceObjectives();
        PlacePlayerAtStart();
        FitCamera(worldTop);

        if (FindAnyObjectByType<ObjectiveTracker>() == null)
            gameObject.AddComponent<ObjectiveTracker>();
    }

    public int LevelAt(float y)
    {
        for (int i = BandTop.Length - 1; i >= 0; i--)
        {
            if (BandTop[i] > BandBottom[i] && y >= BandBottom[i])
                return i;
        }

        return 0;
    }

    float StackBackgrounds()
    {
        float cursor = 0f;

        for (int i = 0; i < LevelNames.Length; i++)
        {
            BandBottom[i] = cursor;
            BandTop[i] = cursor;

            GameObject level = FindLevel(LevelNames[i]);
            if (level == null)
                continue;

            SpriteRenderer renderer = level.GetComponent<SpriteRenderer>();
            if (renderer == null || renderer.sprite == null)
                continue;

            Sprite sprite = renderer.sprite;
            float pixelsPerUnit = sprite.pixelsPerUnit > 1f ? sprite.pixelsPerUnit : 100f;
            float nativeWidth = sprite.rect.width / pixelsPerUnit;
            float nativeHeight = sprite.rect.height / pixelsPerUnit;
            if (nativeWidth < 0.01f || nativeHeight < 0.01f)
                continue;

            float scale = TargetWidth / nativeWidth;
            float height = nativeHeight * scale;
            float pivotY = sprite.pivot.y / pixelsPerUnit;

            renderer.sortingOrder = -30 + i;
            renderer.enabled = true;
            level.transform.localScale = new Vector3(scale, scale, 1f);
            level.transform.position = new Vector3(0f, cursor + pivotY * scale, 1f);

            BandBottom[i] = cursor;
            BandTop[i] = cursor + height;
            cursor += height - SeamOverlap;
        }

        return Mathf.Max(cursor, BandTop[LevelNames.Length - 1]);
    }

    static GameObject FindLevel(string keyword)
    {
        SpriteRenderer[] renderers = FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Exclude);
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.sortingOrder > -5)
                continue;
            if (renderer.gameObject.name.Contains(keyword))
                return renderer.gameObject;
        }

        return null;
    }

    void BuildSteps(float worldTop)
    {
        steps.Clear();

        GameObject previous = GameObject.Find("Linked Steps");
        if (previous != null)
        {
            previous.SetActive(false);
            Destroy(previous);
        }

        Sprite platformSprite = null;
        GameObject platformRoot = GameObject.Find("Platforms");
        if (platformRoot != null)
        {
            SpriteRenderer existing = platformRoot.GetComponentInChildren<SpriteRenderer>(true);
            if (existing != null)
                platformSprite = existing.sprite;

            for (int i = platformRoot.transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = platformRoot.transform.GetChild(i).gameObject;
                child.SetActive(false);
                Destroy(child);
            }
        }

        if (platformSprite == null || worldTop <= BandBottom[0] + 2f)
            return;

        GameObject parent = new GameObject("Linked Steps");
        float previousX = 0f;
        int index = 0;
        float y = BuildCorePlatforms(parent.transform, platformSprite, ref previousX, ref index);
        while (y < worldTop - 1.35f)
        {
            float x = NextPlatformX(previousX);
            float width = Random.Range(1.8f, 2.6f);
            Transform step = CreateStep(parent.transform, platformSprite, index, x, y, width, false);
            if (Mathf.Abs(x - previousX) > 2.4f)
                MakeSliding(step);
            steps.Add(step);
            previousX = x;
            y += Random.Range(2.05f, 2.55f);
            index++;
        }

        BuildLevelGate(platformSprite);
    }

    float BuildCorePlatforms(Transform parent, Sprite platformSprite, ref float previousX, ref int index)
    {
        const int needed = 18;
        float start = BandBottom[0] + 1.15f;
        float limit = BandTop[0] - 1.35f;
        var rows = new List<float>();
        float cursor = start;
        rows.Add(cursor);
        while (cursor + 2.15f <= limit)
        {
            cursor += 2.15f;
            rows.Add(cursor);
        }

        int extras = Mathf.Max(0, needed - rows.Count);
        for (int row = 0; row < rows.Count && index < needed; row++)
        {
            bool floor = row == 0;
            float x = floor ? 0f : NextPlatformX(previousX);
            float width = floor ? 6.5f : Random.Range(1.8f, 2.6f);
            Transform step = CreateStep(parent, platformSprite, index, x, rows[row], width, floor);
            if (!floor)
                MakeSliding(step);
            steps.Add(step);
            previousX = x;
            index++;

            if (floor || extras <= 0 || index >= needed)
                continue;

            float other = x >= 0f ? Random.Range(-4.2f, -1.5f) : Random.Range(1.5f, 4.2f);
            float otherWidth = Random.Range(1.8f, 2.6f);
            Transform companion = CreateStep(parent, platformSprite, index, other, rows[row], otherWidth, false);
            MakeSliding(companion);
            steps.Add(companion);
            extras--;
            index++;
        }

        return cursor + Random.Range(2.05f, 2.55f);
    }

    void BuildLevelGate(Sprite platformSprite)
    {
        GameObject gate = new GameObject("Level 2 Gate");
        gate.transform.position = new Vector3(0f, BandTop[0] + 0.8f, 0f);
        gate.transform.localScale = new Vector3(16f, 3.4f, 1f);
        if (platformSprite != null)
        {
            SpriteRenderer renderer = gate.AddComponent<SpriteRenderer>();
            renderer.sprite = platformSprite;
            renderer.sortingOrder = 6;
            renderer.color = new Color(1f, 0.28f, 0.16f, 0.9f);
        }

        BoxCollider2D blocker = gate.AddComponent<BoxCollider2D>();
        blocker.size = Vector2.one;
        gate.AddComponent<LevelGate>();
    }

    static void MakeSliding(Transform step)
    {
        SlidingPlatform slide = step.gameObject.AddComponent<SlidingPlatform>();
        slide.Begin(Random.Range(2.2f, 3.1f), Random.Range(0.85f, 1.25f));
        SpriteRenderer glass = step.GetComponent<SpriteRenderer>();
        if (glass != null)
            glass.color = new Color(0.65f, 0.9f, 1f, 0.92f);
    }

    static float NextPlatformX(float previousX)
    {
        float direction = Random.value < 0.5f ? -1f : 1f;
        float x = Mathf.Clamp(previousX + direction * Random.Range(1.8f, 4.2f), -4.4f, 4.4f);
        if (Mathf.Abs(x - previousX) < 1.3f)
            x = Mathf.Clamp(-previousX + Random.Range(-1.2f, 1.2f), -4.4f, 4.4f);
        return x;
    }

    static Transform CreateStep(Transform parent, Sprite platformSprite, int index, float x, float y, float width, bool safeFloor)
    {
        GameObject step = new GameObject("Link " + (index + 1));
        step.transform.SetParent(parent);
        step.transform.position = new Vector3(x, y, 0f);
        step.transform.localScale = new Vector3(width, safeFloor ? 0.32f : 0.24f, 1f);

        SpriteRenderer renderer = step.AddComponent<SpriteRenderer>();
        renderer.sprite = platformSprite;
        renderer.sortingOrder = 0;
        renderer.color = safeFloor
            ? new Color(0.82f, 0.95f, 1f, 0.95f)
            : new Color(0.78f, 0.94f, 1f, 0.88f);

        BoxCollider2D collider = step.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        collider.usedByEffector = true;
        PlatformEffector2D effector = step.AddComponent<PlatformEffector2D>();
        effector.useOneWay = true;
        effector.surfaceArc = 160f;
        if (!safeFloor)
            step.AddComponent<CrumblePlatform>().standTime = 2f;
        if (index % 3 == 2)
            SpikeHazard.BuildUnder(step.transform);
        return step.transform;
    }

    void PlaceObjectives()
    {
        for (int level = 1; level <= 5; level++)
        {
            GameObject goal = GameObject.Find("Objective " + level);
            if (goal == null)
                continue;

            Transform best = HighestStep(BandBottom[level - 1], BandTop[level - 1]);
            if (best != null)
            {
                if (level == 1)
                {
                    goal.transform.SetParent(null, true);
                    goal.transform.position = InsideCore(best.position + new Vector3(0f, 1.2f, 0f));
                }
                else
                {
                    goal.transform.SetParent(best, true);
                    goal.transform.position = best.position + new Vector3(0f, 1.05f, 0f);
                }
            }

            SpriteRenderer renderer = goal.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sprite != null)
            {
                float height = renderer.sprite.bounds.size.y;
                float scale = height > 0.01f ? 1.25f / height : 1f;
                Vector3 lossy = goal.transform.parent != null ? goal.transform.parent.lossyScale : Vector3.one;
                goal.transform.localScale = new Vector3(
                    scale / Mathf.Max(0.01f, lossy.x),
                    scale / Mathf.Max(0.01f, lossy.y),
                    1f);
            }

            ObjectivePickup pickup = goal.GetComponent<ObjectivePickup>();
            if (pickup == null)
                pickup = goal.AddComponent<ObjectivePickup>();
            pickup.levelNumber = level;
            pickup.isCoin = false;
            if (level == 1)
                PlaceCoreObjectives(goal, renderer != null ? renderer.sprite : null);
            else
                SpawnExtraObjectives(goal, level, renderer != null ? renderer.sprite : null);
        }
    }

    void PlaceCoreObjectives(GameObject goal, Sprite icon)
    {
        SpriteRenderer goalRenderer = goal.GetComponent<SpriteRenderer>();
        if (icon == null && goalRenderer != null)
            icon = goalRenderer.sprite;
        if (icon == null)
            return;

        var places = new System.Collections.Generic.List<Transform>();
        foreach (Transform step in steps)
        {
            if (step.position.y < BandBottom[0] + 0.8f || step.position.y > BandTop[0] - 2.2f)
                continue;
            places.Add(step);
        }

        places.Sort((a, b) => a.position.y.CompareTo(b.position.y));
        if (places.Count == 0)
            return;

        var used = new System.Collections.Generic.HashSet<int>();
        for (int n = 0; n < 5; n++)
        {
            int index = Mathf.Clamp(Mathf.RoundToInt((n + 0.5f) * (places.Count - 1) / 5f), 0, places.Count - 1);
            while (used.Contains(index) && index < places.Count - 1)
                index++;
            used.Add(index);

            GameObject gem = n == 0 ? goal : new GameObject("Objective 1 Extra " + n);
            float side = n % 2 == 0 ? 0.75f : -0.75f;
            gem.transform.SetParent(null, true);
            gem.transform.position = InsideCore(places[index].position + new Vector3(side, 1.25f, 0f));
            ShowObjective(gem, icon);

            ObjectivePickup pickup = gem.GetComponent<ObjectivePickup>();
            if (pickup == null)
                pickup = gem.AddComponent<ObjectivePickup>();
            pickup.levelNumber = 1;
            pickup.isCoin = false;
        }
    }

    static void ShowObjective(GameObject gem, Sprite icon)
    {
        SpriteRenderer renderer = gem.GetComponent<SpriteRenderer>();
        if (renderer == null)
            renderer = gem.AddComponent<SpriteRenderer>();
        renderer.sprite = icon;
        renderer.enabled = true;
        renderer.sortingOrder = 20;
        renderer.color = Color.white;

        float height = icon.bounds.size.y;
        float scale = height > 0.01f ? 1.15f / height : 1f;
        gem.transform.localScale = new Vector3(scale, scale, 1f);
    }

    void SpawnExtraObjectives(GameObject original, int level, Sprite icon)
    {
        if (icon == null)
            return;

        float bottom = BandBottom[level - 1];
        float top = BandTop[level - 1];
        System.Collections.Generic.List<Transform> places = new System.Collections.Generic.List<Transform>();
        foreach (Transform step in steps)
        {
            if (step.position.y < bottom + 0.4f || step.position.y > top - 0.8f)
                continue;
            if (original.transform.parent == step)
                continue;
            places.Add(step);
        }

        places.Sort((a, b) => a.position.y.CompareTo(b.position.y));
        int extras = level == 1 ? 4 : 2;
        int spawned = 0;
        var used = new System.Collections.Generic.HashSet<int>();
        for (int n = 0; n < extras && places.Count > 0; n++)
        {
            int index = Mathf.Clamp(Mathf.RoundToInt((n + 1f) * (places.Count - 1f) / (extras + 1f)), 0, places.Count - 1);
            while (used.Contains(index) && index < places.Count - 1)
                index++;
            used.Add(index);
            GameObject extra = new GameObject("Objective " + level + " Extra " + (spawned + 1));
            Vector3 lossy = Vector3.one;
            if (level == 1)
            {
                extra.transform.position = InsideCore(CoreChallengeSpot(places[index], spawned));
            }
            else
            {
                extra.transform.SetParent(places[index], false);
                extra.transform.position = places[index].position + new Vector3(0f, 1.05f, 0f);
                lossy = places[index].lossyScale;
            }
            Vector3 world = original.transform.lossyScale;
            extra.transform.localScale = new Vector3(
                world.x / Mathf.Max(0.01f, lossy.x),
                world.y / Mathf.Max(0.01f, lossy.y),
                1f);
            SpriteRenderer renderer = extra.AddComponent<SpriteRenderer>();
            renderer.sprite = icon;
            if (renderer != null)
            {
                renderer.enabled = true;
                renderer.sortingOrder = 20;
            }
            ObjectivePickup pickup = extra.AddComponent<ObjectivePickup>();
            pickup.levelNumber = level;
            pickup.isCoin = false;
            spawned++;
        }
    }

    Vector3 InsideCore(Vector3 spot)
    {
        float half = TargetWidth * 0.5f - 1.1f;
        spot.x = Mathf.Clamp(spot.x, -half, half);
        spot.y = Mathf.Clamp(spot.y, BandBottom[0] + 1.4f, BandTop[0] - 1.15f);
        return spot;
    }

    static Vector3 CoreChallengeSpot(Transform step, int which)
    {
        Vector3 at = step.position;
        switch (which)
        {
            case 0:
                return at + new Vector3(-2.8f, 1.65f, 0f);
            case 1:
                return at + new Vector3(0.2f, 2.45f, 0f);
            case 2:
                return at + new Vector3(0f, 1.05f, 0f);
            default:
                return at + new Vector3(-1.4f, 2.15f, 0f);
        }
    }

    Transform HighestStep(float bottom, float top)
    {
        Transform best = null;
        foreach (Transform step in steps)
        {
            float y = step.position.y;
            if (y < bottom + 0.5f || y > top - 0.4f)
                continue;
            if (best == null || y > best.position.y)
                best = step;
        }

        return best;
    }

    void PlacePlayerAtStart()
    {
        GameObject player = GameObject.Find("Explorer");
        if (player == null)
            return;

        Transform floor = steps.Count > 0 ? steps[0] : null;
        float y = BandBottom[0] + 1.2f;
        float x = 0f;
        if (floor != null)
        {
            Collider2D ground = floor.GetComponent<Collider2D>();
            float top = ground != null ? ground.bounds.max.y : floor.position.y + 0.2f;
            y = top + 0.08f;
            x = floor.position.x;
        }

        player.transform.position = new Vector3(x, y, 0f);

        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        if (body != null)
            body.linearVelocity = Vector2.zero;
    }

    void FitCamera(float worldTop)
    {
        Camera camera = Camera.main;
        if (camera == null)
            return;

        CoreCameraFollow follow = camera.GetComponent<CoreCameraFollow>();
        if (follow == null)
            follow = camera.gameObject.AddComponent<CoreCameraFollow>();

        GameObject player = GameObject.Find("Explorer");
        follow.target = player != null ? player.transform : follow.target;
        follow.minY = BandBottom[0] + camera.orthographicSize;
        follow.maxY = Mathf.Max(follow.minY, worldTop - camera.orthographicSize);
        camera.transform.position = new Vector3(0f, follow.minY, -10f);
        Debug.Log("Ascent connected from core through outer space. Platforms: " + steps.Count);
    }
}
