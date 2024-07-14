using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Passcode : Entity
{
    public string code;
    public string enteredText;
    public string defaultText;
    public PasscodeUI passcodeUI;
    public UnityEvent onSolve;

    private static Color accGreen = new(0.6f, 0.8f, 0.2f);

    public void Start()
    {
        passcodeUI.exit.onClick.AddListener(() => passcodeUI.SetActive(false));
        for (var num = 0; num < 10; num++)
        {
            var i = num;
            passcodeUI.keys[i].onClick.AddListener(() => InputDigit(i));
        }
    }
    
    public void Update()
    {
        if (!passcodeUI.gameObject.activeSelf)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            passcodeUI.SetActive(false);
        }
        
        foreach (char c in Input.inputString)
        {
            if (c >= '0' && c <= '9')
            {
                InputDigit(c - '0');
            }
        }
    }

    public override void Interact(Player player)
    {
        ResetText();
        passcodeUI.SetActive(true);
    }

    private void ResetText()
    {
        passcodeUI.text.color = Color.black;
        passcodeUI.text.text = defaultText;
        enteredText = "";
    }

    public void InputDigit(int d)
    {
        if (enteredText.Length == code.Length)
        {
            ResetText();
        }
        
        enteredText += d;
        
        if (enteredText.Length == code.Length)
        {
            if (enteredText.Equals(code))
            {
                StartCoroutine(SolveSequence());
            }
            else
            {
                passcodeUI.text.color = Color.red;
            }
        }

        passcodeUI.text.text = enteredText + new String('_', code.Length - enteredText.Length);
    }

    IEnumerator SolveSequence()
    {
        passcodeUI.text.color = accGreen;
        yield return new WaitForSeconds(1);
        passcodeUI.SetActive(false);
        onSolve.Invoke();
    }
    
}