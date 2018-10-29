using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Handles the player's use and interaction with items.
/// </summary>
public class PlayerItemHandler : MonoBehaviour
{
    [SerializeField]
    private Player player;

    private PlayerInventoryHandler inventoryHandler
    {
        get
        {
            return player.inventoryHandler;
        }
    }

    /// <summary>
    /// Global offset of items held in hand
    /// </summary>
    [SerializeField]
    [Tooltip("Global offset of whatever the player is holding, can be individually changed for each pickup.")]
    public Vector3 handOffset = new Vector3(0.5f, -0.5f, 0.5f);

    [SerializeField]
    private float _interactRange = 20f;
    public float interactRange
    {
        get
        {
            return _interactRange;
        }
    }

    [SerializeField]
    private float throwForce = 1f;

    [SerializeField]
    private float startThrowDistance = 2.5f;

    [SerializeField]
    private Vector3 throwOffset;

    [SerializeField]
    private RectTransform popupPrefab;

    /// <summary>
    /// Registers the item handler to <see cref="PlayerClickManager"/>
    /// </summary>
    public void RegisterDefaultHandler(PlayerClickManager manager)
    {
        manager.RegisterLeftClickDefault(handleLeftClick);
        manager.RegisterRightClickDefault(handleLeftClick);
    }

    private void OnValidate()
    {
        RegisterDefaultHandler(player.clickInput);
        player.itemInHand = player.itemInHand; // Update the offset.
    }

    private void Start()
    {
        OnValidate();
    }

    private GameObject _lookingAt;

    /// <summary>
    /// The current object the player is looking at.
    /// Or null if they are not looking at anything.
    /// </summary>
    public GameObject lookingAt
    {
        get
        {
            if (_lookingAt == null || !_lookingAt.activeInHierarchy) // If the object is destroyed, or no longer active in the hierarchy, stop looking at it.
            {
                return null;
            }

            return _lookingAt;
        }

        set
        {
            if (value == _lookingAt)
                return;

            screenPopup = null; // reset the screenPopup;

            bool isItemInHand = player.itemInHand != null && lookingAt == player.itemInHand.item.gameObject;

            if (!isItemInHand && lookingAt != null)
                lookAt(lookingAt, false); // Notify last looked at item we stopped looking at it.

            _lookingAt = value;

            if (!isItemInHand && lookingAt != null)
                lookAt(lookingAt, true); // And notify our newly looked at game object that we are now watching it.

            Hoverable interactable = value.FindComponent<Hoverable>(RedUtil.FindMode.ALL);

            if (interactable != null)
                StartCoroutine(ShowPopup(value, interactable));
        }
    }

    /// <summary>
    /// The popup that is currently on the screen.
    /// </summary>
    private GameObject _screenPopup;

    /// <summary>
    /// If the popup on the screen is already set, destroy it and then set it to value.
    /// Makes sure there is only one popup on the screen.
    /// </summary>
    private GameObject screenPopup
    {
        set
        {
            if (_screenPopup != null)
                Destroy(_screenPopup);

            _screenPopup = value;
        }
    }

    private IEnumerator ShowPopup(GameObject part, Hoverable interactable)
    {
        if (interactable == null || interactable.hoverText == null || interactable.hoverText.Length == 0)
            yield break;

        string[] text = interactable.hoverText;
        string fullText = "";

        foreach (string s in text)
        {
            if (s.Length == 0)
                continue;

            fullText += s + "\n"; // Add each line with a newline.
        }

        if (fullText.Length == 0)
            yield break; // Nothing to display.

        GameObject popup = Instantiate(popupPrefab.gameObject);
        RectTransform transform = popup.GetComponent<RectTransform>();

        screenPopup = popup; // Set the screen popup.

        Text textComp = popup.GetComponentInChildren<Text>();
        textComp.text = fullText; // Set the text to the text component.

        Vector2 offsetMin = textComp.rectTransform.offsetMin;
        Vector2 offsetMax = -1 * textComp.rectTransform.offsetMax; // We want the offset from the right bottom corner, but positive.

        transform.sizeDelta = new Vector2(textComp.preferredWidth + offsetMin.x + offsetMax.x, textComp.preferredHeight + offsetMin.y + offsetMax.y); // Calculate the new width and height of the panel in which the text resides.

        popup.transform.SetParent(player.canvas.transform);

        while(true)
        {
            if (popup == null)
                break;

            transform.position = Input.mousePosition; // Update the position while we're looking at it.

            yield return new WaitForEndOfFrame(); // Only check this once a frame.

            if (lookingAt == null || part == null || lookingAt.transform.root != part.transform.root) // Wait until we're not looking at it anymore.
                break;
        }

        Destroy(popup); // And then destroy it.
    }

    /// <summary>
    /// Forcefully updates the popup.
    /// </summary>
    public void UpdatePopup ()
    {
        // Reset lookingAt to update the popup;
        GameObject lookedAtObject = lookingAt;
        lookingAt = null;
        lookingAt = lookedAtObject;
    }

    /// <summary>
    /// Send a message to the gameobject that we are now either looking at it, or not anymore.
    /// This only works if the gameobject is an <see cref="Hoverable"/>.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="state"></param>
    private void lookAt(GameObject obj, bool state)
    {
        Hoverable i = obj.FindComponent<Hoverable>(RedUtil.FindMode.ALL);

        if (i != null)
        {
            if (state)
                i.StartLookingAtInWorld(player);
            else
                i.StopLookingAtInWorld(player);
        }
    }

    private void Update()
    {
        handleHover(); // First handle what the player is looking at.

        if (Input.GetMouseButton(1)) // Right mouse click
        {
            handleInteract();
        }
    }

    private void handleLeftClick()
    {
        if (player.itemInHand == null)
            return;

        player.itemInHand.item.LeftClickInHand(player);

        if (lookingAt != null)
        {
            Hoverable hoverable = lookingAt.GetComponent<Hoverable>();

            if (hoverable != null)
            {
                hoverable.LeftClickInWorld(player);
            }
        }
    }

    private void handleRightClick()
    {
        if (player.itemInHand == null)
            return;

        player.itemInHand.item.RightClickInHand(player);

        if (lookingAt != null)
        {
            Hoverable hoverable = lookingAt.GetComponent<Hoverable>();

            if (hoverable != null)
            {
                hoverable.RightClickInWorld(player);
            }
        }
    }

    public event Action<Pickup> OnItemPickup;

    private float interactCooldown = 0;

    /// <summary>
    /// Handle a player right clicking, which should try to pick up or interact with whatever they are looking at.
    /// </summary>
    private void handleInteract()
    {
        if (interactCooldown > Time.realtimeSinceStartup)
            return;

        interactCooldown = Time.realtimeSinceStartup + 0.2f; // Add a 0.2 second cooldown.

        if (lookingAt == null)
            return;

        Pickup pickup = lookingAt.FindComponent<Pickup>(RedUtil.FindMode.ALL);

        if (pickup != null)
        {
            Pickup(pickup);
            return;
        }

        Hoverable hoverable = lookingAt.FindComponent<Hoverable>(RedUtil.FindMode.ALL);

        if (hoverable != null)
            hoverable.RightClickInWorld(player);
    }

    /// <summary>
    /// Pick up a Pickup, same as when a player right clicks a pickup.
    /// </summary>
    /// <param name="pickup"></param>
    /// <param name="callEvent">Should we call OnItemPickup?</param>
    /// <returns>True if the item was succesfully added to the inventory, false if there is no space.</returns>
    public bool Pickup(Pickup pickup, bool callEvent = true)
    {
        //pick up only if there is space in inventory
        if (inventoryHandler.inventory.isFull)
        {
            inventoryHandler.WarnFull(); // Display full feedback.
            return false;
        }

        if (OnItemPickup != null)
            OnItemPickup(pickup);

        if (pickup.isRigid)
            Destroy(pickup.GetComponent<Rigidbody>()); // Get rid of rigid body when picking up.

        lookAt(pickup.gameObject, false);

        return true;
    }

    /// <summary>
    /// Drop a pickup
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool DropItem(Pickup item)
    {
        Ray ray = player.cameraHandler.camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        item.transform.SetParent(null);

        bool rigid = item.isRigid;

        if (rigid)
        {
            item.gameObject.AddComponent<Rigidbody>(); // Re-add the rigid body.
        }

        if (Physics.Raycast(ray, out hit, interactRange, 0, QueryTriggerInteraction.Ignore))
        {
            Collider collider = item.GetComponent<Collider>();

            Vector3 rotationLook = -player.cameraHandler.currentDirection;

            rotationLook.y = 0; // Only rotate in x and z

            Quaternion rotation = Quaternion.LookRotation(rotationLook); // Rotation to set object to (derived from player look)

            if (collider != null)
            {
                GameObject toPlace = item.gameObject;

                if (!rigid) // If we're not a rigid body, create one and use that.
                {
                    toPlace = Instantiate(item.gameObject); // Create a copy of the item.
                    toPlace.AddComponent<Rigidbody>(); // Add a rigid body
                    Destroy(toPlace.GetComponent<Pickup>()); // Remove the pickup from the item so we can't pick it up in animation.

                    item.gameObject.SetActive(false); // Disable while the fake takes it's place.

                    collider = toPlace.GetComponent<Collider>();
                }

                Vector3 size = collider.bounds.extents;

                float offset = Math.Max(Math.Max(size.x, size.y), size.z); // Get longest part of the body.

                toPlace.transform.position = hit.point - (ray.direction * offset); // Move it back towards the player to make sure it falls inwards.
                toPlace.transform.rotation = rotation; // Set rotation to the impact normal

                if (!rigid)
                    StartCoroutine(waitUntilStable(item, toPlace)); // Wait until the block is stable, and then freeze it.
            }
            else // Without a collider, we can't add a rigid body anyways.
            {
                item.gameObject.SetActive(true); // Simply enable it.

                item.transform.position = hit.point - (ray.direction * 0.1f); // Move it slightly towards the player, to make sure it covers most things.
                item.transform.rotation = rotation; // Set rotation to the impact normal
            }

            //item dropped, return true
            return true;
        }
        else // No raycast hit? Then just launch it in the general direction.
        {
            GameObject clone = item.gameObject;
            Rigidbody body = clone.GetComponent<Rigidbody>();

            if (!rigid)
            {
                clone = Instantiate(item.gameObject); // Create a copy of the item.
                body = clone.AddComponent<Rigidbody>(); // Add a rigid body
                Destroy(clone.GetComponent<Pickup>()); // Remove the pickup from the item so we can't pick it up in animation.

                item.gameObject.SetActive(false); // Disable while the fake takes it's place.
            }

            //reposition the clone in front of the player
            Vector3 repositionDirection = player.cameraHandler.currentDirection;
            repositionDirection.y = 0;
            Vector3 newPosition = player.body.transform.position + repositionDirection * startThrowDistance;
            clone.transform.position = newPosition + throwOffset;

            body.AddForce((throwOffset + player.cameraHandler.currentDirection).normalized * throwForce);
            
            // Reset clone rotation.
            Vector3 playerVerticalRotation = player.cameraHandler.verticalRotation * new Vector3(1, 0, 0);
            Vector3 rotation = item.handRotation.eulerAngles - playerVerticalRotation;
            clone.transform.Rotate(-rotation);

            // Freeze clone rotation.
            body.freezeRotation = true;
            
            if (!rigid)
                StartCoroutine(waitUntilStable(item, clone)); // Wait until the block is stable, and then freeze it.

            return true;
        }
    }

    /// <summary>
    /// Waits until the fake object is in a stable position (One that wont change without outside forces), and then replace it with the actual object.
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="fake"></param>
    /// <returns></returns>
    private IEnumerator waitUntilStable(Pickup actual, GameObject fake)
    {
        Vector3 lastPosition = fake.transform.position;
        Vector3 lastRotation = fake.transform.rotation.eulerAngles;

        while (true)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Vector3 newPosition = fake.transform.position;
            Vector3 diffPosition = newPosition - lastPosition;
            lastPosition = newPosition;

            Vector3 newRotation = fake.transform.rotation.eulerAngles;
            Vector3 diffRotation = newRotation - lastRotation;
            lastRotation = newRotation;

            if (diffPosition.magnitude < 0.01f && diffRotation.magnitude < 0.01f) // Check if our position and rotation changed minimally in the last two fixed updates.
                break; // If so, break because we are now stable.
        }

        actual.gameObject.SetActive(true);
        actual.transform.position = fake.transform.position;
        actual.transform.rotation = fake.transform.rotation;

        if (lookingAt == fake)
            lookingAt = null; // Unset if we were looking at it.

        Destroy(fake); // Get rid of the fake.
    }

    /// <summary>
    /// Handle the player looking at something.
    /// <see cref="lookingAt"/> is set and unset in this block.
    /// </summary>
    private void handleHover()
    {
        Ray ray = player.cameraHandler.camera.ScreenPointToRay(Input.mousePosition);

        float closest = float.MaxValue;
        GameObject closestObject = null;

        foreach (RaycastHit hit in Physics.RaycastAll(ray, interactRange))
        {
            GameObject gameObject = hit.collider.gameObject;
            Vector2 point = hit.point;

            if (gameObject.transform.root.GetComponent<Player>() == player)
                continue;

            float distance = Vector3.Distance(player.cameraHandler.camera.transform.position, point);

            if (distance < closest)
            {
                closestObject = gameObject;
                closest = distance;
            }
        }

        lookingAt = closestObject;
    }
}
