using System.Collections.Generic;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace RedHeartsFirst
{
    [HarmonyPatch]
    internal static class HeartPatches
    {
        public static HeartOrder Hearts;

        static bool skipPatch;

        enum UsingHearts // I haven't really used this enum 
        {
            Red,
            Blue,
            Black,
        }

        [HarmonyPatch(typeof(HealthPlayer), nameof(HealthPlayer.DealDamage))]
        [HarmonyPrefix]
        static void DealDamagePrefix(HealthPlayer __instance, ref Dictionary<string, float> __state)
        {
            bool hasSpecialHearts = __instance.BlackHearts > 0f || __instance.BlueHearts > 0f || __instance.SpiritHearts > 0f;

            skipPatch = !hasSpecialHearts || Hearts == HeartOrder.Off;
            if (skipPatch) return;


            // I was gonna account for the possibility of player death, but honestly... If they ARE going to die from this then it doesn't really matter? So yay!
            float HPSum = __instance.BlackHearts + __instance.BlueHearts + __instance.SpiritHearts + __instance.HP;

            __state = new Dictionary<string, float>()
            {
                { "Black", __instance.BlackHearts },
                { "Blue", __instance.BlueHearts },
                { "Spirit", __instance.SpiritHearts },
                { "Red", __instance.HP },
                { "HPSum", HPSum },
            };

            // Because Black Hearts do DAMAGE, I'll have to add some TEMPORARY padding of Blue Hearts to cover for the Black Hearts. Trust me on this.
            float temp = __instance.BlackHearts;
            __instance.BlackHearts = 0f;
            __instance.BlueHearts += temp;
        }


        [HarmonyPatch(typeof(HealthPlayer), nameof(HealthPlayer.DealDamage))]
        [HarmonyPostfix]
        static void DealDamagePostfix(HealthPlayer __instance, ref Dictionary<string, float> __state, ref bool __result)
        {
            if (skipPatch) return;

            float newHPsum = __instance.BlackHearts + __instance.BlueHearts + __instance.SpiritHearts + __instance.HP;

            float realDamage = __state["HPSum"] - newHPsum;

            bool damageBlackheart = false;

            // Heart order:
            switch (Hearts)
            {
                case HeartOrder.BlackRedBlue:
                    {
                        damageBlackheart = __state["Black"] > 0f && realDamage > 0f;
                        __instance.BlackHearts = HeartMath(__state["Black"], ref realDamage);
                        __instance.SpiritHearts = HeartMath(__state["Spirit"], ref realDamage);
                        __instance.HP = HeartMath(__state["Red"], ref realDamage);
                        __instance.BlueHearts = HeartMath(__state["Blue"], ref realDamage);
                    }
                    break;
                case HeartOrder.RedBlackBlue:
                    {
                        __instance.SpiritHearts = HeartMath(__state["Spirit"], ref realDamage);
                        __instance.HP = HeartMath(__state["Red"], ref realDamage);
                        damageBlackheart = __state["Black"] > 0f && realDamage > 0f;
                        __instance.BlackHearts = HeartMath(__state["Black"], ref realDamage);
                        __instance.BlueHearts = HeartMath(__state["Blue"], ref realDamage);
                    }
                    break;
                case HeartOrder.BlueRedBlack:
                    {
                        __instance.BlueHearts = HeartMath(__state["Blue"], ref realDamage);
                        __instance.SpiritHearts = HeartMath(__state["Spirit"], ref realDamage);
                        __instance.HP = HeartMath(__state["Red"], ref realDamage);
                        damageBlackheart = __state["Black"] > 0f && realDamage > 0f;
                        __instance.BlackHearts = HeartMath(__state["Black"], ref realDamage);
                    }
                    break;
                default:
                    {
                        Plugin.myLogger.LogError("Unexpected behavior on: DealDamagePrefix");
                        SaveFile.SaveData = HeartOrder.BlackRedBlue;
                        Plugin.myLogger.LogWarning("Heart order has defaulted to: Black, Red, Blue.");
                    }
                    break;
            }

            if (damageBlackheart)
            {
                // Private method. I decided to rewrite it instead.

                __instance.StartCoroutine(DamageAllEnemiesIE_MethodRewrite(__instance, 1.25f + DataManager.GetWeaponDamageMultiplier(DataManager.Instance.CurrentWeaponLevel), Health.DamageAllEnemiesType.BlackHeart));
            }
        }

        static float HeartMath(float heartHP, ref float damage)
        {
            // Return value = Heart value.

            if(damage <= 0)
            {
                return Mathf.Round(heartHP);
            }

            float damageTemp = damage;

            if(heartHP - damageTemp >= 0)
            {
                damage = 0f;
                return Mathf.Round(heartHP - damageTemp);
            }
            else
            {
                damage = Mathf.Round(damage - heartHP);
                return 0f;
            }
        }


        [HarmonyPatch(typeof(HUD_Heart), "Scale", MethodType.Setter)]
        [HarmonyPrefix]
        static bool FixScale(HUD_Heart __instance, ref float ____Scale)
        {
            bool flag = __instance.MyHeartType != HUD_Heart.HeartType.Black;
            if (flag) return true;

            ____Scale = 1f;
            return false;
        }


        [HarmonyPatch(typeof(HUD_Hearts), nameof(HUD_Hearts.UpdateHearts))]
        [HarmonyPrefix]
        static bool SkipUpdate(HUD_Hearts __instance, ref HealthPlayer health, ref bool DoEffects)
        {
            if (Hearts == HeartOrder.Off) return true;

            UpdateHearts_Rewrite(__instance, health, DoEffects);
            return false;
        }


        // PRIVATE METHOD REWRITINGS:
        static IEnumerator DamageAllEnemiesIE_MethodRewrite(HealthPlayer instance, float damage, Health.DamageAllEnemiesType damageType)
        {
            foreach (Health health in new List<Health>(Health.team2))
            {
                if (!(health == null) && !health.GetComponentInParent<Projectile>())
                {
                    if (damageType == Health.DamageAllEnemiesType.BlackHeart)
                    {
                        BiomeConstants.Instance.ShowBlackHeartDamage(health.transform, Vector3.up);
                    }
                    else if (damageType == Health.DamageAllEnemiesType.DeathsDoor)
                    {
                        BiomeConstants.Instance.ShowTarotCardDamage(health.transform, Vector3.up);
                    }
                }
            }
            yield return new WaitForSeconds(1f);
            using (List<Health>.Enumerator enumerator = new List<Health>(Health.team2).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Health health2 = enumerator.Current;
                    if (health2 != null)
                    {
                        health2.DealDamage(damage, instance.gameObject, instance.transform.position, false, Health.AttackTypes.NoKnockBack, false, (Health.AttackFlags)0);
                    }
                }
                yield break;
            }
        }



        private static void UpdateHearts_Rewrite (HUD_Hearts instance, HealthPlayer health, bool DoEffects)
        {
            int num = -1;
            int red = (int)health.HP;
            int redTotal = (int)DataManager.Instance.PLAYER_TOTAL_HEALTH;
            int spirit = (int)health.SpiritHearts;
            int spiritTotal = (int)health.TotalSpiritHearts;
            int blue = (int)health.BlueHearts;
            int black = (int)health.BlackHearts;

            while (++num < instance.HeartIcons.Count)
            {
                HUD_Heart hud_Heart = instance.HeartIcons[num];
                if (Mathf.Ceil(DataManager.Instance.PLAYER_TOTAL_HEALTH / 2f) + Mathf.Ceil(health.TotalSpiritHearts / 2f) + Mathf.Ceil(DataManager.Instance.PLAYER_BLUE_HEARTS / 2f) + Mathf.Ceil(DataManager.Instance.PLAYER_BLACK_HEARTS / 2f) <= (float)num)
                {
                    if (hud_Heart.MyState == HUD_Heart.HeartState.HeartHalf && hud_Heart.MyHeartType == HUD_Heart.HeartType.Blue && false) // the "false" is so this is skipped altogether but I can still come back to it and change it easily. yes. i know this is scuffed and unintuitive. yes. i know.
                    {
                        hud_Heart.Activate(false, true);
                    }
                    else if ((hud_Heart.MyHeartType == HUD_Heart.HeartType.Blue && blue <= 0) || (hud_Heart.MyHeartType == HUD_Heart.HeartType.Black && black <= 0) || (hud_Heart.MyHeartType == HUD_Heart.HeartType.Red && red <= 0) || (hud_Heart.MyHeartType == HUD_Heart.HeartType.Spirit && spirit <= 0))
                    {
                        hud_Heart.Activate(false, false);
                    }
                }
                else
                {
                    hud_Heart.Activate(true, false);
                    if (redTotal >= 1)
                    {
                        if (red >= 2)
                        {
                            hud_Heart.SetSprite(HUD_Heart.HeartState.HeartFull, DoEffects, HUD_Heart.HeartType.Red);
                            red -= 2;
                        }
                        else if (red == 1)
                        {
                            if (redTotal >= 2)
                            {
                                hud_Heart.SetSprite(HUD_Heart.HeartState.HeartHalfFull, DoEffects, HUD_Heart.HeartType.Red);
                            }
                            else
                            {
                                hud_Heart.SetSprite(HUD_Heart.HeartState.HeartHalf, DoEffects, HUD_Heart.HeartType.Red);
                            }
                            red--;
                        }
                        else if (redTotal == 1)
                        {
                            hud_Heart.SetSprite(HUD_Heart.HeartState.HalfHeartContainer, DoEffects, HUD_Heart.HeartType.Red);
                        }
                        else
                        {
                            hud_Heart.SetSprite(HUD_Heart.HeartState.HeartContainer, DoEffects, HUD_Heart.HeartType.Red);
                        }
                        redTotal -= 2;
                    }
                    else if (spiritTotal >= 1)
                    {
                        if (spirit >= 2)
                        {
                            hud_Heart.SetSprite(HUD_Heart.HeartState.HeartFull, DoEffects, HUD_Heart.HeartType.Spirit);
                            spirit -= 2;
                        }
                        else if (spirit == 1)
                        {
                            hud_Heart.SetSprite(HUD_Heart.HeartState.HeartHalf, DoEffects, HUD_Heart.HeartType.Spirit);
                            spirit--;
                        }
                        else if (spiritTotal == 1)
                        {
                            hud_Heart.SetSprite(HUD_Heart.HeartState.HalfHeartContainer, DoEffects, HUD_Heart.HeartType.Spirit);
                        }
                        else
                        {
                            hud_Heart.SetSprite(HUD_Heart.HeartState.HeartContainer, DoEffects, HUD_Heart.HeartType.Spirit);
                        }
                        spiritTotal -= 2;
                    }
                    else if (blue > 0 || black > 0)
                    {
                        /*num >= Mathf.Ceil(DataManager.Instance.PLAYER_TOTAL_HEALTH / 2f) + Mathf.Ceil(health.TotalSpiritHearts / 2f) || true*/

                        if (blue >= 2)
                        {
                            hud_Heart.SetSprite(HUD_Heart.HeartState.HeartFull, DoEffects, HUD_Heart.HeartType.Blue);
                            blue -= 2;
                        }
                        else if (blue == 1)
                        {
                            hud_Heart.SetSprite(HUD_Heart.HeartState.HeartHalf, DoEffects, HUD_Heart.HeartType.Blue);
                            blue--;
                        }
                        else if (black >= 2)
                        {
                            hud_Heart.SetSprite(HUD_Heart.HeartState.HeartFull, DoEffects, HUD_Heart.HeartType.Black);
                            black -= 2;
                        }
                        else if (black == 1)
                        {
                            hud_Heart.SetSprite(HUD_Heart.HeartState.HeartHalf, DoEffects, HUD_Heart.HeartType.Black);
                            black--;
                        }
                    }
                }
            }
        }
    }
}
