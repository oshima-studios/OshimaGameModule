using System.Windows;
using System.Windows.Input;

namespace Oshima.FunGame.OshimaModes
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
            Show();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // 发送聊天消息的逻辑
            string message = ChatInputTextBox.Text;
            ViewModel.GameInfoText += $"You: {message}\n";
            ChatInputTextBox.Clear();
        }

        private void ChatInputTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButton_Click(sender, null);
            }
        }
    }
}
