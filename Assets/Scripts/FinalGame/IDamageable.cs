using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int value, Photon.Realtime.Player attacker,int attackerViewId);
}
