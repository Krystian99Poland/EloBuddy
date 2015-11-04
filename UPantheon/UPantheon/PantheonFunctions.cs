using SharpDX;
using EloBuddy;
using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace UPantheon
{
    public static class PantheonFunctions
    {
        public enum AttackSpell
        {
            Q,
            W,
            E,
            Ignite
        };

        public static Obj_AI_Base GetEnemy(float range, GameObjectType type)
        {
            return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health).Where(a => a.IsEnemy
            && a.Type == type
            && a.Distance(Pantheon) <= range
            && !a.IsDead
            && !a.IsInvulnerable
            && a.IsValidTarget(range)).FirstOrDefault();
        }

        public static Obj_AI_Base GetEnemy(GameObjectType type, AttackSpell spell)
        {
            //KS
            if (spell == AttackSpell.W)//w
            {
                return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health).Where(a => a.IsEnemy
                && a.Type == type
                && a.Distance(Pantheon) <= Program.W.Range
                && !a.IsDead
                && !a.IsInvulnerable
                && a.IsValidTarget(Program.W.Range)
                && a.Health <= PantheonCalcs.W(a)).FirstOrDefault();
            }
            else if (spell == AttackSpell.Q)//q
            {
                return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health).Where(a => a.IsEnemy
                && a.Type == type
                && a.Distance(Pantheon) <= Program.Q.Range
                && !a.IsDead
                && !a.IsInvulnerable
                && a.IsValidTarget(Program.Q.Range)
                && a.Health <= PantheonCalcs.Q(a)).FirstOrDefault();
            }
            else if (spell == AttackSpell.E)//r
            {
                return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health).Where(a => a.IsEnemy
                && a.Type == type
                && a.Distance(Pantheon) <= Program.E.Range
                && !a.IsDead
                && !a.IsInvulnerable
                && a.IsValidTarget(Program.E.Range)
                && a.Health <= PantheonCalcs.E(a)).FirstOrDefault();
            }
            else//ignite
            {
                return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health).Where(a => a.IsEnemy
                && a.Type == type
                && a.Distance(Pantheon) <= Program.Ignite.Range
                && !a.IsDead
                && !a.IsInvulnerable
                && a.IsValidTarget(Program.Ignite.Range)
                && a.Health <= PantheonCalcs.Ignite(a)).FirstOrDefault();
            }
        }

        public static AIHeroClient Pantheon { get { return ObjectManager.Player; } }

        public static void LastHit()
        {
            bool QCHECK = Program.LastHit["LHQ"].Cast<CheckBox>().CurrentValue;
            bool QREADY = Program.Q.IsReady();

            if (QCHECK && QREADY)
            {
                Obj_AI_Minion enemy = (Obj_AI_Minion)GetEnemy(GameObjectType.obj_AI_Minion, AttackSpell.Q);

                if (enemy != null)
                    Program.Q.Cast(enemy);
            }
        }

        public static void KillSteal()
        {
            bool QCHECK = Program.Killsteal["KSQ"].Cast<CheckBox>().CurrentValue;
            bool ECHECK = Program.Killsteal["KSE"].Cast<CheckBox>().CurrentValue;
            bool ICHECK = Program.Killsteal["KSI"].Cast<CheckBox>().CurrentValue;
            bool QREADY = Program.Q.IsReady();
            bool WREADY = Program.W.IsReady();
            bool EREADY = Program.E.IsReady();
            bool IREADY = false;
            if (Program.Ignite != null && Program.Ignite.IsReady())
                IREADY = true;

            if (ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy && a.Distance(Pantheon) <= Program.E.Range).OrderBy(a => a.Health).FirstOrDefault() != null)
            {
                if (QCHECK && QREADY)
                {
                    AIHeroClient enemy = (AIHeroClient)GetEnemy(GameObjectType.AIHeroClient, AttackSpell.Q);

                    if (enemy != null)
                        Program.Q.Cast(enemy);
                }
                if (WREADY)
                {
                    AIHeroClient enemy = (AIHeroClient)GetEnemy(GameObjectType.AIHeroClient, AttackSpell.W);

                    if (enemy != null)
                        Program.W.Cast(enemy.Position);
                }
                if (ECHECK && EREADY)
                {
                    AIHeroClient enemy = (AIHeroClient)GetEnemy(GameObjectType.AIHeroClient, AttackSpell.E);

                    if (enemy != null)
                        Program.E.Cast(enemy.Position);
                }

                if (ICHECK && IREADY)
                {
                    AIHeroClient enemy = (AIHeroClient)GetEnemy(GameObjectType.AIHeroClient, AttackSpell.Ignite);

                    if (enemy != null)
                        Program.Ignite.Cast(enemy);
                }
            }
        }
        public static void Harrass()
        {
            bool QCHECK = Program.Harass["HQ"].Cast<CheckBox>().CurrentValue;
            bool QREADY = Program.Q.IsReady();

            if (QCHECK && QREADY)
            {
                AIHeroClient enemy = (AIHeroClient)GetEnemy(Program.Q.Range, GameObjectType.AIHeroClient);

                if (enemy != null)
                    Program.Q.Cast(enemy);
            }
            if (Orbwalker.CanAutoAttack)
            {
                AIHeroClient enemy = (AIHeroClient)GetEnemy(Pantheon.GetAutoAttackRange(), GameObjectType.AIHeroClient);

                if (enemy != null)
                    Orbwalker.ForcedTarget = enemy;
            }
        }

        public static void Combo()
        {
            bool QCHECK = Program.ComboMenu["QU"].Cast<CheckBox>().CurrentValue;
            bool WCHECK = Program.ComboMenu["WU"].Cast<CheckBox>().CurrentValue;
            bool ECHECK = Program.ComboMenu["EU"].Cast<CheckBox>().CurrentValue;
            bool ItemsCHECK = Program.ComboMenu["IU"].Cast<CheckBox>().CurrentValue;
            bool IgniteCHECK = Program.ComboMenu["IgU"].Cast<CheckBox>().CurrentValue;
            bool QREADY = Program.Q.IsReady();
            bool WREADY = Program.W.IsReady();
            bool EREADY = Program.E.IsReady();


            if (QCHECK && QREADY)
            {
                AIHeroClient enemy = (AIHeroClient)GetEnemy(Program.Q.Range, GameObjectType.AIHeroClient);

                if (enemy != null)
                    Program.Q.Cast(enemy);
            }

            if (WCHECK && WREADY)
            {
                AIHeroClient enemy = (AIHeroClient)GetEnemy(Program.W.Range, GameObjectType.AIHeroClient);

                if (enemy != null)
                    Program.W.Cast(enemy);
            }

            if (ECHECK && EREADY)
            {
                AIHeroClient enemy = (AIHeroClient)GetEnemy(Program.E.Range, GameObjectType.AIHeroClient);

                if (enemy != null)
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                Program.E.Cast(enemy.Position);
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }

            if (ItemsCHECK)
            {
                AIHeroClient enemy = (AIHeroClient)GetEnemy(2500, GameObjectType.AIHeroClient);

                if (enemy != null)
                    UseItems();
            }

            if (IgniteCHECK && Program.Ignite != null && Program.Ignite.IsReady())
            {
                AIHeroClient enemy = (AIHeroClient)GetEnemy(Program.Ignite.Range, GameObjectType.AIHeroClient);

                if (enemy != null)
                    Program.Ignite.Cast(enemy);
            }

            if (Orbwalker.CanAutoAttack)
            {
                AIHeroClient enemy = (AIHeroClient)GetEnemy(Pantheon.GetAutoAttackRange(), GameObjectType.AIHeroClient);

                if (enemy != null)
                    Orbwalker.ForcedTarget = enemy;
            }
        }

        public static void UseItems()
        {
            InventorySlot[] items = Pantheon.InventoryItems;

            foreach (InventorySlot item in items)
            {
                if (item.CanUseItem())
                {
                    if (item.Id == ItemId.Health_Potion
                        && Pantheon.Health <= (Pantheon.MaxHealth * 0.45)
                        && !Pantheon.IsRecalling()
                        && Pantheon.CountEnemiesInRange(2000) <= 1
                        && !Pantheon.IsInShopRange()
                        && !Pantheon.HasBuff("RegenerationPotion"))
                    {
                        item.Cast();
                    }
                    if (item.Id == ItemId.Mana_Potion
                        && Pantheon.Mana <= (Pantheon.MaxMana * 0.45)
                        && !Pantheon.IsRecalling()
                        && Pantheon.CountEnemiesInRange(2000) <= 1
                        && !Pantheon.IsInShopRange()
                        && !Pantheon.HasBuff("FlaskOfCrystalWater"))
                    {
                        item.Cast();
                    }
                    if (item.Id == ItemId.Crystalline_Flask
                        && (Pantheon.Health <= (Pantheon.MaxHealth * 0.45) || Pantheon.Mana <= (Pantheon.MaxMana * 0.45))
                        && !Pantheon.IsRecalling()
                        && Pantheon.CountEnemiesInRange(2000) <= 1
                        && !Pantheon.IsInShopRange()
                        && !Pantheon.HasBuff("ItemCrystalFlask"))
                    {
                        item.Cast();
                    }
                }
            }
        }
    }
}