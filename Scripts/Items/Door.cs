using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Animator))]
public class Door : Hoverable
{
    [SerializeField]
    private bool startOpen = false;

    [SerializeField]
    private AudioClip openDoorSound;

    [SerializeField]
    private AudioClip closeDoorSound;

    /// <summary>
    /// Audio source on the door.
    /// </summary>
    [SerializeField]
    protected AudioSource audioSource;

    public string[] closedText;
    public string[] openText;

    protected virtual void Start()
    {
        OnValidate();
    }

    public void OnValidate()
    {
        open = !startOpen; // Set to the opposite so we can call the methods.

        if (startOpen)
            OpenDoor(false);
        else
            CloseDoor(false);
    }

    private bool open = false;

    public bool IsOpen
    {
        get
        {
            return open;
        }
    }

    private float soundCooldown = 0f;

    public override void RightClickInWorld(Player player)
    {
        if (open)
            return;

        Action deny = () =>
        {
            if (soundCooldown > Time.realtimeSinceStartup)
                return;

            soundCooldown = Time.realtimeSinceStartup + closeDoorSound.length; // Cooldown until the sound is done.

            if (audioSource != null && closeDoorSound != null)
                audioSource.PlayOneShot(closeDoorSound, Options.SFX_MULTIPLIER);
        };

        Pickup itemInHand = player.itemInHand == null ? null : player.itemInHand.item;

        if (itemInHand == null)
        {
            deny();
            return;
        }

        Key key = itemInHand as Key;

        if (key == null || !key.Match(this))
        {
            deny();
            return;
        }

        if (key.singleUse)
        {
            player.inventoryHandler.inventory.Remove(key);
            Destroy(key.gameObject);
        }

        OpenDoor(true, player);
    }

    public void OpenDoor(bool animation = true, Player player = null)
    {
        if (open)
            return;

        if (player == null)
            player = Player.instance;

        Animator animator = GetComponent<Animator>();

        animator.Play(animation ? "Open" : "Opened");
        open = true;

        hoverText = openText;

        if (player != null)
            player.itemHandler.UpdatePopup();

        if (animation && audioSource != null && openDoorSound != null)
            audioSource.PlayOneShot(openDoorSound, Options.SFX_MULTIPLIER);
    }

    public void CloseDoor(bool animation = true, Player player = null)
    {
        if (!open)
            return;

        if (player == null)
            player = Player.instance;

        Animator animator = GetComponent<Animator>();

        animator.Play(animation ? "Close" : "Closed");
        open = false;

        hoverText = closedText;

        if (player != null)
            player.itemHandler.UpdatePopup();

        if (animation && audioSource != null && closeDoorSound != null)
            audioSource.PlayOneShot(closeDoorSound, Options.SFX_MULTIPLIER);
    }
}
