using System;
using System.Collections.Generic;

namespace GameLogic.Manager
{
    public class TurnProcessor
    {
        

        public readonly struct UserInput
        {
            public UserInput(
                int recruitedFarmers,
                int recruitedSoldiers,
                int recruitedBuilders,
                int adjustedFarmers,
                int adjustedSoldiers,
                int adjustedBuilders)
            {
                this.RecruitedFarmers = recruitedFarmers;
                this.RecruitedSoldiers = recruitedSoldiers;
                this.RecruitedBuilders = recruitedBuilders;
                this.AdjustedFarmers = adjustedFarmers;
                this.AdjustedSoldiers = adjustedSoldiers;
                this.AdjustedBuilders = adjustedBuilders;
            }
            public int RecruitedFarmers { get; }
            public int RecruitedSoldiers { get; }
            public int RecruitedBuilders { get; }
            public int AdjustedFarmers { get; }
            public int AdjustedBuilders { get; }
            public int AdjustedSoldiers { get; }
        }

        public readonly struct InitialSettingUps
        {
            public InitialSettingUps(
                int food,
                int building,
                int farmers,
                int soldiers,
                int builders,
                int foes)
            {
                this.Food = food;
                this.Buildings = building;
                this.Farmers = farmers;
                this.Soldiers = soldiers;
                this.Builders = builders;
                this.Foes = foes;
            }
            public int Food { get; }
            public int Buildings { get; }
            public int Farmers { get; }
            public int Builders { get; }
            public int Soldiers { get; }
            public int Foes { get; }
        }
    }
}