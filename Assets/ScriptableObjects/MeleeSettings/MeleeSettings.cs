using UnityEngine;

[CreateAssetMenu(fileName = "MeleeSettings", menuName = "Scriptable Objects/MeleeSettings")]
public class MeleeSettings : ScriptableObject
{
    public bool destroyAfterAttack = false;
    public int damage = 2;
    public float fireRate = 1;
}
