using R2API;
using UnityEngine.Networking;
using RoR2;
using RoR2.Orbs;
using UnityEngine.AddressableAssets;
using UnityEngine;
using EntityStates.GaleShockTrooperStates.Weapon;
using System.Collections.Generic;

namespace GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Content
{
    public class CharacterDamageTypes
    {
        public static DamageAPI.ModdedDamageType SpecialSlugProc;
        public static DamageAPI.ModdedDamageType SpecialSlugVisual;

        public static void Init()
        {
            SetupSpecialSlug();
        }

        private static void SetupSpecialSlug()
        {
            SpecialSlugProc = DamageAPI.ReserveDamageType();
            SpecialSlugVisual = DamageAPI.ReserveDamageType();
            On.RoR2.GlobalEventManager.ProcessHitEnemy += TriggerSlugRicochet;
        }

        private static void TriggerSlugRicochet(On.RoR2.GlobalEventManager.orig_ProcessHitEnemy orig, RoR2.GlobalEventManager self, RoR2.DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
            orig(self, damageInfo, victim);

            if (!NetworkServer.active || damageInfo.procCoefficient <= 0f)
            {
                return;
            }

            if (damageInfo.HasModdedDamageType(SpecialSlugVisual))
            {
                EffectManager.SimpleEffect(FireRicochetSlug.ricochetImpactEffect, damageInfo.position, Quaternion.identity, true);
            }

            if (damageInfo.HasModdedDamageType(SpecialSlugProc))
            {
                TeamIndex teamIndex = TeamIndex.None;
                if (damageInfo.attacker)
                {
                    CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerBody && attackerBody.teamComponent) teamIndex = attackerBody.teamComponent.teamIndex;
                }

                damageInfo.damageType.RemoveModdedDamageType(SpecialSlugProc);

                ChainGunOrb chainGunOrb = new ChainGunOrb(FireRicochetSlug.orbEffectPrefab);
                chainGunOrb.damageValue = damageInfo.damage;
                chainGunOrb.isCrit = damageInfo.crit;
                chainGunOrb.teamIndex = teamIndex;
                chainGunOrb.attacker = damageInfo.attacker;
                chainGunOrb.procCoefficient = 1f;
                chainGunOrb.procChainMask = damageInfo.procChainMask;
                chainGunOrb.origin = damageInfo.position;
                chainGunOrb.speed = 200f;
                chainGunOrb.bouncesRemaining = FireRicochetSlug.ricochetCount;
                chainGunOrb.bounceRange = FireRicochetSlug.ricochetRange;
                chainGunOrb.damageCoefficientPerBounce = 1f;
                chainGunOrb.targetsToFindPerBounce = 1;
                chainGunOrb.canBounceOnSameTarget = false;
                chainGunOrb.damageColorIndex = DamageColorIndex.Item;
                chainGunOrb.damageType = damageInfo.damageType;

                if (victim)
                {
                    HealthComponent victimHealth = victim.GetComponent<HealthComponent>();
                    chainGunOrb.bouncedObjects.Add(victimHealth);
                }
                chainGunOrb.target = chainGunOrb.PickNextTarget(chainGunOrb.origin);
                OrbManager.instance.AddOrb(chainGunOrb);
            }
        }
    }
}
