using UnityEngine;
using UnityEngine.InputSystem;

public class CoreClimber : MonoBehaviour
{
    public float moveSpeed = 7.4f;
    public float groundAcceleration = 55f;
    public float airAcceleration = 42f;
    public float jumpSpeed = 7f;
    public float doubleJumpReach = 2.75f;
    public float coyoteTime = 0.14f;
    public float jumpBufferTime = 0.14f;
    public int lives = 3;

    Rigidbody2D body;
    SpriteRenderer spriteRenderer;
    Vector3 runStart;
    Vector3 safePoint;
    Vector3 previousSafePoint;
    float lastGroundedTime = -10f;
    float lastJumpPressedTime = -10f;
    float failLockUntil;
    int airJumps = 1;
    bool jumpedSinceGround;
    float jumpStartY;
    float leaveGroundUntil;
    SlidingPlatform riding;

    public int JumpId { get; private set; }
    public bool Jumping { get; private set; }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        body.gravityScale = 1.8f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        if (GetComponent<CharacterAnimator>() == null)
            gameObject.AddComponent<CharacterAnimator>();

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = new Vector2(0.36f, 1.15f);
            collider.offset = new Vector2(0f, 0.58f);
        }
    }

    void Start()
    {
        SnapToLowestPlatform();
        runStart = transform.position;
        safePoint = runStart;
        previousSafePoint = runStart;
        ObjectiveTracker.SetLives(lives);
    }

    void SnapToLowestPlatform()
    {
        Collider2D lowest = null;
        foreach (Collider2D collider in FindObjectsByType<Collider2D>(FindObjectsInactive.Exclude))
        {
            if (!collider.enabled || collider.isTrigger || collider.attachedRigidbody == body)
                continue;

            if (lowest == null || collider.bounds.min.y < lowest.bounds.min.y)
                lowest = collider;
        }

        if (lowest == null)
            return;

        BoxCollider2D own = GetComponent<BoxCollider2D>();
        float feet = own != null ? own.offset.y - own.size.y * 0.5f : 0f;
        Vector3 stand = new Vector3(lowest.bounds.center.x, lowest.bounds.max.y - feet + 0.02f, 0f);
        transform.position = stand;
        body.position = stand;
        body.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        if (Time.time >= failLockUntil && transform.position.y < safePoint.y - 3.5f)
            Fail(safePoint);

        if (JumpPressed())
            lastJumpPressedTime = Time.time;
    }

    void FixedUpdate()
    {
        float x = ReadHorizontal();
        bool recentlyGrounded = Time.time - lastGroundedTime <= coyoteTime;
        float acceleration = recentlyGrounded ? groundAcceleration : airAcceleration;

        Vector2 velocity = body.linearVelocity;
        float ride = recentlyGrounded && riding != null ? riding.VelocityX : 0f;
        float targetSpeed = x * moveSpeed + ride;
        velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);

        bool bufferedJump = Time.time - lastJumpPressedTime <= jumpBufferTime;
        if (recentlyGrounded && !jumpedSinceGround && bufferedJump && Time.time >= leaveGroundUntil)
        {
            jumpStartY = body.position.y;
            body.position += new Vector2(0f, 0.08f);
            velocity.y = jumpSpeed;
            lastGroundedTime = -10f;
            lastJumpPressedTime = -10f;
            jumpedSinceGround = true;
            Jumping = true;
            JumpId++;
            leaveGroundUntil = Time.time + 0.16f;
        }
        else if (!recentlyGrounded && airJumps > 0 && bufferedJump)
        {
            float gravity = Mathf.Abs(Physics2D.gravity.y) * body.gravityScale;
            float rise = (jumpStartY + doubleJumpReach) - body.position.y;
            if (rise > 0.2f)
            {
                float needed = Mathf.Sqrt(2f * gravity * rise);
                velocity.y = Mathf.Max(velocity.y, Mathf.Min(needed, jumpSpeed));
            }
            airJumps--;
            lastGroundedTime = -10f;
            lastJumpPressedTime = -10f;
            jumpedSinceGround = true;
            Jumping = true;
            JumpId++;
            leaveGroundUntil = Time.time + 0.12f;
        }

        float clampedX = Mathf.Clamp(body.position.x, -6.6f, 6.6f);
        if (!Mathf.Approximately(clampedX, body.position.x))
        {
            body.position = new Vector2(clampedX, body.position.y);
            velocity.x = 0f;
        }

        body.linearVelocity = velocity;

        if (x != 0f && spriteRenderer != null)
            spriteRenderer.flipX = x < 0f;
    }

    public void PlatformCollapsed(CrumblePlatform platform)
    {
        if (Time.time < failLockUntil || platform == null)
            return;

        float feet = transform.position.y;
        float top = platform.transform.position.y + 1.2f;
        if (feet > top || feet < platform.transform.position.y - 1.5f)
            return;

        Fail(previousSafePoint);
    }

    void Fail(Vector3 respawn)
    {
        if (Time.time < failLockUntil)
            return;

        lives--;
        if (lives <= 0)
        {
            lives = 3;
            foreach (CrumblePlatform platform in FindObjectsByType<CrumblePlatform>(FindObjectsInactive.Exclude))
                platform.Restore();
            respawn = runStart;
            safePoint = runStart;
            previousSafePoint = runStart;
        }

        ObjectiveTracker.SetLives(lives);
        Respawn(respawn);
    }

    public void TouchSpike()
    {
        Fail(safePoint);
    }

    void Respawn(Vector3 point)
    {
        transform.position = point;
        body.linearVelocity = Vector2.zero;
        body.position = point;
        lastGroundedTime = Time.time;
        lastJumpPressedTime = -10f;
        airJumps = 1;
        jumpedSinceGround = false;
        Jumping = false;
        leaveGroundUntil = 0f;
        failLockUntil = Time.time + 0.45f;
    }

    void RememberStand(Vector3 point)
    {
        if ((point - safePoint).sqrMagnitude < 0.15f)
            return;

        previousSafePoint = safePoint;
        safePoint = point;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < leaveGroundUntil || body.linearVelocity.y > 0.2f)
            return;

        if (!HasGroundContact(collision))
            return;

        Jumping = false;

        riding = collision.collider.GetComponent<SlidingPlatform>();
        lastGroundedTime = Time.time;
        airJumps = 1;
        jumpedSinceGround = false;
        jumpStartY = body.position.y;
        Bounds bounds = collision.collider.bounds;
        RememberStand(new Vector3(collision.transform.position.x, bounds.max.y + 0.15f, 0f));

        CrumblePlatform crumble = collision.collider.GetComponent<CrumblePlatform>();
        if (crumble != null)
            crumble.SetOccupied(true);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        CrumblePlatform crumble = collision.collider.GetComponent<CrumblePlatform>();
        if (crumble != null)
            crumble.SetOccupied(false);
        if (collision.collider.GetComponent<SlidingPlatform>() == riding)
            riding = null;
    }

    static bool HasGroundContact(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.4f)
                return true;
        }

        return false;
    }

    static float ReadHorizontal()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return 0f;

        float x = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            x += 1f;
        return Mathf.Clamp(x, -1f, 1f);
    }

    static bool JumpPressed()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return false;

        return keyboard.spaceKey.wasPressedThisFrame
            || keyboard.wKey.wasPressedThisFrame
            || keyboard.upArrowKey.wasPressedThisFrame;
    }
}
