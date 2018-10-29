using UnityEngine;
using System.Collections;

/// <summary>
/// Options script, contains fields to set or get options.
/// Each option will have a static and non-static version.
/// The static version is for static access, ie. Options.PAUSED = false
/// The non-static version is for Unity access or access through scripts (even though scripts CAN use the static version)
/// Unity can't see static variables on a script, and can only bind sliders / inputfields to non-static variables.
/// </summary>
public class Options : MonoBehaviour
{
    // ********************************* Paused *********************************** //

    private static bool _paused = false;

    public static bool PAUSED
    {
        get
        {
            return _paused;
        }

        set
        {
            _paused = value;
        }
    }

    public bool paused
    {
        get
        {
            return _paused;
        }

        set
        {
            _paused = value;
        }
    }

    // ********************************* Music Multiplier *********************************** //

    private const string musicMultiplierKey = "music multiplier";
    private const float musicMultiplierDefault = 1f;

    public static float MUSIC_MULTIPLIER
    {
        get
        {
            return PlayerPrefs.GetFloat(musicMultiplierKey, musicMultiplierDefault);
        }

        set
        {
            PlayerPrefs.SetFloat(musicMultiplierKey, value);
        }
    }

    public float musicMultiplier
    {
        get
        {
            return PlayerPrefs.GetFloat(musicMultiplierKey, musicMultiplierDefault);
        }

        set
        {
            PlayerPrefs.SetFloat(musicMultiplierKey, value);
        }
    }

    // ********************************* SFX Multiplier *********************************** //

    private const string sfxMultiplierKey = "sfx multiplier";
    private const float sfxMultiplierDefault = 1f;

    public static float SFX_MULTIPLIER
    {
        get
        {
            return PlayerPrefs.GetFloat(sfxMultiplierKey, sfxMultiplierDefault);
        }

        set
        {
            PlayerPrefs.SetFloat(sfxMultiplierKey, value);
        }
    }

    public float sfxMultiplier
    {
        get
        {
            return PlayerPrefs.GetFloat(sfxMultiplierKey, sfxMultiplierDefault);
        }

        set
        {
            PlayerPrefs.SetFloat(sfxMultiplierKey, value);
        }
    }
}
