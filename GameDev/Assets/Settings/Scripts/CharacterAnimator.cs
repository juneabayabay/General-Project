using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public float framesPerSecond = 10f;
    public float idleFramesPerSecond = 10f;
    public float targetHeight = 1.15f;

    SpriteRenderer spriteRenderer;
    Sprite[] runFrames;
    Sprite[] idleFrames;
    Sprite[] jumpFrames;
    Sprite[] activeFrames;
    bool activeLoops;
    bool fitted;
    int seenJump;
    float activeRate;
    float timer;
    int frame;

    void Awake()
    {
        runFrames = LoadNumbered("Runner", "run_");
        idleFrames = LoadNumbered("Idle", "idle_");
        jumpFrames = LoadNumbered("Jump", "jump_");
        if (runFrames.Length == 0)
            runFrames = LoadClip("Runner");
        if (idleFrames.Length == 0)
            idleFrames = LoadClip("Idle");

        SpriteRenderer root = GetComponent<SpriteRenderer>();
        GameObject visual = new GameObject("Runner Visual");
        visual.transform.SetParent(transform, false);
        spriteRenderer = visual.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = root != null ? root.sortingOrder : 10;
        if (root != null)
            root.enabled = false;

        Play(idleFrames.Length > 0 ? idleFrames : runFrames, idleFramesPerSecond, true, true);
    }

    static Sprite[] LoadClip(string folder)
    {
        Sprite[] frames = Resources.LoadAll<Sprite>(folder);
        var largest = new System.Collections.Generic.Dictionary<string, Sprite>();
        foreach (Sprite sprite in frames)
        {
            if (sprite == null || sprite.texture == null)
                continue;

            string key = sprite.texture.name;
            Sprite current;
            if (!largest.TryGetValue(key, out current) || sprite.rect.height > current.rect.height)
                largest[key] = sprite;
        }

        var kept = new System.Collections.Generic.List<Sprite>(largest.Values);
        kept.Sort((a, b) => string.CompareOrdinal(a.texture.name, b.texture.name));
        return kept.ToArray();
    }

    static Sprite[] LoadNumbered(string folder, string prefix)
    {
        var frames = new System.Collections.Generic.List<Sprite>();
        for (int i = 0; i < 40; i++)
        {
            Sprite sprite = Resources.Load<Sprite>(folder + "/" + prefix + i.ToString("00"));
            if (sprite == null)
                break;
            frames.Add(sprite);
        }

        return frames.ToArray();
    }

    void FitToFeet(Sprite sprite)
    {
        if (fitted || sprite == null)
            return;

        float unit = sprite.pixelsPerUnit > 1f ? sprite.pixelsPerUnit : 100f;
        float height = sprite.rect.height / unit;
        if (height < 0.01f)
            return;

        float scale = targetHeight / height;
        Transform visual = spriteRenderer.transform;
        visual.localScale = new Vector3(scale, scale, 1f);

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        float feet = collider != null ? collider.offset.y - collider.size.y * 0.5f : 0f;
        float pivotY = sprite.pivot.y / unit;
        visual.localPosition = new Vector3(0f, feet - pivotY * scale, 0f);
        fitted = true;
    }

    void Update()
    {
        if (spriteRenderer == null)
            return;

        if (runFrames == null || runFrames.Length == 0)
            runFrames = LoadNumbered("Runner", "run_");
        if (jumpFrames == null || jumpFrames.Length == 0)
            jumpFrames = LoadNumbered("Jump", "jump_");

        float x = ReadHorizontal();
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        CoreClimber climber = GetComponent<CoreClimber>();
        bool inJump = climber != null && climber.Jumping;
        bool moving = Mathf.Abs(x) > 0.01f || (body != null && Mathf.Abs(body.linearVelocity.x) > 0.45f);

        if (x > 0.01f)
            spriteRenderer.flipX = false;
        else if (x < -0.01f)
            spriteRenderer.flipX = true;
        else if (body != null && Mathf.Abs(body.linearVelocity.x) > 0.45f)
            spriteRenderer.flipX = body.linearVelocity.x < 0f;

        if (inJump && jumpFrames != null && jumpFrames.Length > 0)
        {
            float airTime = JumpAirTime(body);
            float rate = jumpFrames.Length / airTime;
            bool restart = climber.JumpId != seenJump;
            seenJump = climber.JumpId;
            Play(jumpFrames, rate, restart, false);
        }
        else if (moving && runFrames != null && runFrames.Length > 0)
            Play(runFrames, framesPerSecond, false, true);
        else if (idleFrames != null && idleFrames.Length > 0)
            Play(idleFrames, idleFramesPerSecond, false, true);
    }

    float JumpAirTime(Rigidbody2D body)
    {
        float speed = 7f;
        CoreClimber climber = GetComponent<CoreClimber>();
        if (climber != null)
            speed = climber.jumpSpeed;
        float gravity = Mathf.Abs(Physics2D.gravity.y) * (body != null ? body.gravityScale : 1.8f);
        if (gravity < 0.01f)
            gravity = 17.6f;
        return Mathf.Max(0.35f, 2f * speed / gravity);
    }

    void Play(Sprite[] clip, float rate, bool restart, bool loop)
    {
        if (clip == null || clip.Length == 0)
            return;

        if (restart || activeFrames != clip)
        {
            activeFrames = clip;
            activeRate = rate;
            activeLoops = loop;
            frame = 0;
            timer = 0f;
            FitToFeet(clip[0]);
            Show(clip[0]);
            return;
        }

        activeRate = rate;
        activeLoops = loop;
        if (!activeLoops && frame >= clip.Length - 1)
        {
            Show(clip[clip.Length - 1]);
            return;
        }

        timer += Time.deltaTime;
        float step = 1f / Mathf.Max(1f, activeRate);
        if (timer < step)
            return;

        timer -= step;
        frame++;
        if (activeLoops)
            frame %= clip.Length;
        else if (frame >= clip.Length)
            frame = clip.Length - 1;
        Show(clip[frame]);
    }

    void Show(Sprite sprite)
    {
        spriteRenderer.color = Climber.Current;
        if (spriteRenderer.sprite == sprite)
            return;

        spriteRenderer.sprite = sprite;
    }

    static float ReadHorizontal()
    {
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard == null)
            return 0f;

        float x = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            x += 1f;
        return Mathf.Clamp(x, -1f, 1f);
    }
}
