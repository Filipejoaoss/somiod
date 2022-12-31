namespace Application_A
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
            this.labelAppA = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelAppA
            // 
            this.labelAppA.AutoSize = true;
            this.labelAppA.Font = new System.Drawing.Font("Rockwell Extra Bold", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAppA.ForeColor = System.Drawing.Color.DodgerBlue;
            this.labelAppA.Location = new System.Drawing.Point(124, 457);
            this.labelAppA.Name = "labelAppA";
            this.labelAppA.Size = new System.Drawing.Size(148, 20);
            this.labelAppA.TabIndex = 0;
            this.labelAppA.Text = "Application A";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Application_A.Properties.Resources.light_bulb_off1;
            this.pictureBox1.Location = new System.Drawing.Point(35, 23);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(348, 411);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 500);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelAppA);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelAppA;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

