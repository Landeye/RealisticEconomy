using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using RealisticEconomy.Behaviors;   // behaviours
using RealisticEconomy;             // EconomyManager
using TaleWorlds.Library;
using RealisticEconomy.Models;

namespace RealisticEconomy
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            // 1) Apply Harmony patches
            var harmony = new Harmony("com.realeconomy.patch");
            harmony.PatchAll();
            InformationManager.DisplayMessage(
                new InformationMessage("[RealEco] Harmony patched"));
        }

        protected override void OnGameStart(Game game, IGameStarter gsObj)
        {
            base.OnGameStart(game, gsObj);

            if (game.GameType is Campaign && gsObj is CampaignGameStarter starter)
            {
                starter.AddModel(new RealisticSettlementEconomyModel());   // ✅ ← add it once
                starter.AddBehavior(new RealisticEconomyBehavior());       //  no AddModel inside
                starter.AddBehavior(new ResourceBehavior());
                starter.AddBehavior(new CaravanTradeBehavior());
                EconomyManager.Initialize();
            }
        }
    }
}