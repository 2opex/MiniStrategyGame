namespace GameLogic.Models
{
    /// <summary>
    /// 代表聖物持有者。
    /// 一個強大但短暫存在的敵人。
    /// </summary>
    public class RelicHolder : Foe
    {
        /// <summary>
        /// 根據需求文件，戰力相當於 5 名士兵。
        /// </summary>
        public override int CombatPower => 5;
    }
}