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
    private bool _isSubscribedToEvents = false; // Variabile per tenere traccia se siamo già iscritti agli eventi, così da evitare di iscriversi più volte e avere un flusso instabile

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

        for (int i = 0; i < _inventoryManager.MaxNumberOfItems; i++) // Creo dinamicamente gli slot in base al numero massimo di item nell'inventario, così da poterli gestire dinamicamente in base al numero di item nell'inventario
        {
            GameObject slot = Instantiate(_slotPrefab, _slotContainer); // Instanzio lo slot prefab come figlio del container
            if (slot == null)
            {
                Debug.LogError("Failed to instantiate slot prefab.");
                continue;
            }
        }

        _responsiveGrid = _slotContainer.GetComponent<ResponsiveGrid>(); // Prendo il componente ResponsiveGrid dal container per poter aggiornare la griglia dinamicamente in base al numero di item nell'inventario

        if (_responsiveGrid == null)
        {
            Debug.LogError("ResponsiveGrid component is missing on SlotContainer.");
            return;
        }


        _slots = _slotContainer.GetComponentsInChildren<InventorySlotUI>(true); // Prendo tutti gli slot come componenti figli del container, così da poterli gestire dinamicamente in base al numero di item nell'inventario

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
        _responsiveGrid.SetSlotCount(_inventoryManager.MaxNumberOfItems); // Imposto il numero iniziale di item per adattare la griglia

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null) continue; // Salto slot nulli per evitare errori

            _slots[i].Initialize(_inventoryManager, i, _inventoryPanel, _playerItemDropper); // Inizializzo ogni slot passando l'indice e il riferimento all'inventario per poter gestire l'uso degli item al click dello slot
        }

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

        _inventoryManager.OnInventoryChanged += RefreshInventoryUI; // Mi iscrivo agli eventi
        _inventoryManager.OnInventoryFull += ShowFullInventoryMessage;
        _isSubscribedToEvents = true;
    }

    private void UnsubscribeFromEvents()
    {
        if (_inventoryManager == null || !_isSubscribedToEvents) return;
        _inventoryManager.OnInventoryChanged -= RefreshInventoryUI; // Mi disiscrivo dagli eventi per evitare memory leak o errori quando l'oggetto viene disabilitato o distrutto
        _inventoryManager.OnInventoryFull -= ShowFullInventoryMessage;
        _isSubscribedToEvents = false;
    }

    private void RefreshInventoryUI()
    {

        if (_inventoryManager == null || _slots == null)
            return;

        var items = _inventoryManager.ItemsInInventory; // Prendo la lista degli item attualmente nell'inventario

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null) continue; // Salto slot nulli per evitare errori

            if (i < items.Count && items[i] != null)
            {
                _slots[i].SetItem(items[i]); // Se c'è un item corrispondente all'indice dello slot, lo setto nello slot
            }
            else
            {
                _slots[i].ClearSlot(); // Altrimenti pulisco lo slot per mostrare che è vuoto
            }
        }

        if (_fullInventoryMessagePanel != null) HideFullInventoryMessage();

    }

    private void ClearSlotContainer() //E' una soluzione un pò fragile 
    {
        if (_slotContainer == null) return;
        for (int i = _slotContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_slotContainer.GetChild(i).gameObject); // Distruggo tutti i figli del container per pulire la griglia prima di creare nuovi slot
        }
    }

    private void ShowFullInventoryMessage()
    {
        if (_fullInventoryMessagePanel == null)
        {
            Debug.LogError("FullInventoryMessagePanel reference is missing in InventoryUI.");
            return;
        }
        _fullInventoryMessagePanel.gameObject.SetActive(true); // Mostro il pannello del messaggio di inventario pieno
    }

    private void HideFullInventoryMessage()
    {
        if (_fullInventoryMessagePanel == null)
        {
            Debug.LogError("FullInventoryMessagePanel reference is missing in InventoryUI.");
            return;
        }
        _fullInventoryMessagePanel.gameObject.SetActive(false); // Nascondo il pannello del messaggio di inventario pieno
    }

    public void OpenIventory()
    {
               _inventoryPanel.gameObject.SetActive(true); // Mostro il pannello dell'inventario
    }

}
