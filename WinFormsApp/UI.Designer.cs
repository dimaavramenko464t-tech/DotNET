namespace WinFormsApp
{
    partial class UI
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
            button2 = new Button();
            chooseWordFolder = new Button();
            folderBrowserDialog1 = new FolderBrowserDialog();
            openFileDialog1 = new OpenFileDialog();
            button1 = new Button();
            label1 = new Label();
            label3 = new Label();
            listBox1 = new ListBox();
            label2 = new Label();
            saveExcel = new Button();
            listBox2 = new ListBox();
            saveFileDialog1 = new SaveFileDialog();
            SuspendLayout();
            // 
            // button2
            // 
            button2.Location = new Point(622, 63);
            button2.Name = "button2";
            button2.Size = new Size(206, 46);
            button2.TabIndex = 1;
            button2.Text = "Choose one file";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // chooseWordFolder
            // 
            chooseWordFolder.Location = new Point(29, 63);
            chooseWordFolder.Name = "chooseWordFolder";
            chooseWordFolder.Size = new Size(202, 46);
            chooseWordFolder.TabIndex = 2;
            chooseWordFolder.Text = "Choose Folder";
            chooseWordFolder.UseVisualStyleBackColor = true;
            chooseWordFolder.Click += chooseWordFolder_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileOk += openFileDialog1_FileOk;
            // 
            // button1
            // 
            button1.Location = new Point(29, 554);
            button1.Name = "button1";
            button1.Size = new Size(391, 112);
            button1.TabIndex = 4;
            button1.Text = "Convert";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 23);
            label1.Name = "label1";
            label1.Size = new Size(126, 32);
            label1.TabIndex = 5;
            label1.Text = "Word path";
            // 
            // label3
            // 
            label3.Location = new Point(0, 0);
            label3.Name = "label3";
            label3.Size = new Size(100, 23);
            label3.TabIndex = 0;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(29, 126);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(799, 132);
            listBox1.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(29, 287);
            label2.Name = "label2";
            label2.Size = new Size(122, 32);
            label2.TabIndex = 7;
            label2.Text = "Excel path";
            // 
            // saveExcel
            // 
            saveExcel.Location = new Point(29, 337);
            saveExcel.Name = "saveExcel";
            saveExcel.Size = new Size(343, 46);
            saveExcel.TabIndex = 8;
            saveExcel.Text = "Choose save excel directory";
            saveExcel.UseVisualStyleBackColor = true;
            saveExcel.Click += saveExcel_Click;
            // 
            // listBox2
            // 
            listBox2.FormattingEnabled = true;
            listBox2.Location = new Point(29, 399);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(799, 68);
            listBox2.TabIndex = 9;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileOk += saveFileDialog1_FileOk;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(869, 678);
            Controls.Add(listBox2);
            Controls.Add(saveExcel);
            Controls.Add(label2);
            Controls.Add(listBox1);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(chooseWordFolder);
            Controls.Add(button2);
            DoubleBuffered = true;
            ForeColor = SystemColors.ControlText;
            Name = "Form1";
            Text = "WordToExelParser";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        #endregion
        private Button button2;
        private Button chooseWordFolder;
        private FolderBrowserDialog folderBrowserDialog1;
        private OpenFileDialog openFileDialog1;
        private Button button1;
        private Label label1;
        private Label label3;
        private ListBox listBox1;
        private Label label2;
        private Button saveExcel;
        private ListBox listBox2;
        private SaveFileDialog saveFileDialog1;
    }
}
