using System;
using UnityEngine;

namespace TOGSlowMotion
{
    public class SlowmoController : MonoBehaviour
    {
        public float lastClicked = 0;
        public bool isSlowmo;
        public float slowmoTimeScale = 0.5f;

        private void Awake()
        {
            lastClicked = Time.time;
        }
    }
}