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

    /// <summary>
    /// 代表農夫的類別。
    /// </summary>
    public class Farmer : PlayerRole
    {
        /// <summary>
        /// 覆寫 (override) 基底類別的抽象屬性，提供農夫的具體食物消耗值。
        /// </summary>
        public override int FoodConsumption => 1;
    }

    /// <summary>
    /// 代表稻米農夫的類別。
    /// </summary>
    public class RiceFarmer : PlayerRole
    {
        public override int FoodConsumption => 1;
    }

    /// <summary>
    /// 代表小麥農夫的類別。
    /// </summary>
    public class WheatFarmer : PlayerRole
    {
        // 根據需求，所有農夫的食物消耗都是 1
        public override int FoodConsumption => 1;
    }

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