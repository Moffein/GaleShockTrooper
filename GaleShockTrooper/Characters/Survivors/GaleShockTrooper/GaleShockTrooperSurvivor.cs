using BepInEx.Configuration;
using EntityStates.GaleShockTrooperStates.Dash;
using EntityStates.GaleShockTrooperStates.Weapon;
using EntityStates.GaleShockTrooperStates.Weapon.MissilePainter;
using GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Content;
using GaleShockTrooper.Modules;
using GaleShockTrooper.Modules.Characters;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace GaleShockTrooper.Survivors.GaleShockTrooperSurvivor
{
    public class GaleShockTrooperSurvivor : SurvivorBase<GaleShockTrooperSurvivor>
    {
        //used to load the assetbundle for this character. must be unique
        public override string assetBundleName => "galeshocktrooperassetbundle"; //if you do not change this, you are giving permission to deprecate the mod
        //used in the rest of your character setup. must be unique.
        public const string CHARACTER_NAME = "GaleShockTrooper"; //if you do not change this, you are giving permission to deprecate the mod

        //the name of your character prefab. must be unique
        public override string bodyName => CHARACTER_NAME + "Body"; //if you do not change CHARACTER_NAME, you get the point by now
        //the name of the ai master for vengeance and goobo. must be unique
        public override string masterName => CHARACTER_NAME + "MonsterMaster"; //if you do not yadda yadda
        //the names of the prefabs that we are loading from the bundle to build your character. must match the names of the asset names in unity.
        //doesn't have to be unique, but probably should be anyways.
        public override string modelPrefabName => "mdl" + CHARACTER_NAME;
        public override string displayPrefabName => CHARACTER_NAME + "Display";

        public const string TOKEN_PREFIX = GaleShockTrooperPlugin.DEVELOPER_PREFIX + "_" + CHARACTER_NAME + "_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => TOKEN_PREFIX;

        public static BodyIndex bodyIndex;

        public static float passiveFrontArmorMult = 2f / 3f;
        
        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = TOKEN_PREFIX + "NAME",
            subtitleNameToken = TOKEN_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texGaleShockTrooperPortrait"),
            bodyColor = new Color32(64, 149, 128, 255),
            sortPosition = 100,

            crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/SMGCrosshair.prefab").WaitForCompletion(),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthRegen = 1.5f,
            armor = 0f,

            jumpCount = 1
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
            new CustomRendererInfo
            {
                childName = "Armor",
                material = assetBundle.LoadMaterial("matTrooper01"),
            },
            new CustomRendererInfo
            {
                childName = "Back",
                material = assetBundle.LoadMaterial("matTrooperBackpack"),
            },
            new CustomRendererInfo
            {
                childName = "Body",
                material = assetBundle.LoadMaterial("matTrooperBody"),
            },
            new CustomRendererInfo
            {
                childName = "Drone",
                material = assetBundle.LoadMaterial("matTrooper01"),
            },
            new CustomRendererInfo
            {
                childName = "GunRender",
                material = assetBundle.LoadMaterial("matTrooperWeapon"),
            }
        };

        public override UnlockableDef characterUnlockableDef => Content.CharacterUnlockables.characterUnlockableDef;
        
        public override ItemDisplaysBase itemDisplays => new Content.CharacterItemDisplaySetup();

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public override void Initialize()
        {
            ////uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Henry");

            //if (!characterEnabled.Value)
            //    return;

            base.Initialize();

            InitializeCharacter();
            RoR2Application.onLoad += OnLoadActions;
        }

        private void OnLoadActions()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("GaleShockTrooperBody");
        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            Content.CharacterUnlockables.Init();
            
            //the magic. creating your survivor
            base.InitializeCharacterBodyPrefab();
            base.InitializeItemDisplays();
            base.InitializeDisplayPrefab();
            base.InitializeSurvivor();

            Content.CharacterBuffs.Init(assetBundle);
            Content.CharacterDots.Init();
            Content.CharacterDamageTypes.Init();

            Content.CharacterConfig.Init();
            Content.CharacterTokens.Init();

            Content.CharacterAssets.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            AddHooks();
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.score.AutoSprint")) AutoSprintCompat();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void AutoSprintCompat()
        {
            AutoSprint.Core.StateManager.SprintDisabledTypeNames.Add(typeof(EntityStates.GaleShockTrooperStates.Weapon.MissilePainter.PaintMissiles).FullName);
        }

        private void AdditionalBodySetup()
        {
            CharacterBody body = bodyPrefab.GetComponent<CharacterBody>();
            body.spreadBloomCurve = new AnimationCurve()
            {
                keys = new Keyframe[]
                {
                     new Keyframe()
                     {
                         time = 0f,
                         value = 1.5f
                     },
                     new Keyframe()
                     {
                         time = 1f,
                         value = 3f
                     },
                }
            };

            CharacterMotor motor = bodyPrefab.GetComponent<CharacterMotor>();
            if (motor)
            {
                motor.mass = 300f;
            }
        }

        public override void InitializeEntityStateMachines() 
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(EntityStates.GenericCharacterMain), typeof(EntityStates.SpawnTeleporterState));
            //if you set up a custom `CharacterMain` state, set it here. 
                //don't forget to register custom entitystates in your HenryStates.cs

            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
        }

        #region skills
        public override void InitializeSkills()
        {
            Skills.ClearGenericSkills(bodyPrefab);
            AddPassiveSkill();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtiitySkills();
            AddSpecialSkills();
        }

        private void AddPassiveSkill()
        {
            GenericSkill passiveGenericSkill = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "PassiveSkill");
            SkillDef passiveSkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "GaleShockTrooper_PassiveFrontArmor",
                skillNameToken = TOKEN_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = TOKEN_PREFIX + "PASSIVE_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CallSupplyDropShocking.asset").WaitForCompletion().icon

            });
            Skills.AddSkillsToFamily(passiveGenericSkill.skillFamily, passiveSkillDef1);
            SkillDefs.Passive_FrontArmor = passiveSkillDef1;

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        //TODO: Change later.
        private static NetworkSoundEventDef blockSound = Addressables.LoadAssetAsync<NetworkSoundEventDef>("RoR2/Base/ArmorPlate/nseArmorPlateBlock.asset").WaitForCompletion();
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active && damageInfo.attacker && self.body && self.body.bodyIndex == bodyIndex && Skills.HasPassiveSkill(self.body, SkillDefs.Passive_FrontArmor))
            {
                Vector3 attackDirection = -(damageInfo.attacker.transform.position - damageInfo.position);
                if (!BackstabManager.IsBackstab(attackDirection, self.body))
                {
                    bool isBullet = damageInfo.damageType.HasModdedDamageType(ExtraDamageTypes.ExtraDamageTypes.Bullet);
                    bool isProjectile = damageInfo.attacker != damageInfo.inflictor;
                    bool isAoe = damageInfo.damageType.damageType.HasFlag(DamageType.AOE);

                    if (isBullet || isProjectile || isAoe)
                    {
                        EffectManager.SimpleSoundEffect(blockSound.index, damageInfo.position, true);
                        damageInfo.damage *= passiveFrontArmorMult;
                    }
                }
            }
            orig(self, damageInfo);
        }

        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            Modules.Content.AddEntityState(typeof(EntityStates.GaleShockTrooperStates.Weapon.FireShotgun));
            SkillDef skillDef1 = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GaleShockTrooperStates.Weapon.FireShotgun)),
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
                skillNameToken = TOKEN_PREFIX + "PRIMARY_NAME",
                skillDescriptionToken = TOKEN_PREFIX + "PRIMARY_DESCRIPTION",
                mustKeyPress = false,
                resetCooldownTimerOnUse = false,
                skillName = "GaleShockTrooper_AutoShotgun",
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyFireShotgunBlast.asset").WaitForCompletion().icon
            });
            Skills.AddPrimarySkills(bodyPrefab, skillDef1);
            SkillDefs.Primary_AutoShotgun = skillDef1;
        }

        private void AddSecondarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Secondary);
            Modules.Content.AddEntityState(typeof(EntityStates.GaleShockTrooperStates.Weapon.MissilePainter.PaintMissiles));
            Modules.Content.AddEntityState(typeof(EntityStates.GaleShockTrooperStates.Weapon.MissilePainter.FireMissiles));
            SkillDef skillDef1 = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GaleShockTrooperStates.Weapon.MissilePainter.PaintMissiles)),
                stockToConsume = 0,
                baseRechargeInterval = PaintMissiles.baseCooldown,
                rechargeStock = 1,
                activationStateMachineName = "Weapon",
                cancelSprintingOnActivation = true,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                baseMaxStock = PaintMissiles.baseMaxStocks,
                beginSkillCooldownOnSkillEnd = true,
                forceSprintDuringState = false,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                isCombatSkill = true,
                canceledFromSprinting = true,
                requiredStock = 1,
                skillNameToken = TOKEN_PREFIX + "SECONDARY_NAME",
                skillDescriptionToken = TOKEN_PREFIX + "SECONDARY_DESCRIPTION",
                mustKeyPress = true,
                resetCooldownTimerOnUse = false,
                skillName = "GaleShockTrooper_MissilePainter",
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiHarpoons.asset").WaitForCompletion().icon
            });
            skillDef1.autoHandleLuminousShot = false;
            SkillDefs.Secondary_MicroMissile = skillDef1;

            Modules.Content.AddEntityState(typeof(EntityStates.GaleShockTrooperStates.Weapon.ThrowSticky));
            SkillDef skillDef2 = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GaleShockTrooperStates.Weapon.ThrowSticky)),
                stockToConsume = 1,
                baseRechargeInterval = ThrowSticky.baseCooldown,
                rechargeStock = 1,
                activationStateMachineName = "Weapon",
                cancelSprintingOnActivation = true,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                baseMaxStock = ThrowSticky.baseMaxStocks,
                beginSkillCooldownOnSkillEnd = true,
                forceSprintDuringState = false,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                isCombatSkill = true,
                canceledFromSprinting = false,
                requiredStock = 1,
                skillNameToken = TOKEN_PREFIX + "SECONDARY_STICKY_NAME",
                skillDescriptionToken = TOKEN_PREFIX + "SECONDARY_STICKY_DESCRIPTION",
                mustKeyPress = true,
                resetCooldownTimerOnUse = false,
                skillName = "GaleShockTrooper_Sticky",
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/ThrowGrenade.asset").WaitForCompletion().icon
            });
            SkillDefs.Secondary_Stickybomb = skillDef2;

            Skills.AddSecondarySkills(bodyPrefab, new SkillDef[]
            {
                skillDef1,
                skillDef2
            });
        }

        private void AddUtiitySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Utility);
            Modules.Content.AddEntityState(typeof(EntityStates.GaleShockTrooperStates.Dash.EnterShockDash));
            Modules.Content.AddEntityState(typeof(EntityStates.GaleShockTrooperStates.Dash.ShockDashBase));
            Modules.Content.AddEntityState(typeof(EntityStates.GaleShockTrooperStates.Dash.ShockDashR));
            Modules.Content.AddEntityState(typeof(EntityStates.GaleShockTrooperStates.Dash.ShockDashL));
            Modules.Content.AddEntityState(typeof(EntityStates.GaleShockTrooperStates.Dash.ShockDashB));

            SkillDef skillDef1 = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GaleShockTrooperStates.Dash.EnterShockDash)),
                stockToConsume = 1,
                baseRechargeInterval = EnterShockDash.baseCooldown,
                rechargeStock = 1,
                activationStateMachineName = "Body",
                cancelSprintingOnActivation = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                baseMaxStock = 1,
                beginSkillCooldownOnSkillEnd = false,
                forceSprintDuringState = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                isCombatSkill = false,
                canceledFromSprinting = false,
                requiredStock = 1,
                skillNameToken = TOKEN_PREFIX + "UTILITY_NAME",
                skillDescriptionToken = TOKEN_PREFIX + "UTILITY_DESCRIPTION",
                mustKeyPress = false,
                resetCooldownTimerOnUse = false,
                skillName = "GaleShockTrooper_ShockDash",
                keywordTokens = new string[]
                {
                    "KEYWORD_SHOCKING"
                },
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Huntress/HuntressBodyBlink.asset").WaitForCompletion().icon
            });
            Skills.AddUtilitySkills(bodyPrefab, skillDef1);
            SkillDefs.Utility_ShockDash = skillDef1;
        }

        private void AddSpecialSkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Special);
            Modules.Content.AddEntityState(typeof(EntityStates.GaleShockTrooperStates.Weapon.FireRicochetSlug));
            SkillDef skillDef1 = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GaleShockTrooperStates.Weapon.FireRicochetSlug)),
                stockToConsume = 1,
                baseRechargeInterval = FireRicochetSlug.baseCooldown,
                rechargeStock = 1,
                activationStateMachineName = "Weapon",
                cancelSprintingOnActivation = true,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                baseMaxStock = 1,
                beginSkillCooldownOnSkillEnd = false,
                forceSprintDuringState = false,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                isCombatSkill = true,
                canceledFromSprinting = false,
                requiredStock = 1,
                skillNameToken = TOKEN_PREFIX + "SPECIAL_NAME",
                skillDescriptionToken = TOKEN_PREFIX + "SPECIAL_DESCRIPTION",
                mustKeyPress = true,
                resetCooldownTimerOnUse = false,
                skillName = "GaleShockTrooper_RicochetSlug",
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/Bandit2Blast.asset").WaitForCompletion().icon
            });
            Skills.AddSpecialSkills(bodyPrefab, skillDef1);
            SkillDefs.Special_RicochetSlug = skillDef1;
        }
        #endregion skills
        
        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texSkinDefault"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
                //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
                //uncomment this when you have another skin
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin

            ////creating a new skindef as we did before
            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(HENRY_PREFIX + "MASTERY_SKIN_NAME",
            //    assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
            //    defaultRendererinfos,
            //    prefabCharacterModel.gameObject,
            //    HenryUnlockables.masterySkinUnlockableDef);

            SkinDef masterySkin = Skins.CreateSkinDef(TOKEN_PREFIX + "MASTERY_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texSkinMastery"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                Content.CharacterUnlockables.masterySkinUnlockableDef);

            masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matTrooperMastery01");
            masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("matTrooperMasteryBackpack");
            masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("matTrooperMasteryBody");
            masterySkin.rendererInfos[3].defaultMaterial = assetBundle.LoadMaterial("matTrooperMastery01");
            masterySkin.rendererInfos[4].defaultMaterial = assetBundle.LoadMaterial("matTrooperMasteryWeapon");

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            //masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySwordAlt",
            //    null,//no gun mesh replacement. use same gun mesh
            //    "meshHenryAlt");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            //masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            //masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            //{
            //    new SkinDef.GameObjectActivation
            //    {
            //        gameObject = childLocator.FindChildGameObject("GunModel"),
            //        shouldActivate = false,
            //    }
            //};
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            skins.Add(masterySkin);

            #endregion

            skinController.skins = skins.ToArray();
        }
        #endregion skins

        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster()
        {
            //you must only do one of these. adding duplicate masters breaks the game.

            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            Content.CharacterAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
        }
    }
}