using System;
using System.Collections.Generic;

namespace GameLogic.Manager
{
    public class TurnProcessor
    {
        // 將具體的 GameContext 儲存在 private 欄位中
        private readonly GameContext _gameContext;
        // 透過 public 屬性將 IGameContext 介面暴露給外部 (UI層)
        public IGameContext GameContext => _gameContext;

        // 移除原有的 public int Turn 屬性，統一由 GameContext 管理
        // public int Turn { get; private set; }

        public TurnProcessor(InitialSettingUps initialSettingUps)
        {
            // 在建構函式中，建立並初始化 GameContext 的實例
            _gameContext = new GameContext
            {
                Food = initialSettingUps.Food,
                FarmersCount = initialSettingUps.Farmers,
                SoldiersCount = initialSettingUps.Soldiers,
                BuildersCount = initialSettingUps.Builders,
                FoesCount = initialSettingUps.Foes,
                BuildingCompletedCount = initialSettingUps.Buildings,
                Beds = initialSettingUps.Buildings * 2,
                Turns = 1, // 遊戲從第 1 回合開始
                GameFinished = false
            };
        }

        /// <summary>
        /// 遊戲核心：執行一回合的所有邏輯
        /// </summary>
        public void TurnStart(UserInput userInput)
        {
            if (_gameContext.GameFinished) return;

            // ===== 1. 回合結算準備 =====
            _gameContext.Turns++;
            _gameContext.ClearMessages();

            // ===== 2. 玩家操作階段 (招募與調整) =====
            // 根據招募的食物成本，扣除食物
            var recruitCost = _gameContext.IsFoodEnough(
                userInput.RecruitedFarmers,
                userInput.RecruitedSoldiers,
                userInput.RecruitedBuilders).Comsumption;
            _gameContext.Food -= recruitCost;

            // 將新招募的人員加入對應的角色計數中
            _gameContext.FarmersCount += userInput.RecruitedFarmers;
            _gameContext.SoldiersCount += userInput.RecruitedSoldiers;
            _gameContext.BuildersCount += userInput.RecruitedBuilders;

            // 根據玩家調整後的人數，更新角色數量
            // GameForm 中已確保總人數不變，所以這裡直接賦值
            _gameContext.FarmersCount = userInput.AdjustedFarmers;
            _gameContext.SoldiersCount = userInput.AdjustedSoldiers;
            _gameContext.BuildersCount = userInput.AdjustedBuilders;

            // ===== 3. 消耗階段 (食物與床位) =====
            // 3.1. 食物消耗
            int foodConsumption = (_gameContext.FarmersCount * 1) + (_gameContext.BuildersCount * 2) + (_gameContext.SoldiersCount * 3);
            while (_gameContext.Food < foodConsumption)
            {
                // 順序：士兵 > 建築師 > 農夫
                if (_gameContext.SoldiersCount > 0) _gameContext.SoldiersCount--;
                else if (_gameContext.BuildersCount > 0) _gameContext.BuildersCount--;
                else if (_gameContext.FarmersCount > 0) _gameContext.FarmersCount--;
                else break; // 避免無限迴圈
                // 重新計算消耗
                foodConsumption = (_gameContext.FarmersCount * 1) + (_gameContext.BuildersCount * 2) + (_gameContext.SoldiersCount * 3);
            }
            _gameContext.Food -= foodConsumption;

            // 3.2. 床位檢查
            while (_gameContext.RolesCount > _gameContext.Beds)
            {
                // 順序：農夫 > 建築師 > 士兵
                if (_gameContext.FarmersCount > 0) _gameContext.FarmersCount--;
                else if (_gameContext.BuildersCount > 0) _gameContext.BuildersCount--;
                else if (_gameContext.SoldiersCount > 0) _gameContext.SoldiersCount--;
                else break; // 避免無限迴圈
            }

            // ===== 4. 戰鬥階段 =====
            int initialFoes = _gameContext.FoesCount;
            int soldiers = _gameContext.SoldiersCount;
            int foesKilledBySoldiers = Math.Min(initialFoes, soldiers); // 士兵殺敵
            _gameContext.FoesCount -= foesKilledBySoldiers;

            // 剩餘的敵人會攻擊我方角色
            int remainingFoes = _gameContext.FoesCount;
            for (int i = 0; i < remainingFoes; i++)
            {
                if (_gameContext.SoldiersCount > 0) _gameContext.SoldiersCount--;
                else if (_gameContext.FarmersCount > 0) _gameContext.FarmersCount--;
                else if (_gameContext.BuildersCount > 0) _gameContext.BuildersCount--;
            }

            // ===== 5. 生產階段 =====
            _gameContext.Food += _gameContext.FarmersCount * 2;
            _gameContext.BuildingCompletedCount += _gameContext.BuildersCount;
            _gameContext.Beds += _gameContext.BuildersCount * 2;

            // ===== 6. 敵人生成 =====
            // 此處使用一個簡單的公式：每回合增加 (回合數 / 2) 個敵人
            int newFoes = _gameContext.Turns / 2;
            _gameContext.FoesCount += newFoes;

            // ===== 7. 遊戲結束檢查 =====
            if (_gameContext.RolesCount == 0 && _gameContext.Food < 2) // 食物少於最便宜的招募成本
            {
                _gameContext.GameFinished = true;
                _gameContext.AddMessage("你失去了所有的人民，村莊被遺棄了...");
            }
        }

        public readonly struct UserInput
        {
            public UserInput(
                int recruitedFarmers,
                int recruitedSoldiers,
                int recruitedBuilders,
                int adjustedFarmers,
                int adjustedSoldiers,
                int adjustedBuilders)
            {
                this.RecruitedFarmers = recruitedFarmers;
                this.RecruitedSoldiers = recruitedSoldiers;
                this.RecruitedBuilders = recruitedBuilders;
                this.AdjustedFarmers = adjustedFarmers;
                this.AdjustedSoldiers = adjustedSoldiers;
                this.AdjustedBuilders = adjustedBuilders;
            }
            public int RecruitedFarmers { get; }
            public int RecruitedSoldiers { get; }
            public int RecruitedBuilders { get; }
            public int AdjustedFarmers { get; }
            public int AdjustedBuilders { get; }
            public int AdjustedSoldiers { get; }
        }

        public readonly struct InitialSettingUps
        {
            public InitialSettingUps(
                int food,
                int building,
                int farmers,
                int soldiers,
                int builders,
                int foes)
            {
                this.Food = food;
                this.Buildings = building;
                this.Farmers = farmers;
                this.Soldiers = soldiers;
                this.Builders = builders;
                this.Foes = foes;
            }
            public int Food { get; }
            public int Buildings { get; }
            public int Farmers { get; }
            public int Builders { get; }
            public int Soldiers { get; }
            public int Foes { get; }
        }
    }
}