using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;
using SharpDX;

namespace UPantheon
{
    internal class Program
    {
        public static Menu ComboMenu, DrawingsMenu, SettingsMenu, LastHit, Killsteal, Harass, menu;
        public static Spell.Targeted Q;
        public static Spell.Targeted W;
        public static Spell.Skillshot E;
        public static Spell.Targeted Ignite;
        public static AIHeroClient _Player { get { return ObjectManager.Player; } }
        public static int Mana { get { return (int)_Player.Mana; } }

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Pantheon")
                return;

            Bootstrap.Init(null);

            Q = new Spell.Targeted(SpellSlot.Q, 600);
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Skillshot(SpellSlot.E, 400, SkillShotType.Cone);

            menu = MainMenu.AddMenu("UPantheon", "UPantheon");
            menu.AddGroupLabel("Good luck and Have Fun");


            ComboMenu = menu.AddSubMenu("Combo", "combomenu");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("QU", new CheckBox("Use Q"));
            ComboMenu.Add("WU", new CheckBox("Use W"));
            ComboMenu.Add("EU", new CheckBox("Use E"));
            ComboMenu.Add("IU", new CheckBox("Use Items"));
            ComboMenu.Add("IgU", new CheckBox("Use Ignite"));

            Harass = menu.AddSubMenu("Harass", "harass");
            Harass.AddGroupLabel("Harass Settings");
            Harass.Add("HQ", new CheckBox("Use Q"));

            LastHit = menu.AddSubMenu("Last Hit", "lasthitmenu");
            LastHit.AddGroupLabel("Last Hit Settings");
            LastHit.Add("LHQ", new CheckBox("Use Q"));

            Killsteal = menu.AddSubMenu("Killsteal", "killstealmenu");
            Killsteal.AddGroupLabel("Killsteal Settings");
            Killsteal.Add("KSER", new CheckBox("Activate Killsteal"));
            Killsteal.Add("KSQ", new CheckBox("Use Q"));
            Killsteal.Add("KSI", new CheckBox("Use Ignite"));

            DrawingsMenu = menu.AddSubMenu("Drawings", "drawingsmenu");
            DrawingsMenu.AddGroupLabel("Drawings Settings");
            DrawingsMenu.Add("DQ", new CheckBox("Draw Q/E"));
   //         DrawingsMenu.Add("DR", new CheckBox("Draw R"));

            SettingsMenu = menu.AddSubMenu("Settings", "settingsmenu");
            SettingsMenu.Add("SHM", new CheckBox("Auto-Use Mana and Health Potions"));

            SpellDataInst Sum1 = _Player.Spellbook.GetSpell(SpellSlot.Summoner1);
            SpellDataInst Sum2 = _Player.Spellbook.GetSpell(SpellSlot.Summoner2);
            if (Sum1.Name == "summonerdot")
                Ignite = new Spell.Targeted(SpellSlot.Summoner1, 600);
            else if (Sum2.Name == "summonerdot")
                Ignite = new Spell.Targeted(SpellSlot.Summoner2, 600);

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Program.DrawingsMenu["DQ"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, Q.Range, System.Drawing.Color.BlueViolet);
            }

        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                PantheonFunctions.Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                PantheonFunctions.LastHit();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                PantheonFunctions.Harrass();
            }
            if (Program.Killsteal["KSER"].Cast<CheckBox>().CurrentValue)
            {
                PantheonFunctions.KillSteal();
            }
            if (Program.SettingsMenu["SHM"].Cast<CheckBox>().CurrentValue)
            {
                PantheonFunctions.UseItems();
            }
        }
    }
}
