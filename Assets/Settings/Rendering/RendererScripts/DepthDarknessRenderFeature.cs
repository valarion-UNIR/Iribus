using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class OscurecerRenderFeature : ScriptableRendererFeature
{
    class OscurecerPass : ScriptableRenderPass
    {
        const string m_PassName = "OscurecerPass";
        Material m_BlitMaterial;

        public void Setup(Material mat)
        {
            m_BlitMaterial = mat;
            requiresIntermediateTexture = true;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();

            // Seguridad: Si la textura activa es la pantalla directa, saltamos
            if (resourceData.isActiveTargetBackBuffer)
            {
                //Debug.LogWarning("OscurecerPass: saltando porque está escribiendo directo en el backbuffer.");
                return;
            }

            var source = resourceData.activeColorTexture;

            // Crear la textura de destino
            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = "CameraColor-OscurecerPass";
            destinationDesc.clearBuffer = false;

            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            // Parámetros del blit
            RenderGraphUtils.BlitMaterialParameters para = new(source, destination, m_BlitMaterial, 0);
            renderGraph.AddBlitPass(para, passName: m_PassName);

            // Actualizar el resourceData para que lo siguiente use el destino
            resourceData.cameraColor = destination;
        }
    }

    public RenderPassEvent injectionPoint = RenderPassEvent.AfterRendering;
    public Material material;

    OscurecerPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new OscurecerPass
        {
            renderPassEvent = injectionPoint
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null)
        {
            Debug.LogWarning("OscurecerRenderFeature: Material no asignado, efecto no se aplicará.");
            return;
        }

        m_ScriptablePass.Setup(material);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}

