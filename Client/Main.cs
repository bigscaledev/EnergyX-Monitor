using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            captureTimer.Start();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            captureTimer.Stop();
        }

        private static async Task Capture()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
            }
            byte[] data;
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                data = stream.ToArray();
            }

            // Convert the image data to Base64 encoding
            string base64Image = Convert.ToBase64String(data);
            byte[] imageBytes = Convert.FromBase64String(base64Image);

            using (HttpClient client = new HttpClient())
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                ByteArrayContent imageContent = new ByteArrayContent(imageBytes);
                content.Add(imageContent, "image", "image.jpg");

                string serverUrl = "http://127.0.0.1:8000/api/capture";
                HttpResponseMessage response = await client.PostAsync(serverUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Image uploaded successfully.");
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
        }

        private void captureTimer_Tick(object sender, EventArgs e)
        {
            Capture();
        }
    }
}
