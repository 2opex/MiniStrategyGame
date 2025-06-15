using System;
using System.Collections.Generic;
using System.Linq;
using GameAbstract;
using GameAbstract.Enum;
using GameLogic.Model;

namespace GameLogic.Manager
{
    public class TurnProcessor
    {
        private readonly GameContext _context;
        public IGameContext GameContext => _context;

        public TurnProcessor(InitialSettingUps initialSettingUps)
        {
            _context = new GameContext(initialSettingUps);
        }

        /// <summary>
        /// 執行一個完整的遊戲回合
        /// </summary>
        public void TurnStart(UserInput userInput)
        {
            _context.ClearMessages();

            // 0. 天氣階段
            HandleWeather();

            // 1. 玩家操作階段 (招募與角色調整)
            HandlePlayerActions(userInput);

            // 2. 消耗階段 (食物與床位)
            HandleConsumption();
            if (CheckLossCondition()) return;

            // 3. 戰鬥階段
            HandleCombat();
            if (CheckLossCondition()) return;

            // 4. 生產階段
            HandleProduction();

            // 5. 敵人生成階段
            HandleEnemySpawning();

            // 回合結束，進入下一回合
            _context.Turns++;
        }

        private void HandleWeather()
        {
            // 每兩回合變動一次天氣
            if (_context.Turns % 2 == 1 && _context.Turns > 1)
            {
                var random = new Random();
                var weatherValues = Enum.GetValues(typeof(WeatherType));
                _context.Weather = (WeatherType)weatherValues.GetValue(random.Next(weatherValues.Length));
                _context.AddMessage($"天氣變了！現在是 {GetWeatherString(_context.Weather)}。");
            }
            else
            {
                _context.AddMessage($"天氣是 {GetWeatherString(_context.Weather)}。");
            }
        }

        private void HandlePlayerActions(UserInput input)
        {
            // 處理招募
            var costResult = _context.IsFoodEnough(input.RecruitedFarmers, input.RecruitedSoldiers, input.RecruitedBuilders);
            if (costResult.IsEnough)
            {
                _context.Food -= costResult.Comsumption;
                for (int i = 0; i < input.RecruitedFarmers; i++) _context.Farmers.Add(new Farmer());
                for (int i = 0; i < input.RecruitedSoldiers; i++) _context.Soldiers.Add(new Soldier());
                for (int i = 0; i < input.RecruitedBuilders; i++) _context.Builders.Add(new Builder());
                _context.AddMessage($"招募了 {input.RecruitedFarmers} 農夫, {input.RecruitedSoldiers} 士兵, {input.RecruitedBuilders} 建築師。花費 {costResult.Comsumption} 食物。");
            }

            // 處理角色數量調整 (此處直接使用UI傳入的最終數量)
            _context.Farmers.Clear();
            _context.Soldiers.Clear();
            _context.Builders.Clear();
            for (int i = 0; i < input.AdjustedFarmers; i++) _context.Farmers.Add(new Farmer());
            for (int i = 0; i < input.AdjustedSoldiers; i++) _context.Soldiers.Add(new Soldier());
            for (int i = 0; i < input.AdjustedBuilders; i++) _context.Builders.Add(new Builder());
            _context.AddMessage($"角色數量調整為: {input.AdjustedFarmers} 農夫, {input.AdjustedSoldiers} 士兵, {input.AdjustedBuilders} 建築師。");
        }

        private void HandleConsumption()
        {
            // 1. 食物消耗
            int totalFoodConsumption = _context.Farmers.Sum(f => f.FoodConsumption) +
                                       _context.Soldiers.Sum(s => s.FoodConsumption) +
                                       _context.Builders.Sum(b => b.FoodConsumption);
            _context.AddMessage($"本回合總食物需求: {totalFoodConsumption}。");

            if (_context.Food < totalFoodConsumption)
            {
                _context.AddMessage("食物不足！開始依規則裁減角色...");
                // 規則: 先裁減消耗量最大的角色 (士兵 > 建築師 > 農夫)
                while (_context.Food < totalFoodConsumption)
                {
                    Role roleToCull = _context.Soldiers.Count > 0 ? (Role)_context.Soldiers[0] :
                                      _context.Builders.Count > 0 ? (Role)_context.Builders[0] :
                                      _context.Farmers.Count > 0 ? (Role)_context.Farmers[0] : null;
                    if (roleToCull == null) break;

                    if (roleToCull is Soldier) _context.Soldiers.RemoveAt(0);
                    else if (roleToCull is Builder) _context.Builders.RemoveAt(0);
                    else if (roleToCull is Farmer) _context.Farmers.RemoveAt(0);

                    _context.AddMessage($"裁減了一名 {GetRoleString(roleToCull)}。");
                    totalFoodConsumption = _context.RolesCount > 0 ? _context.Farmers.Sum(f => f.FoodConsumption) + _context.Soldiers.Sum(s => s.FoodConsumption) + _context.Builders.Sum(b => b.FoodConsumption) : 0;
                }
            }
            _context.Food -= totalFoodConsumption;
            _context.AddMessage($"食物消耗完畢，剩餘 {_context.Food} 食物。");

            // 2. 床位檢查
            _context.Beds = _context.BuildingCompletedCount * 2;
            if (_context.RolesCount > _context.Beds)
            {
                int peopleToCull = _context.RolesCount - _context.Beds;
                _context.AddMessage($"床位不足！需裁減 {peopleToCull} 人...");
                // 規則: 先裁減農夫 > 建築師 > 士兵
                int culled = Math.Min(peopleToCull, _context.Farmers.Count);
                if (culled > 0) { _context.Farmers.RemoveRange(0, culled); peopleToCull -= culled; _context.AddMessage($"裁減了 {culled} 名農夫。"); }

                culled = Math.Min(peopleToCull, _context.Builders.Count);
                if (culled > 0) { _context.Builders.RemoveRange(0, culled); peopleToCull -= culled; _context.AddMessage($"裁減了 {culled} 名建築師。"); }

                culled = Math.Min(peopleToCull, _context.Soldiers.Count);
                if (culled > 0) { _context.Soldiers.RemoveRange(0, culled); _context.AddMessage($"裁減了 {culled} 名士兵。"); }
            }
        }

        private void HandleCombat()
        {
            if (_context.FoesCount <= 0) return;

            int totalKillPower = _context.Soldiers.Sum(s => s.KillPower);
            int foesKilled = Math.Min(_context.FoesCount, totalKillPower);
            _context.FoesCount -= foesKilled;
            _context.AddMessage($"士兵們奮勇作戰，擊敗了 {foesKilled} 名敵人。");

            if (_context.FoesCount > 0)
            {
                _context.AddMessage($"仍有 {_context.FoesCount} 名敵人殘存！我方開始出現傷亡...");
                // 規則: 1:1 消耗我方角色，順序：士兵 > 農夫 > 建築師
                int casualties = _context.FoesCount;

                int culled = Math.Min(casualties, _context.Soldiers.Count);
                if (culled > 0) { _context.Soldiers.RemoveRange(0, culled); casualties -= culled; _context.AddMessage($"損失了 {culled} 名士兵。"); }

                culled = Math.Min(casualties, _context.Farmers.Count);
                if (culled > 0) { _context.Farmers.RemoveRange(0, culled); casualties -= culled; _context.AddMessage($"損失了 {culled} 名農夫。"); }

                culled = Math.Min(casualties, _context.Builders.Count);
                if (culled > 0) { _context.Builders.RemoveRange(0, culled); _context.AddMessage($"損失了 {culled} 名建築師。"); }
            }
        }

        private void HandleProduction()
        {
            // 農夫生產食物
            int foodProduced = _context.Farmers.Sum(f => f.FoodProduction);
            if (_context.Weather == WeatherType.ColdWinter)
            {
                foodProduced /= 2;
                _context.AddMessage("寒冬來臨，一般作物產量減半！");
            }
            _context.Food += foodProduced;
            _context.AddMessage($"農夫們生產了 {foodProduced} 份食物。");

            // 建築師建造房屋
            int buildingsProduced = _context.Builders.Sum(b => b.BuildingProduction);
            _context.BuildingCompletedCount += buildingsProduced;
            _context.Beds = _context.BuildingCompletedCount * 2;
            _context.AddMessage($"建築師們建造了 {buildingsProduced} 棟房屋，現在總床位數為 {_context.Beds}。");
        }

        private void HandleEnemySpawning()
        {
            // 敵人生成公式：每回合增加 (回合數 / 2) + 1 個敵人
            int newFoes = (_context.Turns / 2) + 1;
            _context.FoesCount += newFoes;
            _context.AddMessage($"地平線出現了 {newFoes} 名新的敵人！");
        }

        private bool CheckLossCondition()
        {
            if (_context.RolesCount <= 0)
            {
                _context.GameFinished = true;
                _context.AddMessage("你失去了所有的人民...遊戲結束。");
                return true;
            }
            return false;
        }

        private string GetWeatherString(WeatherType weather)
        {
            switch (weather)
            {
                case WeatherType.HotSummer: return "炎夏";
                case WeatherType.ColdWinter: return "寒冬";
                default: return "一般";
            }
        }

        private string GetRoleString(Role role)
        {
            if (role is Farmer) return "農夫";
            if (role is Builder) return "建築師";
            if (role is Soldier) return "士兵";
            return "未知角色";
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