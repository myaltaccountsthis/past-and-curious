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
        
    }

    void LateUpdate() {
        gameData.time += Time.deltaTime;
    }
}
