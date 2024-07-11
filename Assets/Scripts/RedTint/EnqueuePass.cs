using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnqueuePass : MonoBehaviour
{
    [SerializeField] private Material material; 
    private RedTintRenderPass redTintRenderPass;

    private void OnEnable()
    {

        redTintRenderPass = new RedTintRenderPass(material);
        // Subscribe the OnBeginCamera method to the beginCameraRendering event.
        RenderPipelineManager.beginCameraRendering += OnBeginCamera;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
        redTintRenderPass.Dispose();

    }

    private void OnBeginCamera(ScriptableRenderContext context, Camera cam)
    {

        // Use the EnqueuePass method to inject a custom render pass
        cam.GetUniversalAdditionalCameraData()
            .scriptableRenderer.EnqueuePass(redTintRenderPass);
    }
}
