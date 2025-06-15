namespace GameLogic.Model
{
    /// <summary>
    /// 代表所有敵人的抽象基礎類別
    /// </summary>
    public abstract class Enemy
    {
        public abstract string Name { get; }
        /// <summary>
        /// 戰力，相當於需要多少名我方士兵才能抵銷
        /// </summary>
        public abstract int CombatPower { get; }

        // 新增一個屬性來標記敵人是否為當回合新生成的
        public bool IsNewlySpawned { get; set; } = true;

        /// <summary>
        /// 執行戰鬥前的特殊能力
        /// </summary>
        internal virtual void ExecuteSpecialAbility(GameContext context)
        {
            // 預設敵人沒有特殊能力
        }
    }

    /// <summary>
    /// 一般敵人
    /// </summary>
    public class GenericEnemy : Enemy
    {
        public override string Name => "一般敵人";
        public override int CombatPower => 1;
    }

    /// <summary>
    /// 食物盜賊
    /// </summary>
    public class FoodThief : Enemy
    {
        public override string Name => "食物盜賊";
        public override int CombatPower => 1;

        internal override void ExecuteSpecialAbility(GameContext context)
        {
            int foodToSteal = 3;
            int stolenAmount = System.Math.Min(context.Food, foodToSteal);
            if (stolenAmount > 0)
            {
                context.Food -= stolenAmount;
                context.AddMessage($"{Name} 偷走了 {stolenAmount} 份食物！");
            }
        }
    }

    /// <summary>
    /// 房屋破壞者
    /// </summary>
    public class HouseBreaker : Enemy
    {
        public override string Name => "房屋破壞者";
        public override int CombatPower => 2;

        internal override void ExecuteSpecialAbility(GameContext context)
        {
            int buildingsToDestroy = 1;
            if (context.BuildingCompletedCount >= buildingsToDestroy)
            {
                context.BuildingCompletedCount -= buildingsToDestroy;
                context.Beds = context.BuildingCompletedCount * 2;
                context.AddMessage($"{Name} 摧毀了 {buildingsToDestroy} 棟房屋！");
            }
        }
    }

    /// <summary>
    /// 聖物持有者
    /// </summary>
    public class RelicHolder : Enemy
    {
        public override string Name => "聖物持有者";
        public override int CombatPower => 5;
    }
}