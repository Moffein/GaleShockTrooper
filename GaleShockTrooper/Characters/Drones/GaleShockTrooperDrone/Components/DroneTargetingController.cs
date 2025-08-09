using RoR2;
using System.Linq;
using System;
using UnityEngine;

namespace GaleShockTrooper.Characters.Drones.GaleShockTrooperDrone.Components
{
    public class DroneTargetingController : MonoBehaviour
    {
        private BullseyeSearch search;
        private TeamComponent teamComponent;
        private HurtBox currentTarget;
        private CharacterBody characterBody;

        public static int searchFrequency = 20;

        private float searchStopwatch;

        private void Awake()
        {
            search = new BullseyeSearch();
            teamComponent = base.GetComponent<TeamComponent>();
            characterBody = base.GetComponent<CharacterBody>();
            searchStopwatch = 0f;
        }

        private void FixedUpdate()
        {
            searchStopwatch += Time.fixedDeltaTime;
            float freq = 1f / searchFrequency;
            if (searchStopwatch >= freq)
            {
                searchStopwatch -= freq;
                DoSearch();
            }
        }

        public void DoSearch()
        {
            Ray aimRay;
            if (characterBody.inputBank)
            {
                aimRay = characterBody.inputBank.GetAimRay();
            }
            else
            {
                aimRay = new Ray()
                {
                    origin = transform.position,
                    direction = transform.forward
                };
            }

            search.teamMaskFilter = TeamMask.all;
            search.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
            search.teamMaskFilter.RemoveTeam(TeamIndex.Neutral);
            search.filterByLoS = true;
            search.searchOrigin = aimRay.origin;
            search.searchDirection = aimRay.direction;
            search.sortMode = BullseyeSearch.SortMode.Angle;
            search.maxDistanceFilter = EntityStates.GaleShockTrooperDroneStates.FireAutoTurret.lockonRange;
            search.maxAngleFilter = EntityStates.GaleShockTrooperDroneStates.FireAutoTurret.lockonAngle;
            search.RefreshCandidates();
            search.FilterOutGameObject(base.gameObject);
            currentTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
        }

        public HurtBox GetCurrentTarget()
        {
            return currentTarget;
        }
    }
}
