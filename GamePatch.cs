using HarmonyLib;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace REPOLevelScaler
{

    [HarmonyPatch(typeof(LevelGenerator), "Start")]
    public class PatchGenerationStart
    {
        //internal static Vector3 ChosenScale;
        //internal static float Scaler = 1f;
        public static void Prefix(LevelGenerator __instance)
        {
            AddOurComponent(__instance, out LevelScaler myScaler);

            if (!AreWeInGame() || !SemiFunc.IsMasterClientOrSingleplayer())
                return;

            bool grow = false;
            bool shrink = false;
            float chosenValue;

            if (Plugin.Rand.Next(0, 100) <= ModConfig.GrowOdds.Value)
                grow = true;

            if (Plugin.Rand.Next(0, 100) <= ModConfig.ShrinkOdds.Value)
                shrink = true;

            if(grow && shrink)
            {
                Plugin.Spam("Coin flipping between grow and shrink!");
                int number = Plugin.Rand.Next(0, 100);

                if (number > 50)
                    shrink = false;
                else
                    grow = false;
            }

            if (shrink)
                chosenValue = (float)Plugin.Rand.Next(55, 85) / 100f;
            else if (grow)
                chosenValue = (float)Plugin.Rand.Next(130, 225) / 100f;
            else
                chosenValue = 1f;

            Plugin.Spam($"Scaler set to {chosenValue}\ngrow - {grow}\nshrink - {shrink}");

            if (SemiFunc.IsMultiplayer())
                myScaler.photonView.RPC("SetScaleValues", Photon.Pun.RpcTarget.All, chosenValue);
            else
                myScaler.SetScaleValues(chosenValue);
        }

        private static void AddOurComponent(LevelGenerator levelGenerator, out LevelScaler myScaler)
        {
            if (levelGenerator.gameObject.GetComponent<LevelScaler>() == null)
                levelGenerator.gameObject.AddComponent<LevelScaler>();

            myScaler = levelGenerator.gameObject.GetComponent<LevelScaler>();
        }

        internal static bool AreWeInGame()
        {
            if (SemiFunc.RunIsLobbyMenu())
                return false;

            if (RunManager.instance.levelCurrent == RunManager.instance.levelMainMenu)
                return false;

            if (RunManager.instance.levelCurrent == RunManager.instance.levelArena)
                return false;

            return true;
        }
    }

   [HarmonyPatch(typeof(LevelPoint), "ModuleConnectSetup")]
    public class LevelPointFix
    {
        static bool Prefix(LevelPoint __instance, ref IEnumerator __result)
        {
            __result = ModuleConnectSetupScaled(__instance);
            return false;
        }

        private static IEnumerator ModuleConnectSetupScaled(LevelPoint point)
        {
            while (!LevelGenerator.Instance.Generated)
            {
                yield return new WaitForSeconds(0.1f);
            }

            Plugin.Spam($"ModuleConnect for {point.name} is being properly scaled!");
            float num = 999f;
            foreach (LevelPoint levelPathPoint in LevelGenerator.Instance.LevelPathPoints)
            {
                if (levelPathPoint.ModuleConnect)
                {
                    float num2 = Vector3.Distance(point.transform.position, levelPathPoint.transform.position);
                    if (num2 < 15f * LevelScaler.instance.Scaler && num2 < num && Vector3.Dot(levelPathPoint.transform.forward, point.transform.forward) <= -0.8f && Vector3.Dot(levelPathPoint.transform.forward, (point.transform.position - levelPathPoint.transform.position).normalized) > 0.8f)
                    {
                        num = num2;
                        point.ConnectedPoints.Add(levelPathPoint);
                    }
                }
            }

            point.ModuleConnected = true;
        }
    }
/*
    [HarmonyPatch(typeof(LevelPoint), "NavMeshCheck")]
    public class LevelPointNavFix
    {
        static bool Prefix(LevelPoint __instance, ref IEnumerator __result)
        {
            __result = NavMeshCheckFix(__instance);
            return false;
        }

        private static IEnumerator NavMeshCheckFix(LevelPoint point)
        {
            while (!LevelGenerator.Instance.Generated)
            {
                yield return new WaitForSeconds(0.1f);
            }

            Plugin.Spam("NavMeshCheckFix");

            yield return new WaitForSeconds(0.5f);
            bool flag = false;
            NavMeshHit val = default(NavMeshHit);
            if (!NavMesh.SamplePosition(point.transform.position, out val, 1f * LevelScaler.instance.Scaler, NavMesh.AllAreas))
            {
                point.transform.position = val.position;
            }

            if (!point.Room)
            {
                flag = true;
                Plugin.Error($"Level Point did not find a room volume!! {point.gameObject.name} / {point.gameObject.transform.parent.gameObject.name}");
            }

            foreach (LevelPoint connectedPoint in point.ConnectedPoints)
            {
                if (!connectedPoint)
                {
                    flag = true;
                    Plugin.Error($"Level Point not fully connected! (connectedPoint) {point.gameObject.name} / {point.gameObject.transform.parent.gameObject.name}");
                    continue;
                }

                bool flag2 = false;
                foreach (LevelPoint connectedPoint2 in connectedPoint.ConnectedPoints)
                {
                    if (connectedPoint2 == point)
                    {
                        flag2 = true;
                        break;
                    }
                }

                if (!flag2)
                {
                    flag = true;
                    Plugin.Error($"Level Point not fully connected! (flag2) {point.gameObject.name}");
                }
            }

            if (flag && Application.isEditor)
            {
                Object.Instantiate(AssetManager.instance.debugLevelPointError, point.transform.position, Quaternion.identity);
            }
        }
    } */

    [HarmonyPatch(typeof(LevelGenerator), "EnemySetup")]
    public class AfterEnemyPatch
    {
        public static void Postfix()
        {
            if (!PatchGenerationStart.AreWeInGame())
                return;

            if (SemiFunc.IsMultiplayer())
                LevelScaler.instance.photonView.RPC("OtherScales", Photon.Pun.RpcTarget.All);
            else
                LevelScaler.instance.OtherScales();

        }
    }

    [HarmonyPatch(typeof(LevelGenerator), "SpawnModule")]
    public class SpawnOffsetPatch
    {
        public static void Prefix(ref Vector3 position)
        {
            if (!PatchGenerationStart.AreWeInGame())
                return;

            //position *= LevelScaler.instance.Scaler;

        }
    }

    [HarmonyPatch(typeof(ValuableDirector), "Spawn")]
    public class ValuableScaling
    {
        public static void Prefix(GameObject _valuable, ValuableVolume _volume)
        {
            if (ModConfig.ScaleValuables.Value)
            {
                _valuable.transform.localScale = LevelScaler.instance.ChosenScale;
                _volume.transform.localScale = LevelScaler.instance.ChosenScale;
            }
        }
    }
/*
    [HarmonyPatch(typeof(TruckDoor), "UpdateObject")]
    public static class DoorOpenFix
    {
        //internal static float timer = 0f;
        internal static float desiredPosY;
        internal static bool gotDesired = false;
        public static void Postfix(TruckDoor __instance)
        {
            //if (!__instance.fullyOpen && timer != 0f)
                //timer = 0f;

            if (__instance.fullyOpen && !gotDesired)
            {
                desiredPosY = __instance.transform.position.y * LevelScaler.instance.Scaler;
                gotDesired = true;
            }
            

            if (__instance.fullyOpen && __instance.transform.position.y != desiredPosY)
            {
                //timer += Time.deltaTime;
                //if (timer < 1f)
                    //__instance.transform.position = Vector3.Lerp(__instance.transform.position, new(__instance.transform.position.x, desiredPosY, __instance.transform.position.z), timer);
                //else
                    __instance.transform.position = new(__instance.transform.position.x, desiredPosY, __instance.transform.position.z);
            }
        }
    }*/

    //MapObjectSetup
    [HarmonyPatch(typeof(Map), "MapObjectSetup")]
    public class MapScalePatch
    {
        public static void Postfix(GameObject _parent, GameObject _object)
        {
            if (!PatchGenerationStart.AreWeInGame())
                return;

            _object.transform.localScale = LevelScaler.instance.ChosenScale;
            Plugin.Spam($"${_parent.name} map object set to chosen scale - {LevelScaler.instance.ChosenScale}");
        }
    }
}
