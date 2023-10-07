using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyView2 : MonoBehaviour
{
    [SerializeField]
    public GameObject RoomList;

    [SerializeField]
    public GameObject LoadDisplay;

    public bool IsLoading
    {
        set
        {
            LoadDisplay.SetActive(value);
            RoomList.SetActive(!value);
        }
    }
}
