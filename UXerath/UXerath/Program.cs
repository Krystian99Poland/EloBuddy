﻿using System;
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

namespace UXerath
{
    class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static Menu Menu, SkillMenu, FarmingMenu, MiscMenu, DrawMenu;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Xerath")
                return;

            Bootstrap.Init(null);
            // Skille                // Range skili V
            Q = new Spell.Skillshot(SpellSlot.Q, 1600, SkillShotType.Linear);
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Circular);
            E = new Spell.Skillshot(SpellSlot.E, 1150, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.R, 5600, SkillShotType.Circular);
            // Menu
            Menu = MainMenu.AddMenu("UXerath", "UXerath");
            Menu.AddGroupLabel("UXerath V0.1");

            Menu.AddSeparator();

            Menu.AddLabel("Made by Krystian");
            SkillMenu = Menu.AddSubMenu("Skills", "Skills");
            SkillMenu.AddGroupLabel("Skills");
            SkillMenu.AddLabel("Combo");
            SkillMenu.Add("QCombo", new CheckBox("Use Q on Combo"));
            SkillMenu.Add("WCombo", new CheckBox("Use W on Combo"));
            SkillMenu.Add("ECombo", new CheckBox("Use E on Combo"));
            SkillMenu.Add("RCombo", new CheckBox("Use R on Combo"));

            SkillMenu.AddLabel("Harass");
            SkillMenu.Add("QHarass", new CheckBox("Use Q on Harass"));
            SkillMenu.Add("WHarass", new CheckBox("Use W on Harass"));

            FarmingMenu = Menu.AddSubMenu("Farming", "Farming");
            FarmingMenu.AddGroupLabel("Farming");
            FarmingMenu.AddLabel("LastHit");
            FarmingMenu.Add("Qlasthit", new CheckBox("Use Q on LastHit"));
            FarmingMenu.Add("QlasthitMana", new Slider("Mana % To Use Q", 30, 0, 100));
            FarmingMenu.Add("Wlasthit", new CheckBox("Use W on LastHit"));
            FarmingMenu.Add("WlasthitMana", new Slider("Mana % To Use W", 30, 0, 100));

            FarmingMenu.AddLabel("LaneClear");
            FarmingMenu.Add("QLaneClear", new CheckBox("Use Q on LaneClear"));
            FarmingMenu.Add("QlaneclearMana", new Slider("Mana % To Use Q", 30, 0, 100));
            FarmingMenu.Add("ELaneClear", new CheckBox("Use E on LaneClear"));
            FarmingMenu.Add("ElaneclearMana", new Slider("Mana % To Use E", 30, 0, 100));

            MiscMenu = Menu.AddSubMenu("Misc", "Misc");
            MiscMenu.AddGroupLabel("Misc");
            MiscMenu.AddLabel("KillSteal");
            MiscMenu.Add("Qkill", new CheckBox("Use Q KillSteal"));
            MiscMenu.Add("Wkill", new CheckBox("Use W KillSteal"));
            MiscMenu.Add("Rkill", new CheckBox("Use R KillSteal"));

            DrawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            DrawMenu.AddGroupLabel("Drawings");
            DrawMenu.AddLabel("Drawings");
            DrawMenu.Add("drawQ", new CheckBox("Draw Q"));
            DrawMenu.Add("drawW", new CheckBox("Draw W"));
            DrawMenu.Add("drawE", new CheckBox("Draw E"));
            DrawMenu.Add("drawR", new CheckBox("Draw R"));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Chat.Print("UXerath loaded :)", System.Drawing.Color.White);

        }
        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }
            KillSteal();
        }
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null) return;
            var useQ = SkillMenu["QCombo"].Cast<CheckBox>().CurrentValue;
            var useW = SkillMenu["WCombo"].Cast<CheckBox>().CurrentValue;
            var useE = SkillMenu["ECombo"].Cast<CheckBox>().CurrentValue;
            var useR = SkillMenu["RCombo"].Cast<CheckBox>().CurrentValue;

            if (target.IsValidTarget(Q.Range))
            // kolejność skili
            {
                if (E.IsReady() && useE && target.IsValidTarget(E.Range) && !target.IsZombie)
                {
                    E.Cast(target);
                }
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsZombie)
                {
                    Q.Cast(target);
                }
                if (W.IsReady() && useW && target.IsValidTarget(W.Range) && !target.IsZombie)
                {
                    W.Cast(target);
                }
                if (R.IsReady() && useR && target.IsValidTarget(R.Range) && target.HasBuff("Slow") && !target.IsZombie)
                {
                    R.Cast(target);
                }
            }
        }
        private static void KillSteal()
        // Tutaj robimy Killsteal to samo co w Combo 
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null) return;
            var useQ = MiscMenu["Qkill"].Cast<CheckBox>().CurrentValue;
            var useE = MiscMenu["Ekill"].Cast<CheckBox>().CurrentValue;
            var useR = MiscMenu["Rkill"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range) && !target.IsZombie && target.Health <= _Player.GetSpellDamage(target, SpellSlot.Q))
            {
                Q.Cast(target);
            }
            if (E.IsReady() && useE && target.IsValidTarget(E.Range) && !target.IsZombie && target.Health <= _Player.GetSpellDamage(target, SpellSlot.E))
            {
                W.Cast(target);
            }
            if (R.IsReady() && useR && target.IsValidTarget(R.Range) && !target.IsZombie && target.Health <= _Player.GetSpellDamage(target, SpellSlot.R))
            {
                R.Cast(target);
            }
        }
        private static void Harass()
        // copy past XD
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null) return;
            var useQ = SkillMenu["QHarass"].Cast<CheckBox>().CurrentValue;
            var useW = SkillMenu["WHarass"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range) && !target.IsZombie)
            {
                Q.Cast(target);
            }
            if (W.IsReady() && useW && target.IsValidTarget(W.Range) && !target.IsZombie)
            {
                W.Cast(target);
            }
        }
        private static void LaneClear()
        // :)
        {
            var useQ = FarmingMenu["QLaneClear"].Cast<CheckBox>().CurrentValue;
            var Qmana = FarmingMenu["QlaneclearMana"].Cast<Slider>().CurrentValue;
            var useW = FarmingMenu["WLaneClear"].Cast<CheckBox>().CurrentValue;
            var Wmana = FarmingMenu["WlaneclearMana"].Cast<Slider>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && Player.Instance.ManaPercent > Qmana && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }
                if (useW && W.IsReady() && Player.Instance.ManaPercent > Wmana && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.W))
                {
                    W.Cast(minion);
                }
            }
        }
        private static void LastHit()
        {
            var useQ = FarmingMenu["Qlasthit"].Cast<CheckBox>().CurrentValue;
            var Qmana = FarmingMenu["QlasthitMana"].Cast<Slider>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Yellow, BorderWidth = 1, Radius = Q.Range }.Draw(_Player.Position);
            }
            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Green, BorderWidth = 1, Radius = W.Range }.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Blue, BorderWidth = 1, Radius = E.Range }.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = R.Range }.Draw(_Player.Position);
            }
        }
    }
}