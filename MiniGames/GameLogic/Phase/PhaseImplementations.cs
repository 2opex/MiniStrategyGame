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
            var input = context.CurrentUserInput;
            var (IsEnough, Comsumption) = context.IsFoodEnough(input.RecruitedFarmers, input.RecruitedSoldiers, input.RecruitedBuilders);
            if (IsEnough)
            {
                context.Food -= Comsumption;
                for (int i = 0; i < input.RecruitedFarmers; i++) context.Farmers.Add(new Farmer());
                for (int i = 0; i < input.RecruitedSoldiers; i++) context.Soldiers.Add(new Soldier());
                for (int i = 0; i < input.RecruitedBuilders; i++) context.Builders.Add(new Builder());
                context.AddMessage($"招募了 {input.RecruitedFarmers} 農夫, {input.RecruitedSoldiers} 士兵, {input.RecruitedBuilders} 建築師。花費 {Comsumption} 食物。");
            }

            context.Farmers.Clear();
            context.Soldiers.Clear();
            context.Builders.Clear();
            for (int i = 0; i < input.AdjustedFarmers; i++) context.Farmers.Add(new Farmer());
            for (int i = 0; i < input.AdjustedSoldiers; i++) context.Soldiers.Add(new Soldier());
            for (int i = 0; i < input.AdjustedBuilders; i++) context.Builders.Add(new Builder());
            context.AddMessage($"角色數量調整為: {input.AdjustedFarmers} 農夫, {input.AdjustedSoldiers} 士兵, {input.AdjustedBuilders} 建築師。");
        }
    }

    // --- 階段 2: 消耗 ---
    internal class ConsumptionPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {
            if (context.RolesCount <= 0) return;

        }
    }

    // --- 階段 3: 戰鬥 ---
    internal class CombatPhase : ITurnPhase
    {
        public void Execute(GameContext context)
        {

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