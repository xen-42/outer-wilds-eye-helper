using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Collections;
using UnityEngine;

namespace EyeHelper
{
    [HarmonyPatch]
    public class EyeHelper : ModBehaviour
    {
        public static EyeState eyeState;

        public override void SetupPauseMenu(IPauseMenuManager pauseMenu)
        {
            base.SetupPauseMenu(pauseMenu);

            if (LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse)
            {
                pauseMenu.MakeSimpleButton("RETURN TO OUTER WILDS", 3, true).OnSubmitAction += () =>
                {
                    // This just unpauses
                    Locator.GetSceneMenuManager().pauseMenu.OnSkipToNextTimeLoop();
                    PlayerData._currentGameSave.warpedToTheEye = false;
                    eyeState = EyeState.Undefined;
                    LoadManager.LoadScene(OWScene.SolarSystem, LoadManager.FadeType.ToBlack, 0.5f);
                };
            }
            if (LoadManager.GetCurrentScene() == OWScene.SolarSystem)
            {
                pauseMenu.MakeSimpleButton("GO TO THE EYE", 3, true).OnSubmitAction += () =>
                {
                    // This just unpauses
                    Locator.GetSceneMenuManager().pauseMenu.OnSkipToNextTimeLoop();
                    eyeState = EyeState.AboardVessel;
                    StartCoroutine(LoadEye());
                };

                pauseMenu.MakeSimpleButton("GO TO THE ANCIENT GLADE", 3, true).OnSubmitAction += () =>
                {
                    // This just unpauses
                    Locator.GetSceneMenuManager().pauseMenu.OnSkipToNextTimeLoop();
                    eyeState = EyeState.ForestIsDark;
                    StartCoroutine(LoadEye());
                };
            }
        }

        private IEnumerator LoadEye()
        {
            // Wait a bit in case we're impatient and just woke up else it glitches out at the eye scene and disables movement
            var cameraEffects = GameObject.FindObjectOfType<PlayerCameraEffectController>();
            if (cameraEffects._isOpeningEyes)
            {
                yield return new WaitForSeconds(cameraEffects._eyeAnimDuration);
            }

            // Else it overrides the state
            PlayerData._currentGameSave.warpedToTheEye = eyeState == EyeState.AboardVessel;
            LoadManager.LoadScene(OWScene.EyeOfTheUniverse, LoadManager.FadeType.ToBlack, 0.5f);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EyeStateManager), nameof(EyeStateManager.Awake))]
        public static void Patch(EyeStateManager __instance)
        {
            if (eyeState != EyeState.Undefined)
            {
                __instance._initialState = eyeState;
                eyeState = EyeState.Undefined;
            }
        }
    }
}