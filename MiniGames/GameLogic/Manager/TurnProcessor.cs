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
        private readonly List<ITurnPhase> _turnPhases;

        public IGameContext GameContext => _context;

        public TurnProcessor(InitialSettingUps initialSettingUps)
        {
            _context = new GameContext(initialSettingUps);

            _turnPhases =
            [
                new RelicActivationPhase(),
                new WeatherPhase(),
                new PlayerActionPhase(),
                new EnemyActionPhase(),
                new ConsumptionPhase(),
                new CombatPhase(),
                new PlantingPhase(),
                new CropGrowthPhase(),
                new BuilderProductionPhase(),
                new EnemySpawningPhase(),
                new EndOfTurnPhase()
            ];
        }

        public void TurnStart(UserInput userInput)
        {
            _context.ClearMessages();
            _context.CurrentUserInput = userInput;

            foreach (var phase in _turnPhases)
            {
                phase.Execute(_context);
                if (_context.GameFinished)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 遊戲初始設定的資料結構
        /// </summary>
        public readonly struct InitialSettingUps
        {
            public InitialSettingUps(int food, int buildings, int genericFarmers, int wheatFarmers, int riceFarmers, int soldiers, int builders, int foes)
            {
                Food = food;
                Buildings = buildings;
                GenericFarmers = genericFarmers;
                WheatFarmers = wheatFarmers;
                RiceFarmers = riceFarmers;
                Soldiers = soldiers;
                Builders = builders;
                Foes = foes;
            }
            public int Food { get; }
            public int Buildings { get; }
            public int GenericFarmers { get; }
            public int WheatFarmers { get; }
            public int RiceFarmers { get; }
            public int Soldiers { get; }
            public int Builders { get; }
            public int Foes { get; }
        }

        /// <summary>
        /// UI 傳入玩家操作的資料結構
        /// </summary>
        public readonly struct UserInput
        {
            public UserInput(int recruitedGenericFarmers, int recruitedWheatFarmers, int recruitedRiceFarmers, int recruitedSoldiers, int recruitedBuilders,
                             int adjustedGenericFarmers, int adjustedWheatFarmers, int adjustedRiceFarmers, int adjustedSoldiers, int adjustedBuilders)
            {
                RecruitedGenericFarmers = recruitedGenericFarmers;
                RecruitedWheatFarmers = recruitedWheatFarmers;
                RecruitedRiceFarmers = recruitedRiceFarmers;
                RecruitedSoldiers = recruitedSoldiers;
                RecruitedBuilders = recruitedBuilders;
                AdjustedGenericFarmers = adjustedGenericFarmers;
                AdjustedWheatFarmers = adjustedWheatFarmers;
                AdjustedRiceFarmers = adjustedRiceFarmers;
                AdjustedSoldiers = adjustedSoldiers;
                AdjustedBuilders = adjustedBuilders;
            }

            // 招募區
            public int RecruitedGenericFarmers { get; }
            public int RecruitedWheatFarmers { get; }
            public int RecruitedRiceFarmers { get; }
            public int RecruitedSoldiers { get; }
            public int RecruitedBuilders { get; }

            // 調整區 (代表該回合結束時，各角色的最終數量)
            public int AdjustedGenericFarmers { get; }
            public int AdjustedWheatFarmers { get; }
            public int AdjustedRiceFarmers { get; }
            public int AdjustedSoldiers { get; }
            public int AdjustedBuilders { get; }
        }
    }
}