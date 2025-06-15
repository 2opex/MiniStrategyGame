using GameAbstract;
using GameAbstract.Enum;
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
        public int FoesCount { get; set; }
        public int Turns { get; set; }
        public bool GameFinished { get; set; }
        public WeatherType Weather { get; set; }

        private readonly List<string> _messages = new List<string>();
        public IReadOnlyCollection<string> Messages => _messages.AsReadOnly();

        internal List<Farmer> Farmers { get; } = new List<Farmer>();
        internal List<Soldier> Soldiers { get; } = new List<Soldier>();
        internal List<Builder> Builders { get; } = new List<Builder>();

        public GameContext(TurnProcessor.InitialSettingUps initial)
        {
            this.Food = initial.Food;
            this.BuildingCompletedCount = initial.Buildings;
            this.Beds = this.BuildingCompletedCount * 2;
            for (int i = 0; i < initial.Farmers; i++) Farmers.Add(new Farmer());
            for (int i = 0; i < initial.Soldiers; i++) Soldiers.Add(new Soldier());
            for (int i = 0; i < initial.Builders; i++) Builders.Add(new Builder());
            this.FoesCount = initial.Foes;
            this.Turns = 1;
            this.Weather = WeatherType.Normal;
        }

        public (bool IsEnough, int Comsumption) IsFoodEnough(int newFarmers, int newSoldiers, int newBuilders)
        {
            var farmerCost = new Farmer().RecruitmentCost * newFarmers;
            var soldierCost = new Soldier().RecruitmentCost * newSoldiers;
            var builderCost = new Builder().RecruitmentCost * newBuilders;
            var totalCost = farmerCost + soldierCost + builderCost;

            if (this.Weather == WeatherType.HotSummer)
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