using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionSettings", menuName = "Scriptable Objects/Explosion Settings")]
public class ExplosionSettings : ScriptableObject
{
    public int damage = 2;
    public float radius = 2.5f;
    public ParticleSystem explosionVFX;
    public LayerMask affectedLayers;
    public GameObject explosionPrefab;
}