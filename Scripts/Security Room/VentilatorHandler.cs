using UnityEngine;
using System.Collections;

public class VentilatorHandler : MonoBehaviour
{

    [SerializeField]
    private float ventilatorVolume;

    [SerializeField]
    private float ventilatorPitch;

    private AudioSource audioSource;

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

        audioSource.volume = ventilatorVolume * Options.SFX_MULTIPLIER;
        audioSource.pitch = ventilatorPitch;
        audioSource.loop = false;
	}

    private void Update ()
    {
        audioPaused = Options.PAUSED; // The audio pauses when the game pauses.
    }

    public void startVentilator ()
    {
        // Play the ventilator audio.
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
