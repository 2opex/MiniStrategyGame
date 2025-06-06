namespace GameLogic.Models
{
    /// <summary>
    /// 代表農夫的類別。
    /// </summary>
    public class Farmer : PlayerRole
    {
        /// <summary>
        /// 覆寫 (override) 基底類別的抽象屬性，提供農夫的具體食物消耗值。
        /// </summary>
        public override int FoodConsumption => 1;

        public override ResourcePacket PerformProduction(IGameContext context)
        {
            // 回傳一個只包含食物的資源包裹
            return new ResourcePacket { Food = 2 };
        }
    }
}