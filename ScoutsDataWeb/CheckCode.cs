namespace JC_SoccerWeb  
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Web;
    using System.Web.UI;

    public class CheckCode : Page
    {
        private void CreateCheckCodeImage(string checkCode)
        {
            if ((checkCode != null) && (checkCode.Trim() != string.Empty))
            {
                Bitmap image = new Bitmap((int) Math.Ceiling((double) (checkCode.Length * 12.5)), 0x16);
                Graphics graphics = Graphics.FromImage(image);
                try
                {
                    Random random = new Random();
                    graphics.Clear(Color.White);
                    for (int i = 0; i < 0x19; i++)
                    {
                        int num2 = random.Next(image.Width);
                        int num3 = random.Next(image.Width);
                        int num4 = random.Next(image.Height);
                        int num5 = random.Next(image.Height);
                        graphics.DrawLine(new Pen(Color.Silver), num2, num4, num3, num5);
                    }
                    Font font = new Font("Arial", 12f, FontStyle.Italic | FontStyle.Bold);
                    LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                    graphics.DrawString(checkCode, font, brush, (float) 2f, (float) 2f);
                    for (int j = 0; j < 100; j++)
                    {
                        int x = random.Next(image.Width);
                        int y = random.Next(image.Height);
                        image.SetPixel(x, y, Color.FromArgb(random.Next()));
                    }
                    graphics.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                    MemoryStream stream = new MemoryStream();
                    image.Save(stream, ImageFormat.Gif);
                    base.Response.ClearContent();
                    base.Response.ContentType = "image/Gif";
                    base.Response.BinaryWrite(stream.ToArray());
                }
                finally
                {
                    graphics.Dispose();
                    image.Dispose();
                }
            }
        }

        private string GenerateCheckCode()
        {
            string str = string.Empty;
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char) (0x30 + ((ushort) (num % 10)));
                }
                else
                {
                    ch = (char) (0x41 + ((ushort) (num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            base.Response.Cookies.Add(new HttpCookie("CheckCodeS", str));
            return str;
        }

        private void InitializeComponent()
        {
            base.Load += new EventHandler(this.Page_Load);
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {
            this.CreateCheckCodeImage(this.GenerateCheckCode());
        }
    }
}

