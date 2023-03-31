using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoGames
{
    public class GameOfLifeController : MonoBehaviour
    {
        public static bool initialized = false;
        public static bool compute = false;

        private float lastUpdate = 0;

        [Range(0.01f, 1)]
        public float step = 0.1f;

        private void Update()
        {
            if (!initialized)
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            if (now - lastUpdate > step)
            {
                lastUpdate = now;
                compute = true;
            }
        }
    }
}
