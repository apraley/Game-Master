using UnityEngine;

public class RoomLootPickup : MonoBehaviour, IInteractable
{
    public enum LootType
    {
        Pills,
        BedPanAmmo,
        WalkerDamageUpgrade,
        PillBottleDamageUpgrade,
        FbpFireRateUpgrade,
        Heal,
    }

    [SerializeField] private LootType lootType = LootType.Pills;
    [SerializeField] private int value = 5;
    [SerializeField] private AudioSource pickupAudio;

    public void Interact(PlayerController player)
    {
        WeaponSystem weaponSystem = FindObjectOfType<WeaponSystem>();
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();

        switch (lootType)
        {
            case LootType.Pills:
                weaponSystem?.AddAmmo(WeaponSystem.WeaponType.PillBottle, value);
                break;
            case LootType.BedPanAmmo:
                weaponSystem?.AddAmmo(WeaponSystem.WeaponType.FBP9000, value);
                break;
            case LootType.WalkerDamageUpgrade:
                weaponSystem?.UpgradeWeaponDamage(WeaponSystem.WeaponType.Walker, value);
                break;
            case LootType.PillBottleDamageUpgrade:
                weaponSystem?.UpgradeWeaponDamage(WeaponSystem.WeaponType.PillBottle, value);
                break;
            case LootType.FbpFireRateUpgrade:
                weaponSystem?.UpgradeWeaponFireRate(WeaponSystem.WeaponType.FBP9000, value * 0.1f);
                break;
            case LootType.Heal:
                playerHealth?.Heal(value);
                break;
        }

        if (pickupAudio != null)
        {
            pickupAudio.transform.parent = null;
            pickupAudio.Play();
            Destroy(pickupAudio.gameObject, pickupAudio.clip.length + 0.1f);
        }

        Destroy(gameObject);
    }
}
