using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPickerControl : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;

    public ItemData GetItem()
    {
        return _itemData;
    }

    public void Pick()
    {
        Destroy(gameObject);
    }

    private void OnMouseDown() //Con un sistema centralizzato è meglio e non devo baypassare IsPointerOver
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        PlayerItemCollector playerItemCollector = FindFirstObjectByType<PlayerItemCollector>();

        if (playerItemCollector == null)
        {
            Debug.LogWarning("PlayerItemCollector not found in the scene.");
            return;
        }

        playerItemCollector.TryPickItem(this);
    }
}

