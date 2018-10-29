using UnityEngine;
using System.Collections;
using System;

public class GuardDoor : Door
{
    [SerializeField]
    private GuardDoorTrigger trigger;

    [SerializeField]
    private ParticleSystem sparks;

    [SerializeField]
    private string[] afterTrigger;

    protected override void Awake()
    {
        base.Awake();

        Action<Player> closeDoorOnEnter = null;

        closeDoorOnEnter = (p) =>
        {
            CloseDoor();
            trigger.OnFirstTrigger -= closeDoorOnEnter; // Close after entering once.
            closedText = afterTrigger;
            hoverText = afterTrigger;

            sparks.Stop();
        };

        trigger.OnFirstTrigger += closeDoorOnEnter;
    }
}
