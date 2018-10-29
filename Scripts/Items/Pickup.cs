using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A pickup is an item that can be picked up and then held in your hand.
/// </summary>
public class Pickup : Hoverable
{
    /// <summary>
    /// Offset of this pickup when held.
    /// </summary>
    [SerializeField]
    [Tooltip("Local offset of this pickup, used if the pickup needs to be held differently. Global offset can be set in the player.")]
    public Vector3 handOffset = new Vector3(0, 0, 0);

    [SerializeField]
    [Tooltip("Rotation of this pickup when in hand.")]
    private Vector3 handRotationEuler = new Vector3(0, 0, 0);

    private bool _isRigid = false;

    /// <summary>
    /// Did this item have a rigid body on initialization.
    /// </summary>
    public bool isRigid
    {
        get
        {
            return _isRigid;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _isRigid = GetComponent<Rigidbody>() != null; // Remember if we had a rigid body.
    }

    [HideInInspector]
    public Quaternion handRotation;

    [SerializeField]
    private Sprite _inventoryIcon = null;

    public Sprite inventoryIcon
    {
        get
        {
            return _inventoryIcon;
        }

        private set
        {
            _inventoryIcon = value;
        }
    }

    /// <summary>
    /// Called whenever some part of this pickup is changed.
    /// </summary>
    public event Action Update;

    protected virtual void OnValidate()
    {
        if (inventoryIcon == null)
            inventoryIcon = Resources.Load<Sprite>("missing item");

        handRotation = Quaternion.Euler(handRotationEuler);

        if (Update != null)
            Update();
    }

    /// <summary>
    /// Called when a player left clicks this item in their hand.
    /// </summary>
    /// <param name="player"></param>
    public virtual void LeftClickInHand(Player player)
    {

    }

    /// <summary>
    /// Called when a player right clicks this item in their hand.
    /// </summary>
    /// <param name="player"></param>
    public virtual void RightClickInHand(Player player)
    {

    }
}
