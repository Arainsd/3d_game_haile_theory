using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// An Interactable is an item that you can interact with.
/// Right clicking will cause OnInteract to be called.
/// </summary>
public class Interactable : Hoverable
{
    [SerializeField]
    protected UnityEvent onInteract;

    public void Interact()
    {
        if (onInteract != null)
            onInteract.Invoke();
    }
}
