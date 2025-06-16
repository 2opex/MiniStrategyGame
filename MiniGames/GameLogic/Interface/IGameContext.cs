using GameAbstract.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Interface
{
    /// <summary>
    /// 提供給 UI 層使用的遊戲狀態唯讀介面
    /// </summary>
    public interface IGameContext
    {
        int Food { get; }
        int Beds { get; }
        int BuildingCompletedCount { get; }
        int RolesCount { get; }
        int FarmersCount { get; }
        int SoldiersCount { get; }
        int BuildersCount { get; }
        int FoesCount { get; }
        int Turns { get; }
        bool GameFinished { get; }
        WeatherType Weather { get; }
        IReadOnlyCollection<string> Messages { get; }
        (bool IsEnough, int Comsumption) IsFoodEnough(int newGeneric, int newWheat, int newRice, int newSoldiers, int newBuilders);
    }

}
