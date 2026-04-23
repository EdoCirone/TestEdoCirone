using UnityEngine;

public enum InventoryUIType
{
    Player,
    Container
}

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private RectTransform _slotContainer;
    [SerializeField] private RectTransform _inventoryPanel;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private PlayerItemDropper _playerItemDropper;

    [SerializeField] private InventoryInteractionManager _interactionManager; // Riferimento al PlayerInteractionManager per verificare se una cassa è aperta (per evitare di usare un item mentre si interagisce con la cassa, anche qui devo disaccoppiare)
    [SerializeField] private InventoryUIType _inventoryUIType = InventoryUIType.Player; // Tipo di UI per differenziare comportamenti futuri (es. player vs container)

    private InventorySlotUI[] _slots;
    private ResponsiveGrid _responsiveGrid;

    // Traccia lo stato di iscrizione agli eventi per evitare doppie iscrizioni
    private bool _isSubscribedToEvents = false;
    public InventoryUIType GetInventoryUIType() => _inventoryUIType; 

    private void Awake()
    {
        if (_slotContainer == null)
        {
            Debug.LogError("SlotContainer reference is missing in InventoryUI.");
            return;
        }

        if (_slotPrefab == null)
        {
            Debug.LogError("SlotPrefab reference is missing in InventoryUI.");
            return;
        }

        if (_inventoryManager == null)
        {
            Debug.LogError("InventoryManager reference is missing in InventoryUI.");
            return;
        }

        ClearSlotContainer();

        for (int i = 0; i < _inventoryManager.MaxNumberOfItems; i++)
        {
            GameObject slot = Instantiate(_slotPrefab, _slotContainer);
            if (slot == null)
            {
                Debug.LogError("Failed to instantiate slot prefab.");
                continue;
            }
        }

        _responsiveGrid = _slotContainer.GetComponent<ResponsiveGrid>();

        if (_responsiveGrid == null)
        {
            Debug.LogError("ResponsiveGrid component is missing on SlotContainer.");
            return;
        }


        _slots = _slotContainer.GetComponentsInChildren<InventorySlotUI>(true);

    }

    private void Start()
    {
        if (_inventoryManager == null)
        {
            Debug.LogError("InventoryManager reference is missing in InventoryUI.");
            return;
        }
        if (_slotContainer == null)
        {
            Debug.LogError("SlotContainer reference is missing in InventoryUI.");
            return;
        }
        if (_responsiveGrid == null)
        {
            Debug.LogError("ResponsiveGrid component is missing on SlotContainer.");
            return;
        }
        _responsiveGrid.SetSlotCount(_inventoryManager.MaxNumberOfItems);
        InitializeSlots();

        RefreshInventoryUI();

    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (_inventoryManager == null || _isSubscribedToEvents) return;

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null) continue;
            _slots[i].OnSlotClickedEvent += HandleSlotClicked;
        }

        _inventoryManager.OnInventoryChanged += RefreshInventoryUI;
        _isSubscribedToEvents = true;
    }

    private void UnsubscribeFromEvents()
    {
        if (_inventoryManager == null || !_isSubscribedToEvents) return;

        _inventoryManager.OnInventoryChanged -= RefreshInventoryUI;

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null) continue;
            _slots[i].OnSlotClickedEvent -= HandleSlotClicked;

        }
        _isSubscribedToEvents = false;
    }

    private void RefreshInventoryUI()
    {

        if (_inventoryManager == null || _slots == null)
            return;

        var items = _inventoryManager.ItemsInInventory;

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null) continue;

            if (i < items.Count && items[i] != null)
            {
                _slots[i].SetItem(items[i]);
            }
            else
            {
                _slots[i].ClearSlot();
            }
        }

    }

    private void ClearSlotContainer()
    {
        if (_slotContainer == null) return;
        for (int i = _slotContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_slotContainer.GetChild(i).gameObject);
        }
    }

    private void InitializeSlots()
    {
        // Metodo separato per inizializzare i slot dopo averli creati, se necessario in futuro.
        if (_slots == null) return;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null) continue;
            _slots[i].Initialize(_inventoryManager, i, _inventoryPanel, _playerItemDropper, _interactionManager);
        }
    }

    private void HandleSlotClicked(int index)
    {
      if(_interactionManager == null)
        {
            Debug.LogWarning("InventoryInteractionManager reference is missing in InventoryUI.");
            return;
        }
        _interactionManager.HandleSlotClick(this, index);
    }

    public void SetInventory(InventoryManager inventory)
    {
        _inventoryManager = inventory;
        InitializeSlots();
        RefreshInventoryUI();
    }

}
