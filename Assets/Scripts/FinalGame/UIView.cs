using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIView : MonoBehaviour
{
    [SerializeField]
    private GameObject SpectetorMassage;

    public void SetSpectetorState()
    {
        SpectetorMassage.SetActive(true);
    }

    public void SetGameState()
    {
        SpectetorMassage.SetActive(false) ;
    }
}
