namespace BrunnianLink
{
    partial class Form1
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
            tbPath = new TextBox();
            btGenerate = new Button();
            btOpen = new Button();
            cbModelChoice = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            nudLevel = new NumericUpDown();
            btViewText = new Button();
            ((System.ComponentModel.ISupportInitialize)nudLevel).BeginInit();
            SuspendLayout();
            // 
            // tbPath
            // 
            tbPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbPath.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tbPath.AutoCompleteSource = AutoCompleteSource.FileSystem;
            tbPath.Location = new Point(12, 12);
            tbPath.Name = "tbPath";
            tbPath.Size = new Size(468, 23);
            tbPath.TabIndex = 0;
            tbPath.Text = "D:\\klad\\lego\\BrunnianLink\\out\\meh.ldr";
            // 
            // btGenerate
            // 
            btGenerate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btGenerate.Location = new Point(19, 507);
            btGenerate.Name = "btGenerate";
            btGenerate.Size = new Size(75, 23);
            btGenerate.TabIndex = 1;
            btGenerate.Text = "Generate";
            btGenerate.UseVisualStyleBackColor = true;
            btGenerate.Click += BtGenerate_Click;
            // 
            // btOpen
            // 
            btOpen.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btOpen.Location = new Point(100, 507);
            btOpen.Name = "btOpen";
            btOpen.Size = new Size(75, 23);
            btOpen.TabIndex = 1;
            btOpen.Text = "Open";
            btOpen.UseVisualStyleBackColor = true;
            btOpen.Click += BtOpen_Click;
            // 
            // cbModelChoice
            // 
            cbModelChoice.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cbModelChoice.DropDownStyle = ComboBoxStyle.Simple;
            cbModelChoice.FormattingEnabled = true;
            cbModelChoice.Items.AddRange(new object[] { "Brunnian links", "Gosper tiling", "Hat Polykite Plates Y-scale trick", "Hat Polykite Technic", "Chiral Spectre", "Hilbert 3D", "Cairo tiling", "Octagonal snowflake", "Penrose rhombus tiling", "Random Maze generator", "Cross dissection tiling", "Truchet random tiling", "Heighway Twin Dragon (Truchet)", "Sierpinski (Truchet)", "Peano (Truchet)", "TerDragon", "Conway Radin pinwheel tiling", "Chair tiling", "Domino tiling", "Domino variant (9 tiles)", "Semi Detached House", "1x2 wedge tile tiling", "penta tile tiling", "Wanderer (reflections) tiling", "Wanderer (rotations) tiling", "Ammann-Beenker tiling", "Shield tiling", "Labyrinth tiling", "Hofstetter arrowed tiling", "Wunderlich I", "Wunderlich II", "Wunderlich III", "Mini Tangram", "Socolar tiling", "Wheel tiling", "Pentomino", "Pinwheel 10", "Triangle-Square (Socolar)", "Square Triangle PinWheel Variant", "Half Hex" });
            cbModelChoice.Location = new Point(93, 75);
            cbModelChoice.Name = "cbModelChoice";
            cbModelChoice.Size = new Size(387, 383);
            cbModelChoice.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 73);
            label1.Name = "label1";
            label1.Size = new Size(44, 15);
            label1.TabIndex = 3;
            label1.Text = "Model:";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(19, 466);
            label2.Name = "label2";
            label2.Size = new Size(37, 15);
            label2.TabIndex = 4;
            label2.Text = "Level:";
            // 
            // nudLevel
            // 
            nudLevel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            nudLevel.Location = new Point(100, 464);
            nudLevel.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            nudLevel.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudLevel.Name = "nudLevel";
            nudLevel.Size = new Size(120, 23);
            nudLevel.TabIndex = 5;
            nudLevel.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // btViewText
            // 
            btViewText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btViewText.Location = new Point(181, 507);
            btViewText.Name = "btViewText";
            btViewText.Size = new Size(116, 23);
            btViewText.TabIndex = 6;
            btViewText.Text = "View .ldr as text";
            btViewText.UseVisualStyleBackColor = true;
            btViewText.Click += BtViewText_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(492, 545);
            Controls.Add(btViewText);
            Controls.Add(nudLevel);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(cbModelChoice);
            Controls.Add(btOpen);
            Controls.Add(btGenerate);
            Controls.Add(tbPath);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)nudLevel).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tbPath;
        private Button btGenerate;
        private Button btOpen;
        private ComboBox cbModelChoice;
        private Label label1;
        private Label label2;
        private NumericUpDown nudLevel;
        private Button btViewText;
    }
}