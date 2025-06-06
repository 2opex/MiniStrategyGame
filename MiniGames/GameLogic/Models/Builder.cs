namespace GameLogic.Models
{
    /// <summary>
    /// 代表建築師的類別。
    /// </summary>
    public class Builder : PlayerRole
    {
        /// <summary>
        /// 提供建築師的具體食物消耗值。
        /// </summary>
        public override int FoodConsumption => 2;

        public override ResourcePacket PerformProduction(IGameContext context)
        {
            // 回傳一個只包含建築的資源包裹
            return new ResourcePacket { Buildings = 1 };
        }
    }
}