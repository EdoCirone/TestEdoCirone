using System;
using UnityEngine;

// Event bus audio: cue logici e clip diretti
public static class AudioEvents
{
    public static event Action<AudioCueType> OnAudioCueRequested;
    public static event Action<AudioClip> OnAudioClipRequested;

    public static void RaiseAudioCue(AudioCueType cueType)
    {
        OnAudioCueRequested?.Invoke(cueType);
    }

    public static void RaiseAudioClip(AudioClip clip)
    {
        if(clip == null)
        {
            Debug.LogWarning("AudioEvents: Attempted to raise OnPlayAudioClip with a null clip.");
            return;
        }
        OnAudioClipRequested?.Invoke(clip);
    }

}
