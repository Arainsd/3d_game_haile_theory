using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndDoorTrigger : MonoBehaviour
{
    [Tooltip("The scene that will be loaded after the gameplay.")]
    [SerializeField]
    private string endSceneName;

    [SerializeField]
    private GameObject outOfGameMusicPrefab;

    private void OnTriggerEnter(Collider col)
    {
        Player player = col.gameObject.FindComponent<Player>(RedUtil.FindMode.PARENTS_AND_SELF);

        if (player == null)
            return;

        SceneManager.LoadScene(endSceneName); // Load the scene for ending the game.

        if (outOfGameMusicPrefab != null)
            Instantiate(outOfGameMusicPrefab); // Create a new out of game music
    }
}
