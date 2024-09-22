using System.Windows.Forms;

namespace Oshima.FunGame.OshimaModes
{
    partial class FastAutoUI
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainTableLayoutPanel = new TableLayoutPanel();
            flowLayoutPanel6 = new FlowLayoutPanel();
            flowLayoutPanel4 = new FlowLayoutPanel();
            flowLayoutPanel3 = new FlowLayoutPanel();
            leftTableLayoutPanel = new TableLayoutPanel();
            rightTableLayoutPanel = new TableLayoutPanel();
            richTextBox = new RichTextBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            flowLayoutPanel2 = new FlowLayoutPanel();
            flowLayoutPanel5 = new FlowLayoutPanel();
            mainTableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainTableLayoutPanel
            // 
            mainTableLayoutPanel.ColumnCount = 3;
            mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            mainTableLayoutPanel.Controls.Add(flowLayoutPanel6, 2, 2);
            mainTableLayoutPanel.Controls.Add(flowLayoutPanel4, 2, 0);
            mainTableLayoutPanel.Controls.Add(flowLayoutPanel3, 0, 0);
            mainTableLayoutPanel.Controls.Add(leftTableLayoutPanel, 0, 1);
            mainTableLayoutPanel.Controls.Add(rightTableLayoutPanel, 2, 1);
            mainTableLayoutPanel.Controls.Add(richTextBox, 1, 1);
            mainTableLayoutPanel.Controls.Add(flowLayoutPanel1, 1, 2);
            mainTableLayoutPanel.Controls.Add(flowLayoutPanel2, 1, 0);
            mainTableLayoutPanel.Controls.Add(flowLayoutPanel5, 0, 2);
            mainTableLayoutPanel.Dock = DockStyle.Fill;
            mainTableLayoutPanel.Location = new Point(0, 0);
            mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            mainTableLayoutPanel.RowCount = 3;
            mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            mainTableLayoutPanel.Size = new Size(1688, 943);
            mainTableLayoutPanel.TabIndex = 0;
            // 
            // flowLayoutPanel6
            // 
            flowLayoutPanel6.Dock = DockStyle.Fill;
            flowLayoutPanel6.Location = new Point(1269, 757);
            flowLayoutPanel6.Name = "flowLayoutPanel6";
            flowLayoutPanel6.Size = new Size(416, 183);
            flowLayoutPanel6.TabIndex = 8;
            // 
            // flowLayoutPanel4
            // 
            flowLayoutPanel4.Dock = DockStyle.Fill;
            flowLayoutPanel4.Location = new Point(1269, 3);
            flowLayoutPanel4.Name = "flowLayoutPanel4";
            flowLayoutPanel4.Size = new Size(416, 88);
            flowLayoutPanel4.TabIndex = 6;
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.Dock = DockStyle.Fill;
            flowLayoutPanel3.Location = new Point(3, 3);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new Size(416, 88);
            flowLayoutPanel3.TabIndex = 5;
            // 
            // leftTableLayoutPanel
            // 
            leftTableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            leftTableLayoutPanel.ColumnCount = 2;
            leftTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            leftTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            leftTableLayoutPanel.Location = new Point(3, 97);
            leftTableLayoutPanel.Name = "leftTableLayoutPanel";
            leftTableLayoutPanel.RowCount = 3;
            leftTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            leftTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            leftTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            leftTableLayoutPanel.Size = new Size(416, 654);
            leftTableLayoutPanel.TabIndex = 0;
            // 
            // rightTableLayoutPanel
            // 
            rightTableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rightTableLayoutPanel.ColumnCount = 2;
            rightTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rightTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rightTableLayoutPanel.Location = new Point(1269, 97);
            rightTableLayoutPanel.Name = "rightTableLayoutPanel";
            rightTableLayoutPanel.RowCount = 3;
            rightTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            rightTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            rightTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            rightTableLayoutPanel.Size = new Size(416, 654);
            rightTableLayoutPanel.TabIndex = 1;
            // 
            // richTextBox
            // 
            richTextBox.Dock = DockStyle.Fill;
            richTextBox.Location = new Point(425, 97);
            richTextBox.Name = "richTextBox";
            richTextBox.ReadOnly = true;
            richTextBox.Size = new Size(838, 654);
            richTextBox.TabIndex = 2;
            richTextBox.Text = "";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(425, 757);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(838, 183);
            flowLayoutPanel1.TabIndex = 3;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Dock = DockStyle.Fill;
            flowLayoutPanel2.Location = new Point(425, 3);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(838, 88);
            flowLayoutPanel2.TabIndex = 4;
            // 
            // flowLayoutPanel5
            // 
            flowLayoutPanel5.Dock = DockStyle.Fill;
            flowLayoutPanel5.Location = new Point(3, 757);
            flowLayoutPanel5.Name = "flowLayoutPanel5";
            flowLayoutPanel5.Size = new Size(416, 183);
            flowLayoutPanel5.TabIndex = 7;
            // 
            // FastAutoUI
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1688, 943);
            Controls.Add(mainTableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FastAutoUI";
            Text = "游戏界面";
            mainTableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel leftTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel rightTableLayoutPanel;
        private System.Windows.Forms.RichTextBox richTextBox;

        #endregion

        private FlowLayoutPanel flowLayoutPanel4;
        private FlowLayoutPanel flowLayoutPanel3;
        private FlowLayoutPanel flowLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel2;
        private FlowLayoutPanel flowLayoutPanel6;
        private FlowLayoutPanel flowLayoutPanel5;
    }
}
