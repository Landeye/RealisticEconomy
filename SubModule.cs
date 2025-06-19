using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace RealisticEconomy
{
    public class SubModule : MBSubModuleBase
    {
        // Called when *any* game starts (main menu, campaign, battles, etc.)
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        // Called when a campaign actually begins loading
        protected override void OnGameStart(Game game, IGameStarter starter)
        {
            base.OnGameStart(game, starter);

            if (starter is CampaignGameStarter campaignStarter)
            {
                // Register your behavior so its RegisterEvents() runs:
                campaignStarter.AddBehavior(new Behaviors.RealisticEconomyBehavior());
            }
        }
    }
}
