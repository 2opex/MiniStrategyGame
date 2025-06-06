namespace GameLogic.Models
{
    /// <summary>
    /// 代表一般敵人。
    /// 戰力為 1，沒有特殊行動。
    /// </summary>
    public class NormalFoe : Foe
    {
        public override int CombatPower => 1;
    }
}