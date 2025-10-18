using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [SerializeField] public int currentXP = 0;
    [SerializeField] public int xpToLevelUp = 100;
    [SerializeField] public int level = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     
    public void AddXP(int amount)
    {
        currentXP += amount;

        Debug.Log("XP Collected: " + amount + "Total: " + currentXP);

        if(currentXP >= xpToLevelUp)
        {
            level++;
            currentXP -= xpToLevelUp;
            Debug.Log("Level Up. New Level: " + level);
        }

    }
}
