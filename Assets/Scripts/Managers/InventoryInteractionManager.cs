using UnityEngine;

public class InventoryInteractionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryUI _playerInventoryUI;
    [SerializeField] private InventoryUI _containerInventoryUI;
    [SerializeField] private PlayerInventoryHandler _playerInventoryHandler;

    private InventoryManager _playerInventory;
    private InventoryManager _currentContainerInventory;
    private InventoryAnimation _playerAnimation;
    private InventoryAnimation _containerAnimation;

    bool _isContainerOpen = false;
    public bool IsContainerOpen() => _isContainerOpen;

    public InventoryManager GetCurrentContainerInventory()
    {
        return _currentContainerInventory;
    }

    private void Start()
    {
        if (_containerInventoryUI == null || _playerInventoryUI == null || _playerInventoryHandler == null) return;

        _playerAnimation = _playerInventoryUI.gameObject.GetComponent<InventoryAnimation>();
        _containerAnimation = _containerInventoryUI.gameObject.GetComponent<InventoryAnimation>();
    }

    public void OpenContainer(InventoryManager playerInventory, InventoryManager containerInventory)
    {
        _currentContainerInventory = containerInventory;
        _playerInventory = playerInventory;


        _isContainerOpen = true;
        Time.timeScale = 0f; // Pause the game when the chest is open

        _playerInventoryUI.SetInventory(_playerInventory);
        _containerInventoryUI.SetInventory(_currentContainerInventory);

        _playerInventoryUI.gameObject.SetActive(true);

        if (!_playerAnimation.IsInventoryOpen())
        {
            _playerAnimation?.OnOpenInventory();
        }


        _containerInventoryUI.gameObject.SetActive(true);
        _containerAnimation = _containerInventoryUI.gameObject.GetComponent<InventoryAnimation>();
        if (!_containerAnimation.IsInventoryOpen())
        {
            _containerAnimation?.OnOpenInventory();
        }
    }

    public void CloseContainer()
    {
        _isContainerOpen = false;
        _containerAnimation?.OnCloseInventory();
        _playerAnimation?.OnCloseInventory();

        Time.timeScale = 1f; // Resume the game when the chest is closed
        //_containerInventoryUI.gameObject.SetActive(false);

    }

    public InventoryManager GetPlayerInventory()
    {
        return _playerInventory;
    }

    public void HandleSlotClick(InventoryUI sourceUI, int index)
    {
        if (sourceUI == null)
        {
            Debug.LogWarning("Source InventoryUI is null in HandleSlotClick.");
            return;
        }

        if (_isContainerOpen)
        {
            if (sourceUI.GetInventoryUIType() == InventoryUIType.Player)
            {
                TransferItem(_playerInventory, _currentContainerInventory, index);
            }
            else if (sourceUI.GetInventoryUIType() == InventoryUIType.Container)
            {
                TransferItem(_currentContainerInventory, _playerInventory, index);
            }
        }

        if (sourceUI.GetInventoryUIType() == InventoryUIType.Player)
        {
            if (_playerInventoryHandler == null)
            {
                Debug.LogWarning("PlayerInventoryHandler reference is missing in InventoryInteractionManager.");
                return;
            }
            _playerInventoryHandler.UseItem(index);
        }

    }

    private void TransferItem(InventoryManager fromManager, InventoryManager toManager, int index)
    {
        if (fromManager == null || toManager == null)
        {
            Debug.LogWarning("One or both InventoryManager references are null in TransferItem.");
            return;
        }

        if (index < 0 || index >= fromManager.ItemsInInventory.Count)
        {
            Debug.LogWarning("Invalid item index in TransferItem.");
            return;
        }

        ItemData itemToTransfer = fromManager.ItemsInInventory[index];

        if (itemToTransfer == null) return;

        bool addedToTarget = toManager.AddItem(itemToTransfer);
        if (addedToTarget)
        {
            fromManager.RemoveItem(index);
        }
    }

}
