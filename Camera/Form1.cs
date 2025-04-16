using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace CameraTest
{
    public partial class Form1 : Form
    {


        private FilterInfoCollection camera;
        private VideoCaptureDevice imageCamera;
        private int userId;
        public Form1()
        {
            if (imageCamera != null && imageCamera.IsRunning)
            {
                imageCamera.Stop();
                imageCamera = null;
            }
            InitializeComponent();
            camera = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (camera.Count == 0 || camera == null)
            {
                MessageBox.Show("Không có camera trong máy!");
                return;
            }

            this.FormBorderStyle = FormBorderStyle.None;

            this.Bounds = Screen.PrimaryScreen.Bounds;

            this.TopMost = true;

            this.CreateParams.ExStyle |= 0x80;


            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                userId = int.Parse(args[1]);
                MessageBox.Show($"Chào mừng người dùng ID: {userId}");
            }
            else
            {
                MessageBox.Show("Không nhận được User ID!");
            }


        }



        private void LuuAnhVaoDatabase(int userId, string imagePath)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=AssignmentPRN;User ID=sa;Password=123";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "UPDATE Users SET Avatar = @path WHERE UserID = @userId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@path", imagePath);
                cmd.Parameters.AddWithValue("@userId", userId);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void XoaUserTheoId(int userId)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=AssignmentPRN;User ID=sa;Password=123";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM Users WHERE UserID = @userId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Chưa có ảnh để lưu!");
                return;
            }

            imageCamera.SignalToStop();
            imageCamera.WaitForStop();

            DialogResult result = MessageBox.Show("Ảnh vừa chụp đã hiển thị. Bạn có muốn lưu ảnh này không?",
                                                 "Xác nhận lưu ảnh",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                saveFileDialog1.InitialDirectory = @"D:\FPTU\Kì5\PRN212\AssignmentPRN\AssignmentPRN_SLN\DataAccess\Images\Avar\";

                saveFileDialog1.FileName = $"avatar_{userId}.jpg";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image.Save(saveFileDialog1.FileName);

                    string savedPath = saveFileDialog1.FileName;

                    LuuAnhVaoDatabase(userId, savedPath);

                    MessageBox.Show("Đăng ký thành công!", "Thông báo");
                }
                else
                {
                    XoaUserTheoId(userId);
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                    MessageBox.Show("Đăng ký thất bại!", "Thông báo");
                }
                imageCamera.Stop();
                imageCamera = null;
                this.Close();
            }


        }



        private void imageCamera_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = bitmap;
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            if (imageCamera != null && imageCamera.IsRunning)
            {
                imageCamera.Stop();
                imageCamera = null;
            }
            imageCamera = new VideoCaptureDevice(camera[0].MonikerString);
            imageCamera.NewFrame += new NewFrameEventHandler(imageCamera_NewFrame);
            imageCamera.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (imageCamera != null && imageCamera.IsRunning)
            {
                imageCamera.Stop();
                imageCamera = null;
            }
        }

        
    }
}
