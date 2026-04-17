using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    [SerializeField] private InventoryManager _inventoryManager;

    public void TryPickItem(ItemPickerControl itemPickerControl)
    {
        if (_inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager reference is null in PlayerItemCollector.");
            return;
        }

        if (itemPickerControl == null)
        {
            Debug.LogWarning("ItemPickerControl reference is null.");
            return;
        }

        ItemData item = itemPickerControl.GetItem();

        if (item == null)
        {
            Debug.LogWarning("ItemData reference is null in ItemPickerControl.");
            return;
        }

        bool added = _inventoryManager.AddItem(item);
        if (added)

        {
            itemPickerControl.Pick(); // Distrugge l'oggetto nella scena solo se l'item è stato aggiunto con successo all'inventario, altrimenti rimane nella scena per poter essere raccolto da un altro player o in un secondo momento
        }
    }


}
