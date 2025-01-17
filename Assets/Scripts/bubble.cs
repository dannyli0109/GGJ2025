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
    [Tooltip("Minimum upward speed for bubbles.")]
    public float minBubbleSpeed = 0.5f;
    [Tooltip("Maximum upward speed for bubbles.")]
    public float maxBubbleSpeed = 2f;
    [Tooltip("The range of horizontal drifting speed.")]
    public float horizontalDrift = 0.3f;
    [Tooltip("The time in seconds before bubbles are destroyed.")]
    public float destroyTime = 10f;
    [Tooltip("The distance to move opposite direction")]
    public float bouncebackDistance = 1f;

    private void Update()
    {
        // Spawn a bubble whenever the player presses 'F'
        if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnBubble();
        }
    }

    private void SpawnBubble()
    {
        // Random horizontal spawn offset
        float xOffset = 0; // Random.Range(-spawnRange, spawnRange);
        Vector3 spawnPos = new Vector3(transform.position.x + xOffset,
                                       transform.position.y,
                                       transform.position.z);

        // Instantiate the bubble
        GameObject newBubble = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);

        // Add the BubbleBehaviour script to control the bubble's movement
        BubbleBehaviour bubbleBehavior = newBubble.AddComponent<BubbleBehaviour>();
        bubbleBehavior.verticalSpeed = Random.Range(minBubbleSpeed, maxBubbleSpeed);
        bubbleBehavior.horizontalDrift = horizontalDrift;
        bubbleBehavior.destroyTime = destroyTime;
        bubbleBehavior.bouncebackDistance = bouncebackDistance;
    }
}
/// <summary>
/// Controls the movement and destruction of individual bubble objects.
/// </summary>
public class BubbleBehaviour : MonoBehaviour
{
    [HideInInspector] public float verticalSpeed;
    [HideInInspector] public float horizontalDrift;
    [HideInInspector] public float destroyTime;
    [HideInInspector] public float bouncebackDistance = 1f;

    private float _driftDirection;
    private float _distance = 0;

    private void Start()
    {
        // Randomize the direction of the horizontal drift
        _driftDirection = Random.Range(-1f, 1f);
        // Destroy the bubble after a set time
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        // if gameobject is destroyed
        if (gameObject.IsDestroyed()) return;

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
}
