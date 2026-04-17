using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    // Non serve che sia singleton perchè non è un manager globale, è un componente che gestisce l'inventario di un singolo player,
    //quindi può essere istanziato più volte se necessario (es. per più player o per NPC con inventari).
    //Se fosse un manager globale allora si potrebbe considerare di renderlo singleton,
    //ma in questo caso non è necessario e potrebbe limitare la flessibilità del design.

    [SerializeField] private int _maxNumberOfItems = 9;
    [SerializeField] private PlayerStats _playerStats;

    [SerializeField] private List<ItemData> _itemsInInventory = new List<ItemData>();

    public IReadOnlyList<ItemData> ItemsInInventory => _itemsInInventory; //Espongo la lista per la ui con l'interfaccia ReadOnly perchè voglio leggere ma non modificare

    public bool AddItem(ItemData item) 
    {
        if (item == null)
        {
            Debug.LogWarning("Cannot add null item to inventory.");
            return false;
        }

        if (_itemsInInventory.Count >= _maxNumberOfItems)
        {
            Debug.LogWarning("Inventory is full!");
            return false;
        }

        _itemsInInventory.Add(item);
        return true;
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= _itemsInInventory.Count)
        {
            Debug.LogWarning("Invalid item index.");
            return;
        }
        _itemsInInventory.RemoveAt(index);
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
        if (_itemsInInventory.Count > 0)
        {
            UseItem(0); // Usa il primo item nella lista per testare
        }
        else
        {
            Debug.LogWarning("No items in inventory to use.");
        }
    }

    #endregion
}
