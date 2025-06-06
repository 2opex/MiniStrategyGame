using GameLogic.Enums; // 引用 Enums 命名空間
using GameLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic.Manager
{
    public class TurnProcessor
    {
        private readonly GameContext _gameContext;
        private readonly Random _random = new Random(); // 用於產生隨機天氣
        public IGameContext GameContext => _gameContext;

        public TurnProcessor(InitialSettingUps initialSetting)
        {
            _gameContext = new GameContext
            {
                Food = initialSetting.Food,
                BuildingCompletedCount = initialSetting.Buildings,
                Turns = 1,
                GameFinished = false,
                Weather = WeatherType.Normal, // 初始天氣為 "一般"
                //PlayerRoles = new List<PlayerRole>()
            };

            for (int i = 0; i < initialSetting.Farmers; i++) _gameContext.PlayerRoles.Add(new Farmer());
            for (int i = 0; i < initialSetting.Builders; i++) _gameContext.PlayerRoles.Add(new Builder());
            for (int i = 0; i < initialSetting.Soldiers; i++) _gameContext.PlayerRoles.Add(new Soldier());
            for (int i = 0; i < initialSetting.Foes; i++) _gameContext.Foes.Add(new NormalFoe());

            UpdateBeds();
        }

        public void TurnStart(UserInput userInput)
        {
            if (_gameContext.GameFinished) return;

            _gameContext.ClearMessages();

            // 0. 天氣變動階段
            ProcessWeather();
            _gameContext.AddMessage($"======= 回合 {_gameContext.Turns} 開始 ({_gameContext.Weather}) =======");

            // 1. 玩家操作 - 招募
            ProcessRecruitment(userInput);

            // 2. 消耗階段 (食物 & 床位)
            ProcessConsumption();
            if (CheckGameEnd()) return;

            // 3. 戰鬥階段
            ProcessCombat();
            if (CheckGameEnd()) return;

            // 4. 生產階段
            ProcessProduction();

            // 5. 敵人生成
            ProcessFoeSpawning();

            // 6. 回合結束
            _gameContext.Turns++;
            _gameContext.AddMessage("======= 回合結束 =======");
        }

        private void ProcessWeather()
        {
            // 根據需求，每兩回合變動一次天氣。我們設定在奇數回合變天。
            if ((_gameContext.Turns % 2) == 1)
            {
                var weatherValues = Enum.GetValues(typeof(WeatherType));
                var newWeather = (WeatherType)weatherValues.GetValue(_random.Next(weatherValues.Length));
                _gameContext.Weather = newWeather;
                _gameContext.AddMessage($"天氣發生了變化！現在是：{newWeather}。");
            }
        }

        private void ProcessRecruitment(UserInput userInput)
        {
            double costMultiplier = 1.0;
            if (_gameContext.Weather == WeatherType.ScorchingSummer)
            {
                costMultiplier = 1.5; // 炎夏成本變為 1.5 倍
                _gameContext.AddMessage("天氣炎熱，招募成本變為 1.5 倍！");
            }

            var recruitCost = (int)(((userInput.RecruitedFarmers * 2) + (userInput.RecruitedBuilders * 4) + (userInput.RecruitedSoldiers * 6)) * costMultiplier);
            _gameContext.Food -= recruitCost;

            for (int i = 0; i < userInput.RecruitedFarmers; i++) _gameContext.PlayerRoles.Add(new Farmer());
            for (int i = 0; i < userInput.RecruitedBuilders; i++) _gameContext.PlayerRoles.Add(new Builder());
            for (int i = 0; i < userInput.RecruitedSoldiers; i++) _gameContext.PlayerRoles.Add(new Soldier());

            _gameContext.AddMessage($"招募了 {userInput.RecruitedFarmers} 農夫, {userInput.RecruitedBuilders} 建築師, {userInput.RecruitedSoldiers} 士兵。");
            _gameContext.AddMessage($"花費了 {recruitCost} 食物。");
        }

        private void ProcessProduction()
        {
            int foodProduced = 0;
            int buildingsConstructed = 0;

            foreach (var role in _gameContext.PlayerRoles)
            {
                var production = role.PerformProduction(_gameContext);

                // 檢查寒冬對食物產量的影響
                if (_gameContext.Weather == WeatherType.ColdWinter && production.Food > 0)
                {
                    production.Food /= 2; // 食物產量減半
                }

                foodProduced += production.Food;
                buildingsConstructed += production.Buildings;
            }

            if (_gameContext.Weather == WeatherType.ColdWinter && foodProduced > 0)
            {
                _gameContext.AddMessage("寒冬來臨，作物收成不佳，食物產量減半！");
            }

            _gameContext.Food += foodProduced;
            _gameContext.BuildingCompletedCount += buildingsConstructed;
            UpdateBeds();

            _gameContext.AddMessage($"生產結算：獲得 {foodProduced} 食物，建造 {buildingsConstructed} 棟房屋。");
        }

        // --- 以下方法與上一版相同，保持不變 ---

        private void ProcessConsumption()
        {
            // 食物消耗
            int totalFoodConsumption = _gameContext.PlayerRoles.Sum(r => r.FoodConsumption);
            _gameContext.AddMessage($"本回合總食物消耗: {totalFoodConsumption}");

            if (_gameContext.Food < totalFoodConsumption)
            {
                _gameContext.AddMessage($"食物不足！需要 {totalFoodConsumption}, 但只有 {_gameContext.Food}。開始裁減角色...");
                var rolesToCull = _gameContext.PlayerRoles.OrderByDescending(r => r.FoodConsumption).ToList();
                foreach (var role in rolesToCull)
                {
                    _gameContext.PlayerRoles.Remove(role);
                    _gameContext.AddMessage($"裁減了一名 {role.GetType().Name}。");
                    totalFoodConsumption = _gameContext.PlayerRoles.Sum(r => r.FoodConsumption);
                    if (_gameContext.Food >= totalFoodConsumption) break;
                }
            }
            _gameContext.Food -= totalFoodConsumption;

            // 床位檢查
            UpdateBeds();
            if (_gameContext.RolesCount > _gameContext.Beds)
            {
                int excessPopulation = _gameContext.RolesCount - _gameContext.Beds;
                _gameContext.AddMessage($"床位不足！有 {_gameContext.RolesCount} 人, 但只有 {_gameContext.Beds} 個床位。開始裁減 {excessPopulation} 名角色...");
                CullRolesByType(typeof(Farmer), ref excessPopulation);
                CullRolesByType(typeof(Builder), ref excessPopulation);
                CullRolesByType(typeof(Soldier), ref excessPopulation);
            }
        }

        private void ProcessCombat()
        {
            if (_gameContext.FoesCount == 0)
            {
                _gameContext.AddMessage("沒有敵人，平安的一回合。");
                return;
            }

            foreach (var foe in _gameContext.Foes.ToList())
            {
                var impact = foe.PerformSpecialAction(_gameContext);
                if (impact.Food < 0)
                {
                    _gameContext.Food += impact.Food;
                    _gameContext.AddMessage($"{foe.GetType().Name} 偷走了 {-impact.Food} 食物！");
                }
                if (impact.Buildings < 0)
                {
                    _gameContext.BuildingCompletedCount += impact.Buildings;
                    _gameContext.AddMessage($"{foe.GetType().Name} 破壞了 {-impact.Buildings} 棟房屋！");
                }
            }
            UpdateBeds();

            int playerCombatPower = _gameContext.SoldiersCount;
            _gameContext.AddMessage($"我方戰力 (士兵數): {playerCombatPower}");
            int totalFoeCombatPower = _gameContext.Foes.Sum(f => f.CombatPower);
            _gameContext.AddMessage($"敵人總戰力: {totalFoeCombatPower}");

            var foesToRemove = new List<Foe>();
            var sortedFoes = _gameContext.Foes.OrderBy(f => f.CombatPower).ToList();
            foreach (var foe in sortedFoes)
            {
                if (playerCombatPower >= foe.CombatPower)
                {
                    playerCombatPower -= foe.CombatPower;
                    foesToRemove.Add(foe);
                    _gameContext.AddMessage($"成功擊殺一名 {foe.GetType().Name}。");
                }
            }
            foesToRemove.ForEach(f => _gameContext.Foes.Remove(f));

            int remainingFoeCombatPower = _gameContext.Foes.Sum(f => f.CombatPower);
            if (remainingFoeCombatPower > 0)
            {
                _gameContext.AddMessage($"戰鬥失敗！殘存敵人戰力 {remainingFoeCombatPower}，我方將承受傷亡...");
                CullRolesByType(typeof(Soldier), ref remainingFoeCombatPower);
                CullRolesByType(typeof(Farmer), ref remainingFoeCombatPower);
                CullRolesByType(typeof(Builder), ref remainingFoeCombatPower);
            }
            else
            {
                _gameContext.AddMessage("戰鬥勝利！所有敵人都被消滅了。");
            }
        }

        private void ProcessFoeSpawning()
        {
            int normalFoesToSpawn = _gameContext.Turns / 2;
            int thievesToSpawn = _gameContext.Turns / 4;
            int breakersToSpawn = _gameContext.Turns / 5;

            for (int i = 0; i < normalFoesToSpawn; i++) _gameContext.Foes.Add(new NormalFoe());
            for (int i = 0; i < thievesToSpawn; i++) _gameContext.Foes.Add(new FoodThief());
            for (int i = 0; i < breakersToSpawn; i++) _gameContext.Foes.Add(new HouseBreaker());

            if (normalFoesToSpawn + thievesToSpawn + breakersToSpawn > 0)
            {
                _gameContext.AddMessage($"新的敵人出現了！({normalFoesToSpawn} 一般, {thievesToSpawn} 盜賊, {breakersToSpawn} 破壞者)");
            }
        }

        private void UpdateBeds()
        {
            _gameContext.Beds = _gameContext.BuildingCompletedCount * 2;
        }

        private void CullRolesByType(Type roleType, ref int amountToCull)
        {
            if (amountToCull <= 0) return;
            var rolesOfType = _gameContext.PlayerRoles.Where(r => r.GetType() == roleType).ToList();
            int count = Math.Min(amountToCull, rolesOfType.Count);
            for (int i = 0; i < count; i++)
            {
                _gameContext.PlayerRoles.Remove(rolesOfType[i]);
                _gameContext.AddMessage($"因故失去了一名 {roleType.Name}。");
            }
            amountToCull -= count;
        }

        private bool CheckGameEnd()
        {
            if (_gameContext.RolesCount == 0)
            {
                _gameContext.GameFinished = true;
                _gameContext.AddMessage("所有角色都陣亡了... 遊戲結束。");
                //System.Windows.Forms.MessageBox.Show("遊戲結束！你的人民無法存活下去。");
                return true;
            }
            return false;
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