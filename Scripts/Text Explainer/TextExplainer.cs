using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextExplainer : MonoBehaviour
{
    private List<TextExplainerMessage> messages = new List<TextExplainerMessage>();
    private Text textBox;

    [SerializeField]
    [Tooltip("After a message is done writing, how long do we wait before moving to the next?")]
    public float waitAfterMessage = 1f;

    [SerializeField]
    private float charactersPerSecond;

    [SerializeField]
    public float waitAfterNextLine = 0.5f;

    public float secondsPerCharacter
    {
        get
        {
            return 1f / charactersPerSecond;
        }
    }

    [SerializeField]
    private string sceneToMoveTo;

    private void Awake()
    {
        messages.AddRange(this.FindComponents<TextExplainerMessage>(RedUtil.FindMode.CHILDREN));
        textBox = this.FindComponent<Text>(RedUtil.FindMode.SELF);

        Cursor.lockState = CursorLockMode.None; // Set lockstate to none.
        Cursor.visible = true;
    }

    private TextExplainerMessage current;
    private float waitAfterMessageTimer = 0f;

    private void Update()
    {
        if (current == null)
            nextMessage();

        if (finished)
            return;

        current.UpdateMessage(textBox);

        if (Input.GetKeyDown(KeyCode.S)) // Skip the entire segment.
        {
            finish(); // Instantly go to the next scene.
        }
        else if (Input.anyKeyDown)
        {
            if (!current.IsDone())
                current.Finish(textBox);
            else
                nextMessage();
        }

        if (current.IsDone()) // Is the message done displaying
        {
            waitAfterMessageTimer += Time.deltaTime;

            if (waitAfterMessageTimer > waitAfterMessage)
            {
                waitAfterMessageTimer = 0; // Once we're over it, disable the timer.
                nextMessage(); // And go to the next message.
            }
        }
    }

    private bool finished = false;

    private void finish()
    {
        Cursor.lockState = CursorLockMode.None; // Set lockstate to none.
        Cursor.visible = true;

        SceneManager.LoadScene(sceneToMoveTo);

        finished = true;
    }

    private void nextMessage()
    {
        int index = current == null ? 0 : messages.IndexOf(current) + 1;

        if (messages.Count <= index)
        {
            finish();
            return;
        }

        current = messages[index];
    }
}
