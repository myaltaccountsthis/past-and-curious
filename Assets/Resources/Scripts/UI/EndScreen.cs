using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;

    public void UpdateUI(int score, float time) {
        gameObject.SetActive(true);
        UpdateScore(score);
        UpdateTime(time);
    }

    public void UpdateScore(int score) {
        scoreText.text = "" + score;
    }

    public void UpdateTime(float time) {
        timeText.text = string.Format("{0:D}:{1:D2}", (int)time / 60, (int)time % 60);
    }

    public void PlayAgain() {
        SceneManager.LoadScene("Home");
    }

    public void Exit() {
        Application.Quit();
    }
}
