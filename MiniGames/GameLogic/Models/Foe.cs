using GameLogic;

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
}