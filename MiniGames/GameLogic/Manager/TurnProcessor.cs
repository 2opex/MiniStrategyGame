using GameLogic.Enums; // 引用 Enums 命名空間
using GameLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic.Manager
{
    public class TurnProcessor
    {
        private readonly GameContext gameContext;
        private readonly Random random = new();
        public IGameContext GameContext => gameContext;

        public TurnProcessor(InitialSettingUps initialSetting)
        {
            gameContext = new GameContext
            {
                Food = initialSetting.Food,
                BuildingCompletedCount = initialSetting.Buildings,
                Turns = 1,
                GameFinished = false,
                Weather = WeatherType.Normal,
                HasRelic = false,      // 初始化聖物狀態
                IsRelicActive = false, // 初始化聖物狀態
                //PlayerRoles = new List<PlayerRole>()
            };

            for (int i = 0; i < initialSetting.Farmers; i++) gameContext.PlayerRoles.Add(new Farmer());
            for (int i = 0; i < initialSetting.Builders; i++) gameContext.PlayerRoles.Add(new Builder());
            for (int i = 0; i < initialSetting.Soldiers; i++) gameContext.PlayerRoles.Add(new Soldier());
            for (int i = 0; i < initialSetting.Foes; i++) gameContext.Foes.Add(new NormalFoe());

            UpdateBeds();
        }

        public void TurnStart(UserInput userInput)
        {
            if (gameContext.GameFinished) return;

            gameContext.ClearMessages();

            // 0. 天氣與聖物啟動
            ProcessWeather();
            ActivateRelicEffect(); // 檢查並啟動聖物
            gameContext.AddMessage($"======= 回合 {gameContext.Turns} 開始 ({gameContext.Weather}) =======");
            if (gameContext.IsRelicActive) gameContext.AddMessage("聖物的光輝正庇佑著你！");

            // 1. 玩家操作 - 招募
            ProcessRecruitment(userInput);

            // 2. 消耗階段
            ProcessConsumption();
            if (CheckGameEnd()) return;

            // 3. 戰鬥階段
            ProcessCombat();
            if (CheckGameEnd()) return;

            // 3.5. 特殊敵人清理
            ProcessFoeCleanup();

            // 4. 生產階段
            ProcessProduction();

            // 5. 敵人生成
            ProcessFoeSpawning();

            // 6. 回合結束
            gameContext.Turns++;
            gameContext.AddMessage("======= 回合結束 =======");
        }

        private void ActivateRelicEffect()
        {
            // 如果已獲得聖物但效果尚未啟動，則在此回合啟動它
            if (gameContext.HasRelic && !gameContext.IsRelicActive)
            {
                gameContext.IsRelicActive = true;
                gameContext.AddMessage("聖物的力量被激活了！所有角色的食物消耗降低！");
            }
        }

        private void ProcessWeather()
        {
            // 根據需求，每兩回合變動一次天氣。我們設定在奇數回合變天。
            if ((gameContext.Turns % 2) == 1)
            {
                var weatherValues = Enum.GetValues(typeof(WeatherType));
                var newWeather = (WeatherType)weatherValues.GetValue(random.Next(weatherValues.Length));
                gameContext.Weather = newWeather;
                gameContext.AddMessage($"天氣發生了變化！現在是：{newWeather}。");
            }
        }

        private void ProcessRecruitment(UserInput userInput)
        {
            double costMultiplier = 1.0;
            if (gameContext.Weather == WeatherType.ScorchingSummer)
            {
                costMultiplier = 1.5;
                gameContext.AddMessage("天氣炎熱，招募成本變為 1.5 倍！");
            }

            // 農夫的招募成本都一樣 (消耗1，成本2)
            var recruitCost = (int)(((userInput.RecruitedFarmers * 2) +
                                     (userInput.RecruitedWheatFarmers * 2) + // 新增
                                     (userInput.RecruitedRiceFarmers * 2) +  // 新增
                                     (userInput.RecruitedBuilders * 4) +
                                     (userInput.RecruitedSoldiers * 6)) * costMultiplier);
            gameContext.Food -= recruitCost;

            for (int i = 0; i < userInput.RecruitedFarmers; i++) gameContext.PlayerRoles.Add(new Farmer());
            for (int i = 0; i < userInput.RecruitedWheatFarmers; i++) gameContext.PlayerRoles.Add(new WheatFarmer()); // 新增
            for (int i = 0; i < userInput.RecruitedRiceFarmers; i++) gameContext.PlayerRoles.Add(new RiceFarmer());   // 新增
            for (int i = 0; i < userInput.RecruitedBuilders; i++) gameContext.PlayerRoles.Add(new Builder());
            for (int i = 0; i < userInput.RecruitedSoldiers; i++) gameContext.PlayerRoles.Add(new Soldier());

            gameContext.AddMessage($"招募了 {userInput.RecruitedFarmers} 農夫, {userInput.RecruitedWheatFarmers} 小麥農夫, {userInput.RecruitedRiceFarmers} 稻米農夫, {userInput.RecruitedBuilders} 建築師, {userInput.RecruitedSoldiers} 士兵。");
            gameContext.AddMessage($"花費了 {recruitCost} 食物。");
        }


        private void ProcessProduction()
        {
            int foodFromHarvest = 0;
            int buildingsConstructed = 0;
            var harvestedCrops = new List<Crop>();

            // --- 階段 4.1: 作物生長與收成 ---
            gameContext.AddMessage("--- 作物生長報告 ---");
            foreach (var crop in gameContext.PlantedCrops)
            {
                // 處理寒冬對水稻的影響
                if (crop.Type == CropType.Rice && gameContext.Weather == WeatherType.ColdWinter)
                {
                    crop.IsStalled = true;
                    gameContext.AddMessage("寒冬來臨，水稻停止生長。");
                    continue; // 跳過此作物的後續處理
                }

                crop.IsStalled = false; // 如果不是寒冬，確保作物正常生長
                crop.TurnsGrown++;

                if (crop.IsMature)
                {
                    int finalYield = crop.Yield;
                    // 處理寒冬對一般作物的影響
                    if (crop.Type == CropType.General && gameContext.Weather == WeatherType.ColdWinter)
                    {
                        finalYield /= 2;
                        gameContext.AddMessage($"寒冬影響，一份 {crop.Type} 作物收成減半為 {finalYield} 食物。");
                    }
                    else
                    {
                        gameContext.AddMessage($"一份 {crop.Type} 作物成熟了！收穫 {finalYield} 食物。");
                    }
                    foodFromHarvest += finalYield;
                    harvestedCrops.Add(crop);
                }
                else
                {
                    gameContext.AddMessage($"一份 {crop.Type} 作物正在生長... ({crop.TurnsGrown}/{crop.GrowthTime})");
                }
            }

            // 將已收成的作物從列表中移除
            harvestedCrops.ForEach(c => gameContext.PlantedCrops.Remove(c));

            foreach (var role in gameContext.PlayerRoles.OfType<IBuild>())
            {
                buildingsConstructed += role.Build();
            }

            // --- 階段 4.3: 農夫種植新作物 ---
            foreach (var farmer in gameContext.PlayerRoles.OfType<Farmer>())
            {
                gameContext.PlantedCrops.Add(new Crop(CropType.General, gameContext.Turns));
            }
            foreach (var wheatFarmer in gameContext.PlayerRoles.OfType<WheatFarmer>())
            {
                gameContext.PlantedCrops.Add(new Crop(CropType.Wheat, gameContext.Turns));
            }
            foreach (var riceFarmer in gameContext.PlayerRoles.OfType<RiceFarmer>())
            {
                gameContext.PlantedCrops.Add(new Crop(CropType.Rice, gameContext.Turns));
            }

            // --- 階段 4.4: 結算資源 ---
            gameContext.Food += foodFromHarvest;
            gameContext.BuildingCompletedCount += buildingsConstructed;
            UpdateBeds();

            gameContext.AddMessage($"生產結算：共收穫 {foodFromHarvest} 食物，建造 {buildingsConstructed} 棟房屋。");
        }

        private void ProcessConsumption()
        {
            // --- 食物消耗 ---
            int totalFoodConsumption = 0;
            foreach (var role in gameContext.PlayerRoles)
            {
                int consumption = role.FoodConsumption;
                // 套用聖物效果：食物消耗-1，最低為1
                if (gameContext.IsRelicActive)
                {
                    consumption = Math.Max(1, consumption - 1);
                }
                totalFoodConsumption += consumption;
            }

            gameContext.AddMessage($"本回合總食物消耗: {totalFoodConsumption}");

            if (gameContext.Food < totalFoodConsumption)
            {
                gameContext.AddMessage($"食物不足！需要 {totalFoodConsumption}, 但只有 {gameContext.Food}。開始裁減角色...");

                // 裁減時也要考慮聖物效果
                var rolesToCull = gameContext.PlayerRoles.OrderByDescending(r =>
                    gameContext.IsRelicActive ? Math.Max(1, r.FoodConsumption - 1) : r.FoodConsumption
                ).ToList();

                foreach (var role in rolesToCull)
                {
                    gameContext.PlayerRoles.Remove(role);
                    gameContext.AddMessage($"裁減了一名 {role.GetType().Name}。");

                    // 重新計算總消耗
                    totalFoodConsumption = gameContext.PlayerRoles.Sum(r =>
                        gameContext.IsRelicActive ? Math.Max(1, r.FoodConsumption - 1) : r.FoodConsumption);

                    if (gameContext.Food >= totalFoodConsumption) break;
                }
            }
            gameContext.Food -= totalFoodConsumption;

            // 床位檢查
            UpdateBeds();
            if (gameContext.RolesCount > gameContext.Beds)
            {
                int excessPopulation = gameContext.RolesCount - gameContext.Beds;
                gameContext.AddMessage($"床位不足！有 {gameContext.RolesCount} 人, 但只有 {gameContext.Beds} 個床位。開始裁減 {excessPopulation} 名角色...");
                CullRolesByType(typeof(Farmer), ref excessPopulation);
                CullRolesByType(typeof(Builder), ref excessPopulation);
                CullRolesByType(typeof(Soldier), ref excessPopulation);
            }
        }

        private void ProcessCombat()
        {
            if (gameContext.FoesCount == 0)
            {
                gameContext.AddMessage("沒有敵人，平安的一回合。");
                return;
            }

            foreach (var foe in gameContext.Foes.ToList())
            {
                var impact = foe.PerformSpecialAction(gameContext);
                if (impact.Food < 0)
                {
                    gameContext.Food += impact.Food;
                    gameContext.AddMessage($"{foe.GetType().Name} 偷走了 {-impact.Food} 食物！");
                }
                if (impact.Buildings < 0)
                {
                    gameContext.BuildingCompletedCount += impact.Buildings;
                    gameContext.AddMessage($"{foe.GetType().Name} 破壞了 {-impact.Buildings} 棟房屋！");
                }
            }
            UpdateBeds();

            int playerCombatPower = gameContext.SoldiersCount;
            gameContext.AddMessage($"我方戰力 (士兵數): {playerCombatPower}");
            int totalFoeCombatPower = gameContext.Foes.Sum(f => f.CombatPower);
            gameContext.AddMessage($"敵人總戰力: {totalFoeCombatPower}");

            var foesToRemove = new List<Foe>();
            var sortedFoes = gameContext.Foes.OrderBy(f => f.CombatPower).ToList();
            foreach (var foe in sortedFoes)
            {
                if (playerCombatPower >= foe.CombatPower)
                {
                    playerCombatPower -= foe.CombatPower;
                    foesToRemove.Add(foe);
                    gameContext.AddMessage($"成功擊殺一名 {foe.GetType().Name}。");
                }
            }
            foesToRemove.ForEach(f => gameContext.Foes.Remove(f));

            // 檢查是否擊敗了聖物持有者
            if (!gameContext.HasRelic && foesToRemove.OfType<RelicHolder>().Any())
            {
                gameContext.HasRelic = true;
                gameContext.AddMessage("你擊敗了聖物持有者！一個神秘的聖物掉落了，它的力量將在下個回合展現。");
            }

            int remainingFoeCombatPower = gameContext.Foes.Sum(f => f.CombatPower);
            if (remainingFoeCombatPower > 0)
            {
                gameContext.AddMessage($"戰鬥失敗！殘存敵人戰力 {remainingFoeCombatPower}，我方將承受傷亡...");
                CullRolesByType(typeof(Soldier), ref remainingFoeCombatPower);
                CullRolesByType(typeof(Farmer), ref remainingFoeCombatPower);
                CullRolesByType(typeof(Builder), ref remainingFoeCombatPower);
            }
            else
            {
                gameContext.AddMessage("戰鬥勝利！所有敵人都被消滅了。");
            }
        }

        private void ProcessFoeCleanup()
        {
            // 根據需求，如果聖物持有者沒有在本回合被擊殺，它就會消失。
            var holders = gameContext.Foes.OfType<RelicHolder>().ToList();
            if (holders.Any())
            {
                foreach (var holder in holders)
                {
                    gameContext.Foes.Remove(holder);
                }
                gameContext.AddMessage("聖物持有者在黎明前消失了...");
            }
        }

        private void ProcessFoeSpawning()
        {
            // 讓聖物持有者在第10回合且場上沒有聖物持有者時，作為Boss出現
            if (gameContext.Turns > 0 && gameContext.Turns % 10 == 0 && !gameContext.Foes.OfType<RelicHolder>().Any())
            {
                gameContext.AddMessage("一個散發著不祥氣息的身影出現了... 是聖物持有者！");
                gameContext.Foes.Add(new RelicHolder());
                return; // Boss出現的回合，不生成其他小怪
            }

            int normalFoesToSpawn = gameContext.Turns / 2;
            int thievesToSpawn = gameContext.Turns / 4;
            int breakersToSpawn = gameContext.Turns / 5;

            for (int i = 0; i < normalFoesToSpawn; i++) gameContext.Foes.Add(new NormalFoe());
            for (int i = 0; i < thievesToSpawn; i++) gameContext.Foes.Add(new FoodThief());
            for (int i = 0; i < breakersToSpawn; i++) gameContext.Foes.Add(new HouseBreaker());

            if (normalFoesToSpawn + thievesToSpawn + breakersToSpawn > 0)
            {
                gameContext.AddMessage($"新的敵人出現了！({normalFoesToSpawn} 一般, {thievesToSpawn} 盜賊, {breakersToSpawn} 破壞者)");
            }
        }

        private void UpdateBeds()
        {
            gameContext.Beds = gameContext.BuildingCompletedCount * 2;
        }

        private void CullRolesByType(Type roleType, ref int amountToCull)
        {
            if (amountToCull <= 0) return;
            var rolesOfType = gameContext.PlayerRoles.Where(r => r.GetType() == roleType).ToList();
            int count = Math.Min(amountToCull, rolesOfType.Count);
            for (int i = 0; i < count; i++)
            {
                gameContext.PlayerRoles.Remove(rolesOfType[i]);
                gameContext.AddMessage($"因故失去了一名 {roleType.Name}。");
            }
            amountToCull -= count;
        }

        private bool CheckGameEnd()
        {
            if (gameContext.RolesCount == 0)
            {
                gameContext.GameFinished = true;
                gameContext.AddMessage("所有角色都陣亡了... 遊戲結束。");
                //System.Windows.Forms.MessageBox.Show("遊戲結束！你的人民無法存活下去。");
                return true;
            }
            return false;
        }

        public readonly struct UserInput(
            int recruitedFarmers,
            int recruitedWheatFarmers, // 新增
            int recruitedRiceFarmers,  // 新增
            int recruitedSoldiers,
            int recruitedBuilders,
            int adjustedFarmers,
            int adjustedSoldiers,
            int adjustedBuilders)
        {
            public int RecruitedFarmers { get; } = recruitedFarmers;
            public int RecruitedWheatFarmers { get; } = recruitedWheatFarmers; // 新增
            public int RecruitedRiceFarmers { get; } = recruitedRiceFarmers;   // 新增
            public int RecruitedSoldiers { get; } = recruitedSoldiers;
            public int RecruitedBuilders { get; } = recruitedBuilders;
            public int AdjustedFarmers { get; } = adjustedFarmers;
            public int AdjustedBuilders { get; } = adjustedBuilders;
            public int AdjustedSoldiers { get; } = adjustedSoldiers;
        }

        public readonly struct InitialSettingUps(
            int food,
            int building,
            int farmers,
            int soldiers,
            int builders,
            int foes)
        {
            public int Food { get; } = food;
            public int Buildings { get; } = building;
            public int Farmers { get; } = farmers;
            public int Builders { get; } = builders;
            public int Soldiers { get; } = soldiers;
            public int Foes { get; } = foes;
        }
    }
}