using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Scrollbar _healthBar;
    [SerializeField] private TextMeshProUGUI _textStats;

    [Header("Player References")]
    [SerializeField] private PlayerStats _playerStats;

    private void OnEnable()
    {
        if (_playerStats == null) return;

        _playerStats.OnHealthChanged += UpdateHealthBar;
        _playerStats.OnStatsChanged += UpdateStats;
    }

    private void OnDisable()
    {
        if (_playerStats == null) return;

        _playerStats.OnHealthChanged -= UpdateHealthBar;
        _playerStats.OnStatsChanged -= UpdateStats;

    }

    private void Start()
    {
        if (_playerStats == null)
        {
            Debug.LogError("Player reference is missing in HUDManager.");
            return;
        }

        if (_healthBar == null)
        {
            Debug.LogError("Health Bar reference is missing in HUDManager.");
            return;
        }
        if (_textStats == null)
        {

            Debug.LogError("Text Stats reference is missing in HUDManager.");
            return;
        }

        UpdateHealthBar();
        UpdateStats();
    }

    private void UpdateHealthBar()
    {
        _healthBar.size = _playerStats.CurrentHealth / _playerStats.MaxHealth;
    }

    private void UpdateStats()
    {
        _textStats.text = $"Speed: {_playerStats.Speed}\n" +
                            $"Strength: {_playerStats.Strength}";

    }
}
