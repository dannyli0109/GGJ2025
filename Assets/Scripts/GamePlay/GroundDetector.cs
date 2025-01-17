using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public enum DetectType
    {
        Box,
        Circle
    }
    public DetectType detectType;
    public LayerMask groundLayerMask;
    [HideInInspector] public Collider2D groundCollider;
    float detectDistance;
    BoxCollider2D col;

    private void Awake()
    {
        col = transform.parent.GetComponent<BoxCollider2D>();
        detectDistance = col.size.x / 2;
    }

    public void Detect()
    {
        switch (detectType)
        {
            case DetectType.Box:
                groundCollider = BoxDetect();
                break;
            case DetectType.Circle:
                groundCollider = CircleDetect();
                break;
            default:
                groundCollider = null;
                break;
        }
    }

    public Collider2D BoxDetect()
    {
        return Physics2D.BoxCast(col.bounds.center, col.size, 0, Vector2.down, detectDistance, groundLayerMask).collider;
    }

    public Collider2D CircleDetect()
    {
        return Physics2D.OverlapCircle(transform.position, detectDistance, groundLayerMask);
    }

    private void OnDrawGizmosSelected()
    {
        var box = transform.parent.GetComponent<BoxCollider2D>();
        float r = box.size.x / 2;
        Gizmos.color = Color.green;
        switch (detectType)
        {
            case DetectType.Box:
                Gizmos.DrawWireCube(box.bounds.center + Vector3.down * r, new Vector3(box.size.x, box.size.y, 0));
                break;
            case DetectType.Circle:
                Gizmos.DrawWireSphere(transform.position, r);
                break;
            default:
                break;
        }
    }
}
