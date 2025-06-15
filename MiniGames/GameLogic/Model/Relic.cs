namespace GameLogic.Model
{
    /// <summary>
    /// 代表所有聖物的抽象基礎類別
    /// </summary>
    public abstract class Relic
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public int Duration { get; set; }

        public Relic(int duration)
        {
            Duration = duration;
        }
    }

    /// <summary>
    /// 聖物：使建築師建造的房屋提供更多床位
    /// </summary>
    public class IronWallRelic : Relic
    {
        public override string Name => "鋼鐵壁壘聖物";
        public override string Description => "建築師的工藝受神啟發，接下來3回合，每棟房屋提供3個床位。";
        public IronWallRelic() : base(3) { }
    }

    /// <summary>
    /// 聖物：增加士兵的攻擊力
    /// </summary>
    public class SharpeningStoneRelic : Relic
    {
        public override string Name => "鋒利磨刀石";
        public override string Description => "士兵的武器更加鋒利，接下來3回合，每位士兵的戰力+1。";
        public SharpeningStoneRelic() : base(3) { }
    }
}