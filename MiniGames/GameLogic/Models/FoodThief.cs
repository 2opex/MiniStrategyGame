using GameLogic;
using System; // 需要 System 命名空間來使用 Math.Min

namespace GameLogic.Models
{
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
}