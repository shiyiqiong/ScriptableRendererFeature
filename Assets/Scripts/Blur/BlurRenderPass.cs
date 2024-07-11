using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRenderPass : ScriptableRenderPass
{
    private BlurSettings defaultSettings;
    private Material material;
    private RenderTextureDescriptor blurTextureDescriptor;
    private RTHandle blurTextureHandle;

    private static readonly int horizontalBlurId = Shader.PropertyToID("_HorizontalBlur");
    private static readonly int verticalBlurId = Shader.PropertyToID("_VerticalBlur");

    public BlurRenderPass(Material material, BlurSettings defaultSettings)
    {
        this.material = material;
        this.defaultSettings = defaultSettings;        
        blurTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
    }

    //Unity在执行渲染过程之前调用这个方法
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        //Set the blur texture size to be the same as the camera target size.
        blurTextureDescriptor.width = cameraTextureDescriptor.width;
        blurTextureDescriptor.height = cameraTextureDescriptor.height;

        //Check if the descriptor has changed, and reallocate the RTHandle if necessary.
        RenderingUtils.ReAllocateIfNeeded(ref blurTextureHandle, blurTextureDescriptor);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        //Get a CommandBuffer from pool.
        CommandBuffer cmd = CommandBufferPool.Get();

        RTHandle cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;

        UpdateBlurSettings();

        // Blit from the camera target to the temporary render texture,
        // using the first shader pass.
        Blit(cmd, cameraTargetHandle, blurTextureHandle, material, 0);
        // Blit from the temporary render texture to the camera target,
        // using the second shader pass.
        Blit(cmd, blurTextureHandle, cameraTargetHandle, material, 1);

        //Execute the command buffer and release it back to the pool.
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
        
    }

    private void UpdateBlurSettings()
    {
        if (material == null) return;

         // Use the Volume settings or the default settings if no Volume is set.
        var volumeComponent = VolumeManager.instance.stack.GetComponent<BlurVolumeComponent>();
        if(volumeComponent.isActive.overrideState && volumeComponent.isActive.value)
        {
            float horizontalBlur = volumeComponent.horizontalBlur.overrideState ? volumeComponent.horizontalBlur.value : defaultSettings.horizontalBlur;
            float verticalBlur = volumeComponent.verticalBlur.overrideState ? volumeComponent.verticalBlur.value : defaultSettings.verticalBlur;
            material.SetFloat(horizontalBlurId, horizontalBlur);
            material.SetFloat(verticalBlurId, verticalBlur);
        }
        else
        {
            material.SetFloat(horizontalBlurId, defaultSettings.horizontalBlur);
            material.SetFloat(verticalBlurId, defaultSettings.verticalBlur);
        }
       
    }

    public void Dispose()
    {
        CoreUtils.Destroy(material);
    }
}