using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GaleShockTrooper
{
    internal class Utils
    {
        internal static List<HealthComponent> FindEnemiesInSphere(float radius, Vector3 position, TeamIndex team)
        {
            List<HealthComponent> hcList = new List<HealthComponent>();
            Collider[] array = Physics.OverlapSphere(position, radius, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < array.Length; i++)
            {
                HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                if (hurtBox)
                {
                    HealthComponent healthComponent = hurtBox.healthComponent;
                    if (healthComponent && !hcList.Contains(healthComponent))
                    {
                        if (healthComponent.body && healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex != team)
                        {
                            hcList.Add(healthComponent);
                        }
                    }
                }
            }
            return hcList;
        }
    }
}
