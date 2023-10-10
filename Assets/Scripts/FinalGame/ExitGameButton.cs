using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGameButton : MonoBehaviour
{
    [SerializeField]
    private Button Button;

    private void Start()
    {
        Button.onClick.AddListener(QuitApplication);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
