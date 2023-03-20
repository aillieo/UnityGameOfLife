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
        [SerializeField]
        public List<int> initState;

        private void OnValidate()
        {
            if (this.initState == null || this.initState.Count != 128)
            {
                this.initState = new List<int>();
                for (int i = 0; i < 128; ++i)
                {
                    this.initState.Add(i);
                }
            }
        }
    }
}
