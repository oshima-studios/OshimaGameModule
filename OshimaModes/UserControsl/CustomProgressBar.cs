namespace Oshima.FunGame.OshimaModes
{
    public class CustomProgressBar : UserControl
    {
        private double _value = 0;
        private double _maximum = 100;
        private Color _progressColor = Color.Red;

        public double Value
        {
            get => _value;
            set
            {
                _value = Math.Max(0, Math.Min(value, _maximum)); // 限制值在范围内
                Invalidate();  // 触发重绘
            }
        }

        public double Maximum
        {
            get => _maximum;
            set
            {
                _maximum = Math.Max(1, value); // 最大值不能小于1
                Invalidate();  // 触发重绘
            }
        }

        public Color ProgressColor
        {
            get => _progressColor;
            set
            {
                _progressColor = value;
                Invalidate();  // 触发重绘
            }
        }

        public CustomProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.Height = 20;  // 设置控件的默认高度
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // 计算进度条填充宽度
            double percent = _value / _maximum;
            int fillWidth = (int)(this.Width * percent);

            // 填充背景（灰色）
            e.Graphics.FillRectangle(Brushes.LightGray, 0, 0, this.Width, this.Height);

            // 填充进度条（自定义颜色）
            e.Graphics.FillRectangle(new SolidBrush(_progressColor), 0, 0, fillWidth, this.Height);

            // 可选：绘制进度百分比文字
            string text = $"{_value} / {_maximum}";
            SizeF textSize = e.Graphics.MeasureString(text, this.Font);
            e.Graphics.DrawString(text, this.Font, Brushes.Black,
                (this.Width - textSize.Width) / 2, (this.Height - textSize.Height) / 2);
        }
    }
}
