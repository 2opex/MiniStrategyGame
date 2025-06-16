using GameAbstract.Enum;
using GameLogic.Interface;
using GameLogic.Manager;
using GameLogic.Model;
using System.Collections.Generic;

namespace GameLogic
{
    /// <summary>
    /// 遊戲狀態的彙總根 (Aggregation Root)
    /// </summary>
    public class GameContext : IGameContext
    {
        // 持有所有管理器的實例
        internal IGameStateManager GameStateManager { get; }
        internal IMessageManager MessagesManager { get; }
        internal IResourceManager ResourcesManager { get; }
        internal IPopulationManager PopulationManager { get; }
        internal IEnemyManager EnemiesManager { get; }
        internal IRelicManager RelicsManager { get; }

        internal TurnProcessor.UserInput CurrentUserInput { get; set; }

        public GameContext(TurnProcessor.InitialSettingUps initial)
        {
            // 在建構式中，初始化所有的管理器
            GameStateManager = new GameStateManager();
            MessagesManager = new MessageManager(GameStateManager);
            ResourcesManager = new ResourceManager(initial);
            PopulationManager = new PopulationManager(initial);
            EnemiesManager = new EnemyManager();
            RelicsManager = new RelicManager();
        }

        // 實作 IGameContext 介面，將請求委派給對應的管理器
        public int Food => ResourcesManager.Food;
        public int Beds => ResourcesManager.Beds;
        public int BuildingCompletedCount => ResourcesManager.BuildingCompletedCount;
        public int RolesCount => PopulationManager.TotalRoles;
        public int FarmersCount => PopulationManager.Farmers.Count;
        public int SoldiersCount => PopulationManager.Soldiers.Count;
        public int BuildersCount => PopulationManager.Builders.Count;
        public int FoesCount => EnemiesManager.Enemies.Count;
        public int Turns => GameStateManager.Turns;
        public bool GameFinished => GameStateManager.IsGameFinished;
        public WeatherType Weather => GameStateManager.Weather;
        public IReadOnlyCollection<string> Messages => MessagesManager.Messages;

        public (bool IsEnough, int Comsumption) IsFoodEnough(int newGeneric, int newWheat, int newRice, int newSoldiers, int newBuilders)
        {
            return ResourcesManager.IsFoodEnoughForRecruitment(newGeneric, newWheat, newRice, newSoldiers, newBuilders, GameStateManager.Weather == WeatherType.HotSummer);
        }

        internal void AddMessage(string message) => MessagesManager.AddMessage(message);
        internal void ClearMessages() => MessagesManager.ClearMessages();
    }
}
