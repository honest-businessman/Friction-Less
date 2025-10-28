using UnityEngine;

public class ScreenController: MonoBehaviour
{
    public Material osAnimationMaterial;
    public Material Game2DMaterial;
    public Material SettingsMaterial;

    private MeshRenderer meshRenderer;
    private SpriteSheetAnimator ssAnimator;
    private ScreenType lastScreen;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public enum ScreenType
    {
        Menu,
        Game2D,
        Settings
    }

    public void SetScreen(ScreenType screenType)
    {
        if(screenType == ScreenType.Menu)
        {
            meshRenderer.sharedMaterial = osAnimationMaterial;
            StartOSAnimation();
            lastScreen = ScreenType.Menu;
        }
        else if(screenType == ScreenType.Game2D)
        {
            if(lastScreen == ScreenType.Menu) { StopOSAnimation(); }
            lastScreen = ScreenType.Game2D;

        }
        else if(screenType == ScreenType.Settings)
        {
            if (lastScreen == ScreenType.Menu) { StopOSAnimation(); }
            lastScreen = ScreenType.Settings;
        }
    }

    private void StartOSAnimation()
    {
        ssAnimator.SetupAnimation();
        ssAnimator.PlayAnimation();
    }
    private void StopOSAnimation()
    {
        ssAnimator.StopAnimation();
    }
}
