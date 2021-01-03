using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace imaj
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofile = new OpenFileDialog();
            ofile.Filter = "resimler(*.jpg,*.png,*.bmp)|*.jpg;*.png;*.bmp";
            if (ofile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            pictureBox1.ImageLocation = ofile.FileName;

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
        private Bitmap gray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Height - 1; i++)
                for (int j = 0; j < bmp.Width - 1; j++)
                {
                    int value = (bmp.GetPixel(j, i).R + bmp.GetPixel(j, i).G + bmp.GetPixel(j, i).B) / 3;
                    Color renk;
                    renk = Color.FromArgb(value, value, value);

                    bmp.SetPixel(j, i, renk);

                }

            return bmp;
        }
        private Bitmap sharp(Bitmap bmp)
        {
            int filterWidth = 3;
            int filterHeight = 3;
            double[,] filter = new double[filterWidth, filterHeight];
            filter[0, 0] = filter[0, 1] = filter[0, 2] = filter[1, 0] = filter[1, 2] = filter[2, 0] = filter[2, 1] = filter[2, 2] = -1;
            filter[1, 1] = 9;
            double factor = 1.0;
            double bias = 0.0;
            Color[,] result = new Color[bmp.Width, bmp.Height];
            for (int x = 0; x < bmp.Width; ++x)
            {
                for (int y = 0; y < bmp.Height; ++y)
                {
                    double red = 0.0, green = 0.0, blue = 0.0;
                    Color imageColor = bmp.GetPixel(x, y);

                    for (int filterX = 0; filterX < filterWidth; filterX++)
                    {
                        for (int filterY = 0; filterY < filterHeight; filterY++)
                        {
                            int imageX = (x - filterWidth / 2 + filterX + bmp.Width) % bmp.Width;
                            int imageY = (y - filterHeight / 2 + filterY + bmp.Height) % bmp.Height;
                            red += imageColor.R * filter[filterX, filterY];
                            green += imageColor.G * filter[filterX, filterY];
                            blue += imageColor.B * filter[filterX, filterY];
                        }
                        int r = Math.Min(Math.Max((int)(factor * red + bias), 0), 255);
                        int g = Math.Min(Math.Max((int)(factor * green + bias), 0), 255);
                        int b = Math.Min(Math.Max((int)(factor * blue + bias), 0), 255);

                        result[x, y] = Color.FromArgb(r, g, b);
                    }
                }
            }
            Bitmap sharpenImage = new Bitmap(bmp.Width, bmp.Height);
            for (int i = 0; i < bmp.Width; ++i)
            {
                for (int j = 0; j < bmp.Height; ++j)
                {
                    sharpenImage.SetPixel(i, j, result[i, j]);
                }
            }
            return sharpenImage;
        }
        private Bitmap Dilation(Bitmap SrcImage)
        {
            Bitmap tempbmp = new Bitmap(SrcImage.Width, SrcImage.Height);
            BitmapData SrcData = SrcImage.LockBits(new Rectangle(0, 0,
            SrcImage.Width, SrcImage.Height), ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb);

            BitmapData DestData = tempbmp.LockBits(new Rectangle(0, 0, tempbmp.Width,
        tempbmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte[,] sElement = new byte[5, 5] {
        {0,0,1,0,0},
        {0,1,1,1,0},
        {1,1,1,1,1},
        {0,1,1,1,0},
        {0,0,1,0,0}
    };

            int size = 5;
            byte max, clrValue;
            int radius = size / 2;
            int ir, jr;

            unsafe
            {

                // Loop for Columns.
                for (int colm = radius; colm < DestData.Height - radius; colm++)
                {
                    // Initialise pointers to at row start.
                    byte* ptr = (byte*)SrcData.Scan0 + (colm * SrcData.Stride);
                    byte* dstPtr = (byte*)DestData.Scan0 + (colm * SrcData.Stride);

                    // Loop for Row item.
                    for (int row = radius; row < DestData.Width - radius; row++)
                    {
                        max = 0;
                        clrValue = 0;

                        // Loops for element array.
                        for (int eleColm = 0; eleColm < 5; eleColm++)
                        {
                            ir = eleColm - radius;
                            byte* tempPtr = (byte*)SrcData.Scan0 +
                                ((colm + ir) * SrcData.Stride);

                            for (int eleRow = 0; eleRow < 5; eleRow++)
                            {
                                jr = eleRow - radius;

                                // Get neightbour element color value.
                                clrValue = (byte)((tempPtr[row * 3 + jr] +
                                    tempPtr[row * 3 + jr + 1] + tempPtr[row * 3 + jr + 2]) / 3);

                                if (max < clrValue)
                                {
                                    if (sElement[eleColm, eleRow] != 0)
                                        max = clrValue;
                                }
                            }
                        }

                        dstPtr[0] = dstPtr[1] = dstPtr[2] = max;

                        ptr += 3;
                        dstPtr += 3;
                    }
                }
            }

            SrcImage.UnlockBits(SrcData);
            tempbmp.UnlockBits(DestData);

            // return dilated bitmap.
            return tempbmp;


        }
        private Bitmap ero(Bitmap SrcImage)
        {
            Bitmap tempbmp = new Bitmap(SrcImage.Width, SrcImage.Height);
            BitmapData SrcData = SrcImage.LockBits(new Rectangle(0, 0,
            SrcImage.Width, SrcImage.Height), ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb);

            BitmapData DestData = tempbmp.LockBits(new Rectangle(0, 0, tempbmp.Width,
        tempbmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte[,] sElement = new byte[5, 5] {
        {0,0,1,0,0},
        {0,1,1,1,0},
        {1,1,1,1,1},
        {0,1,1,1,0},
        {0,0,1,0,0}
    };

            int size = 5;
            byte max, clrValue;
            int radius = size / 2;
            int ir, jr;

            unsafe
            {

                // Loop for Columns.
                for (int colm = radius; colm < DestData.Height - radius; colm++)
                {
                    // Initialise pointers to at row start.
                    byte* ptr = (byte*)SrcData.Scan0 + (colm * SrcData.Stride);
                    byte* dstPtr = (byte*)DestData.Scan0 + (colm * SrcData.Stride);

                    // Loop for Row item.
                    for (int row = radius; row < DestData.Width - radius; row++)
                    {
                        max = 0;
                        clrValue = 0;

                        // Loops for element array.
                        for (int eleColm = 0; eleColm < 5; eleColm++)
                        {
                            ir = eleColm - radius;
                            byte* tempPtr = (byte*)SrcData.Scan0 +
                                ((colm + ir) * SrcData.Stride);

                            for (int eleRow = 0; eleRow < 5; eleRow++)
                            {
                                jr = eleRow - radius;

                                // Get neightbour element color value.
                                clrValue = (byte)((tempPtr[row * 3 + jr] +
                                    tempPtr[row * 3 + jr + 1] + tempPtr[row * 3 + jr + 2]) / 3);

                                if (max < clrValue)
                                {
                                    if (sElement[eleColm, eleRow] != 0)
                                        max = clrValue;
                                }
                            }
                        }

                        dstPtr[0] = dstPtr[1] = dstPtr[2] = max;

                        ptr += 3;
                        dstPtr += 3;
                    }
                }
            }

            SrcImage.UnlockBits(SrcData);
            tempbmp.UnlockBits(DestData);

            // return dilated bitmap.
            return tempbmp;


        }


        private Bitmap blur(Bitmap img)
        {
            Int32 avgR = 0, avgG = 0, avgB = 0;
            Int32 blurPixelCount = 0;

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color pixel = img.GetPixel(x, y);
                    avgR += pixel.R;
                    avgG += pixel.G;
                    avgB += pixel.B;

                    blurPixelCount++;
                }
            }

            avgR = avgR / blurPixelCount;
            avgG = avgG / blurPixelCount;
            avgB = avgB / blurPixelCount;

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    img.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            return img;
        }
        private Bitmap sobel(Bitmap image)
        {
            Bitmap gri = gray(image);
            Bitmap buffer = new Bitmap(gri.Width, gri.Height);//görüntünün boyutlarına sahip boş görüntü oluşturuyorsun
            Color renk;
            int valx, valy, gradient;
            int[,] GX = new int[3, 3];
            int[,] GY = new int[3, 3];
            //Yatay kenar 
            GX[0, 0] = -1; GX[0, 1] = 0; GX[0, 2] = 1;
            GX[1, 0] = -2; GX[1, 1] = 0; GX[1, 2] = 2;
            GX[2, 0] = -1; GX[2, 1] = 0; GX[2, 2] = 1;


            //Dikey kenar

            GY[0, 0] = -1; GY[0, 1] = -2; GY[0, 2] = -1;
            GY[1, 0] = 0; GY[1, 1] = 0; GY[1, 2] = 0;
            GY[2, 0] = 1; GY[2, 1] = 2; GY[2, 2] = 1;


            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    if (i == 0 || i == gri.Height - 1 || j == 0 || j == gri.Width - 1)
                    {
                        renk = Color.FromArgb(255, 255, 255);
                        buffer.SetPixel(j, i, renk);
                        valx = 0;
                        valy = 0;
                    }
                    else
                    {
                        valx = gri.GetPixel(j - 1, i - 1).R * GX[0, 0]
                            + gri.GetPixel(j, i - 1).R * GX[0, 1]
                            + gri.GetPixel(j + 1, i - 1).R * GX[0, 2]
                            + gri.GetPixel(j - 1, i).R * GX[1, 0]
                            + gri.GetPixel(j, i).R * GX[1, 1]
                            + gri.GetPixel(j + 1, i).R * GX[1, 2]
                            + gri.GetPixel(j - 1, i + 1).R * GX[2, 0]
                            + gri.GetPixel(j, i + 1).R * GX[2, 1]
                            + gri.GetPixel(j + 1, i + 1).R * GX[2, 2];

                        valy = gri.GetPixel(j - 1, i - 1).R * GY[0, 0]
                             + gri.GetPixel(j, i - 1).R * GY[0, 1]
                             + gri.GetPixel(j + 1, i - 1).R * GY[0, 2]
                             + gri.GetPixel(j - 1, i).R * GY[1, 0]
                             + gri.GetPixel(j, i).R * GY[1, 1]
                             + gri.GetPixel(j + 1, i).R * GY[1, 2]
                             + gri.GetPixel(j - 1, i + 1).R * GY[2, 0]
                             + gri.GetPixel(j, i + 1).R * GY[2, 1]
                             + gri.GetPixel(j + 1, i + 1).R * GY[2, 2];

                        gradient = (int)(Math.Abs(valx) + Math.Abs(valy));


                        if (gradient < 0)
                            gradient = 0;
                        if (gradient > 255)
                            gradient = 255;

                        renk = Color.FromArgb(gradient, gradient, gradient);
                        buffer.SetPixel(j, i, renk);


                    }
                }
            }
            return buffer; ;
        }



        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                Bitmap image = new Bitmap(pictureBox1.Image);
                Bitmap gri = gray(image);

                pictureBox2.Image = gri;
            }
            if (comboBox2.SelectedIndex == 0)
            {
                Bitmap image = new Bitmap(pictureBox1.Image);
                Bitmap bluur = blur(image);

                pictureBox2.Image = bluur;
            }
            if (comboBox2.SelectedIndex == 1)
            {
                Bitmap image = new Bitmap(pictureBox1.Image);
                Bitmap shr = sharp(image);

                pictureBox2.Image = shr;
            }
            if (comboBox2.SelectedIndex == 2)
            {
                Bitmap image = new Bitmap(pictureBox1.Image);
                Bitmap sobell = sobel(image);

                pictureBox2.Image = sobell;
            }
            if (comboBox3.SelectedIndex == 0)
            {
                Bitmap image = new Bitmap(pictureBox1.Image);
                Bitmap ed = Dilation(image);

                pictureBox2.Image = ed;
            }
            if (comboBox3.SelectedIndex == 1)
            {
                Bitmap image = new Bitmap(pictureBox1.Image);
                Bitmap ed = ero(image);

                pictureBox2.Image = ed;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter= "resimler(*.jpg,*.png,*.bmp)|*.jpg;*.png;*.bmp";
            if(DialogResult.OK == sfd.ShowDialog())
            {
                this.pictureBox2.Image.Save(sfd.FileName, ImageFormat.Jpeg);
            }
        }
    }
}
