using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class CameraAnimationManager : MonoBehaviour
{
    public static CameraAnimationManager Instance { get; private set; }

    [Header("Camera Setup")]
    [SerializeField] private GameObject screen;
    [SerializeField] private GameObject streetLightDirect;
    [SerializeField] private CinemachineCamera menuCam;
    [SerializeField] private CinemachineCamera gameCam;

    [Header("Camera Shake Settings")]
    [SerializeField] private float durationMultiplier = 0.1f;
    [SerializeField] private float speed = 0.07f;
    [SerializeField] private float shakeMultiplier = 0.01f;

    public UnityEvent OnGameCameraSwitchCompleted = new UnityEvent();

    private HealthSystem playerHealth;
    private Camera cam;
    private SpriteSheetAnimator ssa;
    private Coroutine shakeCoroutine;
    private PlayableDirector playableDirector;
    
    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if(screen != null) { ssa = screen.GetComponent<SpriteSheetAnimator>(); }
        cam = Camera.main;
        playableDirector = GetComponentInChildren<PlayableDirector>();

        noise = gameCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {
        PlayerEvents.OnPlayerSpawned += GetPlayerHealthSystem;

        if (ssa != null)
            ssa.OnStreetLightTrigger.AddListener(HandleLightTriggered);
        GameEvents.OnGameStarted += HandleGameStart;

    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerSpawned -= GetPlayerHealthSystem;

        if (ssa != null)
            ssa.OnStreetLightTrigger.RemoveListener(HandleLightTriggered);
        GameEvents.OnGameStarted -= HandleGameStart;

        if (playerHealth != null)
        {
            playerHealth.OnDamageTaken -= HandlePlayerDamageTaken;
        }
    }

    private void GetPlayerHealthSystem(GameObject player)
    {
        if (playerHealth != null)  // Unsubscribe from previous player's health events
        {
            playerHealth.OnDamageTaken -= HandlePlayerDamageTaken;
        }

        playerHealth = player.GetComponent<HealthSystem>();

        if (playerHealth != null)
        {
            playerHealth.OnDamageTaken += HandlePlayerDamageTaken;
        }
    }

    private void HandleLightTriggered()
    {
        Debug.Log("Street light was triggered!");
        streetLightDirect.GetComponent<Animator>().SetTrigger("PlayLightAnimation");
    }

    private void HandleGameStart()
    {
        Debug.Log("Start Sequence was triggered!");
        playableDirector.Play();
    }

    private void HandlePlayerDamageTaken(int remainingHealth, int maxHealth)
    {
        int intensity = Mathf.CeilToInt(((float)(maxHealth - remainingHealth) / maxHealth) * 6);
        ShakeCamera(intensity);
    }

    public void ShakeCamera(int intensity = 1)
    {

        // You can tweak duration and magnitude per intensity level
        float duration = durationMultiplier * intensity;
        float frequency = speed;
        float amplitude = shakeMultiplier * intensity;   


        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, frequency, amplitude));
    }
    private IEnumerator ShakeCoroutine(float duration, float frequency, float amplitude)
    {
        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        // Smoothly fade out noise
        float fadeTime = 1f;
        float elapsed = 0f;
        float startAmp = noise.AmplitudeGain;
        float startFreq = noise.FrequencyGain;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            noise.AmplitudeGain = Mathf.Lerp(startAmp, 0f, elapsed / fadeTime);
            noise.FrequencyGain = Mathf.Lerp(startFreq, 0f, elapsed / fadeTime);
            yield return null;
        }

        noise.AmplitudeGain = 0f;
        shakeCoroutine = null;
    }

}
