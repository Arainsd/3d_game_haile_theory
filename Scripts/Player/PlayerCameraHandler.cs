using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Player camera script, should be attached to a camera inside a player.
/// Controls the player's camera.
/// This camera is allowed to move along the edge of a capsule. (The control area)
/// The camera is controlled by the player's mouse position.
/// If the player's mouse is in the center of the screen, the player will move forward.
/// If it is to the left, the camera will start looking left, and if the player moves it rotates to the left as far as it's looking.
/// Same thing for right.
/// </summary>
public class PlayerCameraHandler : MonoBehaviour
{
    [SerializeField]
    private CapsuleCollider controlArea;

    [SerializeField]
    private Player player;

    [HideInInspector]
    public new Camera camera; // Use new, since MonoBehaviour.camera is a Component, not a Camera

    [SerializeField]
    [Tooltip("How fast does our mouse movement rotate the player's camera horizontally.")]
    private float horizontalSensitivity = 3f;

    [SerializeField]
    [Tooltip("How fast does our mouse movement rotate the player's camera vertically.")]
    private float verticalSensitivity = 3f;

    [SerializeField]
    [Tooltip("What is the max angle we can rotate vertically.\nIf set too high, the player will bend strangely at the bottom and top.")]
    [Range(0, 90)]
    private float verticalLimit = 80;

    [SerializeField]
    [Tooltip("How far our camera is pushed inside the control area, a value of 0 will mean the camera is on the edge of the control area.\nThis is used to prevent the camera from clipping into the vertices of other colliders.")]
    [Range(0, 1)]
    private float inwardOffset = 0.8f;

    /// <summary>
    /// The current direction vector of the camera.
    /// </summary>
    [HideInInspector]
    public Vector3 currentDirection = new Vector3(1f, 0f, 1f);

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    private float _verticalRotation = 0; // Since the body of the player only rotates vertically, we must save the horizontal rotation to this variable.
    public float verticalRotation
    {
        get { return _verticalRotation; }
    }

    private void Update()
    {
        if (Options.PAUSED)
            return;

        if (player.inGUI)
            return;

        Quaternion current = player.body.transform.rotation;

        float vertical = Input.GetAxis("Mouse Y") * verticalSensitivity;
        float horizontal = Input.GetAxis("Mouse X") * horizontalSensitivity;

        _verticalRotation = Mathf.Clamp(verticalRotation + vertical, -verticalLimit, verticalLimit);

        Quaternion addHorizontal = Quaternion.AngleAxis(horizontal, new Vector3(0, 1, 0));

        Vector3 verticalAxis = (current * Quaternion.Euler(0, 90, 0) * new Vector3(1, 0, 1)); // Find the vector axis from the look rotation.
        float actualVerticalRotation = 360 - verticalRotation; // The y rotation is flipped.

        Quaternion addVertical = Quaternion.AngleAxis(actualVerticalRotation, verticalAxis); 

        Quaternion resultWithoutVertical = addHorizontal * current;
        Quaternion result = addVertical * resultWithoutVertical;

        Quaternion rot = result;
        Quaternion bodyRot = resultWithoutVertical;

        player.body.transform.rotation = bodyRot;

        Ray controlRay = getRayInControlArea(rot * new Vector3(1f, 0f, 1f));

        Quaternion offset = Quaternion.Euler(0, -45, 0); // The camera needs to be rotated back to the rotation of the body

        currentDirection = offset * controlRay.direction;
        _lookRotation = offset * Quaternion.LookRotation(controlRay.direction, Vector3.up);


        camera.transform.rotation = _lookRotation; 
        camera.transform.position = controlRay.origin;
    }

    private Quaternion _lookRotation;

    /// <summary>
    /// A quaternion that applies <see cref="currentDirection"/>
    /// </summary>
    public Quaternion lookRotation
    {
        get
        {
            return _lookRotation;
        }
    }

    /// <summary>
    /// Find a position on the control area based on the direction vector specified.
    /// </summary>
    /// <param name="dir">A direction vector, specifying which direction the player is looking in.</param>
    /// <returns></returns>
    private Ray getRayInControlArea(Vector3 dir)
    {
        Vector3 center = controlArea.transform.TransformPoint(controlArea.center);

        Ray centerRay = new Ray(center, dir);

        // We can't cast a ray from inside a collider, so move it out of it.

        Vector3 sizeWorld = controlArea.transform.TransformPoint(new Vector3(controlArea.radius, controlArea.height));
        float longestSide = Math.Max(sizeWorld.x, sizeWorld.y); // Find which side is the largest in the world space.
        float rayLength = longestSide * 1.1f; // Multiply by 1.1 so we are just out of the camera.

        Vector3 inverseRay = centerRay.GetPoint(rayLength); // Create a new ray position that is a rayLength away from the center.
        Ray ray = new Ray(inverseRay, -centerRay.direction); // And then create a reversed ray

        foreach (RaycastHit hit in Physics.RaycastAll(ray, rayLength)) // Cast the ray back into the control area and find an collision.
        {
            if (hit.collider == controlArea)
            {
                Vector3 point = hit.point - (hit.normal * (controlArea.radius * inwardOffset)); // Point of the hit, and then use the hit's normal to push the camera inwards with the offset value.

                return new Ray(point, hit.normal); // If it's our control area, return a ray describing the hit.
            }
        }

        return centerRay;
    }
}
