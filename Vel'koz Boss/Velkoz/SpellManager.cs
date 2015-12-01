using EloBuddy;
using EloBuddy.SDK;

namespace AddonTemplate
{
    public static class SpellManager
    {
        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Skillshot W { get; private set; }
        public static Spell.Skillshot E { get; private set; }
        public static Spell.Skillshot R { get; private set; }

        static SpellManager()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1050, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
            W = new Spell.Skillshot(SpellSlot.W, 1050, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
            E = new Spell.Skillshot(SpellSlot.E, 850, EloBuddy.SDK.Enumerations.SkillShotType.Circular);
            R = new Spell.Skillshot(SpellSlot.R, 1500, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
        }

        public static void Initialize()
        {
        }
    }
}
