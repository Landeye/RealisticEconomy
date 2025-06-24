using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;      // Town
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using RealisticEconomy.Models;                    // FileLogger
using System;
using TaleWorlds.Library;

namespace RealisticEconomy.Behaviors
{
    /// <summary>
    /// Deducts prosperity each day a town’s food stock is below zero.
    /// Scale: −1.0 prosperity per −50 food.
    /// </summary>
    public class StarvationPenaltyBehavior : CampaignBehaviorBase
    {
        private const float ProsperityLossPerFood = 1f / 50f;   // tweak here

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickSettlementEvent
                          .AddNonSerializedListener(this, OnDailySettlementTick);
        }

        private void OnDailySettlementTick(Settlement settlement)
        {
            if (settlement?.Town == null) return;
            Town town = settlement.Town;

            // Current daily food calculation (vanilla model)
            var foodModel = Campaign.Current.Models.SettlementFoodModel;
            ExplainedNumber fn = foodModel.CalculateTownFoodStocksChange(
                                     town, includeMarketStocks: true, false);

            int currentFood = (int)fn.BaseNumber;    // snapshot AFTER daily change
            if (currentFood >= 0) return;             // no penalty if stocked

            float loss = MathF.Abs(currentFood) * ProsperityLossPerFood;
            float newPros = MathF.Max(0f, town.Prosperity - loss);
            town.Prosperity = newPros;

            FileLogger.Log($"[Starve] {town.Name}: food={currentFood}, " +
                           $"−Pros={loss:F1} → {newPros:F0}");
        }

        public override void SyncData(IDataStore store) { }
    }
}
