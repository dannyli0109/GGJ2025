using UnityEngine;

public class BubbleDownOnStand : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float gravityScale = 0.2f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        // Start as Kinematic, no gravity
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.gravityScale = 0f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.contacts[0].normal.y > 0) return;
            // / Switch to Dynamic, enabling gravity
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.gravityScale = gravityScale;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Revert to Kinematic
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;
            // Optionally also zero out velocity
            _rb.velocity = Vector2.zero;
        }
    }
}
