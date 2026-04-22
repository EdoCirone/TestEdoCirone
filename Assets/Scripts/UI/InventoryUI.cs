using UnityEngine;


public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private RectTransform _slotContainer;
    [SerializeField] private RectTransform _inventoryPanel;
    [SerializeField] private RectTransform _fullInventoryMessagePanel;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private PlayerItemDropper _playerItemDropper;


    private InventorySlotUI[] _slots;
    private ResponsiveGrid _responsiveGrid;

    // Traccia lo stato di iscrizione agli eventi per evitare doppie iscrizioni
    private bool _isSubscribedToEvents = false; 


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

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null) continue;

            _slots[i].Initialize(_inventoryManager, i, _inventoryPanel, _playerItemDropper); 
        }

        HideFullInventoryMessage();
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

        _inventoryManager.OnInventoryChanged += RefreshInventoryUI; 
        _inventoryManager.OnInventoryFull += ShowFullInventoryMessage;
        _isSubscribedToEvents = true;
    }

    private void UnsubscribeFromEvents()
    {
        if (_inventoryManager == null || !_isSubscribedToEvents) return;
        _inventoryManager.OnInventoryChanged -= RefreshInventoryUI; 
        _inventoryManager.OnInventoryFull -= ShowFullInventoryMessage;
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

    private void ShowFullInventoryMessage()
    {
        if (_fullInventoryMessagePanel == null)
        {
            Debug.LogError("FullInventoryMessagePanel reference is missing in InventoryUI.");
            return;
        }
        _fullInventoryMessagePanel.gameObject.SetActive(true); 

    }

    public void HideFullInventoryMessage()
    {
        if (_fullInventoryMessagePanel == null)
        {
            Debug.LogError("FullInventoryMessagePanel reference is missing in InventoryUI.");
            return;
        }

        _fullInventoryMessagePanel.gameObject.SetActive(false); 

    }

}
