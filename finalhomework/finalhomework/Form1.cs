using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace finalhomework
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string pathname = string.Empty;
        private Bitmap origImg;
        private Bitmap curImg;
        private Bitmap back;

        //翻转
        private void RotateFormCenter(PictureBox pb, float angle)
        {
            Image img = pb.Image;
            int newWidth = Math.Max(img.Height, img.Width);
            Bitmap bmp = new Bitmap(newWidth, newWidth);
            Graphics g = Graphics.FromImage(bmp);
            Matrix x = new Matrix();
            PointF point = new PointF(img.Width / 2f, img.Height / 2f);
            x.RotateAt(angle, point);
            g.Transform = x;
            g.DrawImage(img, 0, 0);
            g.Dispose();
            img = bmp;
            pb.Image = img;
        }

        //灰化
        public static Bitmap ToGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色  
                    Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值  
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }

        //调RGB亮度
        public unsafe Bitmap img_color_gradation(Bitmap src, int r, int g, int b)
        {
            int width = src.Width;
            int height = src.Height;
            back = new Bitmap(width, height);
            Rectangle rect = new Rectangle(0, 0, width, height);
            //这种速度最快
            BitmapData bmpData = src.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);//24位rgb显示一个像素，即一个像素点3个字节，每个字节是BGR分量。Format32bppRgb是用4个字节表示一个像素
            byte* ptr = (byte*)(bmpData.Scan0);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    //ptr[2]为r值，ptr[1]为g值，ptr[0]为b值
                    int red = ptr[2] + r; if (red > 255) red = 255; if (red < 0) red = 0;
                    int green = ptr[1] + g; if (green > 255) green = 255; if (green < 0) green = 0;
                    int blue = ptr[0] + b; if (blue > 255) blue = 255; if (blue < 0) blue = 0;
                    back.SetPixel(i, j, Color.FromArgb(red, green, blue));
                    ptr += 3; //Format24bppRgb格式每个像素占3字节
                }
                ptr += bmpData.Stride - bmpData.Width * 3;//每行读取到最后“有用”数据时，跳过未使用空间XX
            }
            src.UnlockBits(bmpData);
            return back;
        }

        //打开图片
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Title = "请选择图片";
            file.Filter = "图片文件(*.png, *.jpg, *.jpeg, *.bmp) | *.png; *.jpg; *.jpeg; *.bmp";
            file.InitialDirectory = ".";
            file.ShowDialog();
            if(file.FileName != string.Empty)
            {
                try
                {
                    pathname = file.FileName;
                    FileStream fs = File.OpenRead(pathname);
                    Image img = Image.FromStream(fs);
                    fs.Close();
                    origImg = new Bitmap(img);
                    curImg = new Bitmap(img);
                    this.pictureBox1.Image = curImg;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //保存图片
        private void button2_Click(object sender, EventArgs e)
        {
            if (pathname != string.Empty)
            {
                pictureBox1.Image.Save(pathname, System.Drawing.Imaging.ImageFormat.Jpeg);
                MessageBox.Show("保存成功！");
            }
            else
                MessageBox.Show("当前无图片！", "提示：", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //图片另存为
        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Jpg 图片|*.jpg|Bmp 图片|*.bmp|Gif 图片|*.gif|Png 图片|*.png|Wmf  图片|*.wmf";
            sfd.FilterIndex = 0;
            if(pictureBox1.Image == null)
            {
                MessageBox.Show("当前无图片！", "提示：", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(sfd.ShowDialog() == DialogResult.OK)
            {
                if(pictureBox1.Image != null)
                {
                    pictureBox1.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }

        }

        //图片还原
        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("当前无图片！", "提示：", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                this.pictureBox1.Image = origImg;
                curImg = new Bitmap(pictureBox1.Image);
                RedTextBox.Text = "0";
                GreenTextBox.Text = "0";
                BlueTextBox.Text = "0";
            }
                

        }

        //顺时针90度
        private void button5_Click(object sender, EventArgs e)
        {
            RotateFormCenter(pictureBox1, 90);
            curImg = new Bitmap(pictureBox1.Image);
        }

        //逆时针90度
        private void button6_Click(object sender, EventArgs e)
        {
            RotateFormCenter(pictureBox1, -90);
            curImg = new Bitmap(pictureBox1.Image);
        }

        //镜像翻转
        private void button7_Click(object sender, EventArgs e)
        {
            curImg = new Bitmap(pictureBox1.Image);
            curImg.RotateFlip(RotateFlipType.RotateNoneFlipX);
            pictureBox1.Image = curImg;
            curImg = new Bitmap(pictureBox1.Image);
        }

        //上下翻转
        private void button8_Click(object sender, EventArgs e)
        {
            RotateFormCenter(pictureBox1, 180);
            curImg = new Bitmap(pictureBox1.Image);
        }

        //灰化
        private void button9_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("当前无图片！", "提示：", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ToGray(curImg);
                this.pictureBox1.Image = curImg;
                curImg = new Bitmap(pictureBox1.Image);
            }   
        }

        //调RGB亮度
        private void button10_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("当前无图片！", "提示：", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (RedTextBox.Text != string.Empty && GreenTextBox.Text != string.Empty && BlueTextBox.Text != string.Empty)
                {
                    int r = Convert.ToInt16(this.RedTextBox.Text);
                    int g = Convert.ToInt16(this.GreenTextBox.Text);
                    int b = Convert.ToInt16(this.BlueTextBox.Text);
                    img_color_gradation(curImg, r, g, b);
                    curImg = new Bitmap(pictureBox1.Image);
                    this.pictureBox1.Image = back;
                }
                else
                    MessageBox.Show("请输入亮度！", "提示：", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }
    }
}
