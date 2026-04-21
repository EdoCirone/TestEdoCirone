using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveGrid : MonoBehaviour
{

    [SerializeField] private int _slotCount = 9; // Numero totale di item da visualizzare ( dinamico in base all'inventario)

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

    public void SetSlotCount(int count)
    {
        _slotCount = count;
        UpdateGridCellSize();
    }

    private void UpdateGridCellSize()
    {
        if (_gridLayoutGroup == null || _containerRectTransform == null)
            return;
        
        int numberOfColumns = Mathf.Max(1,_gridLayoutGroup.constraintCount); // Prendo il numero di colonne dal GridLayoutGroup, assicurandomi che sia almeno 1
        int numberOfRows = Mathf.CeilToInt((float)_slotCount / numberOfColumns); // Calcolo il numero di righe necessario in base al numero di item e colonne

        float paddingX = _gridLayoutGroup.padding.left + _gridLayoutGroup.padding.right + _gridLayoutGroup.spacing.x * (numberOfColumns - 1); //Calcolo del padding totale in orizzontale
        float paddingY = _gridLayoutGroup.padding.top + _gridLayoutGroup.padding.bottom + _gridLayoutGroup.spacing.y * (numberOfRows - 1); // Calcolo del padding totale in verticale
        
        float cellWidth = (_containerRectTransform.rect.width - paddingX) / numberOfColumns; 
        float cellHeight = (_containerRectTransform.rect.height - paddingY) / numberOfRows; 
        
        float size = Mathf.Min(cellWidth, cellHeight);// Prendo la dimensione più piccola per mantenere le celle quadrate
        
        _gridLayoutGroup.cellSize = new Vector2(size, size);

    }
}
