using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NewRoomFieldController: IDisposable
{
    private NewRoomFieldView _newRoomFieldView;
    public bool enable
    {
        set
        {
            _newRoomFieldView.gameObject.SetActive(value);
        }
    }
    public NewRoomFieldController(Action<string> onCreate,GameObject canvas)
    {
        _newRoomFieldView = GameObject.Instantiate<NewRoomFieldView>(Resources.Load<NewRoomFieldView>("NewRoomFileld"),canvas.transform);
        _newRoomFieldView.CreateRoomButton.onClick.AddListener(() => onCreate(_newRoomFieldView.RoomName.text));
    }

    public void Dispose()
    {
        if(_newRoomFieldView!=null)
        {
            GameObject.Destroy(_newRoomFieldView.gameObject);
        }
    }
}
