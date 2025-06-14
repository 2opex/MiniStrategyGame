// 檔案路徑: MiniGames/GameLogic/Models/Farmer.cs
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
    }
}