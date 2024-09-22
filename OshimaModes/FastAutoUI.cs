namespace Oshima.FunGame.OshimaModes
{
    public partial class FastAutoUI : Form
    {
        public FastAutoUI()
        {
            InitializeComponent();

            // ���������������Ӹ���
            AddPlayers(7);
        }

        // ��̬�����Ҹ���
        private void AddPlayers(int playerCount)
        {
            int leftIndex = 0;  // ����������Ҽ�����
            int rightIndex = 0; // �Ҳ��ż����Ҽ�����

            for (int i = 1; i <= playerCount; i++)
            {
                // ÿ��������Button��ʾ
                CharacterStatus playerSlot = new();

                if (i % 2 == 1) // ������ң����
                {
                    AddToLeftPanel(playerSlot, leftIndex);
                    leftIndex++;
                }
                else // ż����ң��Ҳ�
                {
                    AddToRightPanel(playerSlot, rightIndex);
                    rightIndex++;
                }
            }
        }

        // ���������ҵ����TableLayoutPanel
        private void AddToLeftPanel(Control control, int index)
        {
            int col = index / 3;   // ÿ������3�����
            int row = index % 3;   // �кŴ�0��2
            leftTableLayoutPanel.Controls.Add(control, col, row);
        }

        // ���ż����ҵ��Ҳ�TableLayoutPanel
        private void AddToRightPanel(Control control, int index)
        {
            // ż����ҵ����й���
            // - ��һ��: Player 10, Player 11, Player 12 (����index��3��ʼ)
            // - �ڶ���: Player 2, Player 4, Player 6 (����index��0��ʼ)

            int col = (index >= 3) ? 0 : 1;  // ���� 3 λ��ҵ�ż�����Ӧ���ڵ�һ�У������ڵڶ���
            int row = index % 3;   // �кŴ�0��2
            rightTableLayoutPanel.Controls.Add(control, col, row);
        }
    }
}
