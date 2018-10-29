using UnityEngine;
using System.Collections;

public class MirrorController : MonoBehaviour
{
    [SerializeField]
    private float controlSensitivity = 5f;

    [SerializeField]
    private Player player;

    [SerializeField]
    private Transform center;

    private Pickup pickup;

    /// <summary>
    /// Is true once the mirror is dropped and CAN be controlled, but not neccesarily being controlled by the player.
    /// </summary>
    public bool isControllable { get; private set; }

    /// <summary>
    /// Is true once the mirror is being controlled by the player. (drop key held)
    /// </summary>
    public bool isControlling { get; private set; }

    private float mouseXAxis
    {
        get
        {
            return Input.GetAxis("Mouse X");
        }
    }
    
	private void Start ()
    {
        pickup = GetComponent<Pickup>();
        isControllable = false;
	}

	private void Update ()
    {
        isControlling = false; // Reset value

        if (Options.PAUSED)
            return;

        InventoryStack itemInHand = player.itemInHand;

        // Make the mirror controllable once it is in hand.
        if (itemInHand != null)
            isControllable = isControllable || pickup.Equals(itemInHand.item);

        // If the mirror is not controllable, return.
        if (!isControllable)
            return;
        
        Pickup droppedItem = player.inventoryHandler.droppedItem;

        // If droppedItem is null, there's no mirror to control, return.
        if (droppedItem == null)
            return;
        
        // The player can control if the dropped item is this mirror.
        bool canControl = pickup.Equals (droppedItem);

        // If the player can't control, return.
        if (!canControl)
            return;

        // If the dropped item is the same as the item in hand, the player picked up the dropped mirror, return.
        if (itemInHand != null && droppedItem.Equals (itemInHand.item)) 
            return;

        // Control the mirror.
        controlMirror();
        
        // If the drop key is released, the mirror is not controllable anymore.
        isControllable = !Input.GetKeyUp(player.inventoryHandler.dropItemKeyCode);
	}

    private float lastYaw;

    /// <summary>
    /// Handles mirror rotation on input.
    /// </summary>
    private void controlMirror ()
    {
        // If the drop key is held, rotate the mirror according to the mouse X axis. 
        if (Input.GetKey(player.inventoryHandler.dropItemKeyCode))
        {
            isControlling = true;

            // Distance between the player and the mirror.
            float distance = Vector3.Distance(player.body.transform.position, transform.position);

            // If the player is too far away, return.
            if (distance > player.itemHandler.interactRange)
                return;

            float yaw = player.cameraHandler.lookRotation.eulerAngles.y;
            float yawChange = yaw - lastYaw;

            lastYaw = yaw;

            transform.RotateAround(center.position, new Vector3(0, 1, 0), yawChange * controlSensitivity);
        }
    }
}
