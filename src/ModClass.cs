using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace AHKM
{
    public class AHKM : Mod
    {
        internal static AHKM Instance;

        public override List<ValueTuple<string, string>> GetPreloadNames()
        {
            return new List<ValueTuple<string, string>>
            {
                //        new ValueTuple<string, string>("White_Palace_18", "White Palace Fly")
            };
        }

        //public AHKM() : base("AHKM")
        //{
        //    Instance = this;
        //}

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;

            AssetManager.Initialize();
            
            Log("Grub");

            Log("Initialized");

            // I'm sorry
            On.CharmIconList.GetSprite += (orig, self, _) =>
            {
                return orig(self, 11);
            };

            ModHooks.LanguageGetHook += (key, title, orig) =>
            {
                if (key.EndsWith("11")) return orig;
                if (key.StartsWith("CHARM_NAME_")) return Language.Language.Get("CHARM_NAME_11", title);
                if (!key.StartsWith("CHARM_DESC_")) return orig;
                var info = Language.Language.Get("CHARM_DESC_11", title)
                    .Split(new[] { "<br>" }, StringSplitOptions.None)[0];
                return info + "<br><br>" + orig.Split(new[] { "<br>" }, StringSplitOptions.None)[2];
            };

            ModHooks.HeroUpdateHook += () =>
            {
                HeroController.instance.GetComponent<tk2dSprite>().color = Color.HSVToRGB(Time.timeSinceLevelLoad - (int)Time.timeSinceLevelLoad, 1, 1);
            };

            // disable random root gameobject, mostly does nothing as they are just disabled, not removed and gamelogic usually enables them at one point or another. so on early scenes, e.g. Scene_Title, with buildindex 1 it only disables either of the cameras, neither of which actually handle the keyboard input of the menu items, so the game can still be closed, therefore making the game technically not unplayable, even if it temporarily is.
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (from, to) =>
            {
                var go = to.GetRootGameObjects()[UnityEngine.Random.Range(0, to.buildIndex) % to.rootCount];
                Log($"Disabling {go}");
                go.SetActive(false);
            };
        }

        public virtual void spawnThing(GameObject gameObject, Vector3 position)
        {
        }
    }
}
