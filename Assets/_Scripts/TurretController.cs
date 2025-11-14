using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.RayTracingAccelerationStructure;

public class TurretController : MonoBehaviour
{

    [SerializeField] private List<TurretSettings> equippedTurrets;
    private int turretIndex = 0;

    [SerializeField] private TurretSettings currentSettings;
    public TurretSettings CurrentSettings => currentSettings; // expose for FiringSystem or others

    private TurretUpgradeStats upgradeStats = new TurretUpgradeStats();

    private void Awake()
    {
        if (equippedTurrets == null)
        {
            Debug.LogError($"{name} is missing TurretSettings references!");
            return;
        }

        ResetTurret();
    }
    public void ResetTurret()
    {
        currentSettings = Instantiate(equippedTurrets[turretIndex]);
    }

    // apply upgrade stats to current turret settings
    public void ApplyUpgrades()
    {
        ResetTurret();

        currentSettings.fireRate *= upgradeStats.fireRateModifier;
        currentSettings.shellDamage += upgradeStats.damageModifier;
        currentSettings.hitscanDamage += upgradeStats.damageModifier;
        currentSettings.shellSpeed *= upgradeStats.shellSpeedModifier;
    }

    public void ChangeTurret(float changeInput)
    {
        if (equippedTurrets.Count <= 1) { return; }

        if (changeInput > 0)
        {
            Debug.Log("Next Turret!");
            // Uses modulo operator to get remainder after division. This is to wrap the index within the count.
            turretIndex = (turretIndex + 1) % equippedTurrets.Count;
        }
        else if (changeInput < 0)
        {
            Debug.Log("Previous Turret!");
            // Adds count to index to prevent negative index result when wrapping with modulo.
            turretIndex = (turretIndex - 1 + equippedTurrets.Count) % equippedTurrets.Count;
        }

        SetNewTurret(currentSettings);
    }

    private void SetNewTurret(TurretSettings newSettings)
    {
        SpriteRenderer sprender = gameObject.GetComponent<SpriteRenderer>();
        sprender.sprite = newSettings.sprite;

        ApplyUpgrades();
    }

    // methods to upgrade turret stats
    public void UpgradeFireRate(float multiplier)
    {
        upgradeStats.fireRateModifier *= multiplier;
        ApplyUpgrades();
    }

    public void UpgradeDamage(int addDamage)
    {
        upgradeStats.damageModifier += addDamage;
        ApplyUpgrades();
    }

    public void UpgradeShellSpeed(float multiplier)
    {
        upgradeStats.shellSpeedModifier *= multiplier;
        ApplyUpgrades();
    }


    public class TurretUpgradeStats
    {
        internal int damageModifier = 0;
        internal float fireRateModifier = 1f;
        internal float shellSpeedModifier = 1f;
    }
}
