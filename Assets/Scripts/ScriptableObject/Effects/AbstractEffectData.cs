using UnityEngine;

// Classe base per tutti gli effetti degli item. Usare uno ScriptableObject permette
// ai designer di creare e configurare effetti nell'inspector senza scrivere codice.
// Aggiungere un nuovo tipo di effetto richiede solo una nuova sottoclasse,
// senza toccare ItemData o InventoryManager.
 public abstract class AbstractEffectData : ScriptableObject
{

    public abstract void ApplyEffect(PlayerStats playerStats, float amount);

}
