using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Spawns bubbles that float upward when the player presses 'F'.
/// Attach this script to a GameObject in the scene and assign a bubble prefab.
/// </summary>
public class BubbleSpawner : MonoBehaviour
{
    public BubbleSpawnerData data;
    private GameObject[] _bubbles = new GameObject[2];

    public GameObject SpawnBubble()
    {
        // offset by gameobject's facing direction
        float xOffset = transform.localScale.x * 2f;

        Vector3 spawnPos = new Vector3(transform.position.x + xOffset,
                                       transform.position.y + 0.2f,
                                       transform.position.z);

        // Instantiate the bubble
        GameObject newBubble = Instantiate(data.bubblePrefab, spawnPos, Quaternion.identity);

        // Add the BubbleBehaviour script to control the bubble's movement
        BubbleBehaviour bubbleBehavior = newBubble.AddComponent<BubbleBehaviour>();
        bubbleBehavior.verticalSpeed = data.bubbleSpeed;
        bubbleBehavior.horizontalDrift = data.horizontalDrift;
        bubbleBehavior.bouncebackDistance = data.bouncebackDistance;
        bubbleBehavior.initialDir = xOffset > 0 ? 1 : -1;
        bubbleBehavior.pushHorizontalSpeed = data.pushHorizontalSpeed;
        bubbleBehavior.pushVerticalSpeed = data.pushVerticalSpeed;
        bubbleBehavior.maxHorizontalDistance = data.maxHorizontalDistance;
        bubbleBehavior.destroyTime = data.destroyTime;
        bubbleBehavior.popClips = data.popClips;
        bubbleBehavior.Init();

        return newBubble;
    }

    public void checkExplode()
    {
        for (int i = 0; i < _bubbles.Length; i++)
        {
            if (_bubbles[i] == null) continue;
            _bubbles[i].GetComponent<BubbleBehaviour>().checkExplode();
        }
    }

    public void onBubble(GameObject bubble)
    {
        for (int i = 0; i < _bubbles.Length; i++)
        {
            if (_bubbles[i] == null)
            {
                _bubbles[i] = bubble;
                break;
            }
        }
    }

    public void offBubble(GameObject bubble)
    {
        for (int i = 0; i < _bubbles.Length; i++)
        {
            if (_bubbles[i] == bubble)
            {
                _bubbles[i] = null;
                break;
            }
        }
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

    private bool _hasBeenPushed = false;        // Once true, bubble moves in pushDirection only
    private Vector2 _pushDirection = Vector2.zero;  // The direction (unit vector) of the push
    [Tooltip("Speed at which the bubble moves after being pushed.")]
    public float pushHorizontalSpeed = 4f;
    public float pushVerticalSpeed = 2f;
    public float maxHorizontalDistance = 5f;
    public float destroyTime = 10f;

    public List<AudioClip> popClips;

    private float _driftDirection;
    private float _distance = 0;
    private bool _isInit = false;
    private bool _shouldMove = true;
    private float _horizontalDistance = 0;
    float pushPower = 50;
    bool destroyed = false;
    AudioSource audioSource;
    bool _shouldPop = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Randomize the direction of the horizontal drift
        // _driftDirection = 1;
        // Destroy the bubble after a set time
        // GameObject bubble = gameObject.transform.GetChild(0).gameObject;
        // bubble.GetComponent<Animator>().Play("broken");
    }

    public void destroyBubble()
    {
        if (destroyed) return;
        GetComponent<Collider2D>().enabled = false;
        PlayPopAudio();
        destroyed = true;
        GameObject bubble = gameObject.transform.GetChild(0).gameObject;
        bubble.GetComponent<Animator>().Play("broken");
    }

    void PlayPopAudio()
    {
        audioSource.clip = popClips[UnityEngine.Random.Range(0, popClips.Count)];
        audioSource.Play();
    }

    public void Init()
    {
        _isInit = true;
        _driftDirection = initialDir;
        Invoke("destroyBubble", destroyTime);
    }

    private void Update()
    {
        // if gameobject is destroyed
        if (gameObject.IsDestroyed()) return;
        if (!_isInit) return;
        if (destroyed) return;
        // if (!_shouldMove) return;
        {
            if (_hasBeenPushed)
            {
                // transform.Translate(_pushDirection * pushSpeed * Time.deltaTime, Space.World);
                // slow down horizontal speed as it approaches max distance
                // it will be fast -> slow
                if (_horizontalDistance > maxHorizontalDistance)
                {
                    _horizontalDistance = maxHorizontalDistance;
                    _hasBeenPushed = false;
                }

                float percent = _horizontalDistance / maxHorizontalDistance;
                float speed = pushHorizontalSpeed * (1 - percent);
                Vector3 movement = new Vector3(
                    _pushDirection.x * speed * Time.deltaTime,
                    _pushDirection.y * pushVerticalSpeed * Time.deltaTime,
                    0f
                );

                transform.Translate(movement);
                _horizontalDistance += Mathf.Abs(movement.x);

                // If you still want the bubble to be destroyed off-screen:
            }
        }

        // -----------------------------------------------------------
        // Existing drifting logic (only used if NOT pushed yet)
        // -----------------------------------------------------------
        {
            if (_shouldMove)
            {
                // Move the bubble upward + drift
                Vector3 movement = new Vector3(
                    _driftDirection * horizontalDrift * Time.deltaTime,
                    verticalSpeed * Time.deltaTime,
                    0f
                );

                // Track horizontal distance for bounceback
                _distance += movement.x;
                if (Mathf.Abs(_distance) > bouncebackDistance)
                {
                    _driftDirection *= -1;
                    _distance = 0;
                }

                float percent = _horizontalDistance / maxHorizontalDistance;
                if (percent < 1 && _hasBeenPushed)
                {
                    movement.x = 0;
                }

                // Apply the movement
                transform.Translate(movement);
            }
        }

        // Continue checking if the bubble goes off-screen
        CheckAndDestroyOffScreen();
    }

    /// <summary>
    /// Simple helper to destroy the bubble if it goes above the screen.
    /// </summary>
    private void CheckAndDestroyOffScreen()
    {
        float screenY = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
        float bubbleHeight = GetComponent<SpriteRenderer>().bounds.size.y;

        if (transform.position.y > screenY + bubbleHeight / 2)
        {
            destroyBubble();
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
            // Debug.Log(collision.contacts[0].normal);
            if (collision.contacts[0].normal.y > 0) return;
            // Make the player a child of the bubble
            // collision.transform.SetParent(transform, true);
            _shouldMove = false;    // Stop the bubble from moving
            collision.gameObject.GetComponent<BubbleSpawner>().onBubble(gameObject);
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
            // collision.transform.SetParent(null, true);
            _shouldMove = true;    // Allow the bubble to move again
            collision.gameObject.GetComponent<BubbleSpawner>().offBubble(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered" + other.tag);
        if (other.CompareTag("Ground") && !other.CompareTag("Player"))
        {
            destroyBubble();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;


        // If the bubble is already pushed, don't re-push it

        // Vector from player's position to bubble's position
        Vector2 bubblePos = transform.position;
        Vector2 playerPos = other.transform.position;
        Vector2 direction = (bubblePos - playerPos);

        // Only push if the player is actually below or to the side (y > 0 => bubble is above the player)
        if (direction.y <= 0)
        {
            // If you want symmetrical logic from above, remove or invert this check.
            return;
        }

        // Compute how far from straight-up this direction is
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        float absAngle = Mathf.Abs(angle);

        // Decide final push direction based on angle thresholds
        Vector2 finalDir;

        if (other.GetComponent<MovementController>().isOnGround)
        {
            // only check which side the player is on
            finalDir = (playerPos.x > bubblePos.x) ? Vector2.left : Vector2.right;
        }
        else
        {
            // 0° - 22.5° => straight up
            if (absAngle < 22.5f)
            {
                finalDir = Vector2.up;
            }
            // 22.5° - 67.5° => diagonal (±45° up)
            else if (absAngle < 67.5f)
            {
                // If angle > 0 => direction is "left" from the bubble's perspective, so push up-left
                finalDir = Vector2.up;
            }
            else
            {
                return;
            }
        }



        // Record that we got pushed in this final direction
        _pushDirection = finalDir;
        _hasBeenPushed = true;
        _shouldMove = true;  // (Optional) Stop the drift if you want
        _horizontalDistance = 0;
    }

    public void checkExplode()
    {
        if (destroyed) return;
        destroyBubble();
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
            if (child.CompareTag("Player"))
                child.SetParent(null, true);
        }
    }
}
