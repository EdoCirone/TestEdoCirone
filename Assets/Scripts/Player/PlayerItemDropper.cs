using UnityEngine;

public class PlayerItemDropper : MonoBehaviour
{

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

        if(Physics.Raycast(ray, out RaycastHit hitInfo)) // Faccio un raycast per trovare il punto nel mondo dove droppare l'item, così da posizionarlo correttamente a terra o su una superficie
        {
            Vector3 dropPosition = hitInfo.point; // Prendo il punto di impatto del raycast come posizione di drop dell'item
            Instantiate(item.Prefab, dropPosition, Quaternion.identity); // Instanzio il prefab dell'item nella posizione di drop con rotazione identità (puoi modificare la rotazione se necessario)
            Debug.Log("Dropped item: " + item.ItemName + " at position: " + dropPosition);
            return true;
        }
        else
        {
            Debug.LogWarning("No valid surface found to drop the item. Item was not dropped.");
        }

        return false;

    }
}

