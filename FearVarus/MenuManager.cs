using Aimtec;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using System.Linq;
namespace Fear_Varus
{
    public static class MenuManager
    {
        public static Menu Root;
        public static Menu Combo;
        public static Menu Harass;
        public static Menu AutoHarass;
        public static Menu AutoHarassWhitelist;
        public static Menu LastHitting;
        public static Menu LastHitting_Q;
        public static Menu Ultimate;
        public static Menu Drawing;

        public static string[] Marksmen =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jhin", "Jinx", "Kalista", "Kindred", "KogMaw",
            "Lucian", "MissFortune", "Quinn", "Sivir", "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Xayah"
        };

        public static void Initialize()
        {
            //Root
            {
                Root = new Menu("Varus", "Rage Varus", true);
                Orbwalker.Implementation.Attach(Root);
            }
            //Combo
            {
                Combo = new Menu("combo", "Combo")
                {
                    new MenuBool("comboQ", "Q"),
                    new MenuBool("comboR", "R")
                };
                Root.Add(Combo);
            }
            //Harass
            {
                Harass = new Menu("harass", "Harass")
                {
                    new MenuBool("harassQ", "Q"),
                    new MenuSlider("harassManaPct", "Mana % >=", 30, 1, 99)
                };
                Root.Add(Harass);
            }
            //Auto Harass
            {
                AutoHarass = new Menu("autoHarass", "Auto Harass")
                {
                    new MenuBool("autoHarassQ", "Q"),
                };
                AutoHarassWhitelist = new Menu("autoHarassWhitelist", "Whitelist");
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                    AutoHarassWhitelist.Add(new MenuBool(enemy.ChampionName, enemy.ChampionName, Marksmen.Contains(enemy.ChampionName)));
                AutoHarass.Add(AutoHarassWhitelist);
                AutoHarass.Add(new MenuSlider("autoHarassManaPct", "Mana % >=", 30, 1, 99));
                Root.Add(AutoHarass);
            }
            //Last Hitting
            {
                LastHitting = new Menu("lastHitting", "Last Hitting");
                LastHitting_Q = new Menu("lastHittingQMenu", "Q")
                {
                    new MenuBool("lastHittingQ", "Enabled"),
                    new MenuBool("lastHittingQOutOfRange", "Only use when out of range"),
                    new MenuBool("lastHittingQWillDie", "Use if cannot kill with auto attack"),
                    new MenuSeperator("lastHittingSep1", "Will Q if minion will die before you can auto attack it.")
                };
                LastHitting.Add(LastHitting_Q);
                LastHitting.Add(new MenuSlider("lastHitManaPct", "Mana % >=", 30, 1, 99));
                Root.Add(LastHitting);
            }
            //Ultimate Settings
            {
                Ultimate = new Menu("ultimate", "Ultimate Settings")
                {
                    new MenuSlider("ultMinRange", "Minimum Range", 1000, 1, 2000),
                    new MenuSeperator("ultSeperator", "Target must be at least this far away to cast ult."),
                    new MenuKeyBind("ultSemi", "Semi-auto cast key", Aimtec.SDK.Util.KeyCode.T, KeybindType.Press)
                };
                Root.Add(Ultimate);
            }
            //Drawing
            {
                Drawing = new Menu("drawing", "Drawing")
                {
                    new MenuBool("drawQ", "Draw Q"),
                    new MenuBool("drawR", "Draw R Minimum Range")
                };
                Root.Add(Drawing);
            }
            //Finally
            {
                Root.Attach();
            }
        }
    }
}
