using UnityEngine;

public class ObjectivePickup : MonoBehaviour
{
    public int levelNumber = 1;
    public bool isCoin;

    float readyAt;

    void Awake()
    {
        CircleCollider2D trigger = GetComponent<CircleCollider2D>();
        if (trigger == null)
            trigger = gameObject.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = 0.55f;
    }

    void Start()
    {
        readyAt = Time.time + 0.6f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time < readyAt)
            return;
        if (other.GetComponent<CoreClimber>() == null && other.GetComponentInParent<CoreClimber>() == null)
            return;

        ObjectiveTracker.Collect(levelNumber, isCoin);
        if (levelNumber == 1 && !isCoin)
            LevelGate.RegisterCoreCollected();
        Destroy(gameObject);
    }
}
