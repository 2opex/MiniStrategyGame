namespace GameLogic.Models
{
    /// <summary>
    /// 代表小麥農夫的類別。
    /// </summary>
    public class WheatFarmer : PlayerRole
    {
        // 根據需求，所有農夫的食物消耗都是 1
        public override int FoodConsumption => 1;

        // 注意：我們將不再使用這個方法來生產食物，
        // 生產邏輯會統一在 TurnProcessor 中處理。
        // 保留這個方法是為了符合 PlayerRole 的抽象定義。
        public override ResourcePacket PerformProduction(IGameContext context)
        {
            return new ResourcePacket(); // 回傳空包裹
        }
    }
}