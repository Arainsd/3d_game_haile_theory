using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextExplainerMessage : MonoBehaviour
{
    [SerializeField]
    private string[] message;

    private int indexOfMessage = 0; // Index of the message we are reading at. 
    private int indexOfString = 0; // Index of the character in the current string at the IndexOfMessage 
    private string messageDone = ""; // The message that has been written so far.

    private float textTimer = 0f;
    private bool done = false;

    private TextExplainer explainer;

    private void Awake()
    {
        explainer = GetComponentInParent<TextExplainer>();
    }

    public void UpdateMessage(Text text)
    {
        if (done)
            return;

        textTimer += Time.deltaTime; // Add the current time to the timer.

        float secondsPerCharacter = explainer.secondsPerCharacter; // Store the value for the entire update.

        while (textTimer > secondsPerCharacter) // Loop as long as we can remove the seconds per character off this.
        {
            textTimer -= secondsPerCharacter; // Decrement the timer by the amount of seconds for one character.
            addCharacter(); // And then add a character.
        }

        text.text = messageDone;
    }

    /// <summary>
    /// Instantly finish the message, this will be done when the player presses any key.
    /// </summary>
    public void Finish(Text text)
    {
        string fullMessage = "";

        foreach (string s in message)
        {
            fullMessage += s + "\n";
        }

        messageDone = fullMessage;
        text.text = fullMessage;
        done = true;
    }

    private void addCharacter()
    {
        if (indexOfMessage >= message.Length) // Is the index over the amount of lines in the message?
        {
            done = true; // Then we've read the full message.
            return;
        }

        string line = message[indexOfMessage];

        if (indexOfString >= line.Length) // Is the index over the amount of characters in the line?
        {
            indexOfMessage++; // Increment the line.

            if (indexOfMessage >= message.Length) // Are we now at the end of the message?
            {
                done = true; // Then we've read the full message.
                return;
            }

            indexOfString = 0; // Reset counter
            messageDone += "\n"; // Add newline.
            textTimer -= explainer.waitAfterNextLine - Time.deltaTime; // Add delay, but subtract this frame.
            return;
        }

        char character = line[indexOfString];

        messageDone += character;
        indexOfString++;
    }

    public bool IsDone()
    {
        return done;
    }
}

