using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Bossfight
{
    public class BossMusicController : NetworkBehaviour
    {
        public static GameObject prefab;
        public static BossMusicController instance;

        public string soundName;
        private uint soundID = 0u;

        public void StartMusicServer()
        {
            if (!NetworkServer.active) return;
            RpcStartMusicServer();
        }

        [ClientRpc]
        private void RpcStartMusicServer()
        {
            if (soundID == 0u)
            {
                musicSources++;
                soundID = Util.PlaySound(soundName, gameObject);
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
            if (soundID != 0u)
            {
                musicSources--;
                AkSoundEngine.StopPlayingID(soundID);
            }
        }

        internal static int musicSources = 0;
        public static void MusicController_LateUpdate(ILContext il)
        {
            var cursor = new ILCursor(il);

            cursor.GotoNext(i => i.MatchStloc(out _));
            cursor.EmitDelegate<Func<bool, bool>>(b =>
            {
                if (b)
                    return true;

                return musicSources > 0;
            });
        }
    }
}
