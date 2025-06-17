using System;
using System.Collections.Generic;
using System.Linq;
using GameAbstract.Enum;
using GameLogic.Interface;
using GameLogic.Model;

namespace GameLogic.Manager.Phases
{

    internal class RelicActivationPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            // 1. 啟動等待中的聖物
            var pendingRelicsCount = context.RelicsManager.ActiveRelics.Count();
            context.RelicsManager.ActivatePendingRelics();
            if (context.RelicsManager.ActiveRelics.Count > pendingRelicsCount && pendingRelicsCount != context.RelicsManager.ActiveRelics.Count)
            {
                context.AddMessage($"聖物的力量開始湧現！");
            }

            // 2. 處理已生效聖物的持續時間
            var relicCountBeforeProcessing = context.RelicsManager.ActiveRelics.Count;
            context.RelicsManager.ProcessRelicDurations();
            if (context.RelicsManager.ActiveRelics.Count < relicCountBeforeProcessing)
            {
                context.AddMessage($"有聖物的力量消退了。");
            }
        }
    }

    // --- 階段 0: 天氣 ---
    internal class WeatherPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            var gameState = context.GameStateManager;
            if (gameState.Turns % 2 == 1 && gameState.Turns > 1)
            {
                var random = new Random();
                var weatherValues = Enum.GetValues(typeof(WeatherType));
                gameState.Weather = (WeatherType)weatherValues.GetValue(random.Next(weatherValues.Length));
                context.AddMessage($"天氣變了！現在是 {GetWeatherString(gameState.Weather)}。");
            }
            else
            {
                context.AddMessage($"天氣是 {GetWeatherString(gameState.Weather)}。");
            }
        }
        private string GetWeatherString(WeatherType weather)
        {
            return weather switch
            {
                WeatherType.HotSummer => "炎夏",
                WeatherType.ColdWinter => "寒冬",
                _ => "一般",
            };
        }
    }

    internal class PlayerActionPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            var input = context.CurrentUserInput;

            // --- 處理角色數量調整 ---
            context.PopulationManager.ClearAndRepopulate(input.AdjustedGenericFarmers, input.AdjustedWheatFarmers, input.AdjustedRiceFarmers, input.AdjustedSoldiers, input.AdjustedBuilders);
            context.AddMessage($"角色數量已調整。");

            // --- 處理招募 ---
            var (isEnough, comsumption) = context.IsFoodEnough(input.RecruitedGenericFarmers, input.RecruitedWheatFarmers, input.RecruitedRiceFarmers, input.RecruitedSoldiers, input.RecruitedBuilders);

            // 只有在食物足夠且玩家有招募意圖時才執行
            if (isEnough && comsumption > 0)
            {
                context.ResourcesManager.SpendFood(comsumption);
                for (int i = 0; i < input.RecruitedGenericFarmers; i++) context.PopulationManager.AddFarmer(new GenericFarmer());
                for (int i = 0; i < input.RecruitedWheatFarmers; i++) context.PopulationManager.AddFarmer(new WheatFarmer());
                for (int i = 0; i < input.RecruitedRiceFarmers; i++) context.PopulationManager.AddFarmer(new RiceFarmer());
                for (int i = 0; i < input.RecruitedSoldiers; i++) context.PopulationManager.AddSoldier(new Soldier());
                for (int i = 0; i < input.RecruitedBuilders; i++) context.PopulationManager.AddBuilder(new Builder());
                context.AddMessage($"招募完成，花費 {comsumption} 食物。");
            }
        }
    }

    internal class EnemyActionPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            if (!context.EnemiesManager.Enemies.Any()) return;

            // 在敵人行動前，重置他們當前回合的戰鬥力為其基礎戰鬥力
            foreach (var enemy in context.EnemiesManager.Enemies)
            {
                enemy.CurrentTurnCombatPower = enemy.CombatPower;
            }

            var oldRelicHolders = context.EnemiesManager.Enemies.OfType<RelicHolder>().Where(e => !e.IsNewlySpawned).ToList();
            if (oldRelicHolders.Any())
            {
                foreach (var holder in oldRelicHolders)
                {
                    context.EnemiesManager.RemoveEnemy(holder);
                }
                context.AddMessage($"未被擊敗的 {oldRelicHolders.First().Name} 消失在迷霧中...");
            }

            context.AddMessage("敵人開始行動...");
            foreach (var enemy in context.EnemiesManager.Enemies.ToList()) // ToList() to copy
            {
                enemy.ExecuteSpecialAbility(context);
            }

            context.EnemiesManager.ClearNewlySpawnedFlags();
        }
    }

    internal class ConsumptionPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            if (context.PopulationManager.TotalRoles <= 0) return;

            // 1. 食物消耗
            int totalFoodConsumption = context.PopulationManager.Farmers.Sum(f => f.FoodConsumption) +
                                       context.PopulationManager.Soldiers.Sum(s => s.FoodConsumption) +
                                       context.PopulationManager.Builders.Sum(b => b.FoodConsumption);
            context.AddMessage($"本回合總食物需求: {totalFoodConsumption}。");

            while (context.ResourcesManager.Food < totalFoodConsumption && context.PopulationManager.TotalRoles > 0)
            {
                context.AddMessage("食物不足！開始依規則裁減角色...");
                Role roleToCull = context.PopulationManager.CullRoleForFoodShortage();
                if (roleToCull != null)
                {
                    context.PopulationManager.RemoveRole(roleToCull);
                    context.AddMessage($"因食物不足，裁減了一名角色。");
                    totalFoodConsumption = context.PopulationManager.Farmers.Sum(f => f.FoodConsumption) +
                                           context.PopulationManager.Soldiers.Sum(s => s.FoodConsumption) +
                                           context.PopulationManager.Builders.Sum(b => b.FoodConsumption);
                }
            }
            context.ResourcesManager.SpendFood(totalFoodConsumption);
            context.AddMessage($"食物消耗完畢，剩餘 {context.ResourcesManager.Food} 食物。");

            // 2. 床位檢查
            while (context.PopulationManager.TotalRoles > context.ResourcesManager.Beds)
            {
                context.AddMessage("床位不足！開始依規則裁減角色...");
                Role roleToCull = context.PopulationManager.CullRoleForBedShortage();
                if (roleToCull != null)
                {
                    context.PopulationManager.RemoveRole(roleToCull);
                    context.AddMessage($"因床位不足，裁減了一名角色。");
                }
                else break;
            }
        }
    }

    internal class CombatPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            if (!context.EnemiesManager.Enemies.Any()) return;

            int extraPowerFromRelic = context.RelicsManager.ActiveRelics.OfType<SharpeningStoneRelic>().Any() ? context.PopulationManager.Soldiers.Count : 0;
            if (extraPowerFromRelic > 0) context.AddMessage("在聖物的加持下，士兵們的戰力大增！");

            int playerAttackPower = context.PopulationManager.Soldiers.Sum(s => s.KillPower) + extraPowerFromRelic;
            int totalEnemyCombatPower = context.EnemiesManager.Enemies.Sum(e => e.CurrentTurnCombatPower);
            context.AddMessage($"我方士兵總戰力為 {playerAttackPower}，敵方總戰力為 {totalEnemyCombatPower}。");

            var sortedEnemies = context.EnemiesManager.Enemies.OrderBy(e => e.CombatPower).ToList();
            int defeatedPower = 0;
            foreach (var enemy in sortedEnemies)
            {
                if (defeatedPower + enemy.CurrentTurnCombatPower <= playerAttackPower)
                {
                    defeatedPower += enemy.CurrentTurnCombatPower;
                    context.EnemiesManager.RemoveEnemy(enemy);
                    context.AddMessage($"擊敗了一名 {enemy.Name}！");

                    if (enemy is RelicHolder)
                    {
                        context.AddMessage("聖物持有者掉落了一個神秘的聖物！它將在下回合生效。");
                        var relics = new Relic[] { new IronWallRelic(), new SharpeningStoneRelic() };
                        var droppedRelic = relics[new Random().Next(relics.Length)];
                        context.RelicsManager.AddPendingRelic(droppedRelic);
                    }
                }
            }

            int remainingEnemyPowerForCasualty = context.EnemiesManager.Enemies.Sum(e => e.CurrentTurnCombatPower);
            if (remainingEnemyPowerForCasualty > 0)
            {
                context.AddMessage($"敵人反擊！我方承受 {remainingEnemyPowerForCasualty} 點戰力衝擊...");

                var rolesToRemove = new List<Role>();
                int casualties = remainingEnemyPowerForCasualty;

                var soldiersLost = context.PopulationManager.Soldiers.Take(casualties).ToList();
                rolesToRemove.AddRange(soldiersLost);
                casualties -= soldiersLost.Count;
                if (soldiersLost.Any()) context.AddMessage($"損失了 {soldiersLost.Count} 名士兵。");

                if (casualties > 0)
                {
                    var farmersLost = context.PopulationManager.Farmers.Take(casualties).ToList();
                    rolesToRemove.AddRange(farmersLost);
                    casualties -= farmersLost.Count;
                    if (farmersLost.Any()) context.AddMessage($"損失了 {farmersLost.Count} 名農夫。");
                }

                if (casualties > 0)
                {
                    var buildersLost = context.PopulationManager.Builders.Take(casualties).ToList();
                    rolesToRemove.AddRange(buildersLost);
                    if (buildersLost.Any()) context.AddMessage($"損失了 {buildersLost.Count} 名建築師。");
                }

                rolesToRemove.ForEach(r => context.PopulationManager.RemoveRole(r));
            }
        }
    }

    internal class PlantingPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            foreach (var farmer in context.PopulationManager.Farmers)
            {
                if (farmer.TendedCrop == null)
                {
                    farmer.TendedCrop = CropFactory.CreateCrop(farmer);
                    context.AddMessage($"一位農夫種下了新的 {farmer.TendedCrop.Name}。");
                }
            }
        }
    }

    internal class CropGrowthPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            foreach (var farmer in context.PopulationManager.Farmers.ToList())
            {
                var crop = farmer.TendedCrop;
                if (crop != null && !crop.IsMature)
                {
                    if (crop is Rice && context.Weather == WeatherType.ColdWinter)
                    {
                        context.AddMessage("寒冬來臨，水稻停止生長。");
                        continue;
                    }
                    crop.CurrentGrowth++;
                }

                if (crop != null && crop.IsMature)
                {
                    int finalYield = crop.Yield;
                    if (crop is GeneralCrop && context.Weather == WeatherType.ColdWinter)
                    {
                        finalYield /= 2;
                        context.AddMessage($"寒冬影響收成！");
                    }
                    context.ResourcesManager.AddFood(finalYield);
                    context.AddMessage($"作物 {crop.Name} 已成熟，收穫了 {finalYield} 份食物。");
                    farmer.TendedCrop = null;
                }
            }
        }
    }

    internal class BuilderProductionPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            if (context.PopulationManager.Builders.Any())
            {
                int buildingsProduced = context.PopulationManager.Builders.Count;
                context.ResourcesManager.AddBuildings(buildingsProduced);

                int bedsPerBuilding = context.RelicsManager.ActiveRelics.OfType<IronWallRelic>().Any() ? 3 : 2;
                if (bedsPerBuilding > 2) context.AddMessage("在聖物的加持下，房屋變得更加堅固寬敞！");

                context.ResourcesManager.UpdateBeds(bedsPerBuilding);
                context.AddMessage($"建築師們新建了 {buildingsProduced} 棟房屋，總床位數更新為 {context.ResourcesManager.Beds}。");
            }
        }
    }

    internal class EnemySpawningPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            context.EnemiesManager.SpawnEnemies(context.GameStateManager.Turns);
            context.AddMessage($"地平線出現了新的敵人！");
        }
    }

    internal class EndOfTurnPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            if (context.PopulationManager.TotalRoles <= 0)
            {
                context.GameStateManager.IsGameFinished = true;
                context.AddMessage("你失去了所有的人民...遊戲結束。");
            }
            else
            {
                context.GameStateManager.NextTurn();
            }
        }
    }
}