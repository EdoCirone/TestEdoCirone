using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip _inventoryOpenClip;
    [SerializeField] private AudioClip _inventoryCloseClip;
    [SerializeField] private AudioClip _inventoryFullClip;
    [SerializeField] private AudioClip _healthFullClip;
    [SerializeField] private AudioClip _deathClip;
    [SerializeField] private AudioClip _pickupClip;
    [SerializeField] private AudioClip _dropClip;
    [SerializeField] private AudioClip _swapClip;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _uiAudioSource;
    [SerializeField] private AudioSource _gameplayAudioSource;

    private void Awake()
    {
        if (_uiAudioSource == null)
        {
            Debug.LogWarning("UI AudioSource reference is not assigned in AudioManager.");
        }
        if (_gameplayAudioSource == null)
        {
            Debug.LogWarning("Gameplay AudioSource reference is not assigned in AudioManager.");
        }
    }


    private void OnEnable()
    {
        AudioEvents.OnAudioCueRequested += HandleAudioCue;
        AudioEvents.OnAudioClipRequested += PlayGameplayAudio;
    }

    private void OnDisable()
    {
        AudioEvents.OnAudioCueRequested -= HandleAudioCue;
        AudioEvents.OnAudioClipRequested -= PlayGameplayAudio;
    }

    private void HandleAudioCue(AudioCueType cueType)
    {
        switch (cueType)
        {
            case AudioCueType.InventoryOpen:
                HandleInventoryOpenClip();
                break;

            case AudioCueType.InventoryClose:
                HandleInventoryCloseClip();
                break;

            case AudioCueType.InventoryFull:
                HandleInventoryFullClip();
                break;

            case AudioCueType.HealthFull:
                HandleHealthFullClip();
                break;

            case AudioCueType.Death:
                HandleDeathClip();
                break;

            case AudioCueType.Pickup:
                HandlePickupClip();
                break;

            case AudioCueType.Drop:
                HandleDropClip();
                break;

            case AudioCueType.Swap:
                HandleSwapClip();
                break;
        }
    }

    private void HandleSwapClip()
    {
        PlayUIAudio(_swapClip);
    }

    private void HandleInventoryFullClip()
    {
        PlayUIAudio(_inventoryFullClip);
    }

    private void HandleHealthFullClip()
    {
        PlayUIAudio(_healthFullClip);
    }

    private void HandleDeathClip()
    {
        PlayGameplayAudio(_deathClip);
    }

    private void HandleInventoryOpenClip()
    {
        PlayUIAudio(_inventoryOpenClip);
    }

    private void HandleInventoryCloseClip()
    {
        PlayUIAudio(_inventoryCloseClip);
    }

    private void HandlePickupClip()
    {
        PlayGameplayAudio(_pickupClip);
    }

    private void HandleDropClip()
    {
        PlayGameplayAudio(_dropClip);
    }

    private void PlayUIAudio(AudioClip clip)
    {
        if (clip == null || _uiAudioSource == null) return;
        _uiAudioSource.PlayOneShot(clip);
    }

    private void PlayGameplayAudio(AudioClip clip)
    {
        if (clip == null || _gameplayAudioSource == null) return;
        _gameplayAudioSource.PlayOneShot(clip);
    }

}
