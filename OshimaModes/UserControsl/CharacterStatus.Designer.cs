namespace Oshima.FunGame.OshimaModes
{
    partial class CharacterStatus
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterStatus));
            CharacterName = new Label();
            HPBar = new CustomProgressBar();
            MPBar = new CustomProgressBar();
            CharacterAvatar = new PictureBox();
            EPBar = new CustomProgressBar();
            EquipSlots = new TableLayoutPanel();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBox5 = new PictureBox();
            pictureBox6 = new PictureBox();
            pictureBox7 = new PictureBox();
            SkillSlots = new TableLayoutPanel();
            pictureBox8 = new PictureBox();
            pictureBox9 = new PictureBox();
            pictureBox10 = new PictureBox();
            pictureBox11 = new PictureBox();
            pictureBox1 = new PictureBox();
            pictureBox12 = new PictureBox();
            pictureBox13 = new PictureBox();
            pictureBox14 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)CharacterAvatar).BeginInit();
            EquipSlots.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            SkillSlots.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox9).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox10).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox11).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox12).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox13).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox14).BeginInit();
            SuspendLayout();
            // 
            // CharacterName
            // 
            CharacterName.Dock = DockStyle.Top;
            CharacterName.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            CharacterName.Location = new Point(0, 0);
            CharacterName.Name = "CharacterName";
            CharacterName.Size = new Size(308, 34);
            CharacterName.TabIndex = 0;
            CharacterName.Text = "角色名称";
            CharacterName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // HPBar
            // 
            HPBar.BorderStyle = BorderStyle.FixedSingle;
            HPBar.Dock = DockStyle.Bottom;
            HPBar.Location = new Point(0, 276);
            HPBar.Maximum = 1000D;
            HPBar.Name = "HPBar";
            HPBar.ProgressColor = Color.PaleVioletRed;
            HPBar.Size = new Size(308, 20);
            HPBar.TabIndex = 1;
            HPBar.Value = 325D;
            // 
            // MPBar
            // 
            MPBar.BorderStyle = BorderStyle.FixedSingle;
            MPBar.Dock = DockStyle.Bottom;
            MPBar.Location = new Point(0, 296);
            MPBar.Maximum = 142D;
            MPBar.Name = "MPBar";
            MPBar.ProgressColor = Color.SteelBlue;
            MPBar.Size = new Size(308, 20);
            MPBar.TabIndex = 2;
            MPBar.Value = 24D;
            // 
            // CharacterAvatar
            // 
            CharacterAvatar.Dock = DockStyle.Left;
            CharacterAvatar.Image = (Image)resources.GetObject("CharacterAvatar.Image");
            CharacterAvatar.Location = new Point(0, 34);
            CharacterAvatar.Name = "CharacterAvatar";
            CharacterAvatar.Size = new Size(181, 182);
            CharacterAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            CharacterAvatar.TabIndex = 3;
            CharacterAvatar.TabStop = false;
            // 
            // EPBar
            // 
            EPBar.BorderStyle = BorderStyle.FixedSingle;
            EPBar.Dock = DockStyle.Bottom;
            EPBar.Location = new Point(0, 316);
            EPBar.Maximum = 200D;
            EPBar.Name = "EPBar";
            EPBar.ProgressColor = Color.LightGoldenrodYellow;
            EPBar.Size = new Size(308, 21);
            EPBar.TabIndex = 4;
            EPBar.Value = 54D;
            // 
            // EquipSlots
            // 
            EquipSlots.ColumnCount = 2;
            EquipSlots.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            EquipSlots.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            EquipSlots.Controls.Add(pictureBox2, 0, 0);
            EquipSlots.Controls.Add(pictureBox3, 1, 0);
            EquipSlots.Controls.Add(pictureBox4, 0, 1);
            EquipSlots.Controls.Add(pictureBox5, 1, 1);
            EquipSlots.Controls.Add(pictureBox6, 0, 2);
            EquipSlots.Controls.Add(pictureBox7, 1, 2);
            EquipSlots.Dock = DockStyle.Right;
            EquipSlots.Location = new Point(180, 34);
            EquipSlots.Name = "EquipSlots";
            EquipSlots.RowCount = 3;
            EquipSlots.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            EquipSlots.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            EquipSlots.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            EquipSlots.Size = new Size(128, 182);
            EquipSlots.TabIndex = 8;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(3, 3);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(58, 50);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 0;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(67, 3);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(58, 50);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 1;
            pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = (Image)resources.GetObject("pictureBox4.Image");
            pictureBox4.Location = new Point(3, 63);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(58, 50);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.TabIndex = 2;
            pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.Image = (Image)resources.GetObject("pictureBox5.Image");
            pictureBox5.Location = new Point(67, 63);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(58, 50);
            pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox5.TabIndex = 3;
            pictureBox5.TabStop = false;
            // 
            // pictureBox6
            // 
            pictureBox6.Image = (Image)resources.GetObject("pictureBox6.Image");
            pictureBox6.Location = new Point(3, 123);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(58, 50);
            pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox6.TabIndex = 4;
            pictureBox6.TabStop = false;
            // 
            // pictureBox7
            // 
            pictureBox7.Image = (Image)resources.GetObject("pictureBox7.Image");
            pictureBox7.Location = new Point(67, 123);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(58, 50);
            pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox7.TabIndex = 5;
            pictureBox7.TabStop = false;
            // 
            // SkillSlots
            // 
            SkillSlots.ColumnCount = 4;
            SkillSlots.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            SkillSlots.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            SkillSlots.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            SkillSlots.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            SkillSlots.Controls.Add(pictureBox1, 0, 0);
            SkillSlots.Controls.Add(pictureBox12, 1, 0);
            SkillSlots.Controls.Add(pictureBox13, 2, 0);
            SkillSlots.Controls.Add(pictureBox14, 3, 0);
            SkillSlots.Dock = DockStyle.Bottom;
            SkillSlots.Location = new Point(0, 216);
            SkillSlots.Name = "SkillSlots";
            SkillSlots.RowCount = 1;
            SkillSlots.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            SkillSlots.Size = new Size(308, 60);
            SkillSlots.TabIndex = 9;
            // 
            // pictureBox8
            // 
            pictureBox8.Location = new Point(0, 0);
            pictureBox8.Name = "pictureBox8";
            pictureBox8.Size = new Size(100, 50);
            pictureBox8.TabIndex = 0;
            pictureBox8.TabStop = false;
            // 
            // pictureBox9
            // 
            pictureBox9.Location = new Point(0, 0);
            pictureBox9.Name = "pictureBox9";
            pictureBox9.Size = new Size(100, 50);
            pictureBox9.TabIndex = 0;
            pictureBox9.TabStop = false;
            // 
            // pictureBox10
            // 
            pictureBox10.Location = new Point(0, 0);
            pictureBox10.Name = "pictureBox10";
            pictureBox10.Size = new Size(100, 50);
            pictureBox10.TabIndex = 0;
            pictureBox10.TabStop = false;
            // 
            // pictureBox11
            // 
            pictureBox11.Location = new Point(0, 0);
            pictureBox11.Name = "pictureBox11";
            pictureBox11.Size = new Size(100, 50);
            pictureBox11.TabIndex = 0;
            pictureBox11.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(3, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(71, 50);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pictureBox12
            // 
            pictureBox12.Image = (Image)resources.GetObject("pictureBox12.Image");
            pictureBox12.Location = new Point(80, 3);
            pictureBox12.Name = "pictureBox12";
            pictureBox12.Size = new Size(71, 50);
            pictureBox12.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox12.TabIndex = 1;
            pictureBox12.TabStop = false;
            // 
            // pictureBox13
            // 
            pictureBox13.Image = (Image)resources.GetObject("pictureBox13.Image");
            pictureBox13.Location = new Point(157, 3);
            pictureBox13.Name = "pictureBox13";
            pictureBox13.Size = new Size(71, 50);
            pictureBox13.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox13.TabIndex = 2;
            pictureBox13.TabStop = false;
            // 
            // pictureBox14
            // 
            pictureBox14.Image = (Image)resources.GetObject("pictureBox14.Image");
            pictureBox14.Location = new Point(234, 3);
            pictureBox14.Name = "pictureBox14";
            pictureBox14.Size = new Size(71, 50);
            pictureBox14.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox14.TabIndex = 3;
            pictureBox14.TabStop = false;
            // 
            // CharacterStatus
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(CharacterAvatar);
            Controls.Add(EquipSlots);
            Controls.Add(SkillSlots);
            Controls.Add(CharacterName);
            Controls.Add(HPBar);
            Controls.Add(MPBar);
            Controls.Add(EPBar);
            Name = "CharacterStatus";
            Size = new Size(308, 337);
            ((System.ComponentModel.ISupportInitialize)CharacterAvatar).EndInit();
            EquipSlots.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            SkillSlots.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox8).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox9).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox10).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox11).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox12).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox13).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox14).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label CharacterName;
        private CustomProgressBar HPBar;
        private CustomProgressBar MPBar;
        private PictureBox CharacterAvatar;
        private CustomProgressBar EPBar;
        private TableLayoutPanel EquipSlots;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private PictureBox pictureBox6;
        private PictureBox pictureBox7;
        private TableLayoutPanel SkillSlots;
        private PictureBox pictureBox8;
        private PictureBox pictureBox9;
        private PictureBox pictureBox10;
        private PictureBox pictureBox11;
        private PictureBox pictureBox1;
        private PictureBox pictureBox12;
        private PictureBox pictureBox13;
        private PictureBox pictureBox14;
    }
}
