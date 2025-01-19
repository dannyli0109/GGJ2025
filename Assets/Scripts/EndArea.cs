using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Destination : MonoBehaviour
{
    List<GameObject> players = new List<GameObject>();
    Animator flagAnimator;
    int playerCount;
    // Start is called before the first frame update
    void Start()
    {
        flagAnimator = GetComponentInChildren<Animator>();
        playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            players.Add(collision.gameObject);
            if (players.Count >= playerCount)
            {
                GetComponent<AudioSource>().Play();
                GetComponent<BoxCollider2D>().enabled = false;
                flagAnimator.SetTrigger("rise");
                StartCoroutine(DelayFunc(LevelComplete, 2));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            players.Remove(collision.gameObject);
        }
    }

    void LevelComplete()
    {
        GameManager.Instance.SwitchNextScene();
        // CircleTransition.Instance.TransitionToNext(true);
    }

    IEnumerator DelayFunc(UnityAction action, float t)
    {
        yield return new WaitForSeconds(t);
        action();
    }
}
