using UnityEngine;
using UnityEngine.Events;

public class MenuAnimationManager : MonoBehaviour
{
    public static MenuAnimationManager Instance { get; private set; }
    [SerializeField]
    private GameObject screen;
    [SerializeField]
    private GameObject streetLightDirect;

    public UnityEvent OnCameraMoveCompleted = new UnityEvent();

    private Camera cam;
    private SpriteSheetAnimator ssa;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        ssa = screen.GetComponent<SpriteSheetAnimator>();
        cam = Camera.main;
    }

    private void Start()
    {
        if (ssa != null)
            ssa.OnStreetLightTrigger.AddListener(HandleLightTriggered);
        MenuManager.Instance.OnStartSequence.AddListener(HandleStartTriggered);
    }

    private void OnDisable()
    {
        if (ssa != null)
            ssa.OnStreetLightTrigger.RemoveListener(HandleLightTriggered);
        MenuManager.Instance.OnStartSequence.RemoveListener(HandleStartTriggered);
    }

    private void HandleLightTriggered()
    {
        Debug.Log("Street light was triggered!");
        streetLightDirect.GetComponent<Animator>().SetTrigger("PlayLightAnimation");
    }

    private void HandleStartTriggered()
    {
        Debug.Log("Start Sequence was triggered!");
        cam.GetComponent<Animator>().SetTrigger("PlayStartMove");
    }

}
