using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/// <summary>
/// Manages all the click input of a player
/// This manager will take click input on the screen, see what UI element it hit, and then pass on the event to a specific handler.
/// If no UI element gets hit by the raycast, the default click handler will be called instead.
/// </summary>
public class PlayerClickManager : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private GraphicRaycaster uiRaycaster;

    [SerializeField]
    private Player player;

    private Action leftClickDefault = () => { };
    private Action rightClickDefault = () => { };
    private Dictionary<Predicate<RaycastResult>, Action<RaycastResult, PointerEventData>> clickHandlers = new Dictionary<Predicate<RaycastResult>, Action<RaycastResult, PointerEventData>>();

    protected void Awake()
    {
        player.itemHandler.RegisterDefaultHandler(this);
    }

    /// <summary>
    /// Register the default right click handler, this is what gets called if no ui element gets clicked
    /// </summary>
    /// <param name="onClick"></param>
    public void RegisterRightClickDefault(Action onClick)
    {
        rightClickDefault = onClick;
    }

    /// <summary>
    /// Register the default left click handler, this is what gets called if no ui element gets clicked
    /// </summary>
    /// <param name="onClick"></param>
    public void RegisterLeftClickDefault(Action onClick)
    {
        leftClickDefault = onClick;
    }

    /// <summary>
    /// Register a click handler.
    /// </summary>
    /// <param name="equals">A predicate that should return true if the attached RaycastResult hits the object you control.</param>
    /// <param name="onClick">If the predicate returns true, this gets called. Indicating your object was clicked.</param>
    public void Register(Predicate<RaycastResult> equals, Action<RaycastResult, PointerEventData> onClick)
    {
        if (clickHandlers.ContainsKey(equals))
            return;

        clickHandlers.Add(equals, onClick);
    }

    /// <summary>
    /// Register a click handler that gets called if a GUI element is present with the specified game object as one of it's parents.
    /// </summary>
    /// <param name="parent">The parent object that we want to listen to.</param>
    /// <param name="onClick">Called whenever any object that is a child of, or the parent itself, is clicked.</param>
    public void RegisterHasParent(GameObject parent, Action<RaycastResult, PointerEventData> onClick)
    {
        Predicate<RaycastResult> hasParent = (r) =>
        {
            GameObject obj = r.gameObject;

            while (obj != null)
            {
                if (obj.Equals(parent))
                    return true;

                obj = obj.transform.parent == null ? null : obj.transform.parent.gameObject;
            }

            return false;
        };

        Register(hasParent, onClick);
    }

    /// <summary>
    /// Call events for this RaycastResult
    /// </summary>
    /// <param name="result"></param>
    /// <returns>True if we handled the event, false if it should be passed on further.</returns>
    private bool callEvent(RaycastResult result, PointerEventData eventData)
    {
        foreach (KeyValuePair<Predicate<RaycastResult>, Action<RaycastResult, PointerEventData>> set in clickHandlers)
        {
            if (set.Key(result)) // Is this our object?
            {
                set.Value(result, eventData); // Handle it!
                return true;
            }
        }

        return false;
    }

    public void OnPointerDown(PointerEventData eventData) // Public by interface requirement
    {
        if (Options.PAUSED)
            return;

        // Raycast on the UI, get everything that's been hit.
        List<RaycastResult> raycastItems = new List<RaycastResult>();
        uiRaycaster.Raycast(eventData, raycastItems);

        if (raycastItems.Count == 0)
        {
            return;
        }

        foreach (RaycastResult result in raycastItems)
        {
            if (callEvent(result, eventData))
                return;
        }
    }

    public void Update()
    {
        if (Options.PAUSED)
            return;

        if (EventSystem.current == null)
            Debug.LogError("This scene contains no EventSystem, please add it as a root object.");

        bool left = Input.GetMouseButtonDown(0);
        bool right = Input.GetMouseButtonDown(1);

        if ((left || right) && (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject()))
        {
            if (left)
                leftClickDefault();
            else
                rightClickDefault();
        }
    }
}
