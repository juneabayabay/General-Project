using UnityEngine;

public class SlidingPlatform : MonoBehaviour
{
    public float VelocityX { get; private set; }

    float homeX;
    float homeY;
    float amplitude = 2.4f;
    float speed = 1.15f;
    float phase;
    Rigidbody2D body;

    public void Begin(float width, float moveSpeed)
    {
        homeX = transform.position.x;
        homeY = transform.position.y;
        float leftRoom = homeX + 5.2f;
        float rightRoom = 5.2f - homeX;
        amplitude = Mathf.Min(width, leftRoom, rightRoom);
        amplitude = Mathf.Max(1.6f, amplitude);
        if (homeX - amplitude < -5.2f)
            homeX = -5.2f + amplitude;
        if (homeX + amplitude > 5.2f)
            homeX = 5.2f - amplitude;
        speed = moveSpeed;
        phase = 0f;
    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        if (body == null)
            body = gameObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.useFullKinematicContacts = true;
    }

    void Start()
    {
        if (Mathf.Approximately(homeY, 0f) && Mathf.Approximately(homeX, 0f))
        {
            homeX = transform.position.x;
            homeY = transform.position.y;
        }

        Collider2D mine = GetComponent<Collider2D>();
        SlidingPlatform[] others = FindObjectsByType<SlidingPlatform>(FindObjectsInactive.Exclude);
        foreach (SlidingPlatform other in others)
        {
            if (other == this)
                continue;
            Collider2D theirs = other.GetComponent<Collider2D>();
            if (mine != null && theirs != null)
                Physics2D.IgnoreCollision(mine, theirs, true);
        }
    }

    void FixedUpdate()
    {
        float angle = phase + Time.time * speed;
        float x = homeX + Mathf.Sin(angle) * amplitude;
        VelocityX = Mathf.Cos(angle) * amplitude * speed;
        body.MovePosition(new Vector2(x, homeY));
    }
}
