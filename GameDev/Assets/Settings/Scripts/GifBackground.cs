using UnityEngine;

public class GifBackground : MonoBehaviour
{
    public float framesPerSecond = 10f;

    SpriteRenderer spriteRenderer;
    Sprite[] frames;
    float timer;
    int index;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        frames = Resources.LoadAll<Sprite>("CoreBg");
        System.Array.Sort(frames, (a, b) => string.CompareOrdinal(a.name, b.name));
        if (spriteRenderer == null || frames == null || frames.Length == 0)
            return;

        spriteRenderer.sprite = frames[0];
    }

    void Update()
    {
        if (spriteRenderer == null || frames == null || frames.Length < 2)
            return;

        timer += Time.deltaTime;
        float step = 1f / Mathf.Max(1f, framesPerSecond);
        if (timer < step)
            return;

        timer -= step;
        index = (index + 1) % frames.Length;
        spriteRenderer.sprite = frames[index];
    }
}
