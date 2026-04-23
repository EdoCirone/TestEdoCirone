using UnityEngine;

public class PlayerInventoryHandler : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private InventoryManager _inventoryManager;

    public void UseItem(int index)
    {
        if (_playerStats == null)
        {
            Debug.LogWarning("PlayerStats reference is missing.");
            return;
        }
        if (_inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager reference is missing.");
            return;
        }

        if (index < 0 || index >= _inventoryManager.MaxNumberOfItems)
        {
            Debug.LogWarning("Invalid item index.");
            return;
        }

        ItemData item = _inventoryManager.ItemsInInventory[index];

        if (item == null)
        {
            Debug.LogWarning("No item in the selected inventory slot.");
            return;
        }
        if (item.Effect == null)
        {
            Debug.LogWarning("The selected item has no effect assigned.");
            return;
        }

        bool effectApplied = item.Effect.ApplyEffect(_playerStats, item.Amount);
        if (effectApplied)
        {
            AudioEvents.RaiseAudioClip(item.Effect.AudioClip);
            _inventoryManager.RemoveItem(index);

        }
    }
}
