using GameAbstract.Enum;
using GameLogic.Interface;
using GameLogic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Manager
{
    #region Manager Implementations

    internal class GameStateManager : IGameStateManager
    {
        public int Turns { get; private set; } = 1;
        public WeatherType Weather { get; set; } = WeatherType.Normal;
        public bool IsGameFinished { get; set; } = false;
        public void NextTurn() => Turns++;
    }

    internal class MessageManager : IMessageManager
    {
        private readonly List<string> _messages = [];
        private readonly IGameStateManager _gameStateManager;

        public MessageManager(IGameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        public IReadOnlyCollection<string> Messages => _messages.AsReadOnly();
        public void AddMessage(string message) => _messages.Add($"[回合 {_gameStateManager.Turns}] {message}");
        public void ClearMessages() => _messages.Clear();
    }

    internal class ResourceManager : IResourceManager
    {
        public int Food { get; private set; }
        public int BuildingCompletedCount { get; private set; }
        public int Beds { get; private set; }

        public ResourceManager(TurnProcessor.InitialSettingUps initial)
        {
            Food = initial.Food;
            BuildingCompletedCount = initial.Buildings;
            UpdateBeds(2); // 初始每棟房 2 個床位
        }

        public void AddBuildings(int amount)
        {
            if (amount > 0)
            {
                BuildingCompletedCount += amount;
            }
        }

        public void DestroyBuildings(int amount)
        {
            if (amount > 0)
            {
                // 確保建築數量不會變為負數
                BuildingCompletedCount = Math.Max(0, BuildingCompletedCount - amount);
            }
        }

        public void AddFood(int amount) => Food += amount;
        public void SpendFood(int amount) => Food -= amount;

        public (bool IsEnough, int Comsumption) IsFoodEnoughForRecruitment(int newGeneric, int newWheat, int newRice, int newSoldiers, int newBuilders, bool isHotSummer)
        {
            var totalCost = (newGeneric + newWheat + newRice) * new GenericFarmer().RecruitmentCost +
                            newSoldiers * new Soldier().RecruitmentCost +
                            newBuilders * new Builder().RecruitmentCost;

            if (isHotSummer)
            {
                totalCost = (int)(totalCost * 1.5);
            }

            return (Food >= totalCost, totalCost);
        }

        public void UpdateBeds(int bedsPerBuilding)
        {
            Beds = BuildingCompletedCount * bedsPerBuilding;
        }
    }

    internal class PopulationManager : IPopulationManager
    {
        private readonly List<Farmer> _farmers = [];
        private readonly List<Soldier> _soldiers = [];
        private readonly List<Builder> _builders = [];

        public IReadOnlyList<Farmer> Farmers => _farmers.AsReadOnly();
        public IReadOnlyList<Soldier> Soldiers => _soldiers.AsReadOnly();
        public IReadOnlyList<Builder> Builders => _builders.AsReadOnly();
        public int TotalRoles => _farmers.Count + _soldiers.Count + _builders.Count;

        public PopulationManager(TurnProcessor.InitialSettingUps initial)
        {
            for (int i = 0; i < initial.GenericFarmers; i++) AddFarmer(new GenericFarmer());
            for (int i = 0; i < initial.WheatFarmers; i++) AddFarmer(new WheatFarmer());
            for (int i = 0; i < initial.RiceFarmers; i++) AddFarmer(new RiceFarmer());
            for (int i = 0; i < initial.Soldiers; i++) AddSoldier(new Soldier());
            for (int i = 0; i < initial.Builders; i++) AddBuilder(new Builder());
        }

        public void AddFarmer(Farmer farmer) => _farmers.Add(farmer);
        public void AddSoldier(Soldier soldier) => _soldiers.Add(soldier);
        public void AddBuilder(Builder builder) => _builders.Add(builder);

        public void ClearAndRepopulate(int genericFarmers, int wheatFarmers, int riceFarmers, int soldiers, int builders)
        {
            _farmers.Clear();
            _soldiers.Clear();
            _builders.Clear();
            for (int i = 0; i < genericFarmers; i++) AddFarmer(new GenericFarmer());
            for (int i = 0; i < wheatFarmers; i++) AddFarmer(new WheatFarmer());
            for (int i = 0; i < riceFarmers; i++) AddFarmer(new RiceFarmer());
            for (int i = 0; i < soldiers; i++) AddSoldier(new Soldier());
            for (int i = 0; i < builders; i++) AddBuilder(new Builder());
        }

        public Role CullRoleForBedShortage()
        {
            if (_farmers.Any()) return _farmers.First();
            if (_builders.Any()) return _builders.First();
            if (_soldiers.Any()) return _soldiers.First();
            return null;
        }

        public Role CullRoleForFoodShortage()
        {
            if (_soldiers.Any()) return _soldiers.First();
            if (_builders.Any()) return _builders.First();
            if (_farmers.Any()) return _farmers.First();
            return null;
        }

        public void RemoveRole(Role role)
        {
            if (role is Farmer f) _farmers.Remove(f);
            else if (role is Soldier s) _soldiers.Remove(s);
            else if (role is Builder b) _builders.Remove(b);
        }

        public void RemoveRange(List<Role> rolesToRemove)
        {
            foreach (var role in rolesToRemove)
            {
                RemoveRole(role);
            }
        }
    }

    internal class EnemyManager : IEnemyManager
    {
        private readonly List<Enemy> _enemies = [];
        public IReadOnlyList<Enemy> Enemies => _enemies.AsReadOnly();
        public void AddEnemy(Enemy enemy) => _enemies.Add(enemy);
        public void RemoveEnemy(Enemy enemy) => _enemies.Remove(enemy);
        public void ClearNewlySpawnedFlags()
        {
            foreach (var enemy in _enemies)
            {
                enemy.IsNewlySpawned = false;
            }
        }
        public void SpawnEnemies(int turn)
        {
            int newFoesCount = (turn / 3) + 1;
            for (int i = 0; i < newFoesCount; i++)
            {
                if (turn > 10 && i % 3 == 0) AddEnemy(new HouseBreaker());
                else if (turn > 5 && i % 2 == 0) AddEnemy(new FoodThief());
                else AddEnemy(new GenericEnemy());
            }
        }
    }

    internal class RelicManager : IRelicManager
    {
        private readonly List<Relic> _activeRelics = [];
        private readonly List<Relic> _pendingRelics = [];

        public IReadOnlyList<Relic> ActiveRelics => _activeRelics.AsReadOnly();

        public void ActivatePendingRelics()
        {
            _activeRelics.AddRange(_pendingRelics);
            _pendingRelics.Clear();
        }

        public void AddPendingRelic(Relic relic) => _pendingRelics.Add(relic);

        public void ProcessRelicDurations()
        {
            // 從後往前遍歷以安全地移除元素
            for (int i = _activeRelics.Count - 1; i >= 0; i--)
            {
                var relic = _activeRelics[i];
                relic.Duration--;
                if (relic.Duration <= 0)
                {
                    _activeRelics.RemoveAt(i);
                }
            }
        }
    }

    #endregion
}
