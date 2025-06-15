using System;
using System.Collections.Generic;
using System.Linq;
using GameAbstract;
using GameAbstract.Enum;
using GameLogic.Interface;
using GameLogic.Manager.Phases;
using GameLogic.Model;

namespace GameLogic.Manager
{
    public class TurnProcessor
    {
        private readonly GameContext _context;
        private readonly List<ITurnPhase> _turnPhases; // 持有一組策略

        public IGameContext GameContext => _context;

        public TurnProcessor(InitialSettingUps initialSettingUps)
        {
            _context = new GameContext(initialSettingUps);

            // 在此定義遊戲回合的完整流程
            _turnPhases =
            [
                new WeatherPhase(),
                new PlayerActionPhase(),
                new ConsumptionPhase(),
                new CombatPhase(),
                new ProductionPhase(),
                new EnemySpawningPhase(),
                new EndOfTurnPhase()
            ];
        }

        /// <summary>
        /// 執行一個完整的遊戲回合
        /// </summary>
        public void TurnStart(UserInput userInput)
        {
            _context.ClearMessages();
            _context.CurrentUserInput = userInput; // 將使用者操作暫存至 Context

            // 依序執行定義好的每一個階段
            foreach (var phase in _turnPhases)
            {
                phase.Execute(_context);
                if (_context.GameFinished)
                {
                    break; // 如果遊戲已結束，則中斷後續階段
                }
            }
        }

        // 這是 UI 傳入玩家操作的資料結構
        public readonly struct UserInput
        {
            public UserInput(
                int recruitedFarmers, int recruitedSoldiers, int recruitedBuilders,
                int adjustedFarmers, int adjustedSoldiers, int adjustedBuilders)
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

        // 這是遊戲初始設定的資料結構
        public readonly struct InitialSettingUps
        {
            public InitialSettingUps(int food, int building, int farmers, int soldiers, int builders, int foes)
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