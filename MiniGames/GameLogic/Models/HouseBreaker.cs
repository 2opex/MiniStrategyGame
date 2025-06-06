using GameLogic;

namespace GameLogic.Models
{
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