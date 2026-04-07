using System;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public enum WeaponType
    {
        Walker = 0,
        PillBottle = 1,
        FBP9000 = 2,
    }

    [Serializable]
    public class WeaponDefinition
    {
        public WeaponType type;
        public string displayName;
        public int damage = 10;
        public float fireRate = 2f;
        public float range = 2f;
        public int ammoCost = 0;
        public GameObject projectilePrefab;
        public float projectileSpeed = 15f;
        public float splashRadius = 0f;
        public AudioClip fireClip;
        public Sprite icon;
    }

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private AudioSource audioSource;

    [Header("Weapons")]
    [SerializeField] private WeaponDefinition walker;
    [SerializeField] private WeaponDefinition pillBottle;
    [SerializeField] private WeaponDefinition fbp9000;

    [Header("Ammo")]
    [SerializeField] private int pillAmmo = 30;
    [SerializeField] private int fbpAmmo = 8;

    private WeaponDefinition[] weaponOrder;
    private int currentWeaponIndex;
    private float lastFireTime;

    public WeaponType CurrentWeaponType => weaponOrder[currentWeaponIndex].type;
    public WeaponDefinition CurrentWeapon => weaponOrder[currentWeaponIndex];

    public event Action<WeaponDefinition, int> OnWeaponChanged;
    public event Action<int> OnAmmoChanged;

    private void Awake()
    {
        weaponOrder = new[] { walker, pillBottle, fbp9000 };
        currentWeaponIndex = 0;
    }

    private void Start()
    {
        BroadcastWeaponState();
    }

    private void Update()
    {
        HandleWeaponSwitch();
        HandleFire();
    }

    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetWeapon(2);
        }
    }

    private void HandleFire()
    {
        if (!Input.GetMouseButton(0))
        {
            return;
        }

        WeaponDefinition weapon = CurrentWeapon;
        if (Time.time - lastFireTime < 1f / Mathf.Max(0.01f, weapon.fireRate))
        {
            return;
        }

        if (!HasAmmoFor(weapon))
        {
            return;
        }

        lastFireTime = Time.time;
        ConsumeAmmo(weapon);

        if (weapon.projectilePrefab == null)
        {
            FireHitscan(weapon);
        }
        else
        {
            FireProjectile(weapon);
        }

        if (audioSource != null && weapon.fireClip != null)
        {
            audioSource.PlayOneShot(weapon.fireClip);
        }

        BroadcastWeaponState();
    }

    private void FireHitscan(WeaponDefinition weapon)
    {
        if (firePoint == null)
        {
            return;
        }

        if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, weapon.range))
        {
            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            damageable?.ApplyDamage(weapon.damage);
        }
    }

    private void FireProjectile(WeaponDefinition weapon)
    {
        if (firePoint == null)
        {
            return;
        }

        GameObject projectileObject = Instantiate(weapon.projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(weapon.damage, weapon.projectileSpeed, weapon.splashRadius, gameObject);
        }
    }

    private bool HasAmmoFor(WeaponDefinition weapon)
    {
        return weapon.type switch
        {
            WeaponType.Walker => true,
            WeaponType.PillBottle => pillAmmo >= weapon.ammoCost,
            WeaponType.FBP9000 => fbpAmmo >= weapon.ammoCost,
            _ => false,
        };
    }

    private void ConsumeAmmo(WeaponDefinition weapon)
    {
        if (weapon.ammoCost <= 0)
        {
            return;
        }

        switch (weapon.type)
        {
            case WeaponType.PillBottle:
                pillAmmo = Mathf.Max(0, pillAmmo - weapon.ammoCost);
                break;
            case WeaponType.FBP9000:
                fbpAmmo = Mathf.Max(0, fbpAmmo - weapon.ammoCost);
                break;
        }
    }

    private int GetDisplayedAmmo(WeaponDefinition weapon)
    {
        return weapon.type switch
        {
            WeaponType.Walker => -1,
            WeaponType.PillBottle => pillAmmo,
            WeaponType.FBP9000 => fbpAmmo,
            _ => 0,
        };
    }

    private void SetWeapon(int index)
    {
        if (index < 0 || index >= weaponOrder.Length)
        {
            return;
        }

        currentWeaponIndex = index;
        BroadcastWeaponState();
    }

    public void AddAmmo(WeaponType type, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (type == WeaponType.PillBottle)
        {
            pillAmmo += amount;
        }
        else if (type == WeaponType.FBP9000)
        {
            fbpAmmo += amount;
        }

        BroadcastWeaponState();
    }

    public void UpgradeWeaponDamage(WeaponType type, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        WeaponDefinition weapon = GetWeaponDefinition(type);
        if (weapon == null)
        {
            return;
        }

        weapon.damage += amount;
        BroadcastWeaponState();
    }

    public void UpgradeWeaponFireRate(WeaponType type, float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        WeaponDefinition weapon = GetWeaponDefinition(type);
        if (weapon == null)
        {
            return;
        }

        weapon.fireRate += amount;
        BroadcastWeaponState();
    }

    private WeaponDefinition GetWeaponDefinition(WeaponType type)
    {
        foreach (WeaponDefinition weapon in weaponOrder)
        {
            if (weapon.type == type)
            {
                return weapon;
            }
        }

        return null;
    }

    private void BroadcastWeaponState()
    {
        WeaponDefinition current = CurrentWeapon;
        int ammo = GetDisplayedAmmo(current);
        OnWeaponChanged?.Invoke(current, ammo);
        OnAmmoChanged?.Invoke(ammo);
    }
}
