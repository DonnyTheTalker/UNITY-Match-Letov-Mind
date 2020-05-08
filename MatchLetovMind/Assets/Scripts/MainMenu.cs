using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneFader _sceneFader;
    public string LevelHub;

    public void Exit()
    {
        Application.Quit();
    }

    public void Play()
    {
        _sceneFader.FadeTo(LevelHub);
    }

}
