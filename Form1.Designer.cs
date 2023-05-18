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
            this.tbPath = new System.Windows.Forms.TextBox();
            this.btGenerate = new System.Windows.Forms.Button();
            this.btOpen = new System.Windows.Forms.Button();
            this.cbModelChoice = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudLevel = new System.Windows.Forms.NumericUpDown();
            this.btViewText = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // tbPath
            // 
            this.tbPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.tbPath.Location = new System.Drawing.Point(14, 16);
            this.tbPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(534, 27);
            this.tbPath.TabIndex = 0;
            this.tbPath.Text = "D:\\klad\\lego\\BrunnianLink\\out\\meh.ldr";
            // 
            // btGenerate
            // 
            this.btGenerate.Location = new System.Drawing.Point(14, 219);
            this.btGenerate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btGenerate.Name = "btGenerate";
            this.btGenerate.Size = new System.Drawing.Size(86, 31);
            this.btGenerate.TabIndex = 1;
            this.btGenerate.Text = "Generate";
            this.btGenerate.UseVisualStyleBackColor = true;
            // 
            // btOpen
            // 
            this.btOpen.Location = new System.Drawing.Point(106, 219);
            this.btOpen.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btOpen.Name = "btOpen";
            this.btOpen.Size = new System.Drawing.Size(86, 31);
            this.btOpen.TabIndex = 1;
            this.btOpen.Text = "Open";
            this.btOpen.UseVisualStyleBackColor = true;
            this.btOpen.Click += new System.EventHandler(this.BtOpen_Click);
            // 
            // cbModelChoice
            // 
            this.cbModelChoice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbModelChoice.FormattingEnabled = true;
            this.cbModelChoice.Items.AddRange(new object[] {
            "Brunnian links",
            "Gosper tiling",
            "Hat Polykite",
            "Hilbert 3D",
            "Cross tiling",
            "Conway Radin pinwheel tiling",
            "Chair tiling",
            "Tennis tiling",
            "Wanderer (reflections) tiling",
            "Wanderer (rotations) tiling",
            "Ammann-Beenker tiling",
            "Penrose rhombus tiling"});
            this.cbModelChoice.Location = new System.Drawing.Point(106, 93);
            this.cbModelChoice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbModelChoice.Name = "cbModelChoice";
            this.cbModelChoice.Size = new System.Drawing.Size(442, 28);
            this.cbModelChoice.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Model:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 164);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Level:";
            // 
            // nudLevel
            // 
            this.nudLevel.Location = new System.Drawing.Point(106, 161);
            this.nudLevel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nudLevel.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLevel.Name = "nudLevel";
            this.nudLevel.Size = new System.Drawing.Size(137, 27);
            this.nudLevel.TabIndex = 5;
            this.nudLevel.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // btViewText
            // 
            this.btViewText.Location = new System.Drawing.Point(199, 219);
            this.btViewText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btViewText.Name = "btViewText";
            this.btViewText.Size = new System.Drawing.Size(133, 31);
            this.btViewText.TabIndex = 6;
            this.btViewText.Text = "View .ldr as text";
            this.btViewText.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 265);
            this.Controls.Add(this.btViewText);
            this.Controls.Add(this.nudLevel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbModelChoice);
            this.Controls.Add(this.btOpen);
            this.Controls.Add(this.btGenerate);
            this.Controls.Add(this.tbPath);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.nudLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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