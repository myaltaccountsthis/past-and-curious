using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasscodeUI : MonoBehaviour
{
    public Button exit;
    public TextMeshProUGUI text;
    public Button[] keys = new Button[10];

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    
    
    
}