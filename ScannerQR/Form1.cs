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
using ZXing;


namespace ScannerQR
{
    public partial class Form1: Form
    {
        private FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[0].MonikerString);
            videoCaptureDevice.NewFrame += new NewFrameEventHandler(videoCaptureDevice_NewFrame);   
            videoCaptureDevice.Start();
            timer1.Start();
        }

        private void videoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(videoCaptureDevice != null && videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice.Stop();
            }
        }
        private User LayUserTheoId(int userId)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=AssignmentPRN;User ID=sa;Password=123";
            User user = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Users WHERE UserID = @userId AND IsDeleted = 0";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    user = new User
                    {
                        UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                        SupplierID = reader.IsDBNull(reader.GetOrdinal("SupplierID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("SupplierID")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        FullName = reader.GetString(reader.GetOrdinal("FullName")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                        DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth"))
                            ? (DateTime?)null
                            : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                        Gender = reader.IsDBNull(reader.GetOrdinal("Gender")) ? null : reader.GetString(reader.GetOrdinal("Gender")),
                        RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                        Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString(reader.GetOrdinal("Avatar")),
                        Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
                    };
                }

                reader.Close();
                conn.Close();
            }

            return user;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(pictureBox1.Image != null)
            {
                BarcodeReader barcodeReader = new BarcodeReader();
                Result result = barcodeReader.Decode((Bitmap)pictureBox1.Image);
                if(result != null)
                {
                    string qrCodeText = result.Text;
                    int id = int.Parse(qrCodeText);
                    User user = LayUserTheoId(id);
                    MessageBox.Show(qrCodeText, "Đăng nhập thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    videoCaptureDevice.Stop();
                    timer1.Stop();
                    this.Close();
                }
            }
        }
    }
    

    public class User
    {
        public int UserID { get; set; }
        public int? SupplierID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public int RoleID { get; set; }
        public string Avatar { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
    }
}
