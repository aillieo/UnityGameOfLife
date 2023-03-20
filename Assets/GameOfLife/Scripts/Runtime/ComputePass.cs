using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AillieoGames
{
    public class ComputePass : ScriptableRenderPass
    {
        public ComputeConfigAsset computeConfig;

        private static readonly int targetBufferId = Shader.PropertyToID("TargetBuffer");
        private static readonly int resultId = Shader.PropertyToID("Result");

        private ComputeBuffer computeBuffer;

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
            RenderTextureDescriptor textureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            textureDescriptor.enableRandomWrite = true;

            cmd.GetTemporaryRT(targetBufferId, textureDescriptor);
            this.ConfigureTarget(targetBufferId);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (this.computeConfig == null || this.computeConfig.computeShader == null)
            {
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get();
            ScriptableRenderer renderer = renderingData.cameraData.renderer;
            Camera cam = renderingData.cameraData.camera;

            if (this.computeBuffer == null || !this.computeBuffer.IsValid())
            {
                this.computeBuffer = new ComputeBuffer(this.computeConfig.initState.Count, 32);
            }

            cmd.SetComputeBufferData(this.computeBuffer, this.computeConfig.initState);

            ComputeShader computeShader = this.computeConfig.computeShader;

            int kernelHandle = computeShader.FindKernel("CSMain");

            cmd.SetComputeBufferParam(computeShader, kernelHandle, "_Tiles", this.computeBuffer);
            cmd.SetComputeIntParam(computeShader, "_TileCount", this.computeConfig.initState.Count);

            cmd.SetComputeTextureParam(computeShader, kernelHandle, resultId, targetBufferId);
            cmd.DispatchCompute(computeShader, kernelHandle, Mathf.CeilToInt(Screen.width / 8), Mathf.CeilToInt(Screen.height / 8), 1);

            this.Blit(cmd, targetBufferId, renderer.cameraColorTarget);

            context.ExecuteCommandBuffer(cmd);

            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(targetBufferId);
        }

        public void Init()
        {
        }

        public void Cleanup()
        {
            if (this.computeBuffer != null && this.computeBuffer.IsValid())
            {
                this.computeBuffer.Dispose();
                this.computeBuffer = null;
            }
        }
    }
}
