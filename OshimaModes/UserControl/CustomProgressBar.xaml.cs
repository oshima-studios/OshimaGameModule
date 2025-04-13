using System.Windows;

namespace Oshima.FunGame.OshimaModes
{
    public partial class CustomProgressBar : System.Windows.Controls.UserControl
    {
        public CustomProgressBar()
        {
            InitializeComponent();
        }

        // 进度值依赖属性
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(CustomProgressBar),
            new PropertyMetadata(0.0, OnValueChanged));

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomProgressBar)d;
            control.progressBar.Value = (double)e.NewValue;
        }

        // 最大值依赖属性
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof(double), typeof(CustomProgressBar),
            new PropertyMetadata(100.0, OnMaximumChanged));

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomProgressBar)d;
            control.progressBar.Maximum = (double)e.NewValue;
        }
    }
}
