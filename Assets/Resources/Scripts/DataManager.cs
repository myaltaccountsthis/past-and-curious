using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Data {
    public float time;
    public int score;
}

public class DataManager : MonoBehaviour
{
    public Data gameData;

    void Awake() {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    void LateUpdate() {
        gameData.time += Time.deltaTime;
    }
}
