using TMPro;
using UnityEngine;

public class UIHandler : MonoBehaviour
{

    public TextMeshProUGUI meterText;
    public PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meterText.text = $"Friction Charge: 0%";
    }

    // Update is called once per frame
    void Update()
    {
        DisplayCharge();
    }

    public void DisplayCharge()
    {
        meterText.text = ($"Friction Charge: {playerController.DriveCharge:0}%");
    }
}
