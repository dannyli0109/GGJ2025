using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BubbleDownOnStand : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float gravityScale = 0.2f;
    private bool isStandOn = false;
    private List<GameObject> gameObjects = new List<GameObject>();
    private int _count = 0;

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
            AddPlayer(collision.gameObject);
        }
    }

    void AddPlayer(GameObject gameObject)
    {
        bool found = false;
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (gameObjects[i] == gameObject)
            {
                found = true;
                break;
            }
        }
        if (!found)
        {
            gameObjects.Add(gameObject);
        }
        // down force
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.gravityScale = gravityScale;
    }

    void RemovePlayer(GameObject gameObject)
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (gameObjects[i] == gameObject)
            {
                gameObjects.RemoveAt(i);
                break;
            }
        }

        if (gameObjects.Count == 0)
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0;
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            RemovePlayer(collision.gameObject);
        }
    }

    private void Update()
    {
        if (gameObjects.Count > 0)
        {
            Debug.Log(gameObjects.Count);
            _rb.velocity = new Vector2(0, -gravityScale * gameObjects.Count);
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }
    }
}
