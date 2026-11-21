using UnityEngine;

public class LevelGate : MonoBehaviour
{
    public int required = 5;

    static int collected;

    Collider2D blocker;
    SpriteRenderer spriteRenderer;
    SpriteRenderer mantle;
    int initial;
    bool open;
    bool ready;
    bool announced;

    void Awake()
    {
        collected = 0;
        blocker = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mantle = FindMantle();
        if (mantle != null)
            mantle.enabled = false;
    }

    void Start()
    {
        initial = CountCorePickups();
        if (initial > 0)
            required = initial;
        ready = true;
    }

    public static void RegisterCoreCollected()
    {
        collected++;
        Refresh();
    }

    public static void Refresh()
    {
        LevelGate[] gates = FindObjectsByType<LevelGate>(FindObjectsInactive.Exclude);
        foreach (LevelGate gate in gates)
            gate.Apply();
    }

    void Update()
    {
        if (!ready || open)
            return;

        if (collected >= required
            || ObjectiveTracker.CollectedCount(1) >= required
            || CountCorePickups() == 0)
            Apply();
    }

    void Apply()
    {
        if (mantle == null)
            mantle = FindMantle();

        bool cleared = initial > 0 && (
            collected >= required
            || ObjectiveTracker.CollectedCount(1) >= required
            || (ready && CountCorePickups() == 0));

        if (!cleared)
            return;

        open = true;
        if (blocker != null)
            blocker.enabled = false;
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
        if (mantle != null)
        {
            mantle.gameObject.SetActive(true);
            mantle.enabled = true;
        }

        ObjectiveTracker.ShowNotice("Mantle unlocked.");
    }

    static int CountCorePickups()
    {
        int waiting = 0;
        ObjectivePickup[] pickups = FindObjectsByType<ObjectivePickup>(FindObjectsInactive.Exclude);
        foreach (ObjectivePickup pickup in pickups)
        {
            if (pickup != null && pickup.levelNumber == 1 && !pickup.isCoin)
                waiting++;
        }

        return waiting;
    }

    static SpriteRenderer FindMantle()
    {
        GameObject named = GameObject.Find("Level 2 - Mantle");
        if (named != null)
        {
            SpriteRenderer renderer = named.GetComponent<SpriteRenderer>();
            if (renderer != null)
                return renderer;
        }

        SpriteRenderer[] renderers = FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include);
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.gameObject.name.Contains("Mantle"))
                return renderer;
        }

        return null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (open || announced)
            return;
        if (collision.collider.GetComponent<CoreClimber>() == null
            && collision.collider.GetComponentInParent<CoreClimber>() == null)
            return;

        announced = true;
        ObjectiveTracker.ShowNotice("Collect every Core objective to unlock the Mantle.");
    }
}
