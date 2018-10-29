using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// A player, centralized class with all the handlers that make up a player.
/// </summary>
public class Player : MonoBehaviour
{
    private static Player _instance;

    public static Player instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    /// <summary>
    /// See <see cref="PlayerCameraHandler"/>.
    /// </summary>
    public PlayerCameraHandler cameraHandler;

    /// <summary>
    /// See <see cref="PlayerMovementHandler"/>.
    /// </summary>
    public PlayerMovementHandler moveHandler;

    /// <summary>
    /// See <see cref="PlayerItemHandler"/>.
    /// </summary>
    public PlayerItemHandler itemHandler;

    /// <summary>
    /// The canvas of the player, it's where we display everything to the player.
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// The body of the player, this is where all rotations / movements should be applied to.
    /// </summary>
    public GameObject body;

    /// <summary>
    /// See <see cref="PlayerInventoryHandler"/>.
    /// </summary>
    public PlayerInventoryHandler inventoryHandler;

    /// <summary>
    /// See <see cref="PlayerClickManager"/>.
    /// </summary>
    public PlayerClickManager clickInput;

    [SerializeField]
    private AudioSource backgroundMusic;

    private Action itemInHandUpdate = null;
    /// <summary>
    /// List of colliders on the item in hand changed to a trigger.
    /// </summary>
    private List<Collider> itemInHandChangedToTrigger = new List<Collider>();

    private InventoryStack _itemInHand = null;

    /// <summary>
    /// The item the player currently has in hand.
    /// </summary>
    public InventoryStack itemInHand
    {
        get
        {
            return _itemInHand;
        }

        set
        {
            if (_itemInHand != null)
            {
                foreach (Collider collider in itemInHandChangedToTrigger)
                    collider.isTrigger = false;

                itemInHandChangedToTrigger.Clear();

                _itemInHand.item.Update -= itemInHandUpdate; // Remove update.
                itemInHandUpdate = null;
            }

            if (value != null)
            {
                value.item.transform.SetParent(cameraHandler.transform);
                value.item.transform.localPosition = itemHandler.handOffset + value.item.handOffset;
                value.item.transform.localRotation = value.item.handRotation;

                foreach (Collider collider in value.item.FindComponents<Collider>(RedUtil.FindMode.CHILDREN_AND_SELF))
                {
                    if (!collider.isTrigger)
                    {
                        collider.isTrigger = true;
                        itemInHandChangedToTrigger.Add(collider); // Remember what colliders we turned to triggers.
                    }
                }

                itemInHandUpdate = () =>
                {
                    itemInHand = value; // If the item gets updated, re-set it as the item in hand.
                };

                value.item.Update += itemInHandUpdate;
            }

            _itemInHand = value;

            inventoryHandler.Refresh();
        }
    }

    private bool _inGUI = false;

    /// <summary>
    /// Is the player currently in a gui, or currently aiming in the world.
    /// If this is set to false, the cursor will be invisible and locked to the center of the screen.
    /// </summary>
    public bool inGUI
    {
        get
        {
            return Cursor.lockState == CursorLockMode.None; // Defined by not being in a locked cursor state.
        }

        set
        {
            Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = value;
            _inGUI = value;

            if (backgroundMusic != null)
            {
                if (value && backgroundMusic.isPlaying)
                    backgroundMusic.Pause();
                else if (!value && !backgroundMusic.isPlaying)
                    backgroundMusic.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = Options.MUSIC_MULTIPLIER; // Set volume to music multiplier.
        }

        if (Options.PAUSED)
            return;

        if (_inGUI != inGUI) // inGUI returns if we are locked (which is an indicator of being in a GUI), and _inGUI returns if we are ACTUALLY set to an in gui state.
            inGUI = _inGUI; // If we are not locked, lock ourselves again in by setting inGUI, so that if we tab out, we go back into locked state.
    }

    public void InteractableTest()
    {
        Debug.Log("I dont do anything yet.");
    }
}
