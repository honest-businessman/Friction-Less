using UnityEngine;

public class TurretController : MonoBehaviour
{
    public TurretSettings settings;

    private void Awake()
    {
        if(CompareTag("Player"))
        {
            settings = Instantiate(settings);
        }
    }

}
