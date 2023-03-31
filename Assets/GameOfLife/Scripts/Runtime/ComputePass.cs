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

        //private ComputeBuffer computeBuffer;

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);

            if (!GameOfLifeController.initialized)
            {
                cmd.Blit(computeConfig.initStateTexture, targetBufferId);
                GameOfLifeController.initialized = true;
            }
        }

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

            if (GameOfLifeController.compute)
            {
                GameOfLifeController.compute = false;

                ComputeShader computeShader = this.computeConfig.computeShader;

                int kernelHandle = computeShader.FindKernel("CSMain");

                cmd.SetComputeTextureParam(computeShader, kernelHandle, resultId, targetBufferId);
                cmd.DispatchCompute(computeShader, kernelHandle, computeConfig.initStateTexture.width / 8, computeConfig.initStateTexture.height / 8, 1);
            }

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
        }
    }
}
