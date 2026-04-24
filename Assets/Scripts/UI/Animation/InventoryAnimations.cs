using UnityEngine;
using DG.Tweening;

public class InventoryAnimation : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] RectTransform _inventoryPanel;
    [SerializeField] RectTransform _inventoryTitlePanel;
    [SerializeField] RectTransform _inventorySlots;

    [Header("Animation Settings")]
    [SerializeField] float _inventoryPanelAnimationDuration = 0.5f;
    [SerializeField] float _inventoryTitlePanelAnimationDuration = 0.3f;
    [SerializeField] float _inventorySlotAnimationDuration = 0.2f;

    [Header("Positions")]
    [SerializeField] Vector2 _inventoryPanelHiddenPosition = new Vector2(0, -500);


    private CanvasGroup _inventoryCanvasGroup;
    private Vector2 _inventoryPanelVisiblePosition;
    private Sequence _openInventoryAnimSequence;
    private Sequence _closeInventoryAnimSequence;
    private bool _isInventoryOpen;

    #region Events

    public event System.Action OnInventoryOpened;
    public event System.Action OnInventoryClosed;

    #endregion

    public bool IsInventoryOpen() => _isInventoryOpen;

    private void Awake()
    {
        if (_inventoryPanel == null || _inventoryTitlePanel == null || _inventorySlots == null)
        {
            Debug.LogError("InventoryAnimation: One or more references are not assigned in the inspector.");
            return;
        }

        // Salvo la posizione visibile prima di nascondere il pannello,
        // così posso ritornarci durante l'animazione di apertura.
        _inventoryPanelVisiblePosition = _inventoryPanel.anchoredPosition;

        _inventoryPanel.anchoredPosition = _inventoryPanelHiddenPosition;

        _inventoryTitlePanel.localScale = Vector3.zero;

        _inventoryCanvasGroup = _inventoryPanel.gameObject.GetComponent<CanvasGroup>();

        if (_inventoryCanvasGroup == null)
        {
            _inventoryCanvasGroup = _inventoryPanel.gameObject.AddComponent<CanvasGroup>();
        }

        _inventoryCanvasGroup.alpha = 0;

        for (int i = 0; i < _inventorySlots.childCount; i++)
        {
            _inventorySlots.GetChild(i).localScale = Vector3.zero;
        }

    }

    private void Update()
    {

        // Input diretto in Update: accettabile per un prototipo.
        // In un progetto più strutturato andrebbe delegato a un Input System centralizzato.

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }


    }

    public void OnOpenInventory()
    {
        // Gli slot appaiono in cascata partendo da metà dell'animazione del titolo,
        // così la sequenza visiva ha un ritmo naturale senza sembrare tutto sincrono.

        float slotStartTime = _inventoryPanelAnimationDuration + (_inventoryTitlePanelAnimationDuration * 0.5f);
        float slotStep = _inventorySlotAnimationDuration * 0.5f;

        _closeInventoryAnimSequence?.Kill();
        _openInventoryAnimSequence?.Kill();

        ResetOpenVisualState();
        //L'audio parte prima dell'animazione e non a OnComplete perché voglio che il suono sia sincronizzato con l'inizio dell'animazione, non con la fine.
        AudioEvents.RaiseAudioCue(AudioCueType.InventoryOpen);

        this.gameObject.SetActive(true);

        _openInventoryAnimSequence = DOTween.Sequence().SetUpdate(true);
        _openInventoryAnimSequence.Append(_inventoryPanel.DOAnchorPos(_inventoryPanelVisiblePosition, _inventoryPanelAnimationDuration).SetEase(Ease.OutBack))
            .Join(_inventoryCanvasGroup.DOFade(1, _inventoryPanelAnimationDuration).SetEase(Ease.OutQuad))
            .Append(_inventoryTitlePanel.DOScale(Vector3.one, _inventoryTitlePanelAnimationDuration).SetEase(Ease.OutBack));

        for (int i = 0; i < _inventorySlots.childCount; i++)
        {
            _openInventoryAnimSequence.Insert(slotStartTime + i * slotStep,
                _inventorySlots.GetChild(i).DOScale(Vector3.one, _inventorySlotAnimationDuration).SetEase(Ease.OutBack));
        }

        _openInventoryAnimSequence.OnComplete(() =>
        {
            OnInventoryOpened?.Invoke();
            _inventoryCanvasGroup.blocksRaycasts = true;
        });

        _isInventoryOpen = true;
    }

    public void OnCloseInventory()
    {

        _openInventoryAnimSequence?.Kill();
        _closeInventoryAnimSequence?.Kill();
        AudioEvents.RaiseAudioCue(AudioCueType.InventoryClose);


        _closeInventoryAnimSequence = DOTween.Sequence().SetUpdate(true);
        _closeInventoryAnimSequence.Append(_inventoryTitlePanel.DOScale(Vector3.zero, _inventoryTitlePanelAnimationDuration).SetEase(Ease.InBack))
            .Join(_inventoryCanvasGroup.DOFade(0, _inventoryPanelAnimationDuration).SetEase(Ease.InQuad))
            .Append(_inventoryPanel.DOAnchorPos(_inventoryPanelHiddenPosition, _inventoryPanelAnimationDuration).SetEase(Ease.InBack))
            .OnComplete(() =>
            {
                OnInventoryClosed?.Invoke();
                this.gameObject.SetActive(false);
            });

        _isInventoryOpen = false;
    }

    public void ToggleInventory()
    {
        if (_isInventoryOpen)
        {
            OnCloseInventory();
        }
        else
        {
            OnOpenInventory();
        }
    }

    private void ResetOpenVisualState()
    {
        // Ripristino lo stato visivo iniziale prima di ogni apertura
        // così l'animazione parte sempre da zero, anche se interrotta a metà.

        for (int i = 0; i < _inventorySlots.childCount; i++)
        {
            _inventorySlots.GetChild(i).localScale = Vector3.zero;
        }

        _inventoryCanvasGroup.alpha = 0;
        _inventoryTitlePanel.localScale = Vector3.zero;

    }
}
