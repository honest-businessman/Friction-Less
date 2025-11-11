using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIBinder : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Display")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Gameplay")]
    [SerializeField] private Toggle trainModeToggle;
    [SerializeField] private Toggle reduceShakeToggle;

    void Start()
    {
        var sm = SettingsManager.Instance;
        if (sm == null)
        {
            Debug.LogError("SettingsManager not found!");
            return;
        }

        // Populate UI
        if (masterSlider) masterSlider.value = sm.masterVolume;
        if (musicSlider) musicSlider.value = sm.musicVolume;
        if (sfxSlider) sfxSlider.value = sm.sfxVolume;
        if (fullscreenToggle) fullscreenToggle.isOn = sm.fullscreen;
        if (trainModeToggle) trainModeToggle.isOn = sm.trainMode;
        if (reduceShakeToggle) reduceShakeToggle.isOn = sm.reduceShake;

        if (resolutionDropdown)
        {
            resolutionDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<string>();
            foreach (var res in Screen.resolutions)
                options.Add($"{res.width} x {res.height}");
            resolutionDropdown.AddOptions(options);

            // Set the dropdown value to the current resolution
            int currentResolutionIndex = System.Array.FindIndex(Screen.resolutions, res => res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height);
            resolutionDropdown.value = currentResolutionIndex >= 0 ? currentResolutionIndex : sm.resolutionIndex;
        }

        // Bind Events
        if (masterSlider) masterSlider.onValueChanged.AddListener(v => { sm.masterVolume = v; ApplyChanges(); });
        if (musicSlider) musicSlider.onValueChanged.AddListener(v => { sm.musicVolume = v; ApplyChanges(); });
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(v => { sm.sfxVolume = v; ApplyChanges(); }); 
        if (fullscreenToggle) fullscreenToggle.onValueChanged.AddListener(v => { sm.fullscreen = v; ApplyChanges(); });
        if (resolutionDropdown) resolutionDropdown.onValueChanged.AddListener(i => { sm.resolutionIndex = i; ApplyChanges(); });
        if (trainModeToggle) trainModeToggle.onValueChanged.AddListener(v => { sm.trainMode = v; ApplyChanges(); });
        if (reduceShakeToggle) reduceShakeToggle.onValueChanged.AddListener(v => { sm.reduceShake = v; ApplyChanges(); });
    }

    public void ApplyChanges()
    {
        SettingsManager.Instance.ApplySettings();
        SettingsManager.Instance.SaveSettings();
    }
    public void CancelChanges()
    {
        SettingsManager.Instance.LoadSettings();
        SettingsManager.Instance.ApplySettings();
    }
}
