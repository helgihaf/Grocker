namespace Marson.Grocker.WinForms
{
    partial class WatcherView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WatcherView));
            this.panelTop = new System.Windows.Forms.Panel();
            this.buttonPause = new System.Windows.Forms.Button();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.buttonPlay = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.textBoxPath);
            this.panelTop.Controls.Add(this.buttonPlay);
            this.panelTop.Controls.Add(this.buttonPause);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(553, 29);
            this.panelTop.TabIndex = 0;
            // 
            // buttonPause
            // 
            this.buttonPause.ImageKey = "pause";
            this.buttonPause.ImageList = this.imageList;
            this.buttonPause.Location = new System.Drawing.Point(3, 2);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(30, 22);
            this.buttonPause.TabIndex = 1;
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // textBoxPath
            // 
            this.textBoxPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPath.Location = new System.Drawing.Point(39, 3);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.ReadOnly = true;
            this.textBoxPath.Size = new System.Drawing.Size(511, 20);
            this.textBoxPath.TabIndex = 0;
            // 
            // richTextBox
            // 
            this.richTextBox.BackColor = System.Drawing.Color.Black;
            this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox.ForeColor = System.Drawing.Color.Lime;
            this.richTextBox.Location = new System.Drawing.Point(0, 29);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.Size = new System.Drawing.Size(553, 314);
            this.richTextBox.TabIndex = 1;
            this.richTextBox.Text = "";
            this.richTextBox.WordWrap = false;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "pause");
            this.imageList.Images.SetKeyName(1, "play");
            // 
            // buttonPlay
            // 
            this.buttonPlay.ImageKey = "play";
            this.buttonPlay.ImageList = this.imageList;
            this.buttonPlay.Location = new System.Drawing.Point(3, 2);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(30, 22);
            this.buttonPlay.TabIndex = 2;
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Visible = false;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // WatcherView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.panelTop);
            this.Name = "WatcherView";
            this.Size = new System.Drawing.Size(553, 343);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.ImageList imageList;
    }
}
