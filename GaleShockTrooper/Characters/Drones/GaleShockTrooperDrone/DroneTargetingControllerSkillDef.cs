using GaleShockTrooper.Characters.Drones.GaleShockTrooperDrone.Components;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaleShockTrooper.Characters.Drones.GaleShockTrooperDrone
{
    public class DroneTargetingControllerSkillDef : SkillDef
    {
        public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new DroneTargetingControllerSkillDef.InstanceData
            {
                dtc = skillSlot.GetComponent<DroneTargetingController>()
            };
        }

        protected class InstanceData : SkillDef.BaseSkillInstanceData
        {
            public DroneTargetingController dtc;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            return base.CanExecute(skillSlot) && HasTarget(skillSlot);
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            return base.IsReady(skillSlot) && HasTarget(skillSlot);
        }

        public bool HasTarget([NotNull] GenericSkill skillSlot)
        {
            DroneTargetingController dtc = ((DroneTargetingControllerSkillDef.InstanceData)skillSlot.skillInstanceData).dtc;
            return dtc != null && dtc.GetCurrentTarget() != null;
        }
    }
}
