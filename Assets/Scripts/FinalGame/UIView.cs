using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIView : MonoBehaviour
{
    [SerializeField]
    private GameObject SpectetorMassage;

    [SerializeField]
    private TMP_Text HPText;

    [SerializeField]
    private TMP_Text FragsText;

    private int _frags = 0;
    private PlayerClass _player;

    private void Start()
    {
        UpdateFragsView();
        if (_player == null)
        {
            SetSpectetorState();
        }
    }

    public void SetPlayer(PlayerClass player)
    {
        _player=player;
        UpdateHPView(player.GetHealth());
        player.OnHPChange += UpdateHPView;
        player.OnKillOther += AddOneFrag;
        player.OnDie += OnDie;
        SetGameState();
    }
    private void AddOneFrag()
    {
        //Debug.LogWarning("AddOneFrag");
        _frags++;
        //Debug.LogWarning(_frags);
        UpdateFragsView();
    }

    private void OnDie()
    {
        _player.OnHPChange -= UpdateHPView;
        _player.OnKillOther -= AddOneFrag;
        _player.OnDie -= OnDie;
        _player = null;
        SetSpectetorState();
    }

    private void UpdateHPView(int newvalue)
    {
        HPText.text = newvalue.ToString()+ "♥";
    }
    private void SetSpectetorState()
    {
        SpectetorMassage.SetActive(true);
        HPText.gameObject.SetActive(false);
    }

    private void SetGameState()
    {
        SpectetorMassage.SetActive(false);
        HPText.gameObject.SetActive(true);
    }

    private void UpdateFragsView()
    {
        //Debug.LogWarning(_frags);
        FragsText.text = _frags.ToString();
    }
}
