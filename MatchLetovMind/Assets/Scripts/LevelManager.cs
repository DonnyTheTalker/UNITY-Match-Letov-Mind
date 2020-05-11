using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Text LevelDuration;
    public static int TilesMatched;
    public GameObject GameOverUI;

    private float _fLevelDuration;
    private bool _gameEnded;

    private void Start()
    {
        TilesMatched = 0;
        _gameEnded = false;
        LevelDuration.enabled = true;

        int curSong = SoundManager.Instance.CurrectClip;
        float tempText = SoundManager.Instance.BackGroundClips[curSong].length;
        tempText -= SoundManager.Instance.MusicSource.time + 2f;

        string newText = string.Format("{0}", (int)tempText) + "." + string.Format("{0}{1}", ((int)(tempText * 10)) % 10,
                                                                                             ((int)(tempText * 100)) % 10);
        _fLevelDuration = tempText;

        LevelDuration.text = "TIME BEFORE COMMUNISM - " + newText;
    }

    private void Update()
    {

        if (_gameEnded) return;

        float tempText = _fLevelDuration - Time.deltaTime;

        if (tempText <= 0) {
            _gameEnded = true;
            LevelDuration.enabled = false;
            GameOverUI.SetActive(true);
            return;
        }

        string newText = string.Format("{0}", (int)tempText) + "." + string.Format("{0}{1}", ((int)(tempText * 10)) % 10,
                                                                                             ((int)(tempText * 100)) % 10);
        _fLevelDuration = tempText;

        LevelDuration.text = "TIME BEFORE COMMUNISM - " + newText;
    }

}
