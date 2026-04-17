using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image _itemIcon;

    private InventoryManager _inventoryManager;
    private int _slotIndex;

   

    public void Initialize(InventoryManager inventoryManager, int index) //Uso questo metodo perchè il mio inventory non è singleton quindi ho bisogno di passare il riferimento all'inventory manager per poter gestire l'uso degli item al click dello slot, e passo anche l'indice dello slot per sapere quale item usare in base alla posizione dello slot nella griglia
    {
        _slotIndex = index;
        _inventoryManager = inventoryManager;

    }

    public void SetItem(ItemData itemData)
    {
        if(_itemIcon == null)
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
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnDrop(PointerEventData eventData)
    {
    }

    #endregion
}

