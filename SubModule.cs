using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace RealisticEconomy
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            // Confirm at startup that our SubModule is active
            InformationManager.DisplayMessage(
                new InformationMessage("[RealEco] SubModule loaded")
            );
        }

        protected override void OnGameStart(Game game, IGameStarter starter)
        {
            base.OnGameStart(game, starter);
            if (starter is CampaignGameStarter campaignStarter)
            {
                // Confirm behavior is being added
                InformationManager.DisplayMessage(
                    new InformationMessage("[RealEco] Adding behavior")
                );
                campaignStarter.AddBehavior(new Behaviors.RealisticEconomyBehavior());
            }
        }
    }
}
