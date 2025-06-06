using System;
using System.Drawing;
using System.Windows.Forms;
using GameLogic;
using GameLogic.Manager;

namespace GameInterface
{
    public partial class GameForm : Form
    {
        private readonly IGameContext gameContext;
        private readonly TurnProcessor turnProcessor;

        private GameForm()
        {
            InitializeComponent();
        }

        public GameForm(TurnProcessor processor) : this()
        {
            this.turnProcessor = processor;
            this.gameContext = turnProcessor.GameContext;
            UpdateUi();
            this.btnNextTurn.Click += BtnNextTurn_Click;
        }

        private void Recruit_ValueChanged(object sender, EventArgs e)
        {
            var result = gameContext.IsFoodEnough((int)nudRecruitFarmer.Value, (int)nudRecruitSoldier.Value,
                (int)nudRecruitBuilder.Value);
            if (result.IsEnough)
                nudFoodConsumption.ForeColor = Color.Black;
            else
                nudFoodConsumption.ForeColor = Color.Red;

            nudFoodConsumption.Value = result.Comsumption;
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

                var recruitCheck = CheckBeforeRecruit();
                if (recruitCheck.IsFoodEnough)
                {
                    this.nudRecruitFarmer.Value = this.nudRecruitSoldier.Value = this.nudRecruitBuilder.Value = 0;
                }
                else
                {
                    MessageBox.Show($@"糧食不足，缺少 {recruitCheck.Shortfall} 單位");
                    return;
                }

                var isFullyDistributed = IsRolesFullyDistributed();
                if (!isFullyDistributed.IsFullyDistributed)
                {
                    MessageBox.Show(@"調整角色數量尚未完成");
                    return;
                }

                turnProcessor.TurnStart(new TurnProcessor.UserInput(recruitCheck.recruitedFarmers,
                    recruitCheck.recruitedSoldiers, recruitCheck.recruitedBuilders, isFullyDistributed.Farmers,
                    isFullyDistributed.Soldiers, isFullyDistributed.Builders));
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
            var result = gameContext.IsFoodEnough(farmers, soldiers, builders);
            return (result.IsEnough, gameContext.Food - result.Comsumption, farmers, soldiers, builders);
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

                // 更新數值
                this.nudFarmer.Value = gameContext.FarmersCount;
                this.nudSoldier.Value = gameContext.SoldiersCount;
                this.nudBuilder.Value = gameContext.BuildersCount;
                this.lblFoesCount.Text = gameContext.FoesCount.ToString();
                this.lblFood.Text = gameContext.Food.ToString();
                this.lblBeds.Text = gameContext.Beds.ToString();
                this.lblBuildings.Text = gameContext.BuildingCompletedCount.ToString();
                this.lblTurn.Text = gameContext.Turns.ToString();
                this.lblWeather.Text = $"天氣: {gameContext.Weather}"; // 更新天氣

                // 更新剩餘人口
                this.nudRemain.Value = gameContext.RolesCount - (this.nudFarmer.Value + this.nudSoldier.Value + this.nudBuilder.Value);

                // 更新 Tag
                this.nudFarmer.Tag = this.nudFarmer.Value;
                this.nudSoldier.Tag = this.nudSoldier.Value;
                this.nudBuilder.Tag = this.nudBuilder.Value;

                // 更新訊息日誌
                this.rtbMessageLog.Clear();
                foreach (var message in gameContext.Messages)
                {
                    this.rtbMessageLog.AppendText(message + "\n");
                }
                // 自動捲動到最下方
                this.rtbMessageLog.SelectionStart = this.rtbMessageLog.Text.Length;
                this.rtbMessageLog.ScrollToCaret();

                // 檢查遊戲是否結束
                if (gameContext.GameFinished)
                {
                    this.btnNextTurn.Enabled = false;
                    this.btnNextTurn.Text = "遊戲結束";
                }
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