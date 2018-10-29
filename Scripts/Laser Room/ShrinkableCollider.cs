using UnityEngine;
using System.Collections;

/// <summary>
/// Added to colliders to reference what Shrinkable object they belong to.
/// </summary>
public class ShrinkableCollider : MonoBehaviour
{
    public Shrinkable colliderFor;
    public new Collider collider;
}
