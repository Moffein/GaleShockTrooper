using R2API;

namespace GaleShockTrooper.Survivors.GaleShockTrooper.HenryContent
{
    public class CharacterDamageTypes
    {
        public static DamageAPI.ModdedDamageType comboFinisherDebuffDamage;

        public static void Init()
        {
            comboFinisherDebuffDamage = DamageAPI.ReserveDamageType();
        }
    }
}
