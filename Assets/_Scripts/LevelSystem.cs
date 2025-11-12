using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [SerializeField] private int intialXpToLevelUp = 30;
    [SerializeField] private float xpCostScaling = 1.2f;

    [SerializeField] private int currentXP = 0;
    [SerializeField] private int currentXpToLevelUp;
    [SerializeField] private int level = 1;

    private void Start()
    {
        currentXpToLevelUp = intialXpToLevelUp;
    }
    public void AddXP(int amount)
    {
        currentXP += amount;

        Debug.Log("XP Collected: " + amount + " Total: " + currentXP);

        if(currentXP >= currentXpToLevelUp)
        {
            level++;
            currentXP -= currentXpToLevelUp;
            currentXpToLevelUp = Mathf.RoundToInt(currentXpToLevelUp * xpCostScaling);
            
            Debug.Log("Level Up. New Level: " + level);

            UpgradeManager.Instance.TriggerUpgrades();
        }

    }
}
