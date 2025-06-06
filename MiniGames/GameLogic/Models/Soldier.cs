namespace GameLogic.Models
{
    /// <summary>
    /// 代表士兵的類別。
    /// </summary>
    public class Soldier : PlayerRole
    {
        /// <summary>
        /// 提供士兵的具體食物消耗值。
        /// </summary>
        public override int FoodConsumption => 3;
    }
}