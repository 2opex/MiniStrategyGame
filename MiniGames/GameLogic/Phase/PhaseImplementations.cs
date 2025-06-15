using System;
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
            if (context.PendingRelics.Any())
            {
                foreach (var relic in context.PendingRelics)
                {
                    context.ActiveRelics.Add(relic);
                    context.AddMessage($"聖物 {relic.Name} 的力量開始湧現！");
                }
                context.PendingRelics.Clear();
            }

            // 2. 處理已生效聖物的持續時間
            if (context.ActiveRelics.Any())
            {
                // 從後往前遍歷以安全地移除元素
                for (int i = context.ActiveRelics.Count - 1; i >= 0; i--)
                {
                    var relic = context.ActiveRelics[i];
                    relic.Duration--;
                    if (relic.Duration <= 0)
                    {
                        context.AddMessage($"聖物 {relic.Name} 的力量消退了。");
                        context.ActiveRelics.RemoveAt(i);
                    }
                }
            }
        }
    }

    // --- 階段 0: 天氣 ---
    internal class WeatherPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            if (context.Turns % 2 == 1 && context.Turns > 1)
            {
                var random = new Random();
                var weatherValues = Enum.GetValues(typeof(WeatherType));
                context.Weather = (WeatherType)weatherValues.GetValue(random.Next(weatherValues.Length));
                context.AddMessage($"天氣變了！現在是 {GetWeatherString(context.Weather)}。");
            }
            else
            {
                context.AddMessage($"天氣是 {GetWeatherString(context.Weather)}。");
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

    // --- 階段 1: 玩家操作 ---
    internal class PlayerActionPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            var input = context.CurrentUserInput;

            // --- 處理招募 ---
            var (IsEnough, Comsumption) = context.IsFoodEnough(input.RecruitedGenericFarmers, input.RecruitedWheatFarmers, input.RecruitedRiceFarmers, input.RecruitedSoldiers, input.RecruitedBuilders);
            if (IsEnough)
            {
                context.Food -= Comsumption;
                for (int i = 0; i < input.RecruitedGenericFarmers; i++) context.Farmers.Add(new GenericFarmer());
                for (int i = 0; i < input.RecruitedWheatFarmers; i++) context.Farmers.Add(new WheatFarmer());
                for (int i = 0; i < input.RecruitedRiceFarmers; i++) context.Farmers.Add(new RiceFarmer());
                for (int i = 0; i < input.RecruitedSoldiers; i++) context.Soldiers.Add(new Soldier());
                for (int i = 0; i < input.RecruitedBuilders; i++) context.Builders.Add(new Builder());
                context.AddMessage($"招募完成，花費 {Comsumption} 食物。");
            }

            // --- 處理角色數量調整 ---
            context.Farmers.Clear();
            context.Soldiers.Clear();
            context.Builders.Clear();

            for (int i = 0; i < input.AdjustedGenericFarmers; i++) context.Farmers.Add(new GenericFarmer());
            for (int i = 0; i < input.AdjustedWheatFarmers; i++) context.Farmers.Add(new WheatFarmer());
            for (int i = 0; i < input.AdjustedRiceFarmers; i++) context.Farmers.Add(new RiceFarmer());
            for (int i = 0; i < input.AdjustedSoldiers; i++) context.Soldiers.Add(new Soldier());
            for (int i = 0; i < input.AdjustedBuilders; i++) context.Builders.Add(new Builder());
            context.AddMessage($"角色數量已調整。");
        }
    }

    /// <summary>
    /// 敵人行動階段：處理所有敵人的特殊能力
    /// </summary>
    internal class EnemyActionPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            if (context.Enemies.Count == 0) return;

            var oldRelicHolders = context.Enemies.OfType<RelicHolder    >().Where(e => !e.IsNewlySpawned).ToList();
            if (oldRelicHolders.Any())
            {
                foreach (var holder in oldRelicHolders)
                {
                    context.Enemies.Remove(holder);
                }
                context.AddMessage($"未被擊敗的 {oldRelicHolders.First().Name} 消失在迷霧中...");
            }

            context.AddMessage("敵人開始行動...");
            // 使用 ToList() 複製一份列表，避免在迭代中修改原列表
            foreach (var enemy in context.Enemies.ToList())
            {
                enemy.ExecuteSpecialAbility(context);
            }

            // 在階段結束時，將所有現存敵人的 IsNewlySpawned 標記設為 false
            foreach (var enemy in context.Enemies)
            {
                enemy.IsNewlySpawned = false;
            }
        }
    }

    // --- 階段 2: 消耗 ---
    internal class ConsumptionPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            if (context.RolesCount <= 0) return;

            // 1. 食物消耗
            int totalFoodConsumption = context.Farmers.Sum(f => f.FoodConsumption) +
                                       context.Soldiers.Sum(s => s.FoodConsumption) +
                                       context.Builders.Sum(b => b.FoodConsumption);
            context.AddMessage($"本回合總食物需求: {totalFoodConsumption}。");

            while (context.Food < totalFoodConsumption && context.RolesCount > 0)
            {
                context.AddMessage("食物不足！開始依規則裁減角色...");
                // 規則: 先裁減消耗量最大的角色 (士兵(3) > 建築師(2) > 農夫(1))
                Role roleToCull = null;
                if (context.Soldiers.Any()) roleToCull = context.Soldiers.First();
                else if (context.Builders.Any()) roleToCull = context.Builders.First();
                else if (context.Farmers.Any()) roleToCull = context.Farmers.First();

                if (roleToCull != null)
                {
                    if (roleToCull is Soldier) context.Soldiers.RemoveAt(0);
                    else if (roleToCull is Builder) context.Builders.RemoveAt(0);
                    else if (roleToCull is Farmer) context.Farmers.RemoveAt(0);
                    context.AddMessage($"因食物不足，裁減了一名角色。");
                    totalFoodConsumption = context.Farmers.Sum(f => f.FoodConsumption) + context.Soldiers.Sum(s => s.FoodConsumption) + context.Builders.Sum(b => b.FoodConsumption);
                }
            }
            context.Food -= totalFoodConsumption;
            context.AddMessage($"食物消耗完畢，剩餘 {context.Food} 食物。");

            // 2. 床位檢查
            context.Beds = context.BuildingCompletedCount * 2;
            while (context.RolesCount > context.Beds)
            {
                context.AddMessage("床位不足！開始依規則裁減角色...");
                // 規則: 先裁減 農夫 > 建築師 > 士兵
                if (context.Farmers.Any()) context.Farmers.RemoveAt(0);
                else if (context.Builders.Any()) context.Builders.RemoveAt(0);
                else if (context.Soldiers.Any()) context.Soldiers.RemoveAt(0);
                else break; // Should not happen
                context.AddMessage($"因床位不足，裁減了一名角色。");
            }
        }
    }

    // --- 階段 3: 戰鬥 ---
    internal class CombatPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            if (context.Enemies.Count == 0) return;

            // 聖物效果：鋒利磨刀石增加士兵戰力
            int extraPowerFromRelic = context.ActiveRelics.OfType<SharpeningStoneRelic>().Any() ? context.Soldiers.Count : 0;
            if (extraPowerFromRelic > 0) context.AddMessage("在聖物的加持下，士兵們的戰力大增！");
            int playerAttackPower = context.Soldiers.Sum(s => s.KillPower) + extraPowerFromRelic;
            int totalEnemyCombatPower = context.Enemies.Sum(e => e.CombatPower);

            context.AddMessage($"我方士兵總戰力為 {playerAttackPower}，敵方總戰力為 {totalEnemyCombatPower}。");

            // 我方攻擊
            int remainingEnemyPower = totalEnemyCombatPower - playerAttackPower;

            // 移除被擊敗的敵人
            if (playerAttackPower > 0)
            {
                // 從最弱的敵人開始移除
                var sortedEnemies = context.Enemies.OrderBy(e => e.CombatPower).ToList();
                int defeatedPower = 0;
                foreach (var enemy in sortedEnemies)
                {
                    if (defeatedPower + enemy.CombatPower <= playerAttackPower)
                    {
                        defeatedPower += enemy.CombatPower;
                        context.Enemies.Remove(enemy);
                        context.AddMessage($"擊敗了一名 {enemy.Name}！");

                        // 規則：如果擊敗的是聖物持有者，則掉落聖物
                        if (enemy is RelicHolder)
                        {
                            context.AddMessage("聖物持有者掉落了一個神秘的聖物！它將在下回合生效。");
                            // 隨機掉落一種聖物
                            var relics = new Relic[] { new IronWallRelic(), new SharpeningStoneRelic() };
                            var droppedRelic = relics[new Random().Next(relics.Length)];
                            context.PendingRelics.Add(droppedRelic);
                        }
                    }
                }
            }

            // 敵方反擊 (如果仍有剩餘戰力)
            int remainingPlayerPowerForCasualty = context.Enemies.Sum(e => e.CombatPower);
            if (remainingPlayerPowerForCasualty > 0)
            {
                context.AddMessage($"敵人反擊！我方承受 {remainingPlayerPowerForCasualty} 點戰力衝擊...");
                // 規則: 依 士兵 > 農夫 > 建築師 順序承受傷亡
                int casualties = remainingPlayerPowerForCasualty;

                int culled = System.Math.Min(casualties, context.Soldiers.Count);
                if (culled > 0) { context.Soldiers.RemoveRange(0, culled); casualties -= culled; context.AddMessage($"損失了 {culled} 名士兵。"); }

                culled = System.Math.Min(casualties, context.Farmers.Count);
                if (culled > 0) { context.Farmers.RemoveRange(0, culled); casualties -= culled; context.AddMessage($"損失了 {culled} 名農夫。"); }

                culled = System.Math.Min(casualties, context.Builders.Count);
                if (culled > 0) { context.Builders.RemoveRange(0, culled); context.AddMessage($"損失了 {culled} 名建築師。"); }
            }
        }
    }

    /// <summary>
    /// 種植階段：所有閒置的農夫會種下新的作物
    /// </summary>
    internal class PlantingPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            foreach (var farmer in context.Farmers)
            {
                // 如果農夫沒有正在照料的作物 (閒置中)
                if (farmer.TendedCrop == null)
                {
                    farmer.TendedCrop = farmer.CreateCrop();
                    context.AddMessage($"一位農夫種下了新的 {farmer.TendedCrop.Name}。");
                }
            }
        }
    }

    /// <summary>
    /// 作物生長與收成階段：只處理農夫的生產活動
    /// </summary>
    internal class CropGrowthPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            // 農夫的作物生長與收成邏輯維持不變
            foreach (var farmer in context.Farmers.ToList())
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
                    context.Food += finalYield;
                    context.AddMessage($"作物 {crop.Name} 已成熟，收穫了 {finalYield} 份食物。");
                    farmer.TendedCrop = null;
                }
            }
        }
    }

    /// <summary>
    /// 建築師生產階段：處理所有建築師的建造活動
    /// </summary>
    internal class BuilderProductionPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            if (context.Builders.Any())
            {
                int buildingsProduced = context.Builders.Count;
                context.BuildingCompletedCount += buildingsProduced;

                // 聖物效果：鋼鐵壁壘增加床位產量
                int bedsPerBuilding = context.ActiveRelics.OfType<IronWallRelic>().Any() ? 3 : 2;
                if (bedsPerBuilding > 2) context.AddMessage("在聖物的加持下，房屋變得更加堅固寬敞！");

                context.Beds = context.BuildingCompletedCount * bedsPerBuilding;
                context.AddMessage($"建築師們新建了 {buildingsProduced} 棟房屋，總床位數更新為 {context.Beds}。");
            }
        }
    }

    // --- 階段 5: 敵人生成 ---
    internal class EnemySpawningPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            // 簡易生成邏輯：隨著回合數增加，生成更強的敵人
            int newFoesCount = (context.Turns / 3) + 1;
            context.AddMessage($"地平線出現了 {newFoesCount} 名新的敵人！");

            for (int i = 0; i < newFoesCount; i++)
            {
                if (context.Turns > 10 && i % 3 == 0)
                {
                    context.Enemies.Add(new HouseBreaker());
                }
                else if (context.Turns > 5 && i % 2 == 0)
                {
                    context.Enemies.Add(new FoodThief());
                }
                else
                {
                    context.Enemies.Add(new GenericEnemy());
                }
            }
        }
    }

    // --- 階段 6: 回合結束 ---
    internal class EndOfTurnPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            if (context.RolesCount <= 0)
            {
                context.GameFinished = true;
                context.AddMessage("你失去了所有的人民...遊戲結束。");
            }
            else
            {
                context.Turns++;
            }
        }
    }
}