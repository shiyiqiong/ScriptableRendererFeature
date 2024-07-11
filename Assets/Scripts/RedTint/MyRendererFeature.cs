using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MyRendererFeature : ScriptableRendererFeature
{
    [SerializeField] 
    private Shader shader;
    private Material material;
    private RedTintRenderPass redTintRenderPass;

    //每帧调用一次，每个摄像机调用一次，允许注入可编程渲染通道
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game) //应用与特定摄像机类型（游戏摄像机）
        {
            renderer.EnqueuePass(redTintRenderPass); //将必要的渲染通道，添加到渲染器通道队列
        }
    }

    //渲染器第一次加载时、启用或禁用渲染器功能时、渲染器功能检查器更改属性时调用
    public override void Create()
    {
         if (shader == null)
        {
            return;
        }
        material = CoreUtils.CreateEngineMaterial(shader);
        redTintRenderPass = new RedTintRenderPass(material);
        redTintRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox; //指定何时执行渲染通道
    }

    //销毁是调用
    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(material);
    }
}
