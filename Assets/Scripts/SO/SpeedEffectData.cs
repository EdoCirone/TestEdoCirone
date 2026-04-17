using UnityEngine;

[CreateAssetMenu(fileName = "SpeedEffectData", menuName = "ScriptableObjects/Effects/SpeedEffectData")]
public class SpeedEffectData : AbstractEffectData
{

    public override void ApplyEffect(PlayerStats playerStats, float amount)
    {
        if (playerStats == null) return;

        playerStats.UpdateSpeed(amount);
    }
}
