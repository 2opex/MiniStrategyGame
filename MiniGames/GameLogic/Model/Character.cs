using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Model
{
    /// <summary>
    /// 代表所有角色的抽象基礎類別
    /// </summary>
    public abstract class Role
    {
        /// <summary>
        /// 每回合食物消耗量
        /// </summary>
        public abstract int FoodConsumption { get; }

        /// <summary>
        /// 每人佔用床位
        /// </summary>
        public virtual int BedConsumption => 1;

        /// <summary>
        /// 招募成本。根據需求文件推斷：士兵消耗3，成本6，因此訂為消耗量的2倍。
        /// </summary>
        public virtual int RecruitmentCost => FoodConsumption * 2;
    }

    /// <summary>
    /// 農夫
    /// </summary>
    public class Farmer : Role
    {
        public override int FoodConsumption => 1;

        /// <summary>
        /// 每回合生產 2 份食物
        /// </summary>
        public int FoodProduction => 2;
    }

    /// <summary>
    /// 建築師
    /// </summary>
    public class Builder : Role
    {
        public override int FoodConsumption => 2;

        /// <summary>
        /// 每回合建造 1 棟房屋
        /// </summary>
        public int BuildingProduction => 1;
    }

    /// <summary>
    /// 士兵
    /// </summary>
    public class Soldier : Role
    {
        public override int FoodConsumption => 3;

        /// <summary>
        /// 每位士兵每回合可消滅 1 名敵人
        /// </summary>
        public int KillPower => 1;
    }
}