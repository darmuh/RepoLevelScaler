using Photon.Pun;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using Unity.VisualScripting;
using static b;

namespace REPOLevelScaler
{
    //enemy sizes synced
    //level sizes synced
    //occasional navmesh errors at smaller scales
    //random doors to tile furniture are all over the place for whatever reason?
    //connect objects are the doors
    //most tiles dont have a block object but I assume it'd be used to block a connection rather than have it open
    //if I want to change valuable sizes I will need to do so without LevelGenerator instance I think
    //ValuableDirector Spawn?
    //still no idea how to fix the random stuff in the tile at incorrect positions, maybe I need to change the scale of the level earlier

    internal class LevelScaler : MonoBehaviour
    {
        internal Vector3 ChosenScale;
        internal float Scaler = 1f;
        internal PhotonView photonView = null!;
        internal static LevelScaler instance = null!;
        private bool EnemyScaleChanged = false;

        private void Awake()
        {
            instance = this;
            photonView = gameObject.GetComponent<PhotonView>();
        }

        [PunRPC]
        internal void SetScaleValues(float value)
        {
            //DoorOpenFix.gotDesired = false;

            Scaler = value;
            ChosenScale = new(Scaler, Scaler, Scaler);

            RenderSettings.fogEndDistance = 15f * Scaler;
            CameraUtils.Instance.MainCamera.farClipPlane = 16f * Scaler;
            //LevelGenerator.TileSize = 5f * Scaler;
            //LevelGenerator.Instance.DebugLevelSize = Scaler;
            //LevelGenerator.Instance.LevelWidth = Mathf.RoundToInt((float)LevelGenerator.Instance.LevelWidth * Scaler);
            //LevelGenerator.Instance.LevelHeight = Mathf.RoundToInt((float)LevelGenerator.Instance.LevelHeight * Scaler);
            LevelGenerator.ModuleHeight = 1f * Scaler;
            LevelGenerator.ModuleWidth = 3f * Scaler;
            List<GameObject> AllLevelModules = [];
            
            if(RunManager.instance.levelCurrent.ConnectObject != null)
                AllLevelModules.Add(RunManager.instance.levelCurrent.ConnectObject);
            if (RunManager.instance.levelCurrent.BlockObject != null)
                AllLevelModules.Add(RunManager.instance.levelCurrent.BlockObject);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.StartRooms);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.ModulesNormal1);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.ModulesNormal2);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.ModulesNormal3);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.ModulesPassage1);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.ModulesPassage2);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.ModulesPassage3);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.ModulesDeadEnd1);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.ModulesDeadEnd2);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.ModulesDeadEnd3);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.ModulesExtraction1);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.ModulesExtraction2);
            AllLevelModules.AddRange(RunManager.instance.levelCurrent.ModulesExtraction3);
            AllLevelModules.RemoveAll(g => g == null);
            AllLevelModules.Do(g => 
            {
                List<LevelPoint> levelPoints = [.. gameObject.GetComponentsInChildren<LevelPoint>()];
                levelPoints.DoIf(p => p.transform.parent != g.transform, p =>
                {
                    PositionFix fixPoint = p.GetComponent<PositionFix>() ?? p.AddComponent<PositionFix>();
                    fixPoint.UpdatePoint(ref p, g);
                });

                PositionFix fixLP = g.GetComponent<PositionFix>() ?? g.AddComponent<PositionFix>();
                fixLP.UpdateObject(ref g);
            });

            EnemyScaling();

            //LevelGenerator.Instance.transform.position = new(LevelGenerator.Instance.transform.position.x, AllLevelModules[0].transform.position.y + 0.1f, LevelGenerator.Instance.transform.position.z);
            //LevelGenerator.Instance.transform.localScale = new(Mathf.Clamp(1f / Scaler, 1f, 99f), Mathf.Clamp(1f / Scaler, 1f, 99f), Mathf.Clamp(1f / Scaler, 1f, 99f));
            //NavMeshSurface surface = LevelGenerator.Instance.GetComponent<NavMeshSurface>();
            //surface.
            Plugin.Spam($"Scale set to {ChosenScale}");
        }

        private void EnemyScaling()
        {
            //float yComp = LevelGenerator.Instance.EnemyParent.transform.position.y * (1f / Scaler);
            List<EnemySetup> allEnemies = [];
            allEnemies.AddRange(EnemyDirector.instance.enemiesDifficulty1);
            allEnemies.AddRange(EnemyDirector.instance.enemiesDifficulty2);
            allEnemies.AddRange(EnemyDirector.instance.enemiesDifficulty3);

            allEnemies.Do(e => e.spawnObjects.Do(g => 
            {
                PositionFix fixPoint = g.GetComponent<PositionFix>() ?? g.AddComponent<PositionFix>();
                fixPoint.UpdateEnemy(ref g);
            }));


            Plugin.Spam("All enemies set to chosen scale!");
        }

        [PunRPC]
        internal void OtherScales()
        {
            //Runs after enemy setup
                
            if (ModConfig.ScaleItems.Value)
            {
                LevelGenerator.Instance.ItemParent.transform.localScale = ChosenScale;
            }
                
        }
    }
}
