using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private float _iconDropSize = 64f; // Dimensione fissa della preview durante il drag.

    private int _slotIndex;
    private InventoryManager _inventoryManager;
    private RectTransform _inventoryPanel;
    private PlayerItemDropper _playerItemDropper;
    private Canvas _canvas;
    private InventoryInteractionManager _interactionManager;

    private Image _iconCopy;
    private static InventorySlotUI _draggedItem; // Va bene per un solo drag attivo. Da rifare se si gestiscono più inventari contemporaneamente.

    private bool _isDroppedSuccessfully = false;

    public event System.Action<int> OnSlotClickedEvent;


    public void Initialize(InventoryManager inventoryManager, int index, RectTransform inventoryPanel, PlayerItemDropper playerItemDropper, InventoryInteractionManager inventoryInteraction)
    {
        _slotIndex = index;
        _inventoryManager = inventoryManager;
        _inventoryPanel = inventoryPanel;
        _playerItemDropper = playerItemDropper;
        _canvas = _inventoryPanel.GetComponentInParent<Canvas>();
        _interactionManager = inventoryInteraction;
    }

    public void SetItem(ItemData itemData)
    {
        if (_itemIcon == null)
        {
            Debug.LogWarning("ItemIcon reference is not assigned in InventorySlotUI.");
            return;
        }

        if (itemData == null || itemData.ItemIcon == null)
        {
            ClearSlot();
            return;
        }

        _itemIcon.sprite = itemData.ItemIcon;
        _itemIcon.enabled = true;
    }

    public void ClearSlot()
    {
        if (_itemIcon == null)
        {
            Debug.LogWarning("ItemIcon reference is missing in InventorySlotUI.");
            return;
        }

        _itemIcon.sprite = null;
        _itemIcon.enabled = false;
    }

    public void OnSlotClicked()
    {
        OnSlotClickedEvent?.Invoke(_slotIndex);
    }

    #region DragAndDrop

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_canvas == null)
        {
            Debug.LogWarning("Canvas reference is missing in InventorySlotUI.");
            return;
        }

        if (_inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager reference is not assigned in InventorySlotUI.");
            return;
        }

        if (_interactionManager != null && _interactionManager.IsContainerOpen())
        {
            return;
        }

        if (_slotIndex < 0 || _slotIndex >= _inventoryManager.ItemsInInventory.Count)
        {
            Debug.LogWarning("Invalid slot index: " + _slotIndex);
            return;
        }

        if (_inventoryManager.ItemsInInventory[_slotIndex] == null)
        {
            return;
        }

        if (_itemIcon == null || _itemIcon.sprite == null)
        {
            Debug.LogWarning("Item icon is missing in InventorySlotUI.");
            return;
        }

        _draggedItem = this;


        // Creo una nuova Image per la preview del drag invece di duplicare quella dello slot,
        // così evito problemi di dimensione e comportamento ereditati dalla gerarchia UI originale.
        GameObject dragIconObject = new GameObject("DraggedItemIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        dragIconObject.transform.SetParent(_canvas.transform, false);

        // Copio solo i dati visivi necessari alla preview del drag.
        _iconCopy = dragIconObject.GetComponent<Image>();
        _iconCopy.sprite = _itemIcon.sprite;
        _iconCopy.color = _itemIcon.color;
        _iconCopy.preserveAspect = true;
        _iconCopy.raycastTarget = false;

        // Assegno una dimensione fissa alla preview del drag.
        RectTransform dragRect = _iconCopy.rectTransform;
        dragRect.sizeDelta = new Vector2(_iconDropSize, _iconDropSize);
        dragRect.localScale = Vector3.one;

        // Nascondo temporaneamente l'icona originale mentre la preview è attiva.
        _itemIcon.enabled = false;
        _isDroppedSuccessfully = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_iconCopy == null)
        {
            Debug.LogWarning("Icon copy is null during drag. This should not happen.");
            return;
        }

        _iconCopy.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (_draggedItem != this)
        {
            Debug.LogWarning("OnEndDrag called on a slot that is not the dragged item. This should not happen.");
            return;
        }

        if (_isDroppedSuccessfully)
        {
            CleanupDrag();
            return;
        }

        bool releasedInsideInventoryPanel = RectTransformUtility.RectangleContainsScreenPoint(_inventoryPanel, eventData.position, eventData.pressEventCamera);

        if (_interactionManager != null && _interactionManager.IsContainerOpen())
        {
            RestoreIconAndCleanup();
            return;
        }

        if (releasedInsideInventoryPanel )
        {
            RestoreIconAndCleanup();
            return;
        }

        if (_playerItemDropper == null)
        {
            Debug.LogWarning("PlayerItemDropper reference is missing.");
            RestoreIconAndCleanup();
            return;
        }

        ItemData itemToDrop = _inventoryManager.ItemsInInventory[_slotIndex];

        if (itemToDrop == null)
        {
            RestoreIconAndCleanup();
            return;
        }

        bool droppedSuccessfully = _playerItemDropper.TryDropItemOutside(itemToDrop, eventData.position);

        if (droppedSuccessfully)
        {
            _inventoryManager.RemoveItem(_slotIndex);
            CleanupDrag();
            return;
        }

        RestoreIconAndCleanup();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager reference is not assigned in InventorySlotUI.");
            return;
        }

        if (_draggedItem == null)
        {
            return;
        }

        if (_draggedItem == this)
        {
            return;
        }

        _inventoryManager.MoveOrSwapItems(_draggedItem._slotIndex, _slotIndex);
        _draggedItem._isDroppedSuccessfully = true;

    }

    private void CleanupDrag()
    {
        if (_iconCopy != null)
        {
            Destroy(_iconCopy.gameObject);
            _iconCopy = null;
        }
        _isDroppedSuccessfully = false;
        _draggedItem = null;
    }

    private void RestoreIconAndCleanup()
    {
        _itemIcon.enabled = true;
        CleanupDrag();
    }

    #endregion
}

