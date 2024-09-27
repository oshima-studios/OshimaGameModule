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
            pictureBox8 = new PictureBox();
            pictureBox9 = new PictureBox();
            pictureBox10 = new PictureBox();
            pictureBox11 = new PictureBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)CharacterAvatar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox9).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox10).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox11).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // CharacterName
            // 
            CharacterName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            CharacterName.Font = new Font("霞鹜文楷", 14.9999981F, FontStyle.Regular, GraphicsUnit.Point, 134);
            CharacterName.Location = new Point(0, 0);
            CharacterName.Name = "CharacterName";
            CharacterName.Size = new Size(315, 31);
            CharacterName.TabIndex = 0;
            CharacterName.Text = "角色名称";
            CharacterName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // HPBar
            // 
            HPBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            HPBar.BorderStyle = BorderStyle.FixedSingle;
            HPBar.Font = new Font("霞鹜文楷", 10.5F, FontStyle.Bold);
            HPBar.Location = new Point(0, 260);
            HPBar.Maximum = 1000D;
            HPBar.Name = "HPBar";
            HPBar.ProgressColor = Color.PaleVioletRed;
            HPBar.Size = new Size(315, 20);
            HPBar.TabIndex = 1;
            HPBar.Value = 325D;
            // 
            // MPBar
            // 
            MPBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MPBar.BorderStyle = BorderStyle.FixedSingle;
            MPBar.Font = new Font("霞鹜文楷", 10.5F, FontStyle.Bold);
            MPBar.Location = new Point(0, 280);
            MPBar.Maximum = 142D;
            MPBar.Name = "MPBar";
            MPBar.ProgressColor = Color.SteelBlue;
            MPBar.Size = new Size(315, 20);
            MPBar.TabIndex = 2;
            MPBar.Value = 24D;
            // 
            // CharacterAvatar
            // 
            CharacterAvatar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            CharacterAvatar.Image = (Image)resources.GetObject("CharacterAvatar.Image");
            CharacterAvatar.Location = new Point(0, 31);
            CharacterAvatar.Name = "CharacterAvatar";
            CharacterAvatar.Size = new Size(315, 192);
            CharacterAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            CharacterAvatar.TabIndex = 3;
            CharacterAvatar.TabStop = false;
            // 
            // EPBar
            // 
            EPBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            EPBar.BorderStyle = BorderStyle.FixedSingle;
            EPBar.Font = new Font("霞鹜文楷", 10.5F, FontStyle.Bold);
            EPBar.Location = new Point(0, 300);
            EPBar.Maximum = 200D;
            EPBar.Name = "EPBar";
            EPBar.ProgressColor = Color.LightGoldenrodYellow;
            EPBar.Size = new Size(315, 21);
            EPBar.TabIndex = 4;
            EPBar.Value = 54D;
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
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel1.BackColor = Color.Transparent;
            flowLayoutPanel1.Controls.Add(pictureBox1);
            flowLayoutPanel1.Controls.Add(pictureBox2);
            flowLayoutPanel1.Location = new Point(0, 324);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(315, 30);
            flowLayoutPanel1.TabIndex = 5;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(3, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(25, 25);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(34, 3);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(25, 25);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // CharacterStatus
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(CharacterName);
            Controls.Add(HPBar);
            Controls.Add(MPBar);
            Controls.Add(EPBar);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(CharacterAvatar);
            Name = "CharacterStatus";
            Size = new Size(315, 357);
            ((System.ComponentModel.ISupportInitialize)CharacterAvatar).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox9).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox10).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox11).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label CharacterName;
        private CustomProgressBar HPBar;
        private CustomProgressBar MPBar;
        private PictureBox CharacterAvatar;
        private CustomProgressBar EPBar;
        private PictureBox pictureBox8;
        private PictureBox pictureBox9;
        private PictureBox pictureBox10;
        private PictureBox pictureBox11;
        private FlowLayoutPanel flowLayoutPanel1;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
    }
}
