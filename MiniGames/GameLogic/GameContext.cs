using System.Collections.Generic;

namespace GameLogic
{
    /// <summary>
    /// 提供給 UI 層存取遊戲狀態的唯讀介面。
    /// 這確保了 UI 不能直接修改遊戲邏輯的數據。
    /// </summary>
    public interface IGameContext
    {
        int Food { get; }
        int Beds { get; }
        int BuildingCompletedCount { get; }
        int RolesCount { get; }
        int FarmersCount { get; }
        int SoldiersCount { get; }
        int BuildersCount { get; }
        int FoesCount { get; }
        int Turns { get; }
        bool GameFinished { get; }
        IReadOnlyCollection<string> Messages { get; }
        (bool IsEnough, int Comsumption) IsFoodEnough(int farmers, int soldiers, int builders);
    }

    /// <summary>
    /// 實作 IGameContext，並管理遊戲的所有狀態。
    /// internal 可確保只有 GameLogic 專案內的類別 (例如 TurnProcessor) 可以建立和修改它。
    /// </summary>
    internal class GameContext : IGameContext
    {
        // 根據需求文件，定義各種角色的食物消耗量與招募成本
        // 招募成本：依角色的食物消耗量設定招募花費，例如「士兵要 6 份食物才能招 1 名」。
        // 此處我們假設招募成本是消耗量的兩倍。
        private const int FarmerRecruitCost = 2; // 農夫消耗 1，成本 2
        private const int BuilderRecruitCost = 4; // 建築師消耗 2，成本 4
        private const int SoldierRecruitCost = 6; // 士兵消耗 3，成本 6

        public int Food { get; internal set; }
        public int Beds { get; internal set; }
        public int BuildingCompletedCount { get; internal set; }
        public int RolesCount => FarmersCount + SoldiersCount + BuildersCount;
        public int FarmersCount { get; internal set; }
        public int SoldiersCount { get; internal set; }
        public int BuildersCount { get; internal set; }
        public int FoesCount { get; internal set; }
        public int Turns { get; internal set; }
        public bool GameFinished { get; internal set; }
        public IReadOnlyCollection<string> Messages => _messages;
        private readonly List<string> _messages = new List<string>();

        /// <summary>
        /// 檢查招募新角色所需的食物是否足夠。
        /// </summary>
        public (bool IsEnough, int Comsumption) IsFoodEnough(int farmers, int soldiers, int builders)
        {
            var totalCost = (farmers * FarmerRecruitCost) +
                            (soldiers * SoldierRecruitCost) +
                            (builders * BuilderRecruitCost);

            return (Food >= totalCost, totalCost);
        }

        /// <summary>
        /// 內部方法，用於在回合結束時清除舊訊息並加入新訊息。
        /// </summary>
        internal void AddMessage(string message)
        {
            _messages.Add(message);
        }

        internal void ClearMessages()
        {
            _messages.Clear();
        }
    }
}