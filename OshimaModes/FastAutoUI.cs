namespace Oshima.FunGame.OshimaModes
{
    public partial class FastAutoUI : Form
    {
        public FastAutoUI()
        {
            InitializeComponent();

            // 根据玩家数量，添加格子
            AddPlayers(7);
        }

        // 动态添加玩家格子
        private void AddPlayers(int playerCount)
        {
            int leftIndex = 0;  // 左侧的奇数玩家计数器
            int rightIndex = 0; // 右侧的偶数玩家计数器

            for (int i = 1; i <= playerCount; i++)
            {
                // 每个格子用Button表示
                CharacterStatus playerSlot = new();

                if (i % 2 == 1) // 奇数玩家，左侧
                {
                    AddToLeftPanel(playerSlot, leftIndex);
                    leftIndex++;
                }
                else // 偶数玩家，右侧
                {
                    AddToRightPanel(playerSlot, rightIndex);
                    rightIndex++;
                }
            }
        }

        // 添加奇数玩家到左侧TableLayoutPanel
        private void AddToLeftPanel(Control control, int index)
        {
            int col = index / 3;   // 每列容纳3个玩家
            int row = index % 3;   // 行号从0到2
            leftTableLayoutPanel.Controls.Add(control, col, row);
        }

        // 添加偶数玩家到右侧TableLayoutPanel
        private void AddToRightPanel(Control control, int index)
        {
            // 偶数玩家的排列规则：
            // - 第一列: Player 10, Player 11, Player 12 (根据index从3开始)
            // - 第二列: Player 2, Player 4, Player 6 (根据index从0开始)

            int col = (index >= 3) ? 0 : 1;  // 超过 3 位玩家的偶数编号应该在第一列，否则在第二列
            int row = index % 3;   // 行号从0到2
            rightTableLayoutPanel.Controls.Add(control, col, row);
        }
    }
}
