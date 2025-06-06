namespace GameLogic.Enums
{
    /// <summary>
    /// 代表不同的天氣類型。
    /// </summary>
    public enum WeatherType
    {
        /// <summary>
        /// 一般天氣，沒有特殊效果。
        /// </summary>
        Normal,

        /// <summary>
        /// 炎夏，招募成本增加。
        /// </summary>
        ScorchingSummer,

        /// <summary>
        /// 寒冬，食物產量減少。
        /// </summary>
        ColdWinter
    }
}