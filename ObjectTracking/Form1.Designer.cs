namespace ObjectTracking
{
	partial class Form1
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.videoSourcePlayer = new AForge.Controls.VideoSourcePlayer();
			this.dataRepeater1 = new Microsoft.VisualBasic.PowerPacks.DataRepeater();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.left = new System.Windows.Forms.Label();
			this.top = new System.Windows.Forms.Label();
			this.width = new System.Windows.Forms.Label();
			this.height = new System.Windows.Forms.Label();
			this.dataRepeater1.ItemTemplate.SuspendLayout();
			this.dataRepeater1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(589, 456);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Upload";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Upload);
			// 
			// videoSourcePlayer
			// 
			this.videoSourcePlayer.Location = new System.Drawing.Point(342, 0);
			this.videoSourcePlayer.Name = "videoSourcePlayer";
			this.videoSourcePlayer.Size = new System.Drawing.Size(593, 450);
			this.videoSourcePlayer.TabIndex = 2;
			this.videoSourcePlayer.VideoSource = null;
			this.videoSourcePlayer.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler(this.videoSourcePlayer_NewFrame);
			// 
			// dataRepeater1
			// 
			// 
			// dataRepeater1.ItemTemplate
			// 
			this.dataRepeater1.ItemTemplate.Controls.Add(this.height);
			this.dataRepeater1.ItemTemplate.Controls.Add(this.width);
			this.dataRepeater1.ItemTemplate.Controls.Add(this.top);
			this.dataRepeater1.ItemTemplate.Controls.Add(this.left);
			this.dataRepeater1.ItemTemplate.Controls.Add(this.label5);
			this.dataRepeater1.ItemTemplate.Controls.Add(this.label4);
			this.dataRepeater1.ItemTemplate.Controls.Add(this.label3);
			this.dataRepeater1.ItemTemplate.Controls.Add(this.label2);
			this.dataRepeater1.ItemTemplate.Size = new System.Drawing.Size(322, 99);
			this.dataRepeater1.Location = new System.Drawing.Point(941, 33);
			this.dataRepeater1.Name = "dataRepeater1";
			this.dataRepeater1.Size = new System.Drawing.Size(330, 427);
			this.dataRepeater1.TabIndex = 3;
			this.dataRepeater1.Text = "dataRepeater1";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(145, 3);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(38, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "Width:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(69, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Top:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(28, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Left:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(1061, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Objects in Image";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(230, 3);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(41, 13);
			this.label5.TabIndex = 3;
			this.label5.Text = "Height:";
			// 
			// left
			// 
			this.left.AutoSize = true;
			this.left.Location = new System.Drawing.Point(28, 3);
			this.left.Name = "left";
			this.left.Size = new System.Drawing.Size(35, 13);
			this.left.TabIndex = 4;
			this.left.Text = "label6";
			// 
			// top
			// 
			this.top.AutoSize = true;
			this.top.Location = new System.Drawing.Point(104, 3);
			this.top.Name = "top";
			this.top.Size = new System.Drawing.Size(35, 13);
			this.top.TabIndex = 5;
			this.top.Text = "label7";
			// 
			// width
			// 
			this.width.AutoSize = true;
			this.width.Location = new System.Drawing.Point(189, 3);
			this.width.Name = "width";
			this.width.Size = new System.Drawing.Size(35, 13);
			this.width.TabIndex = 6;
			this.width.Text = "label8";
			// 
			// height
			// 
			this.height.AutoSize = true;
			this.height.Location = new System.Drawing.Point(277, 3);
			this.height.Name = "height";
			this.height.Size = new System.Drawing.Size(35, 13);
			this.height.TabIndex = 7;
			this.height.Text = "label9";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1283, 491);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dataRepeater1);
			this.Controls.Add(this.videoSourcePlayer);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.dataRepeater1.ItemTemplate.ResumeLayout(false);
			this.dataRepeater1.ItemTemplate.PerformLayout();
			this.dataRepeater1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private AForge.Controls.VideoSourcePlayer videoSourcePlayer;
		private Microsoft.VisualBasic.PowerPacks.DataRepeater dataRepeater1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label height;
		private System.Windows.Forms.Label width;
		private System.Windows.Forms.Label top;
		private System.Windows.Forms.Label left;
		private System.Windows.Forms.Label label5;
	}
}