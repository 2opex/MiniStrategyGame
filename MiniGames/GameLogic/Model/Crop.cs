namespace GameLogic.Model
{
    /// <summary>
    /// 代表所有農作物的抽象基礎類別
    /// </summary>
    public abstract class Crop
    {
        public abstract string Name { get; }
        public abstract int TotalGrowthTime { get; }
        public abstract int Yield { get; }
        public int CurrentGrowth { get; set; } = 0;
        public bool IsMature => CurrentGrowth >= TotalGrowthTime;
    }

    /// <summary>
    /// 一般作物
    /// </summary>
    public class GeneralCrop : Crop
    {
        public override string Name => "一般作物";
        public override int TotalGrowthTime => 1;
        public override int Yield => 2;
    }

    /// <summary>
    /// 小麥
    /// </summary>
    public class Wheat : Crop
    {
        public override string Name => "小麥";
        public override int TotalGrowthTime => 3;
        public override int Yield => 6;
    }

    /// <summary>
    /// 水稻
    /// </summary>
    public class Rice : Crop
    {
        public override string Name => "水稻";
        public override int TotalGrowthTime => 3;
        public override int Yield => 8;
    }
}