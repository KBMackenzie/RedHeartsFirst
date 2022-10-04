﻿using System;
using System.Collections.Generic;
using HarmonyLib;
using BepInEx;
using Lamb;
using System.Collections;
using UnityEngine;

namespace RedHeartsFirst
{
    [HarmonyPatch]
    internal static class HeartPatches
    {
        /* My heart order:
         * 1. Red   2. Spirit   3. Black   4. Blue
         * I might change this later, I don't know yet. */

        public static HeartState Hearts => SaveFile.SaveData;

        static bool skipPatch;

        [HarmonyPatch(typeof(HealthPlayer), nameof(HealthPlayer.DealDamage))]
        [HarmonyPrefix]
        static void DealDamagePrefix(HealthPlayer __instance, ref Dictionary<string, float> __state)
        {
            bool hasSpecialHearts = __instance.BlackHearts > 0f || __instance.BlueHearts > 0f || __instance.SpiritHearts > 0f;

            skipPatch = !hasSpecialHearts || Hearts == HeartState.Off;
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

            // FileLog.Log("Prefix ran. Red Hearts: " + __instance.HP);

            // Because Black Hearts do DAMAGE, I'll have to add some TEMPORARY padding of Blue Hearts to cover for the Black Hearts. Trust me on this.
            float temp = __instance.BlackHearts;
            __instance.BlackHearts = 0f;
            __instance.BlueHearts += temp;
        }


        [HarmonyPatch(typeof(HealthPlayer), nameof(HealthPlayer.DealDamage))]
        [HarmonyPostfix]
        static void DealDamagePostfix(HealthPlayer __instance, ref Dictionary<string, float> __state)
        {
            if (skipPatch) return;

            float newHPsum = __instance.BlackHearts + __instance.BlueHearts + __instance.SpiritHearts + __instance.HP;

            float realDamage = __state["HPSum"] - newHPsum;

            bool damageBlackheart = false;

            // Heart order:
            switch (Hearts)
            {
                case HeartState.BlackRedBlue:
                    {
                        damageBlackheart = __state["Black"] > 0f && realDamage > 0f;
                        __instance.BlackHearts = HeartMath(__state["Black"], ref realDamage);
                        __instance.SpiritHearts = HeartMath(__state["Spirit"], ref realDamage);
                        __instance.HP = HeartMath(__state["Red"], ref realDamage);
                        __instance.BlueHearts = HeartMath(__state["Blue"], ref realDamage);
                    }
                    break;
                case HeartState.RedBlackBlue:
                    {
                        __instance.SpiritHearts = HeartMath(__state["Spirit"], ref realDamage);
                        __instance.HP = HeartMath(__state["Red"], ref realDamage);
                        damageBlackheart = __state["Black"] > 0f && realDamage > 0f;
                        __instance.BlackHearts = HeartMath(__state["Black"], ref realDamage);
                        __instance.BlueHearts = HeartMath(__state["Blue"], ref realDamage);
                    }
                    break;
                case HeartState.BlueRedBlack:
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
                        SaveFile.SaveData = HeartState.BlackRedBlue;
                        Plugin.myLogger.LogWarning("Heart order has defaulted to: Black, Red, Blue.");
                    }
                    break;
            }

            if (damageBlackheart)
            {
                // Private method. Hahalol. :'3 I have to rewrite it. Hm.

                __instance.StartCoroutine(DamageAllEnemiesIE_MethodRewrite(__instance, 1.25f + DataManager.GetWeaponDamageMultiplier(DataManager.Instance.CurrentWeaponLevel), Health.DamageAllEnemiesType.BlackHeart));
            }

            // FileLog.Log("Postfix ran. Red Hearts: " + __instance.HP);
        }

        static float HeartMath(float heartHP, ref float damage) // bool isRed = false
        {
            // Return value = Heart value.

            if(damage <= 0)
            {
                return heartHP;
            }

            float minimum = 0f; // isRed ? 1f : 0f; // Ignore this
            float damageTemp = damage;

            if(heartHP - damageTemp >= minimum)
            {
                damage = 0f;
                return heartHP - damageTemp;
            }
            else
            {
                damage = damage - (heartHP - minimum);
                return minimum; // Ignore this, it's always just 0f.

                // 'minimum' exists because i was considering making the minimum 1f for red hearts
                // Not anymore though!
            }
        }


        // Rewriting the HealthPlayer.DamageAllEnemiesIE method because it's private, LOL. :'D
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
            // yield break;
        }
    }
}