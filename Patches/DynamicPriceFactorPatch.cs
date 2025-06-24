using HarmonyLib;
using TaleWorlds.CampaignSystem.GameComponents;   // DefaultTradeItemPriceFactorModel
using TaleWorlds.Core;                           // ItemCategory
using System;                                    // MathF
using TaleWorlds.Library;

namespace RealisticEconomy.Patches
{
    /// <summary>
    /// Re-weights the vanilla price formula so short-term stock matters more
    /// (makes grain shortages spike quickly, luxury surpluses crash quickly).
    /// </summary>
    [HarmonyPatch(typeof(DefaultTradeItemPriceFactorModel),
                  nameof(DefaultTradeItemPriceFactorModel.GetBasePriceFactor))]
    internal static class DynamicPriceFactorPatch
    {
        // tuning knobs
        private const float ShortCoeff = 0.30f;   // vanilla 0.04
        private const float LongCoeff = 0.05f;   // vanilla 0.10
        private const float Bias = 2.0f;    // vanilla constant

        // Signature for 1.2.12:
        // float GetBasePriceFactor(ItemCategory itemCategory,
        //                           float inStoreValue,
        //                           float supply,
        //                           float demand,
        //                           bool  isSelling,
        //                           int   transferValue)
        [HarmonyPostfix]
        private static void Postfix(
            ref float __result,                  // vanilla factor we will overwrite
            ItemCategory itemCategory,
            float inStoreValue,                  // short-term stock
            float supply,                        // long-term daily supply
            float demand,
            bool isSelling,
            int transferValue)
        {
            float denom = LongCoeff * supply
                         + ShortCoeff * inStoreValue
                         + Bias;

            float factor = demand / denom;
            factor = MathF.Clamp(factor, 0.5f, 2.0f);   // vanilla bounds

            if (isSelling) factor = 1f / factor;              // invert for sellers
            __result = factor;                                // overwrite vanilla
        }
    }
}

