using System.Collections.Generic;
using UnityEngine;

public class VoiceFeedbackSystem : MonoBehaviour
{
    private AudioSource audioSource;
    private Dictionary<string, AudioClip> feedbackClips;

    public void PlayFeedback(string message)
    {
        if (feedbackClips.ContainsKey(message))
        {
            audioSource.PlayOneShot(feedbackClips[message]);
        }
    }
}