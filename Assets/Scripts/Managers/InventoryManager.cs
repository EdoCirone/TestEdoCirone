using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    // Gestisce lo stato dell'inventario di una singola entità.
    // Non è un singleton per scelta: più istanze possono coesistere
    // per player diversi o NPC con inventari propri.

    [Header("Inventory Settings")]
    [SerializeField] private int _maxNumberOfItems = 9;

    // Lista a dimensione fissa dove null rappresenta uno slot vuoto.
    // Mantenere la dimensione costante semplifica il mapping UI:
    // l'indice i nella lista corrisponde sempre allo slot i sullo schermo.
    [Header("Inventory State")]
    [SerializeField] private List<ItemData> _itemsInInventory = new List<ItemData>();

    // Espongo in sola lettura per impedire a codice esterno di bypassare la logica dell'inventario.
    public IReadOnlyList<ItemData> ItemsInInventory => _itemsInInventory;

    public int MaxNumberOfItems => _maxNumberOfItems;

    #region Events
    public event System.Action OnInventoryChanged;
    public event System.Action OnInventoryFull;
    #endregion

    private void Awake()
    {

        while (_itemsInInventory.Count > _maxNumberOfItems)
        {
            _itemsInInventory.RemoveAt(_itemsInInventory.Count - 1);
        }


        for (int i = _itemsInInventory.Count; i < _maxNumberOfItems; i++)
        {
            _itemsInInventory.Add(null);
        }
    }

    public bool AddItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("Cannot add null item to inventory.");
            return false;
        }

        for (int i = 0; i < _itemsInInventory.Count; i++)
        {
            if (_itemsInInventory[i] == null)
            {
                _itemsInInventory[i] = item;
                OnInventoryChanged?.Invoke();
                return true;

            }

        }

        Debug.LogWarning("Inventory is full!");
        OnInventoryFull?.Invoke();
        AudioEvents.RaiseAudioCue(AudioCueType.InventoryFull);
        return false;

    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= _itemsInInventory.Count)
        {
            Debug.LogWarning("Invalid item index.");
            return;
        }

        // Imposto null invece di rimuovere l'elemento così gli indici degli slot rimangono stabili.
        _itemsInInventory[index] = null;

        OnInventoryChanged?.Invoke();
    }

    public void MoveOrSwapItems(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= _itemsInInventory.Count || toIndex < 0 || toIndex >= _itemsInInventory.Count)
        {
            Debug.LogWarning("Invalid item index for move or swap.");
            return;
        }

        if (fromIndex == toIndex)
        {
            return;
        }

        if (_itemsInInventory[fromIndex] == null)
        {
            Debug.LogWarning("Source slot is empty.");
            return;
        }

        // Lo swap funziona sia per spostamento in slot vuoto che per scambio con slot occupato.
        ItemData temp = _itemsInInventory[fromIndex];

        _itemsInInventory[fromIndex] = _itemsInInventory[toIndex];
        _itemsInInventory[toIndex] = temp;

        OnInventoryChanged?.Invoke();
        AudioEvents.RaiseAudioCue(AudioCueType.Swap);

    }


    #region DEBUG METHODS

    [ContextMenu("Remove First Item")]
    private void RemoveFirstItemForDebug()
    {
        for (int i = 0; i < _itemsInInventory.Count; i++)
        {
            if (_itemsInInventory[i] != null)
            {
                RemoveItem(i);
                return;
            }
        }

        Debug.LogWarning("No items to remove.");
    }

    [ContextMenu("Swap Slot 0 And 1")]
    private void SwapFirstTwoSlotsForDebug()
    {
        MoveOrSwapItems(0, 1);
    }

    #endregion
}
