using GameAbstract.Enum;
using GameLogic.Interface;
using GameLogic.Manager;
using GameLogic.Model;
using System.Collections.Generic;

namespace GameLogic
{
    /// <summary>
    /// 遊戲狀態的具體實作 (Internal，僅供 GameLogic 內部使用)
    /// </summary>
    internal class GameContext : IGameContext
    {
        public int Food { get; set; }
        public int Beds { get; set; }
        public int BuildingCompletedCount { get; set; }
        public int RolesCount => Farmers.Count + Soldiers.Count + Builders.Count;
        public int FarmersCount => Farmers.Count;
        public int SoldiersCount => Soldiers.Count;
        public int BuildersCount => Builders.Count;
        public int FoesCount => Enemies.Count;
        internal List<Enemy> Enemies { get; } = [];
        public int Turns { get; set; }
        public bool GameFinished { get; set; }
        public WeatherType Weather { get; set; }

        private readonly List<string> _messages = [];
        public IReadOnlyCollection<string> Messages => _messages.AsReadOnly();

        internal List<Farmer> Farmers { get; } = [];
        internal List<Soldier> Soldiers { get; } = [];
        internal List<Builder> Builders { get; } = [];
        internal List<Relic> ActiveRelics { get; } = [];
        internal List<Relic> PendingRelics { get; } = [];

        internal TurnProcessor.UserInput CurrentUserInput { get; set; }


        public GameContext(TurnProcessor.InitialSettingUps initial)
        {
            Food = initial.Food;
            BuildingCompletedCount = initial.Buildings;
            Beds = BuildingCompletedCount * 2;
            Turns = 1;
            Weather = WeatherType.Normal;

            // 根據新的結構初始化不同種類的農夫
            for (int i = 0; i < initial.GenericFarmers; i++) Farmers.Add(new GenericFarmer());
            for (int i = 0; i < initial.WheatFarmers; i++) Farmers.Add(new WheatFarmer());
            for (int i = 0; i < initial.RiceFarmers; i++) Farmers.Add(new RiceFarmer());

            for (int i = 0; i < initial.Soldiers; i++) Soldiers.Add(new Soldier());
            for (int i = 0; i < initial.Builders; i++) Builders.Add(new Builder());
        }

        public (bool IsEnough, int Comsumption) IsFoodEnough(int newGeneric, int newWheat, int newRice, int newSoldiers, int newBuilders)
        {
            var totalCost = (newGeneric + newWheat + newRice) * new GenericFarmer().RecruitmentCost +
                            newSoldiers * new Soldier().RecruitmentCost +
                            newBuilders * new Builder().RecruitmentCost;

            if (Weather == WeatherType.HotSummer)
            {
                totalCost = (int)(totalCost * 1.5);
            }

            return (Food >= totalCost, totalCost);
        }

        internal void AddMessage(string message)
        {
            _messages.Add($"[回合 {Turns}] {message}");
        }

        internal void ClearMessages()
        {
            _messages.Clear();
        }
    }
}