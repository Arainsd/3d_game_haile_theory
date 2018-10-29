using UnityEngine;
using System.Collections;

/// <summary>
/// A weapon is an object that can be picked up and then swung with the intention to deal damage.
/// </summary>
public class Weapon : Pickup
{
    /// <summary>
    /// The animation being played.
    /// </summary>
    [Tooltip("The animation in this weapon's animator that we will play when swinging it.")]
    public string swingWeaponAnimation = "swing weapon";

    public override void LeftClickInHand(Player player)
    {
        Animator animator = GetComponent<Animator>();
    
        animator.Play(swingWeaponAnimation);
    }
}
