using UnityEngine;

public class PlayerItemDropper : MonoBehaviour
{
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
            Instantiate(item.Prefab, dropPosition, Quaternion.identity);

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
}

