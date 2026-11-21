using System.Collections;
using UnityEngine;

public class CrumblePlatform : MonoBehaviour
{
    public float standTime = 2f;
    public float respawnDelay = 3f;

    float timer;
    bool occupied;
    bool gone;
    SpriteRenderer spriteRenderer;
    Collider2D solid;
    Color baseColor;
    Coroutine respawn;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        solid = GetComponent<Collider2D>();
        baseColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
    }

    void Update()
    {
        if (gone || !occupied)
            return;

        timer += Time.deltaTime;
        if (spriteRenderer != null)
        {
            float t = Mathf.Clamp01(timer / standTime);
            Color warn = new Color(1f, 0.55f, 0.7f, 0.95f);
            spriteRenderer.color = Color.Lerp(baseColor, warn, t);
        }

        if (timer >= standTime)
            Break();
    }

    public void SetOccupied(bool value)
    {
        if (gone)
            return;

        occupied = value;
        if (value)
            return;

        timer = 0f;
        if (spriteRenderer != null)
            spriteRenderer.color = baseColor;
    }

    public void Restore()
    {
        if (respawn != null)
        {
            StopCoroutine(respawn);
            respawn = null;
        }

        gone = false;
        occupied = false;
        timer = 0f;
        if (solid != null)
            solid.enabled = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = baseColor;
        }

        SetSpikesActive(true);
    }

    void Break()
    {
        if (gone)
            return;

        gone = true;
        occupied = false;
        if (solid != null)
            solid.enabled = false;
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        SetSpikesActive(false);

        CoreClimber climber = FindAnyObjectByType<CoreClimber>();
        if (climber != null)
            climber.PlatformCollapsed(this);

        if (respawn != null)
            StopCoroutine(respawn);
        respawn = StartCoroutine(RespawnLater());
    }

    IEnumerator RespawnLater()
    {
        yield return new WaitForSeconds(respawnDelay);
        respawn = null;
        Restore();
    }

    void SetSpikesActive(bool active)
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(active);
    }
}
