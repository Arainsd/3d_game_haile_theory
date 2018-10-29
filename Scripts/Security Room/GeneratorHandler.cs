using UnityEngine;
using System.Collections;

public class GeneratorHandler : Interactable
{

    [Tooltip("The hover text that will appear after the player interacts with the generator.")]
    public string[] afterInteractionHoverText = new string[4];

    [SerializeField]
    private AudioClip generatorAudio;

    [SerializeField]
    private float generatorVolume;

    [SerializeField]
    private float generatorPitch;

    private AudioSource audioSource;
    private bool generatorOn = false;

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
                audioSource.Pause();
            else
                audioSource.UnPause();

            _audioPaused = value;
        }
    }

    private void Start ()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = generatorVolume * Options.SFX_MULTIPLIER;
        audioSource.pitch = generatorPitch;
	}

    private void Update ()
    {
        audioPaused = Options.PAUSED; // The audio pauses when the game pauses.
    }

    public override void RightClickInWorld(Player player)
    {
        // Generator is already on, return.
        if (generatorOn)
            return;

        // Play the generator audio.
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(generatorAudio, generatorVolume);

        // Change the hover text.
        hoverText = afterInteractionHoverText;
        player.itemHandler.UpdatePopup();

        // Call the interact method.
        Interact();

        // Generator is now on.
        generatorOn = true;
    }

}
