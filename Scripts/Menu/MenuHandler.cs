using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles menu interaction.
/// Holds methods for menu buttons.
/// </summary>
public class MenuHandler : MonoBehaviour
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private MenuScreen mainMenuScreen;
    [SerializeField]
    private MenuScreen optionsScreen;
    [SerializeField]
    private MenuScreen pauseScreen;

    [SerializeField]
    private Slider sfxSlider;
    [SerializeField]
    private Slider musicSlider;

    [SerializeField]
    private string mainMenuSceneName;
    [SerializeField]
    private string gameSceneName;

    [SerializeField]
    private AudioClip clickSound;
    [SerializeField]
    private float clickVolume;
    [SerializeField]
    private float clickPitch;

    [SerializeField]
    private AudioClip hoverSound;
    [SerializeField]
    private float hoverVolume;
    [SerializeField]
    private float hoverPitch;

    private AudioSource audioSource;

    private GraphicRaycaster uiRaycaster;

    private float musicVolume;
    private float sfxVolume;

    private void Start()
    {
        // If there is a main menu, make it the current screen.
        if (mainMenuScreen != null)
        {
            currentScreen = mainMenuScreen;
        }

        uiRaycaster = GetComponent<GraphicRaycaster>();
        audioSource = GetComponent<AudioSource>();

        // Get the values from the settings.
        musicVolume = Options.MUSIC_MULTIPLIER;
        sfxVolume = Options.SFX_MULTIPLIER;

        // Set the sliders' values to the settings.
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }

    private bool _gameActive = true;
    private bool gameActive
    {
        get
        {
            return _gameActive;
        }

        set
        {
            // Pause the game if it is not active.
            Options.PAUSED = !value;

            if (value)
            {
                // Disable the menu screens
                currentScreen = null;

                // Deactivate the GUI.
                if (player != null)
                    player.inGUI = false;
            }

            _gameActive = value;
        }
    }

    private MenuScreen _currentScreen;
    private MenuScreen currentScreen
    {
        get
        {
            return _currentScreen;
        }

        set
        {
            // Deactivate the current menu screen before changing to the given one.
            if (_currentScreen != null)
                _currentScreen.active = false;

            if (value != null)
            {
                // Activate the given menu screen.
                value.active = true;

                // Pause the game because a menu screen is active.
                gameActive = false;

                // Activate the GUI
                if (player != null)
                    player.inGUI = true;
            }

            _currentScreen = value;
        }
    }

    private void Update()
    {
        handleInput();
        handleAudio();
    }

    private Button hoveredButton;

    /// <summary>
    /// Plays hover sound when hovering over a new button;
    /// Plays click sound when a button is pressed;
    /// </summary>
    private void handleAudio ()
    {
        // Set up the event data.
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // Raycast on the UI.
        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(eventData, results);

        foreach (RaycastResult result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();

            // If button is null, the result doesn't have a Button component.
            // Therefore, we can continue to the next result.
            if (button == null)
                continue;

            // We know the mouse is over a button.
            // if player clicks the left mouse button, play the click sound.
            if (Input.GetMouseButtonDown(0))
                playClickSound();

            // If button is the same as the currently hoveredButton, there's no need to play the hover sound, return.
            if (button == hoveredButton)
                return;

            hoveredButton = button; // Now we know there is a new hovered button, so change hoveredButton to button.
            playHoverSound(); // Play the hoverSound, as there is a new hovered button.

            return; // A button hover took place, return.
        }
        
        hoveredButton = null; // If this point is reached we know there is no button in the raycast, so no button is hovered. Change hoveredButton to null.
    }

    /// <summary>
    /// Plays the sound for button hovering.
    /// </summary>
    private void playHoverSound ()
    {
        // If audio source is already playing, return.
        if (audioSource.isPlaying)
            return;

        // Play the hover sound.
        audioSource.volume = hoverVolume * Options.SFX_MULTIPLIER;
        audioSource.pitch = hoverPitch;
        audioSource.PlayOneShot(hoverSound);
    }

    /// <summary>
    /// Plays the sound for clicking buttons.
    /// </summary>
    private void playClickSound()
    {
        audioSource.volume = clickVolume * Options.SFX_MULTIPLIER;
        audioSource.pitch = clickPitch;
        audioSource.PlayOneShot(clickSound);
    }

    /// <summary>
    /// Pauses the game if Escape is pressed while playing.
    /// Resumes gameplay or goes back through the menus if Escape is pressed when menus are active.
    /// </summary>
    private void handleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameActive) // If game is active, then pause the game.
            {
                currentScreen = pauseScreen;
            }
            else if (pauseScreen.active) // If pause menu is active, then go back to gameplay.
            {
                gameActive = true;
            }
            else if (optionsScreen.active) // If options is active, then go back to the last menu.
            {
                currentScreen = lastScreen;
            }
        }
    }

    /// <summary>
    /// Start the game.
    /// </summary>
    public void OnPlayClick()
    {
        StartCoroutine(startGameAtEndOfAudio()); // Wait for menu sounds to stop playing, then start the game.
    }

    /// <summary>
    /// Starts gameplay when menu sounds stop playing.
    /// </summary>
    /// <returns></returns>
    private IEnumerator startGameAtEndOfAudio()
    {
        // Wait until there is no menu sound being played.
        if (audioSource.isPlaying)
            yield return new WaitForEndOfFrame();

        // Start gameplay.
        gameActive = true;
        SceneManager.LoadScene(gameSceneName); // Load the game.
    }

    /// <summary>
    /// Resume gameplay.
    /// </summary>
    public void OnResumeClick()
    {
        gameActive = true;
    }

    /// <summary>
    /// Go back to main menu.
    /// </summary>
    public void OnBackToMainMenuClick()
    {
        SceneManager.LoadScene(mainMenuSceneName); // Load the main menu scene.
    }

    /// <summary>
    /// Last used menu screen.
    /// </summary>
    private MenuScreen lastScreen;

    /// <summary>
    /// Open options menu.
    /// </summary>
    public void OnOptionsClick()
    {
        // Set the last menu screen to the current one before changing to the options screen.
        lastScreen = currentScreen;
        currentScreen = optionsScreen;
    }

    /// <summary>
    /// Go back to the last menu screen.
    /// </summary>
    public void OnBackClick()
    {
        if (lastScreen != null)
        {
            currentScreen = lastScreen;
        }
    }

    /// <summary>
    /// Change music volume.
    /// </summary>
    public void OnMusicSlider()
    {
        musicVolume = musicSlider.value;
    }

    /// <summary>
    /// Change sfx volume.
    /// </summary>
    public void OnSfxSlider()
    {
        sfxVolume = sfxSlider.value;
    }

    /// <summary>
    /// Saves music volume and sfx volume to the settings.
    /// </summary>
    public void OnSaveClick()
    {
        Options.MUSIC_MULTIPLIER = musicVolume;
        Options.SFX_MULTIPLIER = sfxVolume;
    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    public void OnExitClick()
    {
        Application.Quit();
    }
}
