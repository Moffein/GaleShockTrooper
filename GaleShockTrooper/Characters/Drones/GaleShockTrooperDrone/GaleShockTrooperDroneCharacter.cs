using EntityStates;
using EntityStates.GaleShockTrooperDroneStates;
using GaleShockTrooper.Characters.Drones.GaleShockTrooperDrone.Components;
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

        public static float baseHealth = 110f;
        public static float baseArmor = 0f;

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

            CharacterBody body = bodyPrefab.GetComponent<CharacterBody>();
            body.bodyFlags = CharacterBody.BodyFlags.Mechanical;
            body.baseMaxHealth = baseHealth;
            body.levelMaxHealth = body.baseMaxHealth * 0.3f;

            body.gameObject.AddComponent<DroneTargetingController>();

            TeamComponent tc = bodyPrefab.GetComponent<TeamComponent>();
            tc.hideAllyCardDisplay = false;
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

            DroneTargetingControllerSkillDef skillDef1 = ScriptableObject.CreateInstance<DroneTargetingControllerSkillDef>();
            skillDef1.activationState = new EntityStates.SerializableEntityStateType(typeof(FireAutoTurret));
            skillDef1.stockToConsume = 1;
            skillDef1.baseRechargeInterval = 0f;
            skillDef1.rechargeStock = 1;
            skillDef1.activationStateMachineName = "Weapon";
            skillDef1.cancelSprintingOnActivation = true;
            skillDef1.fullRestockOnAssign = true;
            skillDef1.dontAllowPastMaxStocks = true;
            skillDef1.baseMaxStock = 1;
            skillDef1.beginSkillCooldownOnSkillEnd = false;
            skillDef1.forceSprintDuringState = false;
            skillDef1.interruptPriority = EntityStates.InterruptPriority.Any;
            skillDef1.isCombatSkill = true;
            skillDef1.canceledFromSprinting = false;
            skillDef1.requiredStock = 0;
            skillDef1.skillNameToken = "GALE_GaleShockTrooperDrone_SPECIAL_DRONE_NAME";
            skillDef1.skillDescriptionToken = "GaleShockTrooperDrone_SPECIAL_DRONE_DESCRIPTION";
            skillDef1.mustKeyPress = false;
            skillDef1.resetCooldownTimerOnUse = false;
            skillDef1.skillName = "GaleShockTrooperDrone_AutoTurret";
            skillDef1.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyFirePistol.asset").WaitForCompletion().icon;

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
