using System.Diagnostics;
using DotNET;

namespace WinFormsApp
{
    public partial class UI : Form
    {
        List<string> wordPath = new List<string>();
        string excelPath;

        public UI()
        {
            InitializeComponent();
            openFileDialog1.Filter = "Word files(*.docx)|*.docx|All files(*.*)|*.*";
            saveFileDialog1.Filter = "Excel files(*.xlsx)|.xlsx";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void chooseWordFolder_Click(object sender, EventArgs e)
        {
            string failedFiles;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                wordPath = DotNET.WordFileCollector.Collect(folderBrowserDialog1.SelectedPath, false, out failedFiles);
                if (!failedFiles.Equals(string.Empty))
                {
                    MessageBox.Show(
                        failedFiles,
                        "Ошибка открытия файлов",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                listBox1.Items.Clear();
                listBox1.Items.AddRange(wordPath.ToArray());
            }
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.AddRange(openFileDialog1.FileName);
            wordPath.Clear();
            wordPath.Add(openFileDialog1.FileName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string code = DotNET.Converter.Main(wordPath, excelPath);
            if (code.Equals("Success"))
            {
                MessageBox.Show("Success");
            }
            else
            {
                MessageBox.Show("Error. Check logs");
            }
        }

        private void saveExcel_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            listBox2.Items.Clear();
            listBox2.Items.AddRange(saveFileDialog1.FileName);
            excelPath = saveFileDialog1.FileName;
        }
    }
}
