using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace Service
{
    public class EmailSenderService
    {
        private string generatedOTP = ""; // Lưu OTP tạm thời
        public string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // Sinh số ngẫu nhiên 6 chữ số
        }

        public bool SendQRCodeEmail(string to, MemoryStream ms)
        {
            bool send = false;
            string from = "lenam7546@gmail.com";
            string subject = "Mã QR của bạn";
            string pass = "unvnxqphjvvkmxlj";

            try
            {
                // Create email with QR code attachment
                string body = $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            text-align: center;
                            padding: 20px;
                        }}
                        .container {{
                            background: #ffffff;
                            padding: 20px;
                            border-radius: 8px;
                            box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
                            max-width: 500px;
                            margin: auto;
                        }}
                        .header {{
                            background: #0078D4;
                            padding: 15px;
                            border-radius: 8px 8px 0px 0px;
                            color: white;
                            font-size: 22px;
                            font-weight: bold;
                        }}
                        .footer {{
                            font-size: 12px;
                            color: #7f8c8d;
                            margin-top: 20px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            Mã QR của bạn
                        </div>
                        <p>Xin chào,</p>
                        <p>Đây là mã QR của bạn. Vui lòng sử dụng mã này để đăng nhập.</p>
                        <img src='cid:qrcode' alt='QR Code' style='max-width:200px;'/>
                        <p class='footer'>Nếu bạn không yêu cầu mã QR này, hãy bỏ qua email này.</p>
                        <p class='footer'>Trân trọng,<br>Đội ngũ hỗ trợ</p>
                    </div>
                </body>
                </html>";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                // Add QR code as inline attachment
                ms.Position = 0; // Đảm bảo vị trí stream ở đầu
                Attachment inline = new Attachment(ms, "qrcode.png", "image/png");
                inline.ContentDisposition.Inline = true;
                inline.ContentId = "qrcode";
                mail.Attachments.Add(inline);

                // Send email
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(from, pass);
                    smtp.Send(mail);
                }

                send = true;
            }
            catch (Exception ex)
            {

            }

            return send;
        }

        public bool SendEmail(string to, string OTP)
        {

            bool send = false;
            string from, subject, pass, body;
            from = "lenam7546@gmail.com";

            subject = "Mã xác nhận đặt lại mật khẩu";

            generatedOTP = OTP; // Tạo mã OTP

            // Nội dung email HTML chuyên nghiệp
            body = $@"
    <html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                text-align: center;
                padding: 20px;
            }}
            .container {{
                background: #ffffff;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
                max-width: 500px;
                margin: auto;
            }}
            .header {{
                background: #0078D4;
                padding: 15px;
                border-radius: 8px 8px 0px 0px;
                color: white;
                font-size: 22px;
                font-weight: bold;
            }}
            .otp-code {{
                font-size: 28px;
                font-weight: bold;
                color: #e74c3c;
                background: #f9f9f9;
                display: inline-block;
                padding: 10px 20px;
                border-radius: 5px;
                margin: 20px 0;
            }}
            .footer {{
                font-size: 12px;
                color: #7f8c8d;
                margin-top: 20px;
            }}
            .button {{
                background: #0078D4;
                color: white;
                padding: 12px 20px;
                text-decoration: none;
                border-radius: 5px;
                display: inline-block;
                margin-top: 20px;
                font-size: 16px;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                Xác nhận đặt lại mật khẩu
            </div>
            <p>Xin chào,</p>
            <p>Bạn vừa yêu cầu đặt lại mật khẩu. Đây là mã xác nhận của bạn:</p>
            <div class='otp-code'>{generatedOTP}</div>
            <p>Vui lòng nhập mã này vào ứng dụng để tiếp tục quá trình đặt lại mật khẩu.</p>
            <a href='#' class='button'>Xác nhận ngay</a>
            <p class='footer'>Nếu bạn không yêu cầu đổi mật khẩu, hãy bỏ qua email này.</p>
            <p class='footer'>Trân trọng,<br>Đội ngũ hỗ trợ</p>
        </div>
    </body>
    </html>";


            pass = "unvnxqphjvvkmxlj"; 

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(from, pass);
                smtp.Send(mail);
                send = true;
                return send;
               
            }
            catch (Exception ex)
            {
                return send;
            }
        }
    }
}
