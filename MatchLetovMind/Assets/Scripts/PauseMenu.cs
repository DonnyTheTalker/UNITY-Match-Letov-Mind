using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject ui;
    public string MainMenuScene = "MainMenu";
    [SerializeField] private SceneFader _sceneFader;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {

            Toggle();

        }
    }

    public void Toggle()
    {
        ui.SetActive(!ui.activeSelf);  
    }

    public void Retry()
    {
        Toggle();
        _sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }

    public void Menu()
    {
        Toggle();
        _sceneFader.FadeTo(MainMenuScene);
    }

}