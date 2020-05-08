using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public Text TilesMatched;
    public string MainMenuScene = "MainMenu";
    [SerializeField] private SceneFader _sceneFader;

    private void Start()
    {
        TilesMatched.text = "STATES KILLED - " + LevelManager.TilesMatched.ToString();
    }

    public void Menu()
    {
        _sceneFader.FadeTo(MainMenuScene);
    }

    public void Next()
    {
        _sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }


}
