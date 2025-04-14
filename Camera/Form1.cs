using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public Form1()
        {
            InitializeComponent();
            camera = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (camera.Count == 0 || camera == null)
            {
                MessageBox.Show("Không có camera trong máy!");
                return;
            }
            // Lấy tham số từ command line
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                int userId = int.Parse(args[1]); // Lấy Id từ tham số
                MessageBox.Show($"Nhận được User ID: {userId}");
            }
            else
            {
                MessageBox.Show("Không nhận được User ID!");
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
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image.Save(saveFileDialog1.FileName);
                    MessageBox.Show("Lưu ảnh thành công!");
                }
            }
            else
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            imageCamera.Start();
        }


        private void imageCamera_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = bitmap;
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            imageCamera = new VideoCaptureDevice(camera[0].MonikerString);
            imageCamera.NewFrame += new NewFrameEventHandler(imageCamera_NewFrame);
            imageCamera.Start();
        }
    }
}
