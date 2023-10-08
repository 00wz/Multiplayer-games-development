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
    private void Start()
    {
        Inputs.Instance.LookCursor(!MenuWindow.activeSelf);
    }

    private void OnDestroy()
    {
        Inputs.Instance.LookCursor(false);
    }
    void Update()
    {
        if (Inputs.Instance.esc)
        {
            MenuWindow.SetActive(!MenuWindow.activeSelf);
            Inputs.Instance.esc = false;

            //if (Inputs.Instance.applicationFocus)
            {
                Inputs.Instance.LookCursor(!MenuWindow.activeSelf);
            }

            Inputs.Instance.IsFreez = MenuWindow.activeSelf;
        }
    }

    public void AddExitListener(UnityEngine.Events.UnityAction action)
    {
        ExitButton.onClick.AddListener(action);
    }
}
