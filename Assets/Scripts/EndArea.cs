using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    List<GameObject> players = new List<GameObject>();
	int playerCount;
    // Start is called before the first frame update
    void Start()
    {
		playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Player"))
        {
            players.Add(collision.gameObject);
			if(players.Count >= playerCount)
			{
				GameManager.Instance.SwitchNextScene();
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
}
