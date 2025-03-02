using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using OfficeOpenXml;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
namespace ReportForm
{
    public partial class Form1: Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Export User Excel";
            saveFileDialog.Filter = "Excel (*.xlsx)|*.xlsx|Excel 2013 (*.xls)|*.xls";
            try
            {
                exportExcel(saveFileDialog.FileName);
                MessageBox.Show("Xuất file thành công! ");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xuất file không thành công! " + ex.Message);

            }
        }
        private void exportExcel(string path)
        {

            Excel.Application application = new Excel.Application();
            application.Application.Workbooks.Add(Type.Missing);
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                application.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText.ToString();
            }
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {

                    application.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value;
                }
            }
            application.Columns.AutoFit();
            application.ActiveWorkbook.SaveCopyAs(path);
            application.ActiveWorkbook.Saved = true;
            application.Quit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = 
        }
    }
}
