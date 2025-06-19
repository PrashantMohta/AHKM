using AHKM.UnityComponents;
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

        //public override List<ValueTuple<string, string>> GetPreloadNames()
        //{
        //    return new List<ValueTuple<string, string>>
        //    {
        //        new ValueTuple<string, string>("White_Palace_18", "White Palace Fly")
        //    };
        //}

        //public AHKM() : base("AHKM")
        //{
        //    Instance = this;
        //}

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;

            Log("Initialized");
            On.HeroController.Start += ApplyRainbowEffect;
        }

        public virtual void spawnThing(GameObject gameObject, Vector3 position) { }

        private void ApplyRainbowEffect(On.HeroController.orig_Start orig, HeroController self)
        {
            orig(self);
            if (self.GetComponent<ColorShifter>() == null)
                self.gameObject.AddComponent<ColorShifter>();
        }
    }
}
