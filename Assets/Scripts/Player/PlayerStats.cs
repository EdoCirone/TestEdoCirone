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

    private void Start()
    {
        _currentHealth = _maxHealth * 0.5f;
    }

    public void Heal(float amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth)
  ;
    }

    public void TakeDamage(float amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth);
    }

    public void UpdateSpeed(float amount)
    {
               _speed += amount;
    }

    public void UpdateStrength(float amount)
    {
        _strength += amount;
    }
}
