using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// A combiner is a container object, you can add items to it by right clicking items on it.
/// If the items in the combiner make a recipe, you will get the result.
/// </summary>
public class Combiner : Hoverable
{
    [Serializable]
    public class CombineRecipe
    {
        public List<Pickup> ingredients;
        public Pickup result;
    }

    [SerializeField]
    public List<CombineRecipe> recipes;

    [SerializeField]
    public Inventory inventory;

    [SerializeField]
    private InventoryDrawer gui;

    [SerializeField]
    private Player player;

    [SerializeField]
    private AudioClip insertSound;

    [SerializeField]
    private AudioClip extractSound;

    [SerializeField]
    private AudioClip successSound;

    [SerializeField]
    private AudioClip failSound;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private KeyCode combineKey = KeyCode.F;

    private bool awoke = false;

    protected override void Awake()
    {
        base.Awake();

        gui = Instantiate(gui); // Create an actual object from the prefab.
        gui.inventory = inventory; // Set inventory
        gui.transform.SetParent(player.canvas.transform, false);
        gui.gameObject.SetActive(false);
        gui.Refresh();

        awoke = true;

        OnValidate();
    }

    protected void OnValidate()
    {
        if (!awoke)
            return; // Dont register if our gui isn't created yet.

        player.clickInput.RegisterHasParent(gui.gameObject, (r, e) =>
        {
            if (e.button.Equals(UnityEngine.EventSystems.PointerEventData.InputButton.Left))
                LeftClickInWorld(player);
            else if (e.button.Equals(UnityEngine.EventSystems.PointerEventData.InputButton.Right))
                RightClickInWorld(player);
        });
    }

    private bool lookingAt = false;

    private void Update()
    {
        if (!lookingAt)
            return;

        handleCombine();
    }

    private void handleCombine()
    {
        if (!Input.GetKeyDown(combineKey))
            return;

        CombineRecipe recipe = findCompleteRecipe();

        if (recipe == null)
        {
            if (failSound != null)
                audioSource.PlayOneShot(failSound, Options.SFX_MULTIPLIER);

            return;
        }

        if (successSound != null)
            audioSource.PlayOneShot(successSound, Options.SFX_MULTIPLIER);

        inventory.Clear();

        inventory.Add(recipe.result, gui.drawPanel.gameObject);
    }

    public override void StartLookingAtInWorld(Player player)
    {
        lookingAt = true;

        base.StartLookingAtInWorld(player);

        gui.gameObject.SetActive(true);
    }

    public override void StopLookingAtInWorld(Player player)
    {
        lookingAt = false;

        base.StopLookingAtInWorld(player);

        gui.gameObject.SetActive(false);
    }

    private float extractCooldown = 0f;

    public override void RightClickInWorld(Player player) // Extract
    {
        if (extractCooldown > Time.realtimeSinceStartup)
            return;

        if (inventory.Count == 0)
            return; // Can't do anything.

        int index = inventory.Count - 1;

        if (index < 0)
            return;

        Pickup last = inventory[index].item;

        if (!player.itemHandler.Pickup(last))
        {
            return;
        }

        if (extractSound != null)
            audioSource.PlayOneShot(extractSound, Options.SFX_MULTIPLIER);

        extractCooldown = Time.realtimeSinceStartup + 0.1f;

        last.gameObject.SetActive(true);
        inventory.Remove(last);
        return;
    }

    public override void LeftClickInWorld(Player player) // Insert
    {
        Pickup itemInHand = player.itemInHand == null ? null : player.itemInHand.item;

        if (itemInHand == null) // No item in hand? Pick up everything inside the combiner.
            return;

        if (inventory.isFull)
            return; // Don't add anything.

        if (insertSound != null)
            audioSource.PlayOneShot(insertSound, Options.SFX_MULTIPLIER);

        inventory.Add(itemInHand, gui.drawPanel.gameObject);
        player.inventoryHandler.inventory.Remove(itemInHand);
        itemInHand.gameObject.SetActive(false);
    }

    /// <summary>
    /// Finds a recipe for which all ingredients are present.
    /// </summary>
    /// <returns></returns>
    private CombineRecipe findCompleteRecipe()
    {
        foreach (CombineRecipe recipe in recipes)
        {
            bool hasAll = true;

            if (recipe.ingredients.Count != inventory.Count)
                continue; // Only check if the same amount of ingredients are in the combiner and in the recipe.

            foreach (Pickup ingredient in recipe.ingredients)
            {
                if (!inventory.Contains(ingredient))
                {
                    hasAll = false;
                    break;
                }
            }

            if (!hasAll)
                continue;

            return recipe;
        }

        return null;
    }
}
