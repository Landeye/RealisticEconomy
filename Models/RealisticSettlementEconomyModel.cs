using TaleWorlds.CampaignSystem.Settlements;       // Settlement, Town
using TaleWorlds.CampaignSystem.ComponentInterfaces; // SettlementEconomyModel
using TaleWorlds.Core;                              // InformationManager
using TaleWorlds.Library;                           // InformationMessage
using RealisticEconomy.Models;                      // FileLogger

namespace RealisticEconomy.Models
{
    public class RealisticSettlementEconomyModel : SettlementEconomyModel
    {
        private static bool _initialized;
        private readonly SettlementEconomyModel _defaultModel
            = Campaign.Current.Models.GetModel<SettlementEconomyModel>();

        // **1) THIS** is the method Bannerlord actually calls every map‐tick
        public override ExplainedNumber CalculateGoldChangeForSettlement(
            Settlement settlement, bool includeDescriptions = false)
        {
            if (!_initialized)
            {
                FileLogger.Clear();
                InformationManager.DisplayMessage(
                    new InformationMessage("[RealEco] Logger initialized and override hooked")
                );
                _initialized = true;
            }

            var result = _defaultModel.CalculateGoldChangeForSettlement(settlement, includeDescriptions);
            int delta = (int)result.ResultNumber;

            // console pop‐up
            InformationManager.DisplayMessage(
                new InformationMessage($"[RealEco] {settlement.Name} Δgold = {delta}")
            );

            // file‐log: include prosperity, daily change, maybe food stocks etc.
            FileLogger.Log(
                $"{settlement.Name}, " +
                $"Prosperity={(int)settlement.Prosperity}, " +
                $"Δgold={delta}, " +
                $"Food={(int)settlement.TownMarketData.FoodStocks}"
            );

            return result;
        }

        // stub out the rest (so your class compiles)
        public override float GetDemandChangeFromValue(float purchaseValue)
            => _defaultModel.GetDemandChangeFromValue(purchaseValue);

        public override (float, float) GetSupplyDemandForCategory(
            Town town, ItemCategory category,
            float dailySupply, float dailyDemand,
            float oldSupply, float oldDemand)
            => _defaultModel.GetSupplyDemandForCategory(
                   town, category,
                   dailySupply, dailyDemand,
                   oldSupply, oldDemand
               );

        public override float GetEstimatedDemandForCategory(
            Town town, ItemData itemData, ItemCategory category)
            => _defaultModel.GetEstimatedDemandForCategory(town, itemData, category);

        public override float GetDailyDemandForCategory(
            Town town, ItemCategory category, int extraProsperity = 0)
            => _defaultModel.GetDailyDemandForCategory(town, category, extraProsperity);
    }
}
