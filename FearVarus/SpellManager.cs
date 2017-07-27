using Aimtec;
using Aimtec.SDK.Prediction.Skillshots;
using Spell = Aimtec.SDK.Spell;

namespace Fear_Varus
{
    public static class SpellManager
    {
        public static Spell Q, E, R;
        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 1150);
            R = new Spell(SpellSlot.R, float.MaxValue);

            Q.SetSkillshot(0.25f, 60, 2000, true, SkillshotType.Line);
            Q.SetCharged("VarusQ", "VarusQ", 750, 1550, 1.5f);
            E.SetSkillshot(0.25f, 60, 1000, true, SkillshotType.Circle);
            R.SetSkillshot(1, 160, 1300, false, SkillshotType.Line);
        }
    }
}
