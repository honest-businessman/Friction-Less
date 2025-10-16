using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class ScreenDistortionFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material distortionMaterial = null;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    class ScreenDistortionPass : ScriptableRenderPass
    {
        private Material material;
        private RTHandle source;
        private RTHandle tempTarget;
        private string profilerTag = "ScreenDistortionPass";

        public ScreenDistortionPass(Material mat)
        {
            material = mat;
        }

        public void SetSource(RTHandle src)
        {
            source = src;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cameraTextureDescriptor.depthBufferBits = 0;
            cameraTextureDescriptor.msaaSamples = 1;

            if (tempTarget == null)
            {
                tempTarget = RTHandles.Alloc(cameraTextureDescriptor, name: "_TempScreenDistortionTex");
            }

            ConfigureTarget(tempTarget);
            ConfigureClear(ClearFlag.None, Color.clear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null || source == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            // Blit from camera to temp
            Blitter.BlitCameraTexture(cmd, source, tempTarget, material, 0);

            // Blit back from temp to camera
            Blitter.BlitCameraTexture(cmd, tempTarget, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            // Keep tempTarget allocated for reuse
        }
    }

    public Settings settings = new Settings();
    private ScreenDistortionPass pass;

    public override void Create()
    {
        pass = new ScreenDistortionPass(settings.distortionMaterial)
        {
            renderPassEvent = settings.renderPassEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.distortionMaterial == null)
            return;

        // Use cameraColorTargetHandle for URP 15+ Forward Renderer
        pass.SetSource(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(pass);
    }
}
