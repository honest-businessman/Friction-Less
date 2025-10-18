using UnityEngine;

public class FactionController : MonoBehaviour
{
    [SerializeField]
    public enum Factions
    {
        Player,
        Enemy,
        Neutral
    }
    [SerializeField]
    private Factions faction = Factions.Neutral;
    public Factions Faction => faction;

    public void SetFaction(Factions faction)
    {
        this.faction = faction;
    }

    public bool IsSameFaction(FactionController other)
    {
        if (other == null) return false;
        return faction == other.faction;
    }
}
