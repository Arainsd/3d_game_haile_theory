using UnityEngine;
using System.Collections;

public class OutOfGameMusic : MonoBehaviour
{
    private void Awake()
    {
        OutOfGameMusic[] instances = FindObjectsOfType<OutOfGameMusic>();
            
        if (instances.Length > 1) // Is there already an out of game music on the scene?
        {
            Destroy(gameObject); // Then we are not needed.
            return;
        }

        DontDestroyOnLoad(this.gameObject); // Make sure we dont get deleted unless we do so ourselves.
    }

    private void OnLevelWasLoaded()
    {
        if (FindObjectOfType<Player>() != null) // Is there a player on the scene now? (Meaning we're ingame)
        {
            Destroy(gameObject); // Destroy ourselves.
        }
    }
}
