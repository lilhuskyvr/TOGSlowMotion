using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;

namespace TOGSlowMotion
{
    public class SlowmoPatch : MonoBehaviour
    {
        private Harmony _harmony;

        public void Inject()
        {
            try
            {
                _harmony = new Harmony("SlowMotion");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                Debug.Log("Slow Motion Loaded");
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }


        [HarmonyPatch(typeof(VRCharController))]
        [HarmonyPatch("Start")]
        // ReSharper disable once UnusedType.Local
        private static class VRCharControllerStartPatch
        {
            [HarmonyPostfix]
            private static void Postfix(VRCharController __instance)
            {
                var slowmoController = __instance.gameObject.AddComponent<SlowmoController>();
                var jsonInput = File.ReadAllText(Application.streamingAssetsPath +
                                                 "/Mods/TOGSlowMotion/Settings.json");
                
                var data = JsonConvert.DeserializeObject<SlowmoControllerData>(jsonInput);
                Debug.Log(data.slowmoTimeScale);
                slowmoController.slowmoTimeScale = data.slowmoTimeScale;
            }
        }

        [HarmonyPatch(typeof(VRCharController))]
        [HarmonyPatch("LateUpdate")]
        // ReSharper disable once UnusedType.Local
        private static class VRCharControllerLateUpdatePatch
        {
            [HarmonyPostfix]
            private static void Postfix(VRCharController __instance)
            {
                var slowmoController = __instance.gameObject.GetComponent<SlowmoController>();
                if (slowmoController.isSlowmo && Mathf.Approximately(Time.timeScale, 1))
                    Time.timeScale = slowmoController.slowmoTimeScale;

                if (__instance.controlInput.OrderController.ButtonAPressed)
                {
                    if (Time.time - __instance.gameObject.GetComponent<SlowmoController>().lastClicked >= 0.5f)
                    {
                        Time.timeScale = Mathf.Approximately(Time.timeScale, 1) ? slowmoController.slowmoTimeScale : 1;

                        slowmoController.lastClicked = Time.time;
                        slowmoController.isSlowmo = !Mathf.Approximately(Time.timeScale, 1);
                    }
                }
            }
        }
    }
}