using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Handles movement of the player.
/// </summary>
public class PlayerMovementHandler : MonoBehaviour
{
    [SerializeField]
    private float gravity = 0.5f;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float jumpHeight;

    /// <summary>
    /// How long we must wait in between jumps, in seconds
    /// </summary>
    [SerializeField]
    private float jumpInterval;

    [SerializeField]
    private float pushPower = 2f;

    private CharacterController controller;

    [SerializeField]
    private Player player;

    [SerializeField]
    private AudioClip jumping;

    [SerializeField]
    private float jumpingVolume;

    [SerializeField]
    private float jumpingPitch;

    [SerializeField]
    private AudioClip walking;

    [SerializeField]
    private float walkingVolume;

    [SerializeField]
    private float walkingPitch;

    [SerializeField]
    private AudioSource audioSource;

    private AudioSource walkingAudioSource;

    private bool _audioPaused;

    /// <summary>
    /// Pauses and unpauses the audioSources.
    /// </summary>
    private bool audioPaused
    {
        set
        {
            if (value == _audioPaused)
                return;

            // If value is true, pause all the audioSources, else unpause them.
            if (value)
            {
                audioSource.Pause();
                walkingAudioSource.Pause();
            }
            else
            {
                audioSource.UnPause();
                walkingAudioSource.UnPause();
            }

            _audioPaused = value;
        }
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        walkingAudioSource = gameObject.AddComponent<AudioSource>();
        walkingAudioSource.clip = walking;
        walkingAudioSource.loop = true;
    }

    private void FixedUpdate()
    {
        audioPaused = Options.PAUSED; // The audio pauses when the game pauses.

        if (Options.PAUSED)
            return;

        handleInput();
    }

    /// <summary>
    /// Y movement of the last update, in pixels per second.
    /// </summary>
    private float lastMoveY = 0;

    /// <summary>
    /// Jumping cooldown timer
    /// </summary>
    private float jumpCooldown = 0f;

    /// <summary>
    /// Gets input from all peripherals, and then creates a combined move vector, and passes it onto the character controller.
    /// </summary>
    private void handleInput()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = -Input.GetAxis("Horizontal"); // Horizontal axis is flipped for our purposes.

        if ((vertical > 0 || horizontal > 0) && Math.Abs(controller.velocity.y) < 0.1f)
        {
            isWalking = true;

            walkingAudioSource.volume = walkingVolume * Options.SFX_MULTIPLIER;
            walkingAudioSource.pitch = walkingPitch;

            if (!walkingAudioSource.isPlaying)
                walkingAudioSource.Play();
        }
        else
        {
            isWalking = false;

            if (walkingAudioSource.isPlaying)
                StartCoroutine(fadeSound(walkingAudioSource));
        }

        Vector3 newMove = Move(Time.deltaTime * vertical * moveSpeed, Time.deltaTime * horizontal * moveSpeed);

        bool jump = controller.isGrounded && Input.GetKey(KeyCode.Space) && jumpCooldown < Time.realtimeSinceStartup;

        newMove.y += lastMoveY * Time.deltaTime; // Include our previous velocity in y so we get smooth movement while jumping, multiply by delta time to get time per frame again.

        if (!jump && !controller.isGrounded)
            newMove.y -= Time.deltaTime * gravity; // Apply gravity if we are in the air.

        if (jump)
        {
            jumpCooldown = Time.realtimeSinceStartup + jumpInterval;
            newMove.y += Time.deltaTime * jumpHeight;

            if (!audioSource.isPlaying)
            {
                audioSource.clip = jumping;
                audioSource.volume = jumpingVolume * Options.SFX_MULTIPLIER;
                audioSource.pitch = jumpingPitch;

                audioSource.Play();
            }
        }

        lastMoveY = controller.isGrounded && newMove.y <= 0 ? 0 : newMove.y / Time.deltaTime; // Save our current velocity, transformed back to time per seconds.

        controller.Move(newMove);
    }

    private bool isWalking = false;
   
    private IEnumerator fadeSound(AudioSource source)
    {
        float startVolume = source.volume;
        float volume = startVolume;

        while (startVolume > 0)
        {
            volume -= 1f * Time.deltaTime;
            source.volume = volume;

            yield return new WaitForEndOfFrame();

            if (isWalking) // If we started walking again, cancel the task
            {
                source.volume = startVolume; // And reset the volume.
                break;
            }
        }

        yield break;
    }

    /// <summary>
    /// Static quaternion that rotates (euler) by 90 degrees in y.
    /// Used to rotate our look quaternion so we can move in the direction we're looking at.
    /// </summary>
    private static readonly Quaternion rotate90 = Quaternion.Euler(0, -90, 0);

    /// <summary>
    /// Create a vector that moves forward and/or sideways when passed to a character controller.
    /// </summary>
    /// <param name="forward"></param>
    /// <param name="sideways"></param>
    /// <returns></returns>
    private Vector3 Move(float forward, float sideways)
    {
        Quaternion rot = player.cameraHandler.lookRotation;

        Vector3 euler = rot.eulerAngles;

        euler.x = 0; // Only rotate with the y angle
        euler.z = 0;

        rot = Quaternion.Euler(euler);

        return rot * rotate90 * new Vector3(forward, 0, sideways);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) // Handle interacting with a rigid body, and apply a push to it.
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3F)
            return;

        Vector3 dir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z); // Get direection we pushed in

        body.AddForce(dir * pushPower, ForceMode.Impulse); // Apply velocity.
    }
}
