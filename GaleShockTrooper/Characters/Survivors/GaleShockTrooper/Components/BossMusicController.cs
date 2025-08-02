using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Components
{
    public class BossMusicController : NetworkBehaviour
    {
        public static BossMusicController instance;

        public string soundName;
        private uint soundID = 0u;

        [ClientRpc]
        public void RpcStartMusicServer()
        {
            soundID = Util.PlaySound(soundName, gameObject);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
            AkSoundEngine.StopPlayingID(soundID);
        }
    }
}
