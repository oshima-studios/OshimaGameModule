using System.Windows;
using System.Windows.Controls;

namespace Oshima.FunGame.OshimaModes
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddPlayers(7); // 初始化时添加7个玩家
        }

        // 动态添加玩家格子
        private void AddPlayers(int playerCount)
        {
            int leftIndex = 0;  // 左侧奇数玩家计数器
            int rightIndex = 0; // 右侧偶数玩家计数器

            for (int i = 1; i <= playerCount; i++)
            {
                // 使用 CharacterStatus 控件表示玩家
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

        // 添加奇数玩家到左侧 Grid
        private void AddToLeftPanel(CharacterStatus control, int index)
        {
            int col = index / 3; // 每列容纳3个玩家
            int row = index % 3; // 行号从0到2

            Grid.SetColumn(control, col);
            Grid.SetRow(control, row);
            leftTableLayoutPanel.Children.Add(control);
        }

        // 添加偶数玩家到右侧 Grid
        private void AddToRightPanel(CharacterStatus control, int index)
        {
            int col = (index >= 3) ? 0 : 1; // 超过3位玩家的偶数编号在第一列，否则在第二列
            int row = index % 3; // 行号从0到2

            Grid.SetColumn(control, col);
            Grid.SetRow(control, row);
            rightTableLayoutPanel.Children.Add(control);
        }
    }
}
