using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Property/BubbleSpawnerData", fileName = "BubbleSpawnerData")]
public class BubbleSpawnerData : ScriptableObject
{
    [Header("Bubble Prefab")]
    [Tooltip("Drag and drop your bubble explode effect prefab here.")]
    public GameObject effectBubbleExplodePrefab;
    [Header("Movement Settings")]
    [Tooltip("Upward speed for bubbles.")]
    public float bubbleSpeed = 0.1f;
    [Tooltip("The range of horizontal drifting speed.")]
    public float horizontalDrift = 0.3f;
    [Tooltip("The distance to move opposite direction")]
    public float bouncebackDistance = 1f;
    [Tooltip("Push horizontal speed")]
    public float pushHorizontalSpeed = 4f;
    [Tooltip("Push vertical speed")]
    public float pushVerticalSpeed = 2f;
    [Tooltip("Max horizontal distance")]
    public float maxHorizontalDistance = 5f;
    [Tooltip("Time to destroy the bubble")]
    public float destroyTime = 10f;
    [Tooltip("bubble pop sound")]
    public List<AudioClip> popClips;
    [Tooltip("blow bubble sound")]
    public List<AudioClip> blowClips;
    [Tooltip("bounce")]
    public List<AudioClip> bounceClips;
}
