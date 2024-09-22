namespace Oshima.FunGame.OshimaModes
{
    public partial class PlayerStatusControl : UserControl
    {
        public Label PlayerNameLabel { get; }
        public PictureBox AvatarBox { get; }
        public CustomProgressBar HPBar { get; }
        public CustomProgressBar MPBar { get; }

        public PlayerStatusControl(int index)
        {
            // 玩家名标签
            PlayerNameLabel = new()
            {
                Text = "Player " + index,
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                AutoSize = true
            };

            // 头像框
            AvatarBox = new()
            {
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Height = 100,
                Width = 100,
                Dock = DockStyle.Top
            };

            // HP条
            HPBar = new()
            {
                Maximum = 100,
                Value = 75.5,  // 支持浮点数
                ProgressColor = Color.Red,  // 自定义颜色
                Dock = DockStyle.Bottom,
                Height = 20
            };

            // MP条
            MPBar = new()
            {
                Maximum = 100,
                Value = 50.7,  // 支持浮点数
                ProgressColor = Color.Blue,  // 自定义颜色
                Dock = DockStyle.Bottom,
                Height = 20
            };

            // 添加控件到用户控件
            this.Controls.Add(MPBar);
            this.Controls.Add(HPBar);
            this.Controls.Add(AvatarBox);
            this.Controls.Add(PlayerNameLabel);
        }
    }
}
