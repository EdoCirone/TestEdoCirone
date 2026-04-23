using UnityEngine;
using DG.Tweening;

public class PanelAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float _animationDuration = 0.5f;

    private Sequence _openAnimSeq;
    private Sequence _closeAnimSeq;

    private Vector3 _finalScale;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        _canvasGroup.alpha = 0;
        _finalScale = this.transform.localScale;
        this.transform.localScale = Vector3.zero;
        _canvasGroup.blocksRaycasts = false;
        this.gameObject.SetActive(false);
    }

    public void Open()
    {
        _openAnimSeq?.Kill();
        _closeAnimSeq?.Kill();
        _canvasGroup.alpha = 0;

        this.gameObject.SetActive(true);
        this.transform.localScale = Vector3.zero;

        _openAnimSeq = DOTween.Sequence().SetUpdate(true);
        _openAnimSeq.Append(this.transform.DOScale(_finalScale, _animationDuration).SetEase(Ease.OutBack))
            .Join(_canvasGroup.DOFade(1, _animationDuration).SetEase(Ease.OutQuad))
            .OnComplete(() => _canvasGroup.blocksRaycasts = true);

    }

    public void Close()
    {
        _openAnimSeq?.Kill();

        _closeAnimSeq?.Kill();

        _canvasGroup.blocksRaycasts = false;
        _closeAnimSeq = DOTween.Sequence().SetUpdate(true);
        _closeAnimSeq.Append(this.transform.DOScale(Vector3.zero, _animationDuration).SetEase(Ease.InBack))
            .Join(_canvasGroup.DOFade(0, _animationDuration).SetEase(Ease.InQuad))
            .OnComplete(() => this.gameObject.SetActive(false));

    }

}
