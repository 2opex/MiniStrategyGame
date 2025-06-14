using GameLogic.Enums;
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
        int BuildersCount { get; }
        int FoesCount { get; }
        int Turns { get; }
        bool GameFinished { get; }
        bool HasRelic { get; } // 新增：是否已獲得聖物
        bool IsRelicActive { get; } // 新增：聖物效果是否已啟動
        IReadOnlyCollection<string> Messages { get; }
        WeatherType Weather { get; } // 新增天氣屬性
        int FarmersCount { get; }
        int WheatFarmersCount { get; } // 新增
        int RiceFarmersCount { get; }  // 新增
        int SoldiersCount { get; }
        (bool IsEnough, int Comsumption) IsFoodEnough(int farmers, int wheatFarmers, int riceFarmers, int soldiers, int builders); // 修改簽名
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
        public bool HasRelic { get; set; } // 新增：是否已獲得聖物
        public bool IsRelicActive { get; set; } // 新增：聖物效果是否已啟動
        public WeatherType Weather { get; set; } // 新增天氣屬性

        public List<Crop> PlantedCrops { get; internal set; } = new List<Crop>(); // 新增作物列表

        public List<PlayerRole> PlayerRoles { get; } = [];
        public List<Foe> Foes { get; } = [];

        // 為了相容 UI，保留計數屬性，但改為從列表中動態計算
        public int RolesCount => PlayerRoles.Count;
        public int BuildersCount => PlayerRoles.OfType<Builder>().Count();
        public int FoesCount => Foes.Count;

        public int FarmersCount => PlayerRoles.OfType<Farmer>().Count();
        public int WheatFarmersCount => PlayerRoles.OfType<WheatFarmer>().Count(); // 新增
        public int RiceFarmersCount => PlayerRoles.OfType<RiceFarmer>().Count();   // 新增
        public int SoldiersCount => PlayerRoles.OfType<Soldier>().Count();

        // --- 訊息相關 ---
        private readonly List<string> messages = [];
        public IReadOnlyCollection<string> Messages => messages;
        public void AddMessage(string message) => messages.Add(message);
        public void ClearMessages() => messages.Clear();

        // 修改 IsFoodEnough 來包含新的農夫類型
        public (bool IsEnough, int Comsumption) IsFoodEnough(int farmers, int wheatFarmers, int riceFarmers, int soldiers, int builders)
        {
            // 根據需求，所有農夫的招募成本都與其消耗掛鉤 (消耗1，成本2)
            var totalCost = (farmers * FarmerRecruitCost) +
                            (wheatFarmers * FarmerRecruitCost) + // 新增
                            (riceFarmers * FarmerRecruitCost) +  // 新增
                            (soldiers * SoldierRecruitCost) +
                            (builders * BuilderRecruitCost);
            return (Food >= totalCost, totalCost);
        }
    }
}