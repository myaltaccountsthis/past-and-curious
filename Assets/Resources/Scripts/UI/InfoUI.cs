using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI text;
    public Button exit;

    void Start() {
        exit.onClick.AddListener(Exit);
    }

    public void DisplayInfo(Sign entity) {
        title.text = entity.title;
        text.text = entity.text;
        gameObject.SetActive(true);
    }

    public void Exit() {
        gameObject.SetActive(false);
    }


}
