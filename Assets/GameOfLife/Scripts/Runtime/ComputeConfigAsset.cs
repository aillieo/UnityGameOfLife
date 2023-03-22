using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AillieoGames
{
    //[CreateAssetMenu(fileName = "ComputeConfigAsset")]
    public class ComputeConfigAsset : ScriptableObject
    {
        public ComputeShader computeShader;

        public Texture2D initStateTexture;
    }
}
