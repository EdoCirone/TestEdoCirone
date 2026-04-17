using UnityEngine;

abstract public class AbstractEffectData : ScriptableObject
{

    public abstract void ApplyEffect(PlayerStats playerStats, float amount);

}
