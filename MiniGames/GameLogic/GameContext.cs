using GameLogic.Models;
using System.Collections.Generic;
using System.Linq;

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

        // 主要狀態屬性，現在可以直接設定
        public int Food { get; set; }
        public int Beds { get; set; }
        public int BuildingCompletedCount { get; set; }
        public int Turns { get; set; }
        public bool GameFinished { get; set; }

        // 使用物件列表來管理角色和敵人
        public List<PlayerRole> PlayerRoles { get; } = new List<PlayerRole>();
        public List<Foe> Foes { get; } = new List<Foe>();

        // 為了相容 UI，保留計數屬性，但改為從列表中動態計算
        public int RolesCount => PlayerRoles.Count;
        public int FarmersCount => PlayerRoles.OfType<Farmer>().Count();
        public int SoldiersCount => PlayerRoles.OfType<Soldier>().Count();
        public int BuildersCount => PlayerRoles.OfType<Builder>().Count();
        public int FoesCount => Foes.Count;

        // --- 訊息相關 ---
        private readonly List<string> _messages = new List<string>();
        public IReadOnlyCollection<string> Messages => _messages;
        public void AddMessage(string message) => _messages.Add(message);
        public void ClearMessages() => _messages.Clear();

        // --- 招募檢查 ---
        public (bool IsEnough, int Comsumption) IsFoodEnough(int farmers, int soldiers, int builders)
        {
            var totalCost = (farmers * FarmerRecruitCost) +
                            (soldiers * SoldierRecruitCost) +
                            (builders * BuilderRecruitCost);
            return (Food >= totalCost, totalCost);
        }
    }
}