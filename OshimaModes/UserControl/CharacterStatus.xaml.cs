using System.Windows.Media;

namespace Oshima.FunGame.OshimaModes
{
    public partial class CharacterStatus : System.Windows.Controls.UserControl
    {
        public CharacterStatus()
        {
            InitializeComponent();
        }

        // 公开控件属性以便外部访问
        public string CharacterNameText
        {
            get => CharacterName.Text;
            set => CharacterName.Text = value;
        }

        public ImageSource CharacterAvatarSource
        {
            get => CharacterAvatar.Source;
            set => CharacterAvatar.Source = value;
        }

        public double HPBarValue
        {
            get => HPBar.Value;
            set => HPBar.Value = value;
        }

        public double HPBarMaximum
        {
            get => HPBar.Maximum;
            set => HPBar.Maximum = value;
        }

        public double MPBarValue
        {
            get => MPBar.Value;
            set => MPBar.Value = value;
        }

        public double MPBarMaximum
        {
            get => MPBar.Maximum;
            set => MPBar.Maximum = value;
        }

        public double EPBarValue
        {
            get => EPBar.Value;
            set => EPBar.Value = value;
        }

        public double EPBarMaximum
        {
            get => EPBar.Maximum;
            set => EPBar.Maximum = value;
        }

        public ImageSource PictureBox1Source
        {
            get => pictureBox1.Source;
            set => pictureBox1.Source = value;
        }

        public ImageSource PictureBox2Source
        {
            get => pictureBox2.Source;
            set => pictureBox2.Source = value;
        }
    }
}
