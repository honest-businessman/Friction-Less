using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    [SerializeField]
    private float lightSize = 3f;
    [SerializeField]
    private float heavySize = 1.5f;
    [SerializeField]
    private int lightPenalty = 2000;
    [SerializeField]
    private int heavyPenalty = 6000;
    [SerializeField]
    private float proximityPenaltyCooldown = 0.5f;
    [SerializeField]
    private int wallErosionPenalty = 4000;
    public int WallErosionPenalty => wallErosionPenalty;
    [SerializeField]
    private float mapObjectSize = 2f;
    public float MapObjectSize => mapObjectSize;
    [SerializeField]
    private int pathClaimPenalty = 2000;
    public int PathClaimPenalty => pathClaimPenalty;


    private GameObject[] colliderObjs;
    private float penaltyTimer = 0f;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Time.time - penaltyTimer >= proximityPenaltyCooldown)
        {
            colliderObjs = GameObject.FindGameObjectsWithTag("Enemy Path Collider");
            foreach (GameObject colliderObj in colliderObjs)
            {
                Bounds bounds = colliderObj.GetComponentInChildren<Collider2D>().bounds;
                UpdateGraph(bounds);
                penaltyTimer = Time.time;
            }
        }
    }
    void UpdateGraph(Bounds bounds)
    {
        bounds.size = Vector3.one * heavySize;
        GraphUpdateObject heavyGuo = new GraphUpdateObject(bounds);
        heavyGuo.updatePhysics = true;
        heavyGuo.resetPenaltyOnPhysics = false;
        heavyGuo.addPenalty = heavyPenalty;

        bounds.size = Vector3.one * lightSize;
        GraphUpdateObject lightGuo = new GraphUpdateObject(bounds);
        lightGuo.updatePhysics = true;
        lightGuo.resetPenaltyOnPhysics = false;
        lightGuo.addPenalty = lightPenalty;

        AstarPath.active.UpdateGraphs(heavyGuo);
        AstarPath.active.UpdateGraphs(lightGuo);
        StartCoroutine(ResetPenalty(heavyGuo));
        StartCoroutine(ResetPenalty(lightGuo));
    }

    private IEnumerator ResetPenalty(GraphUpdateObject guo)
    {
        yield return new WaitForSeconds(proximityPenaltyCooldown);
        GraphUpdateObject newGuo = new GraphUpdateObject(guo.bounds); // Must create new GraphUpdateObject, cannot reuse old one.
        newGuo.updatePhysics = true;
        newGuo.resetPenaltyOnPhysics = false;
        newGuo.addPenalty = -(guo.addPenalty); // Reverts penalty amount
        AstarPath.active.UpdateGraphs(newGuo);
    }

}
