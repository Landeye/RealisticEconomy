using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using RealisticEconomyMod.Behaviors;      // for ResourceBehavior
using RealisticEconomyMod;                // for EconomyManager
using TaleWorlds.Library;

namespace RealisticEconomyMod
{
    public class SubModule : MBSubModuleBase
    {
        // Harmony ID can be anything unique
        private const string HarmonyId = "com.landeye.realisticeconomy";

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            // Apply all [HarmonyPatch] classes in this assembly
            var harmony = new Harmony(HarmonyId);
            harmony.PatchAll();

            // Debug-print so you know patching happened
            InformationManager.DisplayMessage(
                new InformationMessage("[RealEco] Harmony patches applied")
            );
        }

        protected override void OnGameStart(Game game, IGameStarter starter)
        {
            base.OnGameStart(game, starter);

            if (starter is CampaignGameStarter campaignStarter)
            {
                // Initialize our global gold pool before any behaviors run
                EconomyManager.Initialize();

                InformationManager.DisplayMessage(
                    new InformationMessage("[RealEco] EconomyManager initialized")
                );

                // Register your new daily resource snapshot behavior
                campaignStarter.AddBehavior(new ResourceBehavior());

                InformationManager.DisplayMessage(
                    new InformationMessage("[RealEco] ResourceBehavior added")
                );

                // If you have other economy-only behaviors, add them here:
                // campaignStarter.AddBehavior(new AnotherEconomyBehavior());

                InformationManager.DisplayMessage(
                    new InformationMessage("[RealEco] SubModule fully initialized")
                );
            }
        }
    }
}
