namespace GameLogic.Models
{
    // 定義一個列舉來表示作物的類型，方便管理
    public enum CropType
    {
        General, // 一般作物
        Wheat,   // 小麥
        Rice     // 水稻
    }

    /// <summary>
    /// 代表一個正在生長的農作物。
    /// </summary>
    public class Crop
    {
        public CropType Type { get; }
        public int Yield { get; }      // 收成時的食物產量
        public int GrowthTime { get; } // 需要的生長回合數
        public int PlantedTurn { get; }  // 種植下去的回合
        public int TurnsGrown { get; set; } // 已經生長的回合數

        public bool IsStalled { get; set; } // 是否因天氣等因素暫停生長

        public Crop(CropType type, int plantedTurn)
        {
            Type = type;
            PlantedTurn = plantedTurn;
            TurnsGrown = 0;
            IsStalled = false;

            // 根據需求文件初始化不同作物的屬性
            switch (type)
            {
                case CropType.Wheat:
                    Yield = 6;
                    GrowthTime = 3;
                    break;
                case CropType.Rice:
                    Yield = 8;
                    GrowthTime = 3;
                    break;
                case CropType.General:
                default:
                    Yield = 2;
                    GrowthTime = 1;
                    break;
            }
        }

        /// <summary>
        /// 檢查作物是否成熟。
        /// </summary>
        public bool IsMature => !IsStalled && TurnsGrown >= GrowthTime;
    }
}