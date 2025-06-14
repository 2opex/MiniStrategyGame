namespace GameLogic.Models
{
    /// <summary>
    /// 代表玩家角色的抽象基底類別。
    /// 定義了所有角色共有的屬性。
    /// </summary>
    public abstract class PlayerRole
    {
        /// <summary>
        /// 每個角色每回合的食物消耗量。
        /// 這是 abstract，表示每個繼承的子類別都必須提供自己的實作。
        /// </summary>
        public abstract int FoodConsumption { get; }

        /// <summary>
        /// 每個角色佔用的床位。
        /// 根據需求文件，所有角色都只佔用 1 個床位，所以我們可以在基底類別直接定義。
        /// </summary>
        public int BedCost => 1;
    }
}