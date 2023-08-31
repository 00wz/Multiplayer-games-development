using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterGameWindow : MonoBehaviour
{
    [SerializeField]
    private Button _signInButton;
    [SerializeField]
    private Button _createAccountButton;
    [SerializeField]
    private Canvas _enterGameCanvas;
    [SerializeField]
    private Canvas _createAccountCanvas;
    [SerializeField]
    private Canvas _signInCanvas;

    void Start()
    {
        _signInButton.onClick.AddListener(OpenSignInWindow);
        _createAccountButton.onClick.AddListener(OpenCreateAccountWindow);

    }

    private void OpenSignInWindow()
    {
        _signInCanvas.enabled = true;
        _enterGameCanvas.enabled = false;
    }

    private void OpenCreateAccountWindow()
    {
        _createAccountCanvas.enabled = true;
        _enterGameCanvas.enabled = false;
    }

}
