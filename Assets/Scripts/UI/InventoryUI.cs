using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private RectTransform _slotContainer;
    [SerializeField] private GameObject _slotPrefab;

    private InventorySlotUI[] _slots;
    private ResponsiveGrid _responsiveGrid;

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
        _responsiveGrid.SetItemCount(_inventoryManager.MaxNumberOfItems); // Imposto il numero iniziale di item per adattare la griglia

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null) continue; // Salto slot nulli per evitare errori

            _slots[i].Initialize(_inventoryManager, i); // Inizializzo ogni slot passando l'indice e il riferimento all'inventario per poter gestire l'uso degli item al click dello slot
        }

        RefreshInventoryUI();

    }
    private void OnEnable()
    {
        if (_inventoryManager != null)
        {
            _inventoryManager.OnInventoryChanged += RefreshInventoryUI; // Mi iscrivo all'evento di cambio dell'inventario per aggiornare la UI ogni volta che l'inventario cambia
        }
    }

    private void OnDisable()
    {
        if (_inventoryManager != null)
        {
            _inventoryManager.OnInventoryChanged -= RefreshInventoryUI; // Mi disiscrivo dall'evento quando la UI viene disabilitata per evitare errori di riferimento nulli
        }
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

    }

    private void ClearSlotContainer()
    {
        if (_slotContainer == null) return;
        for (int i = _slotContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_slotContainer.GetChild(i).gameObject); // Distruggo tutti i figli del container per pulire la griglia prima di creare nuovi slot
        }
    }

}
