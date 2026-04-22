using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    [SerializeField] private InventoryManager _inventoryManager;

    public event System.Action OnItemCollected;

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
            AudioEvents.RaiseAudioCue(AudioCueType.Pickup);
            OnItemCollected?.Invoke();
            itemPickerControl.Pick(); 
        }
    }


}
