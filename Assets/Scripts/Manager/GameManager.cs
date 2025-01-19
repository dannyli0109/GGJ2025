using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;

public class GameManager : SingletonMono<GameManager>
{
    AudioSource bgmSource;
    public List<AudioClip> bgmClips;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        bgmSource = GetComponent<AudioSource>();
        SwitchBgm(0);
    }

    public void _SwitchNextScene()
    {
        int id = SceneManager.GetActiveScene().buildIndex;
        SwitchScene(id + 1);
    }

    public void SwitchNextScene()
    {
        CircleTransition.Instance.TransitionToNext();
    }

    public void _SwitchScene(int id)
    {
        SceneManager.LoadScene(id);
        SwitchBgm(id);
    }

    public void SwitchScene(int id)
    {
        CircleTransition.Instance.TransitionToScene(id);
    }

    public void SwitchBgm(int id)
    {
        if (id < bgmClips.Count)
        {
            bgmSource.clip = bgmClips[id];
            bgmSource.Play();
        }
    }

    void Update()
    {
        // when r is pressed, restart the current scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
        {
            SwitchScene(0);
        }
    }

    public void _Restart()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        GameManager.instance.SwitchScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Restart()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        CircleTransition.Instance.RestartScene();
    }
}
