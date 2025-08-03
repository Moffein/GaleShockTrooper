using EntityStates.GaleShockTrooperDroneStates;
using GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Components;
using GaleShockTrooper.Modules;
using GaleShockTrooper.Modules.Characters;
using R2API;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GaleShockTrooper.Characters.Drones.GaleShockTrooperDrone
{
    public class GaleShockTrooperDroneCharacter : CharacterBase<GaleShockTrooperDroneCharacter>
    {
        public override string assetBundleName => "galeshocktrooperassetbundle";

        public const string CHARACTER_NAME = "GaleShockTrooperDrone";
        public override string bodyName => CHARACTER_NAME + "Body";

        public override string modelPrefabName => "mdl" + CHARACTER_NAME;

        public const string TOKEN_PREFIX = GaleShockTrooperPlugin.DEVELOPER_PREFIX + "_" + CHARACTER_NAME + "_";

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = TOKEN_PREFIX + "NAME",
            subtitleNameToken = "",

            characterPortrait = assetBundle.LoadAsset<Texture>("texGaleShockTrooperDronePortrait"),
            bodyColor = new Color32(64, 149, 128, 255),
            sortPosition = 100,

            crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerCrosshair.prefab").WaitForCompletion(),

            maxHealth = 110f,
            healthRegen = 1f,
            armor = 0f,

            jumpCount = 1,

            bodyNameToClone = "Drone1",

            moveSpeed = 15f,
            acceleration = 30f,

            hasRagdoll = false,
            hasFootstepController = false,
            modifyCollider = false
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
            new CustomRendererInfo
            {
                childName = "Body",
                material = assetBundle.LoadMaterial("matTrooperWeapon"),
            }
        };

        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }

        public override void Initialize()
        {
            base.Initialize();
            InitializeCharacter();
        }

        public override void InitializeCharacter()
        {
            base.InitializeCharacterBodyPrefab();
            base.InitializeItemDisplays();

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            bodyPrefab.GetComponent<CharacterBody>().bodyFlags = CharacterBody.BodyFlags.Mechanical;
        }

        public override void InitializeCharacterMaster()
        {
            GameObject master = assetBundle.LoadMaster(bodyPrefab, "GaleShockTrooperDroneMaster");
            //GameObject master = Prefabs.CloneGenericMaster(bodyPrefab, "GaleShockTrooperDroneMaster", Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/Drone1Master.prefab").WaitForCompletion());
            MasterDroneTracker.droneMasterPrefab = master;
        }

        public override void InitializeEntityStateMachines()
        {
            Prefabs.ClearEntityStateMachines(bodyPrefab);
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(EntityStates.FlyState), typeof(EntityStates.FlyState));
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
        }

        public override void InitializeSkills()
        {
            Skills.ClearGenericSkills(bodyPrefab);
            AddPrimarySkills();
        }

        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            Modules.Content.AddEntityState(typeof(FireAutoTurret));
            SkillDef skillDef1 = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                activationState = new EntityStates.SerializableEntityStateType(typeof(FireAutoTurret)),
                stockToConsume = 1,
                baseRechargeInterval = 0f,
                rechargeStock = 1,
                activationStateMachineName = "Weapon",
                cancelSprintingOnActivation = true,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                baseMaxStock = 1,
                beginSkillCooldownOnSkillEnd = false,
                forceSprintDuringState = false,
                interruptPriority = EntityStates.InterruptPriority.Any,
                isCombatSkill = true,
                canceledFromSprinting = false,
                requiredStock = 0,
                skillNameToken = "GALE_GaleShockTrooperDrone_SPECIAL_DRONE_NAME",
                skillDescriptionToken = "GaleShockTrooperDrone_SPECIAL_DRONE_DESCRIPTION",
                mustKeyPress = false,
                resetCooldownTimerOnUse = false,
                skillName = "GaleShockTrooperDrone_AutoTurret",
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyFirePistol.asset").WaitForCompletion().icon
            });
            Skills.AddPrimarySkills(bodyPrefab, skillDef1);

            Color orbColor = new Color32(50, 150, 255, 255);
            GameObject orbEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneWeapons/ChainGunOrbEffect.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooperDrone_BulletOrbEffect", false);
            var tr = orbEffect.transform.Find("TrailParent/Trail").GetComponent<TrailRenderer>();
            tr.startColor = orbColor;
            tr.endColor = orbColor;
            Modules.ContentPacks.effectDefs.Add(new EffectDef(orbEffect));
            FireAutoTurret.orbEffectPrefab = orbEffect;

            GameObject mf = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooperDrone_MuzzleflashEffect", false);
            EffectComponent ec = mf.GetComponent<EffectComponent>();
            ec.soundName = "Play_engi_R_turret_shot";
            Modules.ContentPacks.effectDefs.Add(new EffectDef(mf));
            FireAutoTurret.muzzleflashEffectPrefab = mf;
        }

        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texSkinDefault"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);
            skins.Add(defaultSkin);

            SkinDef masterySkin = Skins.CreateSkinDef(TOKEN_PREFIX + "MASTERY_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texSkinMastery"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);
            masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matTrooperMasteryWeapon");
            skins.Add(masterySkin);

            skinController.skins = skins.ToArray();
        }
    }
}
