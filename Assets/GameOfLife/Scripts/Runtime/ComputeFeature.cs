using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AillieoGames
{
    public class ComputeFeature : ScriptableRendererFeature
    {
        public ComputeConfigAsset computeConfig;

        private ComputePass computePass;

        public override void Create()
        {
            if (this.computeConfig == null)
            {
                return;
            }

            this.computePass = new ComputePass() { computeConfig = this.computeConfig };
            this.computePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            this.computePass.Init();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (this.computeConfig == null)
            {
                return;
            }

            renderer.EnqueuePass(this.computePass);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.computeConfig == null)
            {
                return;
            }

            this.computePass.Cleanup();
        }
    }
}
