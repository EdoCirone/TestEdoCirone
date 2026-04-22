using UnityEngine;

public class UIPanelManager : MonoBehaviour
{

    [Header("Panel References")]
    [SerializeField] private PanelAnimation _inventoryFullPanel;
    [SerializeField] private PanelAnimation _healthMaxPanel;
    [SerializeField] private PanelAnimation _deathPanel;

    [Header("Event Sources")]
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private PlayerStats _playerStats;

    private void OnEnable()
    {
        if (_inventoryManager != null)
            _inventoryManager.OnInventoryFull += OnInventoryFull;
        if (_playerStats != null)
        {
            _playerStats.OnHealthAlreadyFull += OnHealthAlreadyFull;
            _playerStats.OnPlayerDeath += OnPlayerDeath;
        }
    }

    private void OnDisable()
    {
        if (_inventoryManager != null)
            _inventoryManager.OnInventoryFull -= OnInventoryFull;
        if (_playerStats != null)
        {
            _playerStats.OnHealthAlreadyFull -= OnHealthAlreadyFull;
            _playerStats.OnPlayerDeath -= OnPlayerDeath;
        }
    }
    public void OnInventoryFull()
    {
        if (_inventoryFullPanel == null) return;
        CloseAllPanels();
        _inventoryFullPanel.Open();
    }

    public void OnHealthAlreadyFull()
    {
        if (_healthMaxPanel == null) return;
        CloseAllPanels();
        _healthMaxPanel.Open();
    }
    public void OnPlayerDeath()
    {
        if (_deathPanel == null) return;
        CloseAllPanels();
        _deathPanel.Open();
        Time.timeScale = 0f;// Metto in pausa il gioco quando il pannello di morte è aperto. Il pannello di morte ha un pulsante per riavviare il livello che ripristina il timeScale a 1.
    }

    public void CloseAllPanels()
    {
        _inventoryFullPanel?.Close();
        _healthMaxPanel?.Close();
        _deathPanel?.Close();
    }

}
