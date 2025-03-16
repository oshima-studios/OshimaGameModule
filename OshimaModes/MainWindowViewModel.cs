using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Oshima.FunGame.OshimaModes
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<PlayerInfo> Players { get; set; } = new ObservableCollection<PlayerInfo>();

        private string _gameInfoText;
        public string GameInfoText
        {
            get { return _gameInfoText; }
            set
            {
                _gameInfoText = value;
                OnPropertyChanged(nameof(GameInfoText));
            }
        }

        public MainWindowViewModel()
        {
            // 模拟添加玩家
            for (int i = 1; i <= 12; i++)
            {
                Players.Add(new PlayerInfo
                {
                    PlayerName = $"Player {i}",
                    HP = $"{i * 10}/{i * 100}",
                    MP = $"{i * 5}/{i * 50}",
                    PlayerImage = new BitmapImage(new System.Uri("pack://application:,,,/Oshima.FunGame.WPFUI;component/Images/default_avatar.png")) // 替换为你的默认头像
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PlayerInfo
    {
        public string PlayerName { get; set; }
        public string HP { get; set; }
        public string MP { get; set; }
        public BitmapImage PlayerImage { get; set; }
    }
}
