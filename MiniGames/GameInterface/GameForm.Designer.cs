using System.Drawing;
using System.Reflection;
using System.Windows.Forms.VisualStyles;

namespace GameInterface
{
    using System.Windows.Forms;

    partial class GameForm
    {
        private Label lblTurn;
        private Label lblFood;
        private Label lblFoesCount;
        private Label lblBeds;
        private Label lblBuildings;
        private Button btnNextTurn;
        private PictureBox pbFarmer;
        private PictureBox pbBuilder;
        private PictureBox pbSoldier;
        private PictureBox pbFoe;
        private PictureBox pbFood;
        private PictureBox pbTurns;
        private PictureBox pbBuildings;
        private PictureBox pbBeds;
        private NumericUpDown nudFarmer;
        private NumericUpDown nudSoldier;
        private NumericUpDown nudBuilder;
        private NumericUpDown nudRemain;
        private Label label1;
        private Label label2;
        private NumericUpDown nudFoodConsumption;
        private NumericUpDown nudRecruitSoldier;
        private NumericUpDown nudRecruitBuilder;
        private NumericUpDown nudRecruitFarmer;
        private System.Windows.Forms.RichTextBox rtbMessageLog;
        private System.Windows.Forms.Label lblWeather;

        private void InitializeComponent()
        {
            this.lblTurn = new System.Windows.Forms.Label();
            this.lblFood = new System.Windows.Forms.Label();
            this.lblFoesCount = new System.Windows.Forms.Label();
            this.lblBeds = new System.Windows.Forms.Label();
            this.lblBuildings = new System.Windows.Forms.Label();
            this.btnNextTurn = new System.Windows.Forms.Button();
            this.nudFarmer = new System.Windows.Forms.NumericUpDown();
            this.nudSoldier = new System.Windows.Forms.NumericUpDown();
            this.nudBuilder = new System.Windows.Forms.NumericUpDown();
            this.nudRemain = new System.Windows.Forms.NumericUpDown();
            this.pbFarmer = new System.Windows.Forms.PictureBox();
            this.pbBuilder = new System.Windows.Forms.PictureBox();
            this.pbSoldier = new System.Windows.Forms.PictureBox();
            this.pbFoe = new System.Windows.Forms.PictureBox();
            this.pbFood = new System.Windows.Forms.PictureBox();
            this.pbTurns = new System.Windows.Forms.PictureBox();
            this.pbBeds = new System.Windows.Forms.PictureBox();
            this.pbBuildings = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudFoodConsumption = new System.Windows.Forms.NumericUpDown();
            this.nudRecruitSoldier = new System.Windows.Forms.NumericUpDown();
            this.nudRecruitBuilder = new System.Windows.Forms.NumericUpDown();
            this.nudRecruitFarmer = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudFarmer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoldier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBuilder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRemain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFarmer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBuilder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSoldier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFoe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFood)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTurns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBeds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBuildings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFoodConsumption)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecruitSoldier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecruitBuilder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecruitFarmer)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbMessageLog
            // 
            this.rtbMessageLog = new System.Windows.Forms.RichTextBox();
            this.rtbMessageLog.Location = new System.Drawing.Point(380, 193);
            this.rtbMessageLog.Name = "rtbMessageLog";
            this.rtbMessageLog.ReadOnly = true;
            this.rtbMessageLog.Size = new System.Drawing.Size(320, 150);
            this.rtbMessageLog.TabIndex = 24;
            this.rtbMessageLog.Text = "";
            this.rtbMessageLog.BackColor = System.Drawing.Color.Linen;
            // 
            // lblWeather
            // 
            this.lblWeather = new System.Windows.Forms.Label();
            this.lblWeather.Location = new System.Drawing.Point(444, 92);
            this.lblWeather.Name = "lblWeather";
            this.lblWeather.Size = new System.Drawing.Size(150, 30);
            this.lblWeather.TabIndex = 25;
            this.lblWeather.Text = "晴天";
            this.lblWeather.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblWeather.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold);
            // 
            // lblTurn
            // 
            this.lblTurn.Location = new System.Drawing.Point(75, 30);
            this.lblTurn.Name = "lblTurn";
            this.lblTurn.Size = new System.Drawing.Size(80, 30);
            this.lblTurn.TabIndex = 0;
            this.lblTurn.Text = "0";
            this.lblTurn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFood
            // 
            this.lblFood.Location = new System.Drawing.Point(255, 30);
            this.lblFood.Name = "lblFood";
            this.lblFood.Size = new System.Drawing.Size(80, 30);
            this.lblFood.TabIndex = 1;
            this.lblFood.Text = "0";
            this.lblFood.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFoesCount
            // 
            this.lblFoesCount.Location = new System.Drawing.Point(504, 143);
            this.lblFoesCount.Name = "lblFoesCount";
            this.lblFoesCount.Size = new System.Drawing.Size(100, 30);
            this.lblFoesCount.TabIndex = 9;
            this.lblFoesCount.Text = "0";
            this.lblFoesCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBeds
            // 
            this.lblBeds.Location = new System.Drawing.Point(615, 34);
            this.lblBeds.Name = "lblBeds";
            this.lblBeds.Size = new System.Drawing.Size(100, 23);
            this.lblBeds.TabIndex = 0;
            this.lblBeds.Text = "0";
            this.lblBeds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBuildings
            // 
            this.lblBuildings.Location = new System.Drawing.Point(435, 34);
            this.lblBuildings.Name = "lblBuildings";
            this.lblBuildings.Size = new System.Drawing.Size(100, 23);
            this.lblBuildings.TabIndex = 0;
            this.lblBuildings.Text = "0";
            this.lblBuildings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNextTurn
            // 
            this.btnNextTurn.Location = new System.Drawing.Point(280, 369);
            this.btnNextTurn.Name = "btnNextTurn";
            this.btnNextTurn.Size = new System.Drawing.Size(150, 50);
            this.btnNextTurn.TabIndex = 10;
            this.btnNextTurn.Text = "結束本回合";
            // 
            // nudFarmer
            // 
            this.nudFarmer.Location = new System.Drawing.Point(83, 146);
            this.nudFarmer.Name = "nudFarmer";
            this.nudFarmer.Size = new System.Drawing.Size(120, 25);
            this.nudFarmer.TabIndex = 12;
            // 
            // nudSoldier
            // 
            this.nudSoldier.Location = new System.Drawing.Point(83, 266);
            this.nudSoldier.Name = "nudSoldier";
            this.nudSoldier.Size = new System.Drawing.Size(120, 25);
            this.nudSoldier.TabIndex = 14;
            // 
            // nudBuilder
            // 
            this.nudBuilder.Location = new System.Drawing.Point(83, 206);
            this.nudBuilder.Name = "nudBuilder";
            this.nudBuilder.Size = new System.Drawing.Size(120, 25);
            this.nudBuilder.TabIndex = 15;
            // 
            // nudRemain
            // 
            this.nudRemain.Location = new System.Drawing.Point(83, 323);
            this.nudRemain.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudRemain.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.nudRemain.Name = "nudRemain";
            this.nudRemain.ReadOnly = true;
            this.nudRemain.Controls[0].Hide();
            this.nudRemain.Size = new System.Drawing.Size(120, 25);
            this.nudRemain.TabIndex = 13;
            // 
            // pbFarmer
            // 
            this.pbFarmer.Image = global::Properties.Resources.farmer;
            this.pbFarmer.Location = new System.Drawing.Point(20, 133);
            this.pbFarmer.Name = "pbFarmer";
            this.pbFarmer.Size = new System.Drawing.Size(50, 50);
            this.pbFarmer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbFarmer.TabIndex = 2;
            this.pbFarmer.TabStop = false;
            // 
            // pbBuilder
            // 
            this.pbBuilder.Image = global::Properties.Resources.Architect;
            this.pbBuilder.Location = new System.Drawing.Point(20, 193);
            this.pbBuilder.Name = "pbBuilder";
            this.pbBuilder.Size = new System.Drawing.Size(50, 50);
            this.pbBuilder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbBuilder.TabIndex = 4;
            this.pbBuilder.TabStop = false;
            // 
            // pbSoldier
            // 
            this.pbSoldier.Image = global::Properties.Resources.Soldier;
            this.pbSoldier.Location = new System.Drawing.Point(20, 253);
            this.pbSoldier.Name = "pbSoldier";
            this.pbSoldier.Size = new System.Drawing.Size(50, 50);
            this.pbSoldier.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSoldier.TabIndex = 6;
            this.pbSoldier.TabStop = false;
            // 
            // pbFoe
            // 
            this.pbFoe.Image = global::Properties.Resources.Villain;
            this.pbFoe.Location = new System.Drawing.Point(444, 133);
            this.pbFoe.Name = "pbFoe";
            this.pbFoe.Size = new System.Drawing.Size(50, 50);
            this.pbFoe.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbFoe.TabIndex = 8;
            this.pbFoe.TabStop = false;
            // 
            // pbFood
            // 
            this.pbFood.Image = global::Properties.Resources.Food;
            this.pbFood.Location = new System.Drawing.Point(200, 20);
            this.pbFood.Name = "pbFood";
            this.pbFood.Size = new System.Drawing.Size(50, 50);
            this.pbFood.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbFood.TabIndex = 8;
            this.pbFood.TabStop = false;
            // 
            // pbTurns
            // 
            this.pbTurns.Image = global::Properties.Resources.Turn;
            this.pbTurns.Location = new System.Drawing.Point(20, 20);
            this.pbTurns.Name = "pbTurns";
            this.pbTurns.Size = new System.Drawing.Size(50, 50);
            this.pbTurns.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbTurns.TabIndex = 8;
            this.pbTurns.TabStop = false;
            // 
            // pbBeds
            // 
            this.pbBeds.Image = global::Properties.Resources.Bed;
            this.pbBeds.Location = new System.Drawing.Point(560, 20);
            this.pbBeds.Name = "pbBeds";
            this.pbBeds.Size = new System.Drawing.Size(50, 50);
            this.pbBeds.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbBeds.TabIndex = 16;
            this.pbBeds.TabStop = false;
            // 
            // pbBuildings
            // 
            this.pbBuildings.Image = global::Properties.Resources.House;
            this.pbBuildings.Location = new System.Drawing.Point(380, 20);
            this.pbBuildings.Name = "pbBuildings";
            this.pbBuildings.Size = new System.Drawing.Size(50, 50);
            this.pbBuildings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbBuildings.TabIndex = 17;
            this.pbBuildings.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(104, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 30);
            this.label1.TabIndex = 18;
            this.label1.Text = "目前角色";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(241, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 30);
            this.label2.TabIndex = 23;
            this.label2.Text = "招募區";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nudFoodConsumption
            // 
            this.nudFoodConsumption.Location = new System.Drawing.Point(220, 323);
            this.nudFoodConsumption.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudFoodConsumption.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.nudFoodConsumption.Name = "nudFoodConsumption";
            this.nudFoodConsumption.ReadOnly = true;
            this.nudFoodConsumption.Size = new System.Drawing.Size(120, 25);
            this.nudFoodConsumption.TabIndex = 20;
            this.nudFoodConsumption.Controls[0].Hide();
            // 
            // nudRecruitSoldier
            // 
            this.nudRecruitSoldier.Location = new System.Drawing.Point(220, 266);
            this.nudRecruitSoldier.Name = "nudRecruitSoldier";
            this.nudRecruitSoldier.Size = new System.Drawing.Size(120, 25);
            this.nudRecruitSoldier.TabIndex = 21;
            // 
            // nudRecruitBuilder
            // 
            this.nudRecruitBuilder.Location = new System.Drawing.Point(220, 206);
            this.nudRecruitBuilder.Name = "nudRecruitBuilder";
            this.nudRecruitBuilder.Size = new System.Drawing.Size(120, 25);
            this.nudRecruitBuilder.TabIndex = 22;
            // 
            // nudRecruitFarmer
            // 
            this.nudRecruitFarmer.Location = new System.Drawing.Point(220, 146);
            this.nudRecruitFarmer.Name = "nudRecruitFarmer";
            this.nudRecruitFarmer.Size = new System.Drawing.Size(120, 25);
            this.nudRecruitFarmer.TabIndex = 19;
            // 
            // GameForm
            // 
            this.BackgroundImage = global::Properties.Resources.Hope_vs_Despair_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(732, 453);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudFoodConsumption);
            this.Controls.Add(this.nudRecruitSoldier);
            this.Controls.Add(this.nudRecruitBuilder);
            this.Controls.Add(this.nudRecruitFarmer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudRemain);
            this.Controls.Add(this.nudSoldier);
            this.Controls.Add(this.nudBuilder);
            this.Controls.Add(this.nudFarmer);
            this.Controls.Add(this.lblTurn);
            this.Controls.Add(this.lblFood);
            this.Controls.Add(this.lblBeds);
            this.Controls.Add(this.lblBuildings);
            this.Controls.Add(this.pbFarmer);
            this.Controls.Add(this.pbBuilder);
            this.Controls.Add(this.pbSoldier);
            this.Controls.Add(this.pbFoe);
            this.Controls.Add(this.pbFood);
            this.Controls.Add(this.pbTurns);
            this.Controls.Add(this.pbBeds);
            this.Controls.Add(this.pbBuildings);
            this.Controls.Add(this.lblFoesCount);
            this.Controls.Add(this.btnNextTurn);
            this.Controls.Add(this.rtbMessageLog);
            this.Controls.Add(this.lblWeather);
            this.Name = "GameForm";
            this.Text = "回合制遊戲 UI";
            ((System.ComponentModel.ISupportInitialize)(this.nudFarmer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoldier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBuilder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRemain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFarmer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBuilder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSoldier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFoe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFood)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTurns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBeds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBuildings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFoodConsumption)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecruitSoldier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecruitBuilder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecruitFarmer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecruitBuilder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecruitFarmer)).EndInit();
            this.ResumeLayout(false);

        }
    }
}