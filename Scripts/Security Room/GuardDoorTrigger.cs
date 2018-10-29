using UnityEngine;
using System.Collections;
using System;

public class GuardDoorTrigger : MonoBehaviour
{
    private bool triggered = false;

    public event Action<Player> OnFirstTrigger;

    private void OnTriggerEnter(Collider col)
    {
        Player player = col.gameObject.FindComponent<Player>(RedUtil.FindMode.PARENTS_AND_SELF);

        if (player == null || triggered)
            return;

        if (OnFirstTrigger != null)
            OnFirstTrigger(player);

        triggered = true;
    }
}
