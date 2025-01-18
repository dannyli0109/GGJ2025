using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bubbleExplode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void breakBubble()
    {
        // Play sound
        // Play particle effect
        // Destroy bubble
        Debug.Log("Bubble popped!");
        Destroy(this.transform.parent.gameObject);
    }
}
