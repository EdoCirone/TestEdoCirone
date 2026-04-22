using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    // Gestisce lo stato dell'inventario di una singola entità.
    // Non è un singleton per scelta: più istanze possono coesistere
    // per player diversi o NPC con inventari propri.

    [Header("Inventory Settings")]
    [SerializeField] private int _maxNumberOfItems = 9;

    [Header("References")]
    [SerializeField] private PlayerStats _playerStats;

    // Lista a dimensione fissa dove null rappresenta uno slot vuoto.
    // Mantenere la dimensione costante semplifica il mapping UI:
    // l'indice i nella lista corrisponde sempre allo slot i sullo schermo.
    [SerializeField] private List<ItemData> _itemsInInventory = new List<ItemData>();

    // Espongo in sola lettura per impedire a codice esterno di bypassare la logica dell'inventario.
    public IReadOnlyList<ItemData> ItemsInInventory => _itemsInInventory;

    public int MaxNumberOfItems => _maxNumberOfItems;

    #region Events
    public event System.Action OnInventoryChanged;
    public event System.Action OnInventoryFull;
    public event System.Action<ItemData> OnItemUsed;
    #endregion

    private void Awake()
    {
        if (_playerStats == null)
        {
            Debug.LogWarning("PlayerStats reference is not assigned in InventoryManager.");
        }

        _itemsInInventory.Clear();

        // Inizializzo con null così la lista ha sempre esattamente _maxNumberOfItems elementi.
        for (int i = 0; i < _maxNumberOfItems; i++)
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


    public void UseItem(int index)
    {

        if (index < 0 || index >= _itemsInInventory.Count)
        {
            Debug.LogWarning("Invalid item index.");
            return;
        }

        if (_playerStats == null)
        {
            Debug.LogWarning("PlayerStats reference is not assigned.");
            return;
        }


        ItemData item = _itemsInInventory[index];

        if (item == null)
        {
            Debug.LogWarning("Item not assigned. ");
            return;
        }

        if (item.Effect == null)
        {
            Debug.LogWarning("Effect not assigned to item. ");
            return;
        }

        bool effectApplied = item.Effect.ApplyEffect(_playerStats, item.Amount);

        if (effectApplied)
        {
            OnItemUsed?.Invoke(item);
            AudioEvents.RaiseAudioClip(item.Effect.AudioClip);
            RemoveItem(index);
        }


    }

    #region DEBUG METHODS

    [ContextMenu("Use Test Item")]
    private void UseFirstItemForDebug()
    {
        for (int i = 0; i < _itemsInInventory.Count; i++)
        {
            if (_itemsInInventory[i] != null)
            {
                UseItem(i);
                return;
            }
        }

        Debug.LogWarning("No items in inventory to use.");
    }

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
