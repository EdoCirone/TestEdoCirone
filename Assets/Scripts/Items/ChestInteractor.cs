using UnityEngine;

public class ChestInteractor : MonoBehaviour
{
    [SerializeField] InventoryManager _chestInventory;
    [SerializeField] InventoryManager _playerInventory;
    [SerializeField] InventoryInteractionManager _interactionManager;

    private void Start()
    {
        if (_chestInventory == null)
        {
            Debug.LogError("Chest inventory reference is missing in ChestInteractor.");
        }
        if (_playerInventory == null)
        {

            Debug.LogError("Player GameObject found, but it does not have an InventoryManager component.");

        }

        if (_interactionManager == null)
        {
            Debug.LogError("InventoryInteractionManager reference is missing in ChestInteractor.");
        }
    }
    private void OnMouseDown()
    {
        OpenChestOnClick();
    }

    public void OpenChestOnClick()
    {
        if (_interactionManager == null || _playerInventory == null || _chestInventory == null)
        {
            Debug.LogWarning("Cannot open chest: missing references in ChestInteractor.");
            return;
        }
        _interactionManager.OpenContainer(_playerInventory, _chestInventory);
    }

    public InventoryManager GetInventory()
    {
        return _chestInventory;
    }
}
