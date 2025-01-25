using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public static void Res()
    {
        GameManager.Instance.SwitchLevel(GameManager.Instance.GetCurLevelId());
    }

    public static void Back()
    {
        //GameManager.Instance.SwitchMainScene();
    }
}
