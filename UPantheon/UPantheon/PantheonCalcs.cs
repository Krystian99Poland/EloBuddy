using EloBuddy;
using System.Linq;
using EloBuddy.SDK;

namespace UPantheon
{

    class PantheonCalcs
    {
        private static AIHeroClient Pantheon { get { return ObjectManager.Player; } }

        public static float Q(Obj_AI_Base target)
        {
            return Pantheon.CalculateDamageOnUnit(target, DamageType.Physical,
                (new float[] { 0, 65, 105, 145, 185, 225 }[Program.Q.Level] + (0.14f * Pantheon.FlatPhysicalDamageMod)));
        }

        public static float W(Obj_AI_Base target)
        {
            return Pantheon.CalculateDamageOnUnit(target, DamageType.Physical,
                (new float[] { 0, 50, 75, 100, 125, 1500 }[Program.W.Level] + (0.10f * Pantheon.FlatPhysicalDamageMod)));
        }
        public static float E(Obj_AI_Base target)
        {
            return Pantheon.CalculateDamageOnUnit(target, DamageType.Physical,
                (new float[] { 0, 80, 140, 200, 260, 320 }[Program.E.Level] + (0.20f * Pantheon.FlatPhysicalDamageMod)));
        }

        public static float Ignite(Obj_AI_Base target)
        {
            return ((10 + (4 * Program._Player.Level)) * 5) - ((target.HPRegenRate / 2) * 5);
        }
    }
}