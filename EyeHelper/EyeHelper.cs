using OWML.Common;
using OWML.ModHelper;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EyeHelper
{
    public class EyeHelper : ModBehaviour
    {
        private static EyeHelper _instance;

        public void Awake()
        {
            _instance = this;
            SceneManager.sceneLoaded += OnSceneChanged;
        }

        private void OnSceneChanged(Scene scene, LoadSceneMode _)
        {
            if (scene.name == LoadManager.SceneToName(OWScene.EyeOfTheUniverse))
            {
                _instance.ModHelper.Events.Unity.FireOnNextUpdate(() => Locator.GetPlayerSuit().SuitUp(instantSuitUp: true));
                // Make sure pause lock is removed
                GameObject.FindObjectOfType<PauseCommandListener>().RemovePauseCommandLock();
            }
        }

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
                    LoadManager.LoadScene(OWScene.SolarSystem, LoadManager.FadeType.ToBlack, 0.5f);
                };
            }
            if (LoadManager.GetCurrentScene() == OWScene.SolarSystem)
            {
                pauseMenu.MakeSimpleButton("GO TO THE EYE", 3, true).OnSubmitAction += () =>
                {
                    // This just unpauses
                    Locator.GetSceneMenuManager().pauseMenu.OnSkipToNextTimeLoop();
                    StartCoroutine(LoadEye(true));
                };

                pauseMenu.MakeSimpleButton("GO TO THE ANCIENT GLADE", 3, true).OnSubmitAction += () =>
                {
                    // This just unpauses
                    Locator.GetSceneMenuManager().pauseMenu.OnSkipToNextTimeLoop();
                    StartCoroutine(LoadEye(false));
                };
            }
        }

        private IEnumerator LoadEye(bool toVessel)
        {
            // Wait a bit in case we're impatient and just woke up else it glitches out at the eye scene and disables movement
            var cameraEffects = GameObject.FindObjectOfType<PlayerCameraEffectController>();
            if (cameraEffects._isOpeningEyes)
            {
                yield return new WaitForSeconds(cameraEffects._eyeAnimDuration);
            }

            // Else it overrides the state
            _instance.ModHelper.Console.WriteLine("Loading the Eye");

            PlayerData._currentGameSave.warpedToTheEye = toVessel;
            LoadManager.LoadScene(OWScene.EyeOfTheUniverse, LoadManager.FadeType.ToBlack, 0.5f);
        }
    }
}