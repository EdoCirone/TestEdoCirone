using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    // Non serve che sia singleton perchè non è un manager globale, è un componente che gestisce l'inventario di un singolo player,
    //quindi può essere istanziato più volte se necessario (es. per più player o per NPC con inventari).
    //Se fosse un manager globale allora si potrebbe considerare di renderlo singleton,
    //ma in questo caso non è necessario e potrebbe limitare la flessibilità del design.
    [Header("Inventory Settings")]
    [SerializeField] private int _maxNumberOfItems = 9;

    [Header("References")]
    [SerializeField] private PlayerStats _playerStats;

    [SerializeField] private List<ItemData> _itemsInInventory = new List<ItemData>();

    public IReadOnlyList<ItemData> ItemsInInventory => _itemsInInventory; //Espongo la lista per la ui con l'interfaccia ReadOnly perchè voglio leggere ma non modificare

    public int MaxNumberOfItems => _maxNumberOfItems; //Espongo il numero massimo di item per generare slot vuoti nella ui

    #region events


    public event System.Action OnInventoryChanged; //Evento per notificare la UI quando l'inventario cambia, così da aggiornare la visualizzazione degli slot in base agli item presenti nell'inventario
    public event System.Action OnInventoryFull; //Evento per notificare quando l'inventario è pieno, così da poter mostrare un messaggio o un feedback al giocatore se cerca di aggiungere un item quando l'inventario è già pieno


    #endregion

    private void Awake()
    {
        if (_playerStats == null)
        {
            Debug.LogWarning("PlayerStats reference is not assigned in InventoryManager.");
        }

        _itemsInInventory.Clear(); // Pulisco la lista per assicurarmi che sia vuota all'inizio

        for (int i = 0; i < _maxNumberOfItems; i++) // Inizializzo la lista con null per avere sempre una lista con un numero di elementi pari al numero massimo di item, così da poter gestire dinamicamente gli slot in base al numero di item presenti nell'inventario (slot vuoti se null, slot pieni se non null)
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

        for (int i = 0; i < _itemsInInventory.Count; i++) // Cerco un slot vuoto (null) per aggiungere l'item, così da mantenere la dimensione della lista sempre pari al numero massimo di item e poter gestire dinamicamente gli slot in base al numero di item presenti nell'inventario (slot vuoti se null, slot pieni se non null)
        {
            if (_itemsInInventory[i] == null)
            {
                _itemsInInventory[i] = item;
                OnInventoryChanged?.Invoke(); // Notifico la UI che l'inventario è cambiato
                return true;

            }

        }

        Debug.LogWarning("Inventory is full!");
        OnInventoryFull?.Invoke(); // Notifico che l'inventario è pieno, così da poter mostrare un messaggio o un feedback al giocatore se cerca di aggiungere un item quando l'inventario è già pieno
        return false;

    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= _itemsInInventory.Count)
        {
            Debug.LogWarning("Invalid item index.");
            return;
        }
        _itemsInInventory[index] = null; // Imposto l'elemento a null invece di rimuoverlo dalla lista, così da mantenere la dimensione della lista sempre pari al numero massimo di item e poter gestire dinamicamente gli slot in base al numero di item presenti nell'inventario (slot vuoti se null, slot pieni se non null)
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
            Debug.LogWarning("Source slots is empty.");
            return;
        }

        ItemData temp = _itemsInInventory[fromIndex]; // Salvo temporaneamente l'item da spostare o scambiare per evitare di perderlo durante l'operazione

        _itemsInInventory[fromIndex] = _itemsInInventory[toIndex];
        _itemsInInventory[toIndex] = temp;
        OnInventoryChanged?.Invoke();

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
            Debug.LogWarning("Item not assigned");
            return;
        }

        if (item.Effect == null)
        {
            Debug.LogWarning("Effect not assigned to item");
            return;
        }

        item.Effect.ApplyEffect(_playerStats, item.Amount);

        RemoveItem(index);
    }

    #region DEBUG METODS

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
