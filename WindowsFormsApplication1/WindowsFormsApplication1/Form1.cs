using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;


using AForge.Video;

using AForge.Video.DirectShow;
using AForge.Math.Geometry;
using System.IO.Ports;


using Point = System.Drawing.Point; 
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        private FilterInfoCollection VideoCapTureDevices;
        ///videocapturedevices isminde tanımladığımız değişken bilgisayara kaç kamera bağlıysa onları tutan bir dizi.
        private VideoCaptureDevice Finalvideo;
        //finalviedeo ise bizim kullanacağımız cihaz.
        

        public Form1()
        {

            InitializeComponent();

        }

        int R; //Trackbarın değişkeneleri
        int G;
        int B;
        
       
        private void Form1_Load(object sender, EventArgs e)
        {
            string[] portlar = SerialPort.GetPortNames();
            //portlar adında dizi oluşturuldu ve bilgisayarda kullanılan tüm seri portları bu diziye attık.
            foreach (string port in portlar)
                //foreach de portlar dizisini en baştan sona okuduk ve her okuduğu değeri string değişkenine attık 
                    portCombo.Items.Add(port);
            //string değişkenini portcombo a ekledik.

        VideoCapTureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            //  VideoCapTureDevices dizisine mevcut kameraları dolduruyoruz.
            foreach (FilterInfo VideoCaptureDevice in VideoCapTureDevices)
            {

                comboBox1.Items.Add(VideoCaptureDevice.Name);
                //kameraları combobox a dolduruyoruz.
            }

            comboBox1.SelectedIndex = 0;
            //Comboboxtaki ilk index numaralı kameranın ekranda görünmesi için
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Finalvideo = new VideoCaptureDevice(VideoCapTureDevices[comboBox1.SelectedIndex].MonikerString);
             //başlaya basıldığıdnda yukarda tanımladığımız finalvideo değişkenine comboboxta seçilmş olan kamerayı atıyoruz.
            Finalvideo.NewFrame += new NewFrameEventHandler(Finalvideo_NewFrame);
            Finalvideo.DesiredFrameRate = 20;//saniyede kaç görüntü alsın istiyorsanız. FPS olaarak
            Finalvideo.DesiredFrameSize = new Size(320, 240);//görüntü boyutları
            Finalvideo.Start();
            //kamerayı başlatıyoruz.
        }

        void Finalvideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            //kısaca bu eventta kameradan alınan görüntüyü kaynakbox a atıyoruz.

            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            kaynakBox.Image = image;
            


            if (Kirmizi.Checked)
            {
                
                // filtre uyguluyoruz.
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // değerlere göre bir merkez belirlenir ve yarıçap ayarlanır ve  yarıçapa göre küre oluşturarak pikselleri tutar. 
                filter.CenterColor = new RGB(Color.FromArgb(215, 0, 0));
                filter.Radius = 100;
               
                filter.ApplyInPlace(image1);

                
                nesnebul(image1);
                
            }

            if (Mavi.Checked)
            {

                   EuclideanColorFiltering filter = new EuclideanColorFiltering();
                
                filter.CenterColor = new RGB(Color.FromArgb(30, 40, 245));
                filter.Radius = 100;
               
                filter.ApplyInPlace(image1);
                
                nesnebul(image1);
                
            }
            if(Yesil.Checked){

                
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                
                filter.CenterColor = new RGB(Color.FromArgb(0, 215, 0));
                filter.Radius = 100;
                
                filter.ApplyInPlace(image1);

                nesnebul(image1);
            
            
            
            }
            

            if (ManuelAyarlama.Checked)
            {
                                            
                
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
               
                filter.CenterColor = new RGB(Color.FromArgb(R, G, B));
                filter.Radius = 100;
                filter.ApplyInPlace(image1);

                nesnebul(image1);

            }

          
          
        }
        public void nesnebul(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 5;
            blobCounter.MinHeight = 5;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            
            BitmapData objectsData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            // grayscaling
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            //BT709 Uyguluyoruz
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));
           
            image.UnlockBits(objectsData);


            blobCounter.ProcessImage(image);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Blob[] blobs = blobCounter.GetObjectsInformation();
            islemBox.Image = image;



            if (CisimTakibi.Checked)
            {
                //cisim takibi 

                foreach (Rectangle recs in rects)
                {
                    if (rects.Length > 0)
                    {
                        Rectangle objectRect = rects[0];
                       
                        Graphics g = kaynakBox.CreateGraphics();
                        using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                        {
                            g.DrawRectangle(pen, objectRect);
                        }
                        //Cizdirilen Dikdörtgenin Koordinatlari aliniyor.
                        int objectX = objectRect.X + (objectRect.Width / 2);
                        int objectY = objectRect.Y + (objectRect.Height / 2);
                  
                        g.Dispose();
                        if (objectRect.Y <= 80 && objectRect.Y >= 0 && objectRect.X<=106 && objectRect.X>=0)
                        {
                            serialPort1.Write("1");
                            button4.BackColor = Color.LightSeaGreen;
                            button5.BackColor = Color.White;
                            button11.BackColor = Color.White;
                            button6.BackColor = Color.White;
                            button7.BackColor = Color.White;
                            button8.BackColor = Color.White;
                            button9.BackColor = Color.White;
                            button10.BackColor = Color.White;
                            button12.BackColor = Color.White;
                        }
                        else if (objectRect.Y <= 80 && objectRect.Y >= 0 && objectRect.X <= 212 && objectRect.X >= 106)
                        {
                            serialPort1.Write("2");
                            button5.BackColor = Color.LightSeaGreen;
                            button11.BackColor = Color.White;
                            button4.BackColor = Color.White;
                            button6.BackColor = Color.White;
                            button7.BackColor = Color.White;
                            button8.BackColor = Color.White;
                            button9.BackColor = Color.White;
                            button10.BackColor = Color.White;
                            button12.BackColor = Color.White;
                        }
                        else if (objectRect.Y <= 80 && objectRect.Y >= 0 && objectRect.X <= 320 && objectRect.X >= 212)
                        {
                            serialPort1.Write("3");
                            button6.BackColor = Color.LightSeaGreen;
                            button5.BackColor = Color.White;
                            button4.BackColor = Color.White;
                            button11.BackColor = Color.White;
                            button7.BackColor = Color.White;
                            button8.BackColor = Color.White;
                            button9.BackColor = Color.White;
                            button10.BackColor = Color.White;
                            button12.BackColor = Color.White;
                        }
                        else if (objectRect.Y <= 160 && objectRect.Y >= 80 && objectRect.X <= 106 && objectRect.X >= 0)
                        {
                            serialPort1.Write("4");
                            button7.BackColor = Color.LightSeaGreen;
                            button5.BackColor = Color.White;
                            button4.BackColor = Color.White;
                            button6.BackColor = Color.White;
                            button11.BackColor = Color.White;
                            button8.BackColor = Color.White;
                            button9.BackColor = Color.White;
                            button10.BackColor = Color.White;
                            button12.BackColor = Color.White;
                        }
                        else if (objectRect.Y <= 160 && objectRect.Y >= 80 && objectRect.X <= 212 && objectRect.X >= 106)
                        {
                            serialPort1.Write("5");
                            button8.BackColor = Color.LightSeaGreen;
                            button5.BackColor = Color.White;
                            button4.BackColor = Color.White;
                            button6.BackColor = Color.White;
                            button7.BackColor = Color.White;
                            button11.BackColor = Color.White;
                            button9.BackColor = Color.White;
                            button10.BackColor = Color.White;
                            button12.BackColor = Color.White;
                        }
                        else if (objectRect.Y <= 160 && objectRect.Y >= 80 && objectRect.X <= 320 && objectRect.X >= 212)
                        {
                            serialPort1.Write("6");
                            button9.BackColor = Color.LightSeaGreen;
                            button5.BackColor = Color.White;
                            button4.BackColor = Color.White;
                            button6.BackColor = Color.White;
                            button7.BackColor = Color.White;
                            button8.BackColor = Color.White;
                            button11.BackColor = Color.White;
                            button10.BackColor = Color.White;
                            button12.BackColor = Color.White;
                        }
                        else if (objectRect.Y <= 240 && objectRect.Y >= 160 && objectRect.X <= 106 && objectRect.X >= 0)
                        {
                            serialPort1.Write("7");
                            button10.BackColor = Color.LightSeaGreen;
                            button5.BackColor = Color.White;
                            button4.BackColor = Color.White;
                            button6.BackColor = Color.White;
                            button7.BackColor = Color.White;
                            button8.BackColor = Color.White;
                            button9.BackColor = Color.White;
                            button11.BackColor = Color.White;
                            button12.BackColor = Color.White;
                        }
                        else if (objectRect.Y <= 240 && objectRect.Y >= 160 && objectRect.X <= 212 && objectRect.X >= 106)
                        {
                            serialPort1.Write("8");
                            button11.BackColor = Color.LightSeaGreen;
                            button5.BackColor = Color.White;
                            button4.BackColor = Color.White;
                            button6.BackColor = Color.White;
                            button7.BackColor = Color.White;
                            button8.BackColor = Color.White;
                            button9.BackColor = Color.White;
                            button10.BackColor = Color.White;
                            button12.BackColor = Color.White;
                            
                        }
                        else if (objectRect.Y <= 240 && objectRect.Y >= 160 && objectRect.X <= 320 && objectRect.X >= 212)
                        {
                            serialPort1.Write("9");
                            button12.BackColor = Color.LightSeaGreen;
                            button5.BackColor = Color.White;
                            button4.BackColor = Color.White;
                            button6.BackColor = Color.White;
                            button7.BackColor = Color.White;
                            button8.BackColor = Color.White;
                            button9.BackColor = Color.White;
                            button10.BackColor = Color.White;
                            button11.BackColor = Color.White;
                            
                           
                        }



                        if (Koordinatlar.Checked){
                        this.Invoke((MethodInvoker)delegate
                        {
                            richTextBox1.Text = objectRect.Location.ToString() + "\n" + richTextBox1.Text + "\n"; ;
                            
                        });
                        }
                        
                       
                    }
                }
            }

            
            
        }

       
        private Point[] ToPointsArray(List<IntPoint> points)
        {
            Point[] array = new Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new Point(points[i].X, points[i].Y);
            }

            return array;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();
                
            }
        }

        
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            R = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            G = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            B = trackBar3.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();

            }

            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void CisimTakibi_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            serialPort1.BaudRate = 9600;
            serialPort1.PortName = portCombo.SelectedItem.ToString();
            serialPort1.Open();
            if (serialPort1.IsOpen == true)
            {
                MessageBox.Show("Port Bağlantısı Yapılmıştır.");
                portKes.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
                portBagla.Enabled = false;

            }
            else
            {
               
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            MessageBox.Show("Port Bağlantısı Kesilmiştir.");
            portKes.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            portBagla.Enabled = true;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
            //combox ın seçildiği item değiştği zaman seçilem itemi seri portumuza aktarmamız gerekir.
        {
            //seralport1 in port ismini seçilen item yap ama ilk önce string değerine çevir.
            //daha sonra serial porumuzu açalaım.
            serialPort1.PortName = portCombo.SelectedItem.ToString();
            serialPort1.Open();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void portCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }


}


