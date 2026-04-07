using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public enum PortraitState
    {
        Healthy,
        Hurt,
        Critical,
        Dead,
    }

    [Header("Text Fields")]
    [SerializeField] private Text healthText;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text weaponText;

    [Header("Images")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Sprite healthyPortrait;
    [SerializeField] private Sprite hurtPortrait;
    [SerializeField] private Sprite criticalPortrait;
    [SerializeField] private Sprite deadPortrait;

    [Header("Runtime Refs")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private WeaponSystem weaponSystem;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image staminaBar;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandleHealthChanged;
            playerHealth.OnPlayerDied += HandlePlayerDied;
        }

        if (weaponSystem != null)
        {
            weaponSystem.OnWeaponChanged += HandleWeaponChanged;
            weaponSystem.OnAmmoChanged += HandleAmmoChanged;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
            playerHealth.OnPlayerDied -= HandlePlayerDied;
        }

        if (weaponSystem != null)
        {
            weaponSystem.OnWeaponChanged -= HandleWeaponChanged;
            weaponSystem.OnAmmoChanged -= HandleAmmoChanged;
        }
    }

    private void Update()
    {
        if (staminaBar != null && playerController != null)
        {
            staminaBar.fillAmount = playerController.StaminaNormalized;
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (healthText != null)
        {
            healthText.text = $"HR: {current}";
        }

        float normalized = max <= 0 ? 0f : (float)current / max;
        SetPortraitState(current <= 0 ? PortraitState.Dead :
            normalized > 0.6f ? PortraitState.Healthy :
            normalized > 0.25f ? PortraitState.Hurt : PortraitState.Critical);
    }

    private void HandlePlayerDied()
    {
        SetPortraitState(PortraitState.Dead);
    }

    private void HandleWeaponChanged(WeaponSystem.WeaponDefinition weapon, int ammo)
    {
        if (weaponText != null)
        {
            weaponText.text = weapon.displayName;
        }

        if (weaponIcon != null)
        {
            weaponIcon.sprite = weapon.icon;
        }

        HandleAmmoChanged(ammo);
    }

    private void HandleAmmoChanged(int ammo)
    {
        if (ammoText == null)
        {
            return;
        }

        ammoText.text = ammo < 0 ? "Pills: --" : $"Pills: {ammo}";
    }

    private void SetPortraitState(PortraitState state)
    {
        if (portraitImage == null)
        {
            return;
        }

        portraitImage.sprite = state switch
        {
            PortraitState.Healthy => healthyPortrait,
            PortraitState.Hurt => hurtPortrait,
            PortraitState.Critical => criticalPortrait,
            PortraitState.Dead => deadPortrait,
            _ => healthyPortrait,
        };
    }
}
