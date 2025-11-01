using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [SerializeField] public int currentXP = 0;
    [SerializeField] public int xpToLevelUp = 100;
    [SerializeField] public int level = 1;
    [SerializeField] private UpgradeManager upgradeManager;
    

    public void AddXP(int amount)
    {
        currentXP += amount;

        Debug.Log("XP Collected: " + amount + "Total: " + currentXP);

        if(currentXP >= xpToLevelUp)
        {
            level++;
            currentXP -= xpToLevelUp;
            xpToLevelUp += 50;
            
            Debug.Log("Level Up. New Level: " + level);

            upgradeManager.TriggerUpgrades();
        }

    }
}
