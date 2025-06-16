using GameAbstract.Enum;
using GameLogic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Interface
{
    public interface IGameStateManager
    {
        int Turns { get; }
        WeatherType Weather { get; set; }
        bool IsGameFinished { get; set; }
        void NextTurn();
    }

    public interface IMessageManager
    {
        IReadOnlyCollection<string> Messages { get; }
        void AddMessage(string message);
        void ClearMessages();
    }

    public interface IPopulationManager
    {
        IReadOnlyList<Farmer> Farmers { get; }
        IReadOnlyList<Soldier> Soldiers { get; }
        IReadOnlyList<Builder> Builders { get; }
        int TotalRoles { get; }
        void AddFarmer(Farmer farmer);
        void AddSoldier(Soldier soldier);
        void AddBuilder(Builder builder);
        void ClearAndRepopulate(int genericFarmers, int wheatFarmers, int riceFarmers, int soldiers, int builders);
        Role CullRoleForFoodShortage();
        Role CullRoleForBedShortage();
        void RemoveRole(Role role);
        void RemoveRange(List<Role> roles); // 提供一個批次移除的方法
    }

    public interface IResourceManager
    {
        int Food { get; }
        int BuildingCompletedCount { get; }
        int Beds { get; }

        (bool IsEnough, int Comsumption) IsFoodEnoughForRecruitment(int newGeneric, int newWheat, int newRice, int newSoldiers, int newBuilders, bool isHotSummer);

        void SpendFood(int amount);
        void AddFood(int amount);
        void AddBuildings(int amount);
        void DestroyBuildings(int amount); // << 新增這個方法
        void UpdateBeds(int bedsPerBuilding);
    }

    public interface IEnemyManager
    {
        IReadOnlyList<Enemy> Enemies { get; }
        void AddEnemy(Enemy enemy);
        void RemoveEnemy(Enemy enemy);
        void SpawnEnemies(int turn);
        void ClearNewlySpawnedFlags();
    }

    public interface IRelicManager
    {
        IReadOnlyList<Relic> ActiveRelics { get; }
        void ActivatePendingRelics();
        void ProcessRelicDurations();
        void AddPendingRelic(Relic relic);
    }
}
