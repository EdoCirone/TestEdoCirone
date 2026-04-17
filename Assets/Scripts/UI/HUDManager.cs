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

    private void Start()
    {
        if (_playerStats == null)
        {
            Debug.LogError("PlayerController reference is missing in HUDManager.");
            return;
        }
        UpdateHealthBar();
        UpdateStats();
    }

    private void Update()
    {
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
