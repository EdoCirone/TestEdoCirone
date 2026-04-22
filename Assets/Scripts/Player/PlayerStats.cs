using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    [Header("Player Settings")]
    [SerializeField] private float _maxHealth = 50f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _strength = 10f;

    private float _currentHealth;

    public float MaxHealth => _maxHealth;
    public float Speed => _speed;
    public float Strength => _strength;
    public float CurrentHealth => _currentHealth;
    public bool IsDead => _currentHealth <= 0f;

    #region Events

    public event System.Action OnHealthChanged;
    public event System.Action OnStatsChanged;
    public event System.Action OnPlayerDeath;
    public event System.Action OnHealthAlreadyFull;

    #endregion

    private void Start()
    {
        _currentHealth = _maxHealth * 0.5f;// Inizializzo la vita a metà per testare subito sia cura sia danno.
        OnHealthChanged?.Invoke();
        OnStatsChanged?.Invoke();
    }

    public bool TryHeal(float amount)
    {
        if (_currentHealth >= _maxHealth)
        {
            OnHealthAlreadyFull?.Invoke();
            AudioEvents.RaiseAudioCue(AudioCueType.HealthFull);
            return false;
        }
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
        OnHealthChanged?.Invoke();
        return true;

    }

    public void TakeDamage(float amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth);
        OnHealthChanged?.Invoke();

        if (_currentHealth <= 0f)
        {
            OnPlayerDeath?.Invoke();
            AudioEvents.RaiseAudioCue(AudioCueType.Death);
        }
    }

    public void UpdateSpeed(float amount)
    {
        _speed = Mathf.Max(0, _speed + amount);
        OnStatsChanged?.Invoke();
    }

    public void UpdateStrength(float amount)
    {
        _strength = Mathf.Max(0, _strength + amount);
        OnStatsChanged?.Invoke();

    }
}
