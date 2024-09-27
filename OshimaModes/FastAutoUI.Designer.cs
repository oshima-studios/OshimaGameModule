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
            rightTableLayoutPanel = new TableLayoutPanel();
            richTextBox = new RichTextBox();
            leftTableLayoutPanel = new TableLayoutPanel();
            characterStatus1 = new CharacterStatus();
            mainTableLayoutPanel.SuspendLayout();
            leftTableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainTableLayoutPanel
            // 
            mainTableLayoutPanel.ColumnCount = 3;
            mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            mainTableLayoutPanel.Controls.Add(rightTableLayoutPanel, 2, 1);
            mainTableLayoutPanel.Controls.Add(richTextBox, 1, 1);
            mainTableLayoutPanel.Controls.Add(leftTableLayoutPanel, 0, 1);
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
            // leftTableLayoutPanel
            // 
            leftTableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            leftTableLayoutPanel.ColumnCount = 2;
            leftTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            leftTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            leftTableLayoutPanel.Controls.Add(characterStatus1, 0, 0);
            leftTableLayoutPanel.Location = new Point(3, 97);
            leftTableLayoutPanel.Name = "leftTableLayoutPanel";
            leftTableLayoutPanel.RowCount = 3;
            leftTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            leftTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            leftTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            leftTableLayoutPanel.Size = new Size(416, 654);
            leftTableLayoutPanel.TabIndex = 0;
            // 
            // characterStatus1
            // 
            characterStatus1.Location = new Point(3, 3);
            characterStatus1.Name = "characterStatus1";
            characterStatus1.Size = new Size(202, 211);
            characterStatus1.TabIndex = 0;
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
            leftTableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel leftTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel rightTableLayoutPanel;
        private System.Windows.Forms.RichTextBox richTextBox;

        #endregion

        private CharacterStatus characterStatus1;
    }
}
