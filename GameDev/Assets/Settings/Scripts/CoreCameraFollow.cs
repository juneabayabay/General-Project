using UnityEngine;

public class CoreCameraFollow : MonoBehaviour
{
    public Transform target;
    public float lookAhead = 1.2f;
    public float minY = -9.44f;
    public float maxY = 118f;
    public float smoothTime = 0.18f;

    float smoothVelocity;

    void LateUpdate()
    {
        if (target == null)
            return;

        float y = Mathf.Clamp(target.position.y + lookAhead, minY, maxY);
        Vector3 position = transform.position;
        position.x = 0f;
        position.y = Mathf.SmoothDamp(position.y, y, ref smoothVelocity, smoothTime);
        transform.position = position;
    }
}
