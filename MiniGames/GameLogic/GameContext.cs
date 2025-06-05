using System.Collections.Generic;

namespace GameLogic
{
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
        IReadOnlyCollection<string> Messages { get; }
        (bool IsEnough, int Comsumption) IsFoodEnough(int value, int i, int value1);
    }
}