using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Library;
using RealisticEconomy.Behaviors;
using RealisticEconomy.Models;
using System.Reflection;

namespace RealisticEconomy
{
    public class SubModule : MBSubModuleBase
    {
        // ────────────────────────────────  load / patch  ────────────────────────────────
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            new Harmony("com.realeconomy.patch").PatchAll();
            InformationManager.DisplayMessage(new InformationMessage("[RealEco] Harmony patched"));
        }

        // ────────────────────────────────  game start  ────────────────────────────────
        protected override void OnGameStart(Game game, IGameStarter gsObj)
        {
            base.OnGameStart(game, gsObj);

            if (game.GameType is Campaign && gsObj is CampaignGameStarter starter)
            {
                // behaviours
                starter.AddBehavior(new RealisticEconomyBehavior());
                starter.AddBehavior(new ResourceBehavior());
                starter.AddBehavior(new CaravanTradeBehavior());
                starter.AddBehavior(new StarvationPenaltyBehavior());
                starter.AddBehavior(new TradeDirectiveBehavior());
                starter.AddBehavior(new SupplyPartyBehavior());
                starter.AddBehavior(new CaravanBiasBehavior());

                // one-shot behaviour that enlarges the Map-Tracks pool
                starter.AddBehavior(new TrackPoolBoosterBehavior());

                EconomyManager.Initialize();
            }
        }

        // ────────────────────────────────  helper  ────────────────────────────────
        internal static void BoostTrackPool()
        {
            var mapTracks = Campaign.Current.GetCampaignBehavior<MapTracksCampaignBehavior>();
            if (mapTracks == null) return;                       // not yet initialised

            var poolField = typeof(MapTracksCampaignBehavior)
                            .GetField("_trackPool", BindingFlags.Instance | BindingFlags.NonPublic);
            var trackPool = poolField?.GetValue(mapTracks);
            if (trackPool == null) return;

            var grow = trackPool.GetType()
                                .GetMethod("IncreaseCapacity",
                                           BindingFlags.Instance | BindingFlags.NonPublic);

            const int EXTRA_CAPACITY = 32_000;   // +32 k segments  ≈ 150-200 game-days
            grow?.Invoke(trackPool, new object[] { EXTRA_CAPACITY });

            InformationManager.DisplayMessage(
                new InformationMessage($"[RealEco] Map-track pool enlarged by {EXTRA_CAPACITY}", Colors.Green));
        }
    }

    /// <summary>
    /// Runs on the very first hourly tick, then removes itself.
    /// Ensures the native MapTracks behavior is present before we poke it.
    /// </summary>
    internal sealed class TrackPoolBoosterBehavior : CampaignBehaviorBase
    {
        private bool _done;

        public override void RegisterEvents()
        {
            // listener must match Action signature → no parameters
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
        }

        private void OnHourlyTick()
        {
            if (_done) return;

            SubModule.BoostTrackPool();
            _done = true;
        }

        public override void SyncData(IDataStore _) { }
    }
}