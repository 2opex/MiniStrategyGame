namespace GameLogic.Models
{
    /// <summary>
    /// 代表一組資源的資料結構。
    /// 使用 struct 是因為它是一個輕量級的資料容器。
    /// </summary>
    public struct ResourcePacket
    {
        public int Food { get; set; }
        public int Buildings { get; set; }

        // 未來若要擴充，只需在此處新增屬性，例如：
        // public int Wood { get; set; }
        // public int Stone { get; set; }
    }
}