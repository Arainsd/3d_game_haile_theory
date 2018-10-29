using UnityEngine;
using System.Collections;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;

    [Tooltip("The direction in which the camera rotates.")]
    [Range(-1, 1)]
    [SerializeField]
    private int rotationDirection = 1;

    [Tooltip("If the camera changes rotation direction when reaching horizontalLimit or rotates continuously.")]
    [SerializeField]
    private bool limitLess;

    [Tooltip("If the rotation of the camera reaches a horizontal limit, it will start rotating in the opposite direction.")]
    [SerializeField]
    private float horizontalLimit;

    private float rotationTracker;

    private void Start ()
    {
        // Initialize rotationTracker with the y rotation of the transform.
        rotationTracker = transform.rotation.eulerAngles.y;
    }

    private void Update()
    {
        float newRotation = rotationTracker + rotationSpeed * rotationDirection;

        // Update rotationTracker.
        // If limitLess is set to true, set rotationTracker to newRotation, else be sure it is inside the horizontal bounds.
        rotationTracker = limitLess ? newRotation : Mathf.Clamp(newRotation, -horizontalLimit, horizontalLimit);

        Vector3 cameraRotation = transform.rotation.eulerAngles;
        cameraRotation.y = rotationTracker; // Only rotate the y axis.
        transform.rotation = Quaternion.Euler(cameraRotation); // Change the rotation of the transform.

        // If limitLess, no need to check for changing the rotationDirection, return.
        if (limitLess)
            return;

        // If rotationTracker reached the horizontal bounds, change direction;
        if (Mathf.Abs(rotationTracker) >= horizontalLimit)
            rotationDirection = -rotationDirection;
    }
}