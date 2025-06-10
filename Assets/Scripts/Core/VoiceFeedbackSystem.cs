using System.Collections.Generic;
using UnityEngine;

public class VoiceFeedbackSystem : MonoBehaviour
{
    private AudioSource audioSource;
    private Dictionary<string, AudioClip> feedbackClips;

    public void PlayFeedback(string message)
    {
        // Use Unity's built-in TTS or prerecorded clips
        if (feedbackClips.ContainsKey(message))
        {
            audioSource.PlayOneShot(feedbackClips[message]);
        }
    }
}