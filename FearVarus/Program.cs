using System;
using System.Drawing;
using System.Linq;
using Aimtec;
using Aimtec.SDK.Damage;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Prediction.Skillshots;
using Aimtec.SDK.TargetSelector;
using static Aimtec.SDK.Events.GameEvents;
using static Aimtec.SDK.Orbwalking.OrbwalkingMode;
using static Fear_Varus.MenuManager;
using static Fear_Varus.SpellManager;

namespace Fear_Varus
{
    internal class Program
    {
        public static Obj_AI_Hero Player;
        private static void Main(string[] args) => GameStart += GameEvents_GameStart;

        public static void GameEvents_GameStart()
        {
            Player = ObjectManager.GetLocalPlayer();
            if (Player.ChampionName != "Varus")
                return;

            MenuManager.Initialize();
            SpellManager.Initialize();
            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.Implementation.OnNonKillableMinion += Implementation_OnNonKillableMinion;
        }  
        private static void Game_OnUpdate()
        {
            if (Ultimate != null && Ultimate["ultSemi"].As<MenuKeyBind>().Enabled)
            {
                SemiCastUlt();
            }

            if (Orbwalker.Implementation.IsWindingUp)
                return;
            switch (Orbwalker.Implementation.Mode)
            {
                case OrbwalkingMode.Combo:
                {
                    Combo();
                    break;
                }
                case Mixed:
                    break;
                case Laneclear:
                case Lasthit:
                    LastHitting();
                    break;
                case None:
                    break;
                case Freeze:
                    break;
                case Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void Combo()
        {
            if (MenuManager.Combo["comboR"].As<MenuBool>().Enabled && R.Ready)
            {

                var target = TargetSelector.GetOrderedTargets(Ultimate["ultMinRange"].As<MenuSlider>().Value).FirstOrDefault(x => Player.GetSpellDamage(x, SpellSlot.R) >= x.Health);
                if (target.IsValidTarget())
                {
                    var pred = R.GetPrediction(target);
                    var collisions = pred.CollisionObjects.Count > 7 ? 7 : pred.CollisionObjects.Count; //Can't have the damage reduced by more than 70%.
                    var ultDamage = Player.GetSpellDamage(target, SpellSlot.R);
                    if (collisions > 0)
                    {
                        float multiplier = collisions / 10;
                        ultDamage = ultDamage - (ultDamage * multiplier);
                    }
                    if (ultDamage >= target.Health)
                        R.Cast(target);
                }
            }
            if (MenuManager.Combo["comboQ"].As<MenuBool>().Enabled && Q.Ready)
            {
                var target = TargetSelector.GetTarget(Q.ChargedMaxRange);
                Q.GetPrediction(target);     
                if (target.IsValidTarget())
                    Q.Cast(target);
            }
            if (MenuManager.Combo["comboE"].As<MenuBool>().Enabled && E.Ready)
            {
                var target = TargetSelector.GetTarget(E.Range);
                var pred = E.GetPrediction(target);
                if (target.IsValidTarget())
                    E.Cast(target);
            }
            {
            }
        }

        private static void OrbwalkerPreAttack(object sender, PreAttackEventArgs e)
        {
            if ((Obj_AI_Hero) sender != ObjectManager.GetLocalPlayer())

                return;

            if (Q.IsCharging)
            {
                e.Cancel = true;
            }
        }

        private static void LastHitting()
        {
            var minManaPct = MenuManager.LastHitting["lastHitManaPct"].As<MenuSlider>().Value;
            if (LastHitting_Q["lastHittingQ"].As<MenuBool>().Enabled && Q.Ready && Player.ManaPercent() >= minManaPct)
            {
                var onlyOutOfRange = LastHitting_Q["lastHittingQOutOfRange"].As<MenuBool>().Enabled;
                var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsInRange(Q.Range) && x.IsEnemy && Player.GetSpellDamage(x, SpellSlot.Q) >= x.Health && x.IsValidTarget()).OrderBy(x => x.Health);
                var target = onlyOutOfRange ? minions.FirstOrDefault(x => !x.IsInRange(Player.AttackRange)) : minions.FirstOrDefault();
                if (target.IsValidTarget())
                    Q.Cast(target);
            }
        }

        private static void SemiCastUlt()
        {
            if (R.Ready)
            {
                var target = TargetSelector.GetTarget(Ultimate["ultMinRange"].As<MenuSlider>().Value);
                if (target.IsValidTarget())
                    R.Cast(target);
            }
        }

        private static void Implementation_OnNonKillableMinion(object sender, NonKillableMinionEventArgs e)
        {
            var minion = e.Target as Obj_AI_Minion;
            if (minion == null)
                return;
            switch (Orbwalker.Implementation.Mode)
            {
                case Lasthit:
                case Laneclear:
                    var minManaPct = MenuManager.LastHitting["lastHitManaPct"].As<MenuSlider>().Value;
                    if (LastHitting_Q["lastHittingQWillDie"].As<MenuBool>().Enabled && Q.Ready && Player.GetSpellDamage(minion, SpellSlot.Q) >= minion.Health && Player.ManaPercent() >= minManaPct)
                        Q.Cast(minion);
                    break;
            }
        }

        private static void Render_OnPresent()
        {
            if (Drawing["drawQ"].As<MenuBool>().Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);
            {
                var ultRange = Ultimate["ultMinRange"].As<MenuSlider>().Value;
                Render.Circle(Player.Position, ultRange, 30, Color.Red);
            }
        }


    }
}
