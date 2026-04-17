using UnityEngine;

public class ItemPickerControl : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private InventoryManager _inventory;

    private void OnMouseDown()
    {
        if (_inventory == null)
        {
            Debug.LogWarning("InventoryManager is not assigned.");
            return;
        }

        if (_itemData == null)
        {
            Debug.LogWarning("ItemData is not assigned.");
            return;
        }

        if (_itemData.Effect == null)
        {
            Debug.LogWarning("Effect not assigned to item");
            return;
        }

        Debug.Log($"Picked up {_itemData.ItemName}");

        bool added = _inventory.AddItem(_itemData);

        if (added)
        {
            Destroy(gameObject); // Distruggo l'oggetto invece che fare un sistema di pooling perchè per questo test mi basta

        }
    }


}
