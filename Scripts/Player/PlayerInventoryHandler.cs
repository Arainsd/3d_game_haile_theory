using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Handles the player inventory.
/// </summary>
public class PlayerInventoryHandler : MonoBehaviour
{
    [SerializeField]
    private Font font;

    [SerializeField]
    private KeyCode dropItem = KeyCode.G;

    public KeyCode dropItemKeyCode
    {
        get
        {
            return dropItem;
        }
    }

    [SerializeField]
    private KeyCode openInventory = KeyCode.I;

    /// <summary>
    /// The inventory of the player.
    /// </summary>
    public Inventory inventory;

    [SerializeField]
    private Player player;

    [SerializeField]
    private CanvasRenderer canvasRenderer;

    [SerializeField]
    private RectTransform inventoryPanel;

    [SerializeField]
    private Text currentItemName;

    [Header("Full inventory feedback")]
    [Tooltip("What color will the inventory flash once it's full.")]
    [SerializeField]
    private Color flashColor = Color.red;

    [SerializeField]
    [Tooltip("The time it takes to complete one flash.")]
    private float flashInterval = 0.4f;

    [SerializeField]
    [Tooltip("How much times we should flash the inventory.")]
    private int flashTimes = 2;

    private PlayerItemHandler itemHandler
    {
        get
        {
            return player.itemHandler;
        }
    }

    private void OnValidate()
    {
        player.itemHandler.OnItemPickup -= handleItemPickup; // Unregister
        inventory.OnRemove -= onRemoveFromInventory; // Unregister

        player.itemHandler.OnItemPickup += handleItemPickup; // Register item pickup listener
        inventory.OnRemove += onRemoveFromInventory; // Register our on remove listener.

        Refresh();
    }

    private void Start()
    {
        OnValidate();

        inventory.OnRemove += onRemoveFromInventory; // Register our on remove listener.

        activateItem(inventory.isEmpty ? null : inventory[0]);
    
        //font for creating texts
        if (font == null) // Only set font if unused
            font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        Refresh();
    }
	
	private void Update()
    {
        if (Options.PAUSED)
            return;

        handleDrop();
        handleChangingItem();
    }

    /// <summary>
    /// Warn the player that the inventory is full by flashing it to another color for a bit.
    /// </summary>
    public void WarnFull()
    {
        if (_flashing)
            return;

        StartCoroutine(flashInventory());
    }

    private bool _flashing = false;

    private IEnumerator flashInventory()
    {
        if (_flashing)
            yield break;

        _flashing = true;

        Color normal = canvasRenderer.GetColor();
        Color red = flashColor;

        float halfInterval = flashInterval / 2f;

        for (int i = 0; i < flashTimes; i++)
        {
            canvasRenderer.SetColor(red);

            yield return new WaitForSeconds(halfInterval); // Flash for half a second.

            canvasRenderer.SetColor(normal);

            yield return new WaitForSeconds(halfInterval); // Then turn normal for half a second
        }

        _flashing = false;
    }

    private Pickup _droppedItem;

    /// <summary>
    /// The last item the player dropped.
    /// </summary>
    public Pickup droppedItem
    {
        get
        {
            return _droppedItem;
        }

        set
        {
            _droppedItem = value;
        }
    }

    /// <summary>
    /// Handle the changing of the current active item.
    /// </summary>
    private void handleChangingItem()
    {
        float delta = Input.GetAxis("Mouse ScrollWheel");

        if (delta == 0)
            return; // No change in the scroll wheel.

        bool change = delta < 0 ? false : true; // False if we need to change back, true if we need to change forward.

        InventoryStack current = player.itemInHand;

        int index = 0;

        if (current != null)
        {
           index = inventory.IndexOf(current);
        }

        int newIndex = (int)Mathf.Repeat(index + (change ? 1 : -1), inventory.Count); // Calculate the index previous or next to the current one.

        InventoryStack atIndex = inventory[newIndex];

        activateItem(atIndex);
    }

    //drops selected item
    private void handleDrop()
    {
        if (Input.GetKeyDown(dropItem))
        {
            //can only drop if an item is selected
            if (player.itemInHand != null) 
            {
                InventoryStack stack = player.itemInHand;

                //calls the item handler to drop item and store if an item was dropped
                bool canDrop = itemHandler.DropItem(stack.item);

                //remove item from inventory only if an item was dropped
                if (canDrop)
                {
                    //update dropped item
                    _droppedItem = stack.item;

                    //removes dropped item from the inventory
                    inventory.Remove(stack);
                }
            }
        }
    }

    private void onRemoveFromInventory(InventoryStack stack, int index)
    { 
        if (stack.IsSame(player.itemInHand))
        {
            if (inventory.Count <= index) // was this the last item in the inventory?
                index--; // Then take the index of the item to the left.

            activateItem(index >= inventory.Count ? null : inventory[index]); // Set the active item to the closest item in the inventory.
        }

        Refresh();
    }

    [SerializeField]
    private InventoryDrawer inventoryDrawer;

    private void handleItemPickup(Pickup item)
    {
        if (item != null && !inventory.Contains(item))
        {
            InventoryStack stack = inventory.Add(item, this.gameObject);

            activateItem(stack);
        }

        Refresh();
    }

    /// <summary>
    /// Refreshes the inventory.
    /// Recalculates positions and states.
    /// </summary>
    public void Refresh()
    {
        if (inventoryDrawer != null)
            inventoryDrawer.Refresh();

        updateStates(player.itemInHand);
    }

    /// <summary>
    /// Activates an item, making it usable in the world again, and setting it as the item in hand.
    /// Also deactivates all other items.
    /// </summary>
    /// <param name="targetStack"></param>
    private void activateItem (InventoryStack targetStack)
    {
        player.itemInHand = targetStack;

        updateStates(targetStack);
    }

    private void updateStates(InventoryStack inHand)
    {
        currentItemName.text = inHand == null ? (inventory.Count == 0 ? "Inventory Empty" : "No item in hand") : inHand.item.gameObject.name;

        inventory.ForEach((stack) =>
        {
            if (stack.IsSame(inHand)) // If the item is found, activate it and change the text color in inventory
            {
                stack.image.color = Color.white;
                stack.item.gameObject.SetActive(true);
            }
            else // Disable all other items 
            {
                stack.image.color = new Color(1f, 1f, 1f, 0.4f);
                stack.item.gameObject.SetActive(false);
            }
        });
    }
}
