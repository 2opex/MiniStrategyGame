using System;
using System.Linq;
using GameAbstract.Enum;
using GameLogic.Interface;
using GameLogic.Model;

namespace GameLogic.Manager.Phases
{
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

        }
    }

    // --- 階段 2: 消耗 ---
    internal class ConsumptionPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
        }
    }

    // --- 階段 3: 戰鬥 ---
    internal class CombatPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {

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
    /// 生長與收成階段：推進作物生長，並收成已成熟的作物
    /// </summary>
    internal class GrowthHarvestPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            foreach (var farmer in context.Farmers)
            {
                var crop = farmer.TendedCrop;
                if (crop != null && !crop.IsMature)
                {
                    // 規則：寒冬時，水稻停止生長
                    if (crop is Rice && context.Weather == WeatherType.ColdWinter)
                    {
                        context.AddMessage("寒冬來臨，水稻停止生長。");
                        continue; // 跳過此作物的生長
                    }

                    crop.CurrentGrowth++;
                }

                // 檢查是否成熟並收成
                if (crop != null && crop.IsMature)
                {
                    int finalYield = crop.Yield;

                    // 規則：寒冬時，一般作物產量減半
                    if (crop is GeneralCrop && context.Weather == WeatherType.ColdWinter)
                    {
                        finalYield /= 2;
                        context.AddMessage($"寒冬影響收成！");
                    }

                    context.Food += finalYield;
                    context.AddMessage($"作物 {crop.Name} 已成熟，收穫了 {finalYield} 份食物。");
                    farmer.TendedCrop = null; // 收成後，農夫變為閒置狀態
                }
            }
        }
    }


    // --- 階段 4: 生產 ---
    internal class ProductionPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {

        }
    }

    // --- 階段 5: 敵人生成 ---
    internal class EnemySpawningPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            int newFoes = (context.Turns / 2) + 1;
            context.FoesCount += newFoes;
            context.AddMessage($"地平線出現了 {newFoes} 名新的敵人！");
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