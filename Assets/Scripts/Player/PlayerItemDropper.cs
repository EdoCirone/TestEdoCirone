using UnityEngine;

public class PlayerItemDropper : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private float _dropCheckRadius = 0.5f;
    [SerializeField] private int _maxDropAttempts = 5;
    [SerializeField] private LayerMask _itemLayerMask;

    public event System.Action OnItemDroppedOutside;

    public bool TryDropItemOutside(ItemData item, Vector2 position)
    {
        if (item == null)
        {
            Debug.LogWarning("Cannot drop null item.");
            return false;
        }

        if (item.Prefab == null)
        {
            Debug.LogWarning("Item prefab is not assigned for item: " + item.ItemName);
            return false;
        }

        Camera camera = Camera.main;

        if (camera == null)
        {
            Debug.LogWarning("Main camera not found. Cannot drop item in the world.");
            return false;
        }

        Ray ray = camera.ScreenPointToRay(position);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 dropPosition = hitInfo.point;
            if (!TryFindFreeDropPosition(dropPosition, out Vector3 finalDropPosition))
            {
                Debug.LogWarning("No free space found to drop the item.");
                return false;
            }


            Instantiate(item.Prefab, finalDropPosition, Quaternion.identity);

            OnItemDroppedOutside?.Invoke();
            AudioEvents.RaiseAudioCue(AudioCueType.Drop);

            return true;
        }
        else
        {
            Debug.LogWarning("No valid surface found to drop the item. Item was not dropped.");
        }

        return false;

    }

    private bool TryFindFreeDropPosition(Vector3 basePosition, out Vector3 finalPosition)
    {
        finalPosition = basePosition;
        finalPosition.y = 0f;

        Collider[] colliders = Physics.OverlapSphere(basePosition, _dropCheckRadius, _itemLayerMask);
        if (colliders.Length > 0)
        {
            Debug.Log("There are items nearby. Cannot drop item here.");
            for (int i = 0; i < _maxDropAttempts; i++)
            {
                Vector3 randomOffset = Random.insideUnitSphere * _dropCheckRadius * 2f;
                randomOffset.y = 0; // Mantieni l'offset solo sull'asse XZ
                Vector3 checkPosition = basePosition + randomOffset;
                Collider[] nearbyColliders = Physics.OverlapSphere(checkPosition, _dropCheckRadius, _itemLayerMask);
                if (nearbyColliders.Length == 0)
                {
                    Debug.Log("Found a free spot to drop the item at: " + checkPosition);
                    checkPosition.y = 0f;
                    finalPosition = checkPosition;
                    return true;
                }
            }
            return false;
        }
        else
        {
            Debug.Log("No items nearby. Safe to drop item here.");
            return true;
        }
    }
}

