using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public TextMeshProUGUI meterText;
    public PlayerController playerController;

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

    void Start()
    {
        meterText.text = $"Friction Charge: 0%";
    }

    void Update()
    {
        DisplayCharge();
    }

    public void DisplayCharge()
    {
        meterText.text = ($"Friction Charge: {playerController.DriveCharge:0}%");
    }
}
