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

        SetNewTurret();
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
        currentSettings.hitscanBounces += upgradeStats.extraHitscanBounces;
        currentSettings.hitscanPenetrations += upgradeStats.extraHitscanPenetrations;
    }
    private void SetNewTurret()
    {
        ApplyUpgrades();

        SpriteRenderer sprender = gameObject.GetComponent<SpriteRenderer>();
        sprender.sprite = currentSettings.sprite;
        sprender.material.color = currentSettings.color;
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

        SetNewTurret();
    }

    public void SetTurretByIndex(int index)
    {
        if (index < 0 || index >= equippedTurrets.Count)
        {
            Debug.LogError("Invalid turret index!");
            return;
        }

        turretIndex = index;
        SetNewTurret();
    }


    // methods to upgrade turret stats
    public void UpgradeFireRate(float modifier)
    {
        upgradeStats.fireRateModifier *= modifier;
        ApplyUpgrades();
    }

    public void UpgradeDamage(float modifier)
    {
        upgradeStats.damageModifier += Mathf.RoundToInt(modifier);
        ApplyUpgrades();
    }

    public void UpgradeShellSpeed(float modifier)
    {
        upgradeStats.shellSpeedModifier *= modifier;
        ApplyUpgrades();
    }
    public void UpgradeAmmunitionPower(float modifier)
    {
        switch (equippedTurrets[turretIndex].turretType)
        {
            case TurretSettings.TurretType.Ricochet:
                upgradeStats.extraHitscanBounces += Mathf.RoundToInt(modifier); break;
            case TurretSettings.TurretType.Sniper:
                upgradeStats.extraHitscanPenetrations += Mathf.RoundToInt(modifier); break;
        }
        upgradeStats.shellSpeedModifier *= modifier;
        ApplyUpgrades();
    }


    public class TurretUpgradeStats
    {
        internal int damageModifier = 0;
        internal float fireRateModifier = 1f;
        internal float shellSpeedModifier = 1f;
        internal int extraHitscanBounces = 0;
        internal int extraHitscanPenetrations = 0;
    }
}
