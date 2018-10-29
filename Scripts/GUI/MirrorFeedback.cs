using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MirrorFeedback : MonoBehaviour
{

    [SerializeField]
    private Player player;

    private bool feedbackOn = true;

    private CanvasRenderer[] canvases;

    private bool _visibility = true;

    /// <summary>
    /// The visibility of the GUI element
    /// </summary>
    private bool visibility
    {
        get
        {
            return _visibility;
        }

        set
        {
            // If the given value is the same as the current visibility, return.
            if (_visibility == value)
                return;

            float alpha = value ? 1f : 0f;

            // Update visibility of the children.
            foreach (CanvasRenderer renderer in canvases)
                renderer.SetAlpha(alpha);

            _visibility = value;
        }
    }

	private void Start ()
    {
        canvases = GetComponentsInChildren<CanvasRenderer>();
        visibility = false;
    }
	
	private void Update ()
    {
        if (Options.PAUSED)
            return;

        updateVisibility();
	}

    /// <summary>
    /// Updates the visibility of the GUI element.
    /// </summary>
    private void updateVisibility ()
    {
        InventoryStack itemInHand = player.itemInHand;

        // If itemInHand is null we know there is no mirror in hand, change visibility and return.
        if (itemInHand == null)
        {
            visibility = false;
            return;
        }

        // If the item in hand is a trigger for the feedback, show feedback.
        visibility = isTrigger (itemInHand.item);
    }

    /// <summary>
    /// Checks if the pickup is a trigger for the feedback (if it is a mirror).
    /// </summary>
    /// <param name="pickup"></param>
    /// <returns></returns>
    private bool isTrigger (Pickup pickup)
    {
        // Get the mirror controller component of the pickup.
        MirrorController controller = pickup.GetComponent<MirrorController>();

        //If the mirror controller is not null, the pickup is a trigger for the feedback.
        return controller != null;
    }
}
