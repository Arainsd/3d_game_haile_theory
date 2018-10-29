using UnityEngine;
using System.Collections;

public class GasRoomDoor : Door
{
    [Tooltip("The hover text to show when the gas room is filled with gas.")]
    [SerializeField]
    private string[] preGas;

    [Tooltip("The hover text to show when the gas room has been ventilated.")]
    [SerializeField]
    private string[] afterGas;

    [SerializeField]
    private AudioClip openFailedSound;

    private bool _isGasVentilated = true;
    private bool isGasVentilated
    {
        get
        {
            return _isGasVentilated;
        }
        
        set
        {
            if (value == _isGasVentilated)
                return;

            // If the room is ventilated, change the hover text to afterGas, else keep it as preGas.
            hoverText = value ? afterGas : preGas;

            _isGasVentilated = value;
        }
    }

    protected override void Start ()
    {
        base.Start();
        isGasVentilated = false;
	}

    private void LateUpdate ()
    {
        // If the gas room is ventilated, return.
        if (isGasVentilated)
            return;

        // Keep the gas room door closed.
        CloseDoor(false);

        // The hover text has been replaced when the closeDoor method was called.
        // Place the preGas hover text back.
        hoverText = preGas;
	}

    /// <summary>
    /// Changes the hover text of the gas room door to let the player know the room is safe.
    /// Makes the gas room door openable.
    /// </summary>
    public void gasVentilated ()
    {
        isGasVentilated = true;
    }
}
