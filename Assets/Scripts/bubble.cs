using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Spawns bubbles that float upward when the player presses 'F'.
/// Attach this script to a GameObject in the scene and assign a bubble prefab.
/// </summary>
public class BubbleSpawner : MonoBehaviour
{
    [Header("Bubble Prefab")]
    [Tooltip("Drag and drop your bubble prefab here.")]
    public GameObject bubblePrefab;

    [Header("Spawn Settings")]
    // [Tooltip("Horizontal range around the spawner's X-position where bubbles can appear.")]
    // public float spawnRange = 5f;

    [Header("Movement Settings")]
    [Tooltip("Upward speed for bubbles.")]
    public float bubbleSpeed = 0.1f;
    [Tooltip("The range of horizontal drifting speed.")]
    public float horizontalDrift = 0.3f;
    [Tooltip("The distance to move opposite direction")]
    public float bouncebackDistance = 1f;

    private GameObject _bubble;

    private void Update()
    {
        // Spawn a bubble whenever the player presses 'F'
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_bubble != null)
            {
                Destroy(_bubble);
            }
            _bubble = SpawnBubble();
        }
    }

    private GameObject SpawnBubble()
    {
        // offset by gameobject's facing direction
        float xOffset = transform.localScale.x * 2;

        Vector3 spawnPos = new Vector3(transform.position.x + xOffset,
                                       transform.position.y,
                                       transform.position.z);

        // Instantiate the bubble
        GameObject newBubble = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);

        // Add the BubbleBehaviour script to control the bubble's movement
        BubbleBehaviour bubbleBehavior = newBubble.AddComponent<BubbleBehaviour>();
        bubbleBehavior.verticalSpeed = bubbleSpeed;
        bubbleBehavior.horizontalDrift = horizontalDrift;
        bubbleBehavior.bouncebackDistance = bouncebackDistance;
        bubbleBehavior.initialDir = xOffset > 0 ? 1 : -1;
        bubbleBehavior.Init();


        return newBubble;
    }
}
/// <summary>
/// Controls the movement and destruction of individual bubble objects.
/// </summary>
public class BubbleBehaviour : MonoBehaviour
{
    [HideInInspector] public float verticalSpeed;
    [HideInInspector] public float horizontalDrift;
    [HideInInspector] public float bouncebackDistance = 1f;
    [HideInInspector] public float initialDir;

    private float _driftDirection;
    private float _distance = 0;
    private bool _isInit = false;
    private bool _shouldMove = true;
    float pushPower = 50;

    private void Start()
    {
        // Randomize the direction of the horizontal drift
        // _driftDirection = 1;
        // Destroy the bubble after a set time
    }

    public void Init()
    {
        _isInit = true;
        _driftDirection = initialDir;
    }

    private void Update()
    {
        // if gameobject is destroyed
        if (gameObject.IsDestroyed()) return;
        if (!_isInit) return;
        if (!_shouldMove) return;

        // Move the bubble upward
        Vector3 movement = new Vector3(_driftDirection * horizontalDrift * Time.deltaTime,
                                       verticalSpeed * Time.deltaTime,
                                       0f);
        // move opposite direction if bubble move more than certain distance
        // the distance is relative
        this._distance += movement.x;
        if (Mathf.Abs(this._distance) > bouncebackDistance)
        {
            _driftDirection *= -1;
            this._distance = 0;
        }


        transform.Translate(movement);

        // Destroy the the bubble if it goes above the screen
        // get screen height
        float screenY = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
        float bubbleHeight = GetComponent<SpriteRenderer>().bounds.size.y;
        if (transform.position.y > screenY + bubbleHeight / 2)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// When the player first lands on the bubble, make the player a child of the bubble.
    /// This lets the player ride the bubble.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if it's the player colliding
        if (collision.gameObject.CompareTag("Player"))
        {
            // only do the logic when the player is on top of the bubble
            Debug.Log(collision.contacts[0].normal);
            if (collision.contacts[0].normal.y > 0) return;
            // Make the player a child of the bubble
            collision.transform.SetParent(transform, true);
            _shouldMove = false;    // Stop the bubble from moving
        }
    }

    /// <summary>
    /// When the player leaves the bubble, remove the parent-child relationship.
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Reset the player's parent to null
            collision.transform.SetParent(null, true);
            _shouldMove = true;    // Allow the bubble to move again
        }
    }

    /// <summary>
    /// A trigger-based push: if the player is inside this bubble's trigger from below or the side,
    /// the bubble moves away from the player.
    /// </summary>
    private void OnTriggerStay2D(Collider2D other)
    {
        // Only respond to the player
        if (!other.CompareTag("Player")) return;

        // If the bubble isn't moving for some reason (player is standing on top), skip
        // unless you specifically want a push even when riding. Adjust to your liking:
        if (!_shouldMove) return;

        // Calculate direction from the player to the bubble
        Vector2 direction = (transform.position - other.transform.position);

        // If player is mostly below the bubble (direction.y > x), push bubble up
        // If from the side, push bubble horizontally away from the player
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absY > absX)
        {
            // We only push up if the bubble is above the player => direction.y > 0
            // (meaning the player is below). Adjust as needed if you want symmetrical logic.
            if (direction.y > 0f)
            {
                transform.Translate(Vector2.up * pushPower * Time.deltaTime, Space.World);
            }
        }
        else
        {
            // Pushing from the left or right
            if (direction.x > 0f)
            {
                // Bubble is to the right => player is on the left => push bubble further right
                transform.Translate(Vector2.right * pushPower * Time.deltaTime, Space.World);
            }
            else
            {
                // Bubble is to the left => player is on the right => push bubble further left
                transform.Translate(Vector2.left * pushPower * Time.deltaTime, Space.World);
            }
        }
    }


    /// <summary>
    /// Detach the player (or any children) before the bubble is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        // For safety, ensure no child remains parented
        // (especially the player, if still attached).
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            // If you're specifically looking for "Player", do:
            // if (child.CompareTag("Player"))
            //     child.SetParent(null, true);

            // Or simply detach all children, if you want:
            child.SetParent(null, true);
        }
    }
}
