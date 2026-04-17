using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveGrid : MonoBehaviour
{


    [Header("Grid Settings")]
    [SerializeField] private int _numberOfColumns = 3;
    [SerializeField] private int _numberOfRows = 3;

    private GridLayoutGroup _gridLayoutGroup;
    private RectTransform _containerRectTransform;

    private void Awake()
    {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        _containerRectTransform = GetComponent<RectTransform>();
    }


    private void Start()
    {
        if (_gridLayoutGroup == null || _containerRectTransform == null)
        {
            Debug.LogError("GridLayoutGroup or RectTransform component is missing.");
            return;
        }

        UpdateGridCellSize();
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdateGridCellSize();
    }

    private void UpdateGridCellSize()
    {
        if (_gridLayoutGroup == null || _containerRectTransform == null)
            return;
        
        float paddingX = _gridLayoutGroup.padding.left + _gridLayoutGroup.padding.right + _gridLayoutGroup.spacing.x * (_numberOfColumns - 1); //Calcolo del padding totale in orizzontale
        float paddingY = _gridLayoutGroup.padding.top + _gridLayoutGroup.padding.bottom + _gridLayoutGroup.spacing.y * (_numberOfRows - 1); // Calcolo del padding totale in verticale
        
        float cellWidth = (_containerRectTransform.rect.width - paddingX) / _numberOfColumns; 
        float cellHeight = (_containerRectTransform.rect.height - paddingY) / _numberOfRows; 
        
        float size = Mathf.Min(cellWidth, cellHeight);// Prendo la dimensione più piccola per mantenere le celle quadrate
        
        _gridLayoutGroup.cellSize = new Vector2(size, size);

    }
}
