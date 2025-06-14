using GameLogic;
using System;

namespace GameLogic.Models
{
    public abstract class Foe
    {
        public abstract int CombatPower { get; }

        /// <summary>
        /// 敵人進行特殊行動，並回傳其造成的影響。
        /// </summary>
        /// <param name="context">唯讀的遊戲狀態，供決策使用。</param>
        /// <returns>一個包含資源變動的資源包裹</returns>
        public virtual ResourcePacket PerformSpecialAction(IGameContext context)
        {
            // 預設回傳一個空的資源包裹，表示沒有造成影響
            return new ResourcePacket();
        }
    }

    /// <summary>
    /// 代表一般敵人。
    /// 戰力為 1，沒有特殊行動。
    /// </summary>
    public class NormalFoe : Foe
    {
        public override int CombatPower => 1;
    }

    public class FoodThief : Foe
    {
        public override int CombatPower => 1;

        public override ResourcePacket PerformSpecialAction(IGameContext context)
        {
            int stealAmount = 3;
            // 盜賊最多只能偷走目前有的食物
            int foodToSteal = Math.Min(stealAmount, context.Food);

            if (foodToSteal > 0)
            {
                // 回傳一個食物為負值的資源包裹，表示資源"減少"
                return new ResourcePacket { Food = -foodToSteal };
            }

            return new ResourcePacket();
        }
    }

    /// <summary>
    /// 代表聖物持有者。
    /// 一個強大但短暫存在的敵人。
    /// </summary>
    public class RelicHolder : Foe
    {
        /// <summary>
        /// 根據需求文件，戰力相當於 5 名士兵。
        /// </summary>
        public override int CombatPower => 5;
    }

    /// <summary>
    /// 代表房屋破壞者。
    /// </summary>
    public class HouseBreaker : Foe
    {
        /// <summary>
        /// 戰力相當於 2 名士兵。
        /// </summary>
        public override int CombatPower => 2;

        /// <summary>
        /// 優先破壞房屋。
        /// </summary>
        public override ResourcePacket PerformSpecialAction(IGameContext context)
        {
            // 如果還有房屋可以破壞
            if (context.BuildingCompletedCount > 0)
            {
                // 回傳一個建築為負值的資源包裹，表示資源"減少"
                return new ResourcePacket { Buildings = -1 };
            }

            return new ResourcePacket();
        }
    }
}