namespace GameLogic.Models
{
    /// <summary>
    /// 代表小麥農夫的類別。
    /// </summary>
    public class WheatFarmer : PlayerRole
    {
        // 根據需求，所有農夫的食物消耗都是 1
        public override int FoodConsumption => 1;
    }
}