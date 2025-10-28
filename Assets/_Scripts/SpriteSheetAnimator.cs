using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class SpriteSheetAnimator : MonoBehaviour
{
    [Header("Source Material (optional)")]
    public Material material; // optional - will use MeshRenderer.material if left empty

    [Header("Sprite sheet layout")]
    public int columns = 8;
    public int rows = 1;

    [Header("Playback")]
    public float framesPerSecond = 10f;
    public int startFrame = 0; // inclusive
    public int endFrame = -1;  // inclusive; -1 means last frame in sheet
    public bool loop = true;

    [Header("Events")]
    public int streetLightOnFrame = 7;
    public int streetLightFrameCounter = 0;
    public UnityEvent OnStreetLightTrigger;

    // runtime
    private Material runtimeMaterial;
    private int totalFrames;
    private Vector2 frameSize;
    private float frameTimer;
    private int currentFrameIndex;
    private bool playing = false;

    public void SetupAnimation()
    {
        var mr = GetComponent<MeshRenderer>();
        if (mr == null)
        {
            Debug.LogError("[SpriteSheetAnimator] No MeshRenderer found on: " + gameObject.name);
            enabled = false;
            return;
        }

        if (material == null)
        {
            // Use the MeshRenderer's material
            if (Application.isPlaying)
            {
                // Create an instance only at runtime
                runtimeMaterial = mr.material;
            }
            else
            {
                // In edit mode, use sharedMaterial to avoid leaks
                runtimeMaterial = mr.sharedMaterial;
            }
        }
        else
        {
            if (Application.isPlaying)
            {
                runtimeMaterial = Instantiate(material);
                mr.material = runtimeMaterial;
            }
            else
            {
                runtimeMaterial = material;
            }
        }

        if (runtimeMaterial == null || runtimeMaterial.mainTexture == null)
        {
            Debug.LogError("[SpriteSheetAnimator] Material or mainTexture missing.");
            enabled = false;
            return;
        }

        columns = Mathf.Max(1, columns);
        rows = Mathf.Max(1, rows);

        totalFrames = columns * rows;
        if (endFrame < 0 || endFrame >= totalFrames) endFrame = totalFrames - 1;
        startFrame = Mathf.Clamp(startFrame, 0, totalFrames - 1);
        endFrame = Mathf.Clamp(endFrame, startFrame, totalFrames - 1);

        frameSize = new Vector2(1f / columns, 1f / rows);
        runtimeMaterial.mainTextureScale = frameSize;

        currentFrameIndex = startFrame;
        ApplyFrame(currentFrameIndex);
        playing = true;
        streetLightFrameCounter = 0;
    }


    void Update()
    {
        if (!enabled || !playing) return;
        if (framesPerSecond <= 0) return;

        frameTimer += Time.deltaTime;
        float interval = 1f / framesPerSecond;
        if (frameTimer >= interval)
        {
            int steps = Mathf.FloorToInt(frameTimer / interval);
            frameTimer -= steps * interval;
            for (int i = 0; i < steps; i++)
            {
                StepFrame();
            }
        }
    }

    private void StepFrame()
    {
        currentFrameIndex++;
        if (currentFrameIndex > endFrame)
        {
            if (loop) currentFrameIndex = startFrame;
            else { currentFrameIndex = endFrame; playing = false; }
        }

        // Street light event trigger
        streetLightFrameCounter++;
        if (streetLightFrameCounter >= streetLightOnFrame)
        {
            
            OnStreetLightTrigger?.Invoke();
            streetLightFrameCounter = 0;
        }

        ApplyFrame(currentFrameIndex);
    }

    private void ApplyFrame(int frame)
    {
        int column = frame % columns;
        int row = frame / columns; // 0 is bottom row

        // Unity UVs start bottom-left, but textures are often arranged top-to-bottom in atlases.
        // We compute offset so it selects the correct frame.
        float offsetX = column * frameSize.x;
        // For Y we invert row so row 0 selects bottom-most tile: offsetY = 1 - frameHeight - row*frameHeight
        float offsetY = 1f - frameSize.y - row * frameSize.y;

        runtimeMaterial.mainTextureOffset = new Vector2(offsetX, offsetY);
    }

    // Public controls
    public void PlayAnimation() { playing = true; }
    public void PauseAnimation() { playing = false; }
    public void StopAnimation()
    {
        playing = false;
        currentFrameIndex = startFrame;
        ApplyFrame(currentFrameIndex);
    }
}
