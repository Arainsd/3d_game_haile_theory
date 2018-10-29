using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerRayFeedback : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private Material shrinkableMaterial;

    private void FixedUpdate()
    {
        bool shouldEnable = doEnable;

        if (shouldEnable && !isEnabled)
            setState(true);
        else if (!shouldEnable && isEnabled)
            setState(false);
    }

    /// <summary>
    /// If the feedback is currently visible
    /// </summary>
    public bool isEnabled { get; private set; }

    private void setState(bool enabled)
    {
        isEnabled = enabled; // We have to track this, otherwise infinite materials get added to the 

        foreach (Shrinkable shrinkable in FindObjectsOfType<Shrinkable>())
        {
            List<Renderer> toRender = shrinkable.gameObject.FindComponents<Renderer>();

            if (shrinkable.outlineRenderer != null)
            {
                toRender.Clear();
                toRender.Add(shrinkable.outlineRenderer);
            }

            foreach (Renderer renderer in toRender)
            {
                List<Material> materials = new List<Material>(renderer.sharedMaterials); // Use a list for easy methods.

                int indexOf = materials.IndexOf(shrinkableMaterial);

                if (!enabled)
                {
                    if (indexOf >= 0)
                        materials.RemoveAt(indexOf);
                }
                else
                {
                    if (indexOf < 0)
                        materials.Add(shrinkableMaterial);
                }

                renderer.materials = materials.ToArray();
            }
        }
    }
    
    /// <summary>
    /// If we should make the feedback visible or not.
    /// </summary>
    private bool doEnable
    {
        get
        {
            bool handItemIsReflectable = player.itemInHand != null && player.itemInHand.item.gameObject.GetComponent<Reflectable>() != null;

            MirrorController controller = player.inventoryHandler.droppedItem != null ? player.inventoryHandler.droppedItem.GetComponent<MirrorController>() : null;

            bool mirrorFeedbackPresent = controller != null && controller.isControlling;

            return handItemIsReflectable || mirrorFeedbackPresent;
        }
    }
}
