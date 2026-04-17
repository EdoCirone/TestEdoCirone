using UnityEngine;

public class ItemPickerControl : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;

    public ItemData GetItem()
    {
        return _itemData;
    }

    public void Pick()
    {
        Destroy(gameObject);
    }

    private void OnMouseDown() // Metodo per testare la raccolta dell'item cliccandoci sopra, da rimuovere o sostituire con un sistema di interazione più adatto (es. trigger collider) in base alle esigenze del gioco
    {
        PlayerItemCollector playerItemCollector = FindFirstObjectByType<PlayerItemCollector>();
        if (playerItemCollector == null)
        {
            Debug.LogWarning("PlayerItemCollector not found in the scene.");
            return;
        }
            playerItemCollector.TryPickItem(this);
    }
}

