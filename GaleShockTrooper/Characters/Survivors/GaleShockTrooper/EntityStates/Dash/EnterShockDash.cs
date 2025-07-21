using UnityEngine;
using RoR2;

namespace EntityStates.GaleShockTrooperStates.Dash
{
    public class EnterShockDash : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            if (isAuthority)
            {
                Vector3 blinkVector = characterDirection.forward;
                string animString = "DashF";
                if (inputBank && inputBank.moveVector != Vector3.zero)
                {
                    blinkVector = inputBank.moveVector.normalized;
                    float blinkAngle = Vector3.SignedAngle(characterDirection.forward, blinkVector, Vector3.up);

                    if (blinkAngle >= 45f && blinkAngle < 135f)
                    {
                        animString = "DashR";
                    }
                    else if (blinkAngle <= -45f && blinkAngle > -135f)
                    {
                        animString = "DashL";
                    }
                    else if (blinkAngle >= 135f || blinkAngle <= -135f)
                    {
                        animString = "DashB";
                    }
                }

                switch (animString)
                {
                    case "DashB":
                        this.outer.SetNextState(new ShockDashB()
                        {
                            blinkVector = blinkVector
                        });
                        break;
                    case "DashL":
                        this.outer.SetNextState(new ShockDashL()
                        {
                            blinkVector = blinkVector
                        });
                        break;
                    case "DashR":
                        this.outer.SetNextState(new ShockDashR()
                        {
                            blinkVector = blinkVector
                        });
                        break;
                    default:
                        this.outer.SetNextState(new ShockDashBase()
                        {
                            blinkVector = blinkVector
                        });
                        break;
                }
            }
        }
    }
}
