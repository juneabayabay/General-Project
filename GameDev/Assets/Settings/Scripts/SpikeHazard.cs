using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    static Sprite sharedSprite;

    public static void BuildUnder(Transform platform)
    {
        if (platform == null)
            return;

        float parentWidth = Mathf.Max(0.2f, platform.localScale.x);
        float parentHeight = Mathf.Max(0.05f, platform.localScale.y);
        int count = Mathf.Max(2, Mathf.RoundToInt(parentWidth / 0.36f));
        float spikeHeight = 0.36f;
        float spikeWidth = 0.26f;

        for (int i = 0; i < count; i++)
        {
            float along = (i + 0.5f) / count;
            GameObject spike = new GameObject("Spike");
            spike.transform.SetParent(platform, false);
            spike.transform.localScale = new Vector3(spikeWidth / parentWidth, spikeHeight / parentHeight, 1f);
            spike.transform.localPosition = new Vector3(Mathf.Lerp(-0.5f, 0.5f, along), -0.5f, 0f);

            SpriteRenderer renderer = spike.AddComponent<SpriteRenderer>();
            renderer.sprite = SpikeSprite();
            renderer.color = Color.black;
            renderer.sortingOrder = 3;

            BoxCollider2D hit = spike.AddComponent<BoxCollider2D>();
            hit.isTrigger = true;
            hit.size = new Vector2(0.7f, 0.85f);
            hit.offset = new Vector2(0f, -0.35f);
            spike.AddComponent<SpikeHazard>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryKill(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryKill(other);
    }

    static void TryKill(Collider2D other)
    {
        CoreClimber climber = other.GetComponent<CoreClimber>();
        if (climber == null)
            climber = other.GetComponentInParent<CoreClimber>();
        if (climber == null)
            return;

        Rigidbody2D body = climber.GetComponent<Rigidbody2D>();
        if (body != null && body.linearVelocity.y > 0.35f)
            return;

        climber.TouchSpike();
    }

    static Sprite SpikeSprite()
    {
        if (sharedSprite != null)
            return sharedSprite;

        const int size = 16;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        Color[] pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            float half = (y / (float)(size - 1)) * (size * 0.5f);
            for (int x = 0; x < size; x++)
            {
                float fromCenter = Mathf.Abs(x - (size - 1) * 0.5f);
                pixels[y * size + x] = fromCenter <= half ? Color.white : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        sharedSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 1f), size);
        return sharedSprite;
    }
}
