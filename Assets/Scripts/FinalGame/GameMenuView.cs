using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuView : MonoBehaviour
{
    [SerializeField]
    private GameObject MenuWindow;

    [SerializeField]
    private Button ExitButton;
    // Update is called once per frame
    void Update()
    {
        if (Inputs.Instance.esc)
        {
            MenuWindow.SetActive(!MenuWindow.activeSelf);
            Inputs.Instance.esc = false;

            //if (Inputs.Instance.applicationFocus)
            {
                Inputs.Instance.SetCursorState(!MenuWindow.activeSelf);
            }

            Inputs.Instance.IsFreez = MenuWindow.activeSelf;
        }
    }

    public void AddExitListener(UnityEngine.Events.UnityAction action)
    {
        ExitButton.onClick.AddListener(action);
    }
}
