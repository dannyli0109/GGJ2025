using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class EndArea : MonoBehaviour
{
    List<GameObject> players = new List<GameObject>();
    Animator flagAnimator;
    public int playerCount;
    // Start is called before the first frame update
    void Start()
    {
        flagAnimator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int playersCount = GameManager.Instance.targets.Count;
        if (collision.CompareTag("Player"))
        {
            players.Add(collision.gameObject);
            if (players.Count >= playersCount)
            {
                GetComponent<AudioSource>().Play();
                GetComponent<BoxCollider2D>().enabled = false;
                flagAnimator.SetTrigger("rise");
                StartCoroutine(DelayFunc(LevelComplete, 1));
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
        GameManager.Instance.SwitchNextLevel();
        // CircleTransition.Instance.TransitionToNext(true);
    }

    IEnumerator DelayFunc(UnityAction action, float t)
    {
        yield return new WaitForSeconds(t);
        action();
    }
}
