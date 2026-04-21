using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image _itemIcon;

    private InventoryManager _inventoryManager;
    private RectTransform _inventoryPanel;
    private Canvas _canvas; //Mi serve per parentare l'icona e farla stare visivamente sopra tutto
    private int _slotIndex;
    private PlayerItemDropper _playerItemDropper; //Riferimento al componente che si occupa di droppare gli item nel mondo
    private static InventorySlotUI _draggedItem; // Variabile STATICA per tenere traccia dell'indice dello slot dell'item attualmente trascinato DA CAMBIARE SE SI VOGLIONO PIU' INVENTARI

    private bool _isDroppedSuccessfully = false;
    private Image _iconCopy;



    public void Initialize(InventoryManager inventoryManager, int index, RectTransform inventoryPanel, PlayerItemDropper playerItemDropper) //Uso questo metodo perchè il mio inventory non è singleton quindi ho bisogno di passare i riferimenti.
    {
        _slotIndex = index;
        _inventoryManager = inventoryManager;
        _inventoryPanel = inventoryPanel; // passo il riferimento al pannello dell'inventario per poterlo usare come riferimento quando voglio posare a terra degli item
        _playerItemDropper = playerItemDropper; // passo il riferimento al componente che si occupa di droppare gli item nel mondo per poterlo usare quando voglio posare a terra degli item
        _canvas = _inventoryPanel.GetComponentInParent<Canvas>();
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
        if (_inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager reference is not assigned in InventorySlotUI.");
            return;
        }
        _inventoryManager.UseItem(_slotIndex); // Chiamo il metodo UseItem dell'inventory manager passando l'indice dello slot per sapere quale item usare in base alla posizione dello slot nella griglia
    }

    #region DragAndDrop

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager reference is not assigned in InventorySlotUI.");
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


        _draggedItem = this; // Imposto l'indice dello slot dell'item attualmente trascinato nella variabile statica 


        //Instanzio una copia, la parento al _canvas per farla stare sopta a tutto, poi gestisco la scala per tornare alle dimenzioni forzate dal layout e levo il raycast per evitare di droppare su se stessa
        _iconCopy = Instantiate(_itemIcon, _canvas.transform);

        _iconCopy.SetNativeSize();
        _iconCopy.rectTransform.localScale = _itemIcon.rectTransform.lossyScale * 2; //lossyScale mantiene la scala forzata dal layout, moltiplico per questione di design 

        _iconCopy.raycastTarget = false;

        //Disabilito la vecchia incona poi vedo se riattivarla o eliminarla
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

        _iconCopy.transform.position = eventData.position; // Sposto lo slot seguendo il mouse durante il drag, così da avere un feedback visivo del drag dell'item
    }

    public void OnEndDrag(PointerEventData eventData) // viene chiamato sullo slot di partenza
    {

        if (_draggedItem != this)
        {
            Debug.LogWarning("OnEndDrag called on a slot that is not the dragged item. This should not happen."); //Non succederà mai ma metti che...
            return;
        }

        bool releasedInsideInventoryPanel = RectTransformUtility.RectangleContainsScreenPoint(_inventoryPanel, eventData.position, eventData.pressEventCamera); // Controllo se il puntatore è rilasciato all'interno del pannello dell'inventario, così da sapere se posare a terra o riposizionare al punto di partenza

        if (_isDroppedSuccessfully)
        {
            CleanupDrag();
            return;
        }

        if (releasedInsideInventoryPanel)
        {
            _itemIcon.enabled = true;
            CleanupDrag();
            return;
        }

        if (_playerItemDropper == null)
        {
            Debug.LogWarning("PlayerItemDropper reference is missing.");
            _itemIcon.enabled = true;
            CleanupDrag();
            return;
        }

        ItemData itemToDrop = _inventoryManager.ItemsInInventory[_slotIndex];

        if (itemToDrop == null)
        {
            _itemIcon.enabled = true;
            CleanupDrag();
            return;
        }

        bool droppedSuccessfully = _playerItemDropper.TryDropItemOutside(itemToDrop, eventData.position);

        if (droppedSuccessfully)
        {
            _inventoryManager.RemoveItem(_slotIndex);
        }
        else
        {
            _itemIcon.enabled = true;
        }

        CleanupDrag();
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
        _draggedItem._isDroppedSuccessfully = true; // Imposto la variabile di successo del drop a true, così da sapere che l'item è stato posato con successo e non deve essere riposizionato al punto di partenza nel metodo OnEndDrag dello slot di partenza

    }

    private void CleanupDrag()
    {
        if (_iconCopy != null)
        {
            Destroy(_iconCopy.gameObject); // Distruggo la copia dell'icona creata per il drag, così da pulire la scena dopo il drag
            _iconCopy = null;
        }
        _isDroppedSuccessfully = false; // Resetto la variabile di successo del drop a false, così da essere pronto per un nuovo drag
        _draggedItem = null; // Resetto la variabile statica dell'indice dello slot dell'item attualmente trascinato, così da essere pronto per un nuovo drag
    }

    #endregion
}

