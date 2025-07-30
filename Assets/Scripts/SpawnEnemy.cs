using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemy;
    private int num = 0;
    private int x;
    private int z;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        x = Random.Range(1, 10);
        z = Random.Range(1, 10);
        num++;

        if(num % 200 == 0)
        {
            Instantiate(enemy, new Vector3(x, 1, z), Quaternion.identity);
        }
    }
}
