namespace GameLogic.Models
{
    /// <summary>
    /// 代表建築師的類別。
    /// </summary>
    public class Builder : PlayerRole, IBuild
    {
        /// <summary>
        /// 提供建築師的具體食物消耗值。
        /// </summary>
        public override int FoodConsumption => 2;

        public int Build()
        {
            return 1; // 建築師每回合可以建造 1 個建築
        }
    }
}