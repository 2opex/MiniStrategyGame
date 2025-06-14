namespace GameLogic.Models
{
    /// <summary>
    /// 代表稻米農夫的類別。
    /// </summary>
    public class RiceFarmer : PlayerRole
    {
        public override int FoodConsumption => 1;
    }
}