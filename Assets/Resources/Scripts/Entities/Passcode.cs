using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Passcode : Entity
{
    public string code;
    private string enteredText = "";
    private string defaultText;
    public PasscodeUI passcodeUI;
    public UnityEvent onSolve;

    private bool notFirstFrame = false;
    private bool active;
    
    private static Color accGreen = new(0.6f, 0.8f, 0.2f);

    public AudioSource keyPress;
    public AudioSource passAccepted;
    public AudioSource passIncorrect;

    public void Start()
    {
        defaultText = new string('_', code.Length);
    }
    
    public void Update()
    {
        if (!active)
        {
            return;
        }
        
        if (notFirstFrame && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
        {
            passcodeUI.gameObject.SetActive(false);
            active = false;
        }
        notFirstFrame = true;
        
        foreach (char c in Input.inputString)
        {
            if (c >= '0' && c <= '9')
            {
                InputDigit(c - '0');
            }
        }
    }

    public void OnDisable()
    {
        active = false;
        if (passcodeUI == null)
            return;
        passcodeUI.gameObject.SetActive(false);
    }

    public override void Interact(Player player)
    {
        active = true;
        notFirstFrame = false;
        passcodeUI.exit.onClick.RemoveAllListeners();
        passcodeUI.exit.onClick.AddListener(() => passcodeUI.gameObject.SetActive(false));
        for (var num = 0; num < 10; num++)
        {
            var i = num;
            passcodeUI.keys[i].onClick.RemoveAllListeners();
            passcodeUI.keys[i].onClick.AddListener(() => InputDigit(i));
        }
        ResetText();
        passcodeUI.gameObject.SetActive(true);
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
                passAccepted.Play();
                StartCoroutine(SolveSequence());
            }
            else
            {
                passIncorrect.Play();
                passcodeUI.text.color = Color.red;
            }
        }
        
        passcodeUI.text.text = enteredText + new String('_', code.Length - enteredText.Length);
        AudioSource.PlayClipAtPoint(keyPress.clip, keyPress.transform.position);
    }

    IEnumerator SolveSequence()
    {
        passcodeUI.text.color = accGreen;
        yield return new WaitForSeconds(1);
        passcodeUI.gameObject.SetActive(false);
        onSolve.Invoke();
        Locked = true;
    }
    
}