using UnityEngine;

public class BubbleDownOnStand : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float gravityScale = 0.2f;
    private bool isStandOn = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        // Start as Kinematic, no gravity
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            // down force
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.gravityScale = gravityScale;
            isStandOn = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // up force
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0;
            isStandOn = false;
        }
    }

    private void Update()
    {
        if (isStandOn)
        {
            _rb.velocity = new Vector2(0, -gravityScale);
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }
    }
}
