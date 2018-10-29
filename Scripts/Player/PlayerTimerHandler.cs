using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PlayerTimerHandler : MonoBehaviour
{
    [SerializeField]
    private Text display;

    [Serializable]
    public struct Duration
    {
        public float minutes;
        public float seconds;

        public float ms
        {
            get
            {
                return (minutes * 60 * 1000) + (seconds * 1000);
            }

            set
            {
                float seconds = value / 1000f;

                this.seconds = seconds % 60; // The amount of seconds is what can't be divided by 60.

                seconds -= this.seconds; // Subtract the amount of real seconds.

                float minutes = seconds / 60f; // Then find the amount of minutes of the seconds left.

                this.minutes = minutes;
            }
        }
    }

    [SerializeField]
    private Duration time;

    private void FixedUpdate()
    {
        if (Options.PAUSED)
            return;

        time = decrement(time, Time.deltaTime);
        setTime((int)time.minutes, (int)time.seconds);
    }

    private Duration decrement(Duration dur, float timeDelta)
    {
        Duration newDuration = dur;

        newDuration.ms = newDuration.ms - (timeDelta * 1000);

        return newDuration;
    }

    private void setTime(int minutes, int seconds)
    {
        string secondsString = "" + seconds;

        if (secondsString.Length == 1)
            secondsString = "0" + secondsString; // Add a 0 if the second timer is a single digit.

        display.text = minutes + ":" + secondsString;
    }
}
