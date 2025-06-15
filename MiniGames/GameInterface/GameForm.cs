using System;
using System.Drawing;
using System.Windows.Forms;
using GameLogic;
using GameLogic.Interface;
using GameLogic.Manager;

namespace GameInterface
{
    public partial class GameForm : Form
    {
        private readonly IGameContext _gameContext;
        private readonly TurnProcessor _turnProcessor;

        private GameForm()
        {
            InitializeComponent(); // 這裡會呼叫 `GameForm.Designer.cs` 中的 UI 初始化
        }

        public GameForm(TurnProcessor processor) : this()
        {
            this._turnProcessor = processor;
            this._gameContext = _turnProcessor.GameContext;
            UpdateUi();
            this.btnNextTurn.Click += BtnNextTurn_Click;
        }

        private void Recruit_ValueChanged(object sender, EventArgs e)
        {
            var (IsEnough, Comsumption) = _gameContext.IsFoodEnough((int)nudRecruitFarmer.Value, (int)nudRecruitSoldier.Value,
                (int)nudRecruitBuilder.Value);
            if (IsEnough)
                nudFoodConsumption.ForeColor = Color.Black;
            else
                nudFoodConsumption.ForeColor = Color.Red;

            nudFoodConsumption.Value = Comsumption;
        }

        private void NudRole_ValueChanged(object sender, EventArgs e)
        {
            if (!(sender is NumericUpDown o)) return;
            var newValue = (int)o.Value;
            int.TryParse((o.Tag?.ToString() ?? null), out int oldValue);
            var diff = newValue - oldValue;
            o.Tag = newValue;
            this.nudRemain.Value -= diff;
        }

        private void BtnNextTurn_Click(object sender, EventArgs e)
        {
            try
            {
                this.nudFarmer.ReadOnly = this.nudSoldier.ReadOnly = this.nudBuilder.ReadOnly = true;
                this.nudRecruitFarmer.ReadOnly =
                    this.nudRecruitSoldier.ReadOnly = this.nudRecruitBuilder.ReadOnly = true;

                var (IsFoodEnough, Shortfall, recruitedFarmers, recruitedSoldiers, recruitedBuilders) = CheckBeforeRecruit();
                if (IsFoodEnough)
                {
                    this.nudRecruitFarmer.Value = this.nudRecruitSoldier.Value = this.nudRecruitBuilder.Value = 0;
                }
                else
                {
                    MessageBox.Show($@"糧食不足，缺少 {Shortfall} 單位");
                    return;
                }

                var (IsFullyDistributed, Farmers, Soldiers, Builders) = IsRolesFullyDistributed();
                if (!IsFullyDistributed)
                {
                    MessageBox.Show(@"調整角色數量尚未完成");
                    return;
                }

                _turnProcessor.TurnStart(new TurnProcessor.UserInput(recruitedFarmers,
                    recruitedSoldiers, recruitedBuilders, Farmers,
                    Soldiers, Builders));
                UpdateUi();
            }
            finally
            {
                this.nudFarmer.ReadOnly = this.nudSoldier.ReadOnly = this.nudBuilder.ReadOnly = false;
                this.nudRecruitFarmer.ReadOnly =
                    this.nudRecruitSoldier.ReadOnly = this.nudRecruitBuilder.ReadOnly = false;
            }
        }

        private (bool IsFoodEnough, long Shortfall, int recruitedFarmers, int recruitedSoldiers, int recruitedBuilders)
            CheckBeforeRecruit()
        {
            var farmers = (int)this.nudRecruitFarmer.Value;
            var soldiers = (int)this.nudRecruitSoldier.Value;
            var builders = (int)this.nudRecruitBuilder.Value;
            var (IsEnough, Comsumption) = _gameContext.IsFoodEnough(farmers, soldiers, builders);
            return (IsEnough, _gameContext.Food - Comsumption, farmers, soldiers, builders);
        }

        private (bool IsFullyDistributed, int Farmers, int Soldiers, int Builders) IsRolesFullyDistributed()
        {
            var farmers = (int)this.nudFarmer.Value;
            var soldiers = (int)this.nudSoldier.Value;
            var builders = (int)this.nudBuilder.Value;
            var remain = (int)this.nudRemain.Value;
            return (remain == 0, farmers, soldiers, builders);
        }

        private void UpdateUi()
        {
            try
            {
                this.nudFarmer.ValueChanged -= NudRole_ValueChanged;
                this.nudSoldier.ValueChanged -= NudRole_ValueChanged;
                this.nudBuilder.ValueChanged -= NudRole_ValueChanged;
                this.nudRecruitFarmer.ValueChanged -= Recruit_ValueChanged;
                this.nudRecruitSoldier.ValueChanged -= Recruit_ValueChanged;
                this.nudRecruitBuilder.ValueChanged -= Recruit_ValueChanged;

                this.nudFarmer.Value = _gameContext.FarmersCount;
                this.nudSoldier.Value = _gameContext.SoldiersCount;
                this.nudBuilder.Value = _gameContext.BuildersCount;
                this.lblFoesCount.Text = _gameContext.FoesCount.ToString();
                this.lblFood.Text = _gameContext.Food.ToString();
                this.lblBeds.Text = _gameContext.Beds.ToString();
                this.lblBuildings.Text = _gameContext.BuildingCompletedCount.ToString();
                this.lblTurn.Text = _gameContext.Turns.ToString();
                this.nudFarmer.Tag = this.nudFarmer.Value;
                this.nudSoldier.Tag = this.nudSoldier.Value;
                this.nudBuilder.Tag = this.nudBuilder.Value;
            }
            finally
            {
                this.nudFarmer.ValueChanged += NudRole_ValueChanged;
                this.nudSoldier.ValueChanged += NudRole_ValueChanged;
                this.nudBuilder.ValueChanged += NudRole_ValueChanged;
                this.nudRecruitFarmer.ValueChanged += Recruit_ValueChanged;
                this.nudRecruitSoldier.ValueChanged += Recruit_ValueChanged;
                this.nudRecruitBuilder.ValueChanged += Recruit_ValueChanged;
            }
        }
    }
}