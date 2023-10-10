using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticView : MonoBehaviour
{
    [SerializeField]
    private GameObject LoadDisplay;

    [SerializeField]
    private TMPro.TMP_Text FragsView;

    public void SetLoadState()
    {
        LoadDisplay.SetActive(true);
        FragsView.gameObject.SetActive(false);
    }

    public void SetDisplayState(int fragsCount)
    {
        FragsView.text ="Score: "+ fragsCount.ToString();
        LoadDisplay.SetActive(false);
        FragsView.gameObject.SetActive(true);
    }
}
