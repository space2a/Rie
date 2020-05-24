using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rie
{
    public partial class Form1 : Form
    {
        public byte[] f1 = null;
        bool editedf1 = false;
        public List<bool> locked = null;

        public Form1()
        {
            InitializeComponent();
            label17.Hide();
            processingpanel.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a file";
            openFileDialog.ShowDialog();
            textBox1.Text = openFileDialog.FileName;
            if (openFileDialog.FileName != "")
            {
                trackBar1.TickFrequency = 0;
                f1 = File.ReadAllBytes(textBox1.Text);
                trackBar1.Maximum = f1.Length;
                trackBar1.TickFrequency = trackBar1.Maximum / 2;
                trackBar1.Value = trackBar1.Maximum / 2;
                label2.Text = trackBar1.Value + "b" + " / " + trackBar1.Maximum + "b" + " (" + ((trackBar1.Value * 1.0 / trackBar1.Maximum) * 100).ToString(".0") + "%)";
            }
            editedf1 = false;
            label17.Hide();
        }

        public void edited(byte[] f1x) { if (f1 == f1x) { return; } f1 = f1x; label17.Show(); editedf1 = true; }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") { return; } else if (!File.Exists(textBox1.Text)) { MessageBox.Show("An error occurred, the referenced file does not exist.", "An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            foreach (Control x in groupBox2.Controls)
            {
                if (x is TextBox)
                {
                    if(!((TextBox)x).ReadOnly)
                        if (!gInt(((TextBox)x).Text)) { return; }
                }
            }

            int gotovalue = trackBar1.Value;
            int sk = 0;
            bool isr = false;

            int sm = int.Parse(textBox3.Text);
            if (!skipbox.Checked) { sm = 0; }

            if (textBox2.Text != "r")
                sk = int.Parse(textBox2.Text);
            else { sk = new Random().Next(1, sm / 2); isr = true; }

            int maxmix = int.Parse(textBox4.Text);

            int mixgroupsize = int.Parse(textBox5.Text);

            int repeatmax = int.Parse(textBox6.Text);

            int trackvalue = trackBar1.Value;

            trackBar1.Maximum = f1.Length;

            new Thread(() => ByteWork(gotovalue, sk, isr, sm, maxmix, mixgroupsize, repeatmax, trackvalue)).Start();
        }

        bool gInt(string text)
        {
            int o = 0;
            if (int.TryParse(text, out o)) { if (o < 0) { errMsg(text); return false; } if (o > trackBar1.Value){ errMsg(text); return false; } } else { errMsg(text); return false; }
            return true;
        }
        void errMsg(string text)
        {
            MessageBox.Show("Invalid number (" + text + ").", "Invalid number", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }



        void ByteWork(int gotovalue, int sk, bool isr, int sm, int maxmix, int mixgroupsize, int repeatmax, int trackvalue)
        {
            this.progressBar1.Invoke((MethodInvoker)delegate {
                this.progressBar1.Value = 0;
            });
            this.processingpanel.Invoke((MethodInvoker)delegate {
            this.processingpanel.Show();
            this.processingpanel.Location = new Point(286, 71);
            });

            if (editedf1 == false)
                f1 = File.ReadAllBytes(textBox1.Text);


            List<byte> final = new List<byte>();
            int i = 0;


            int x = 0;
            int p = 10;

            #region skipbytes
            int s = 0;
            int sd = 0;
            int st = 0;
            #endregion

            #region mixbytes


            int nextIn = 0;
            bool is2 = false;
            bool fll = false;
            int ti = 0;

            List<MixedBytes> mBytes = new List<MixedBytes>();
            mBytes.Add(new MixedBytes());
            if (!mixbox.Checked) { maxmix = 0; }

            Random rnd = new Random();

            #endregion

            #region repeatbytes

            int rp = 0;
            int indexrp = 0;
            List<byte> rbytes = new List<byte>();
            if (!repeatbox.Checked) { repeatmax = 0; }
            #endregion

            foreach (var b in f1)
            {
                if (x == trackvalue / 10) { this.progressBar1.Invoke((MethodInvoker)delegate { if (this.progressBar1.Value != this.progressBar1.Maximum) { this.progressBar1.Value++; } }); x = 0; p += 10; }

                if (sm > 0 && sk > 0 && !fll)
                {
                    if (s == sm) { sd = sk; gotovalue -= sk; s++; s = 0; if (isr) { sk = new Random().Next(1, sm / 2); } }
                    if (sd != 0) { sd--; st++; final.Add(new byte()); continue; }
                }

                if (maxmix > 0 && s >= 100 && s < gotovalue - 150 && !is2 && !fll && mBytes.Count - 1 < maxmix)
                { if (rnd.Next(0, 100) <= 20) { ti = 0; fll = true; nextIn = rnd.Next(getNextIn(trackvalue) / 4, getNextIn(trackvalue)) + mixgroupsize; } }
                else if (nextIn <= 0 && is2 && !fll)
                    fll = true;

                if (fll && sd == 0)
                {
                    var cmBytes = mBytes[mBytes.Count - 1];
                    if (!is2)
                    { if (cmBytes.POS1.Count < mixgroupsize) { cmBytes.POS1.Add(s); } else { is2 = true; fll = false; } }
                    else if (is2)
                    { if (cmBytes.POS2.Count < mixgroupsize) { cmBytes.POS2.Add(s); } else { is2 = false; fll = false; mBytes.Add(new MixedBytes()); } }
                }

                if (!fll && repeatmax > 0 && s >= 100 && rp <= 0 && indexrp == 0)
                {
                    if (rnd.Next(0, 99999) == 0 && s < gotovalue - 150)
                    {
                        rp = repeatmax;
                        indexrp = s;
                    }
                }
                if (rp > 0 && s < gotovalue - 300)
                { rbytes.Add(b); rp--; }

                if (i >= gotovalue)
                {
                    break;
                }

                final.Add(b);

                i++;
                x++;
                ti++;
                s++;
                nextIn--;
            }

            foreach (var mb in mBytes)
            {
                if (mb.POS1.Count == mixgroupsize && mb.POS2.Count == mixgroupsize)
                {
                    List<byte> bk2 = new List<byte>();

                    foreach (var b in mb.POS2)
                    {
                        bk2.Add(final[b]);
                    }

                    for (int mi = 0; mi < mb.POS1.Count; mi++)
                    {
                        final[mb.POS2[mi]] = final[mb.POS1[mi]];
                    }

                    for (int mi = 0; mi < mb.POS2.Count; mi++)
                    {
                        final[mb.POS1[mi]] = final[mb.POS2[mi]];
                    }


                }
                else { continue; }

            }

            List<byte> bk = new List<byte>();

            for (int ix = 0; ix < indexrp; ix++)
            {
                bk.Add(final[ix]);
            }

            for (int xx = 0; xx < rnd.Next(3, 25); xx++)
            {
                foreach (var b in rbytes)
                {
                    bk.Add(b); //Linsertion ne fonctionne pas :(
                }
            }

            for (int ix = bk.Count; ix < final.Count; ix++)
            {
                bk.Add(final[ix]);
            }
            //final = new List<byte>();
            final = bk;

            FileInfo fileInfo = new FileInfo(textBox1.Text);
            File.WriteAllBytes("RIE_" + fileInfo.Name, final.ToArray());

            this.processingpanel.Invoke((MethodInvoker)delegate {
                this.processingpanel.Hide();
            });
        }


        private void button3_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a file";
            openFileDialog.ShowDialog();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label2.Text = trackBar1.Value + "b" + " / " + trackBar1.Maximum + "b" + " (" + ((trackBar1.Value * 1.0 / trackBar1.Maximum) * 100).ToString(".0") + "%)";
        }

        int getNextIn(int trackvalue)
        {
            if (trackvalue > 1000 && trackvalue <= 5000)
                return 200;
            else if (trackvalue > 5000 && trackvalue <= 10000)
                return 1000;
            else if (trackvalue > 10000 && trackvalue <= 25000)
                return 5000;
            else if (trackvalue > 25000 && trackvalue <= 50000)
                return 10000;
            else if (trackvalue > 100000)
                return 12500;
            else
                return 50;
        }


        #region Graphic
        Graphics graphic;
        bool done = true;
        public void Start(Panel panel, float ZOOM = 0.01F, int YTRANSFORM = 13800, int YSIZE = 300, int v = 100000)
        {
            graphic = panel.CreateGraphics();
            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed ;
            graphic.ScaleTransform(ZOOM, ZOOM);
            graphic.TranslateTransform(0, YTRANSFORM);
            Draw(panel, v, YSIZE);
        }

        void Draw(Panel panel, int v = 100000, int YSIZE = 300)
        {
            if (!done)
                return;
            done = false;
            panel.Cursor = Cursors.WaitCursor;
            graphic.Clear(Color.LightCyan);

            // Create points that define line.

            #region skipbytes
            int sm = int.Parse(textBox3.Text);
            int sk = 0;
            bool isr = false;
            if (textBox2.Text != "r")
                sk = int.Parse(textBox2.Text);
            else { sk = new Random().Next(1, sm / 2); isr = true; }
            int s = 0;
            int sd = 0;
            int st = 0;
            if (!skipbox.Checked) { sm = 0; }
            #endregion

            #region mixbytes

            int maxmix = int.Parse(textBox4.Text);

            int nextIn = 0;
            bool is2 = false;
            bool fll = false;
            int ti = 0;
            int mixgroupsize = int.Parse(textBox5.Text);

            int cm = 0;
            List<MixedBytes> mBytes = new List<MixedBytes>();
            if (!mixbox.Checked) { maxmix = 0; }

            Random rnd = new Random();

            #endregion

            for (int i = 0; i < v; i++)
            {
                if (sm > 0 && sk > 0)
                {
                    if (s == sm) { sd = sk; s++; s = 0; if (isr) { sk = new Random().Next(1, sm / 2); } }
                    if (sd != 0) 
                    { 
                        sd--; st++;
                        graphic.FillRectangle(Brushes.Red, i, 100, 1, YSIZE);
                        continue;
                    }
                }

                if (maxmix > 0 && s >= 100 && s < v - 150 && !is2 && !fll)
                { if (rnd.Next(0, 100) <= 20) { ti = 0; fll = true; nextIn = rnd.Next(getNextIn(trackBar1.Value) / 4, getNextIn(trackBar1.Value)) + mixgroupsize; } }
                else if (nextIn <= 0 && is2 && !fll)
                    fll = true;

                if (fll && sd == 0)
                {
                    graphic.FillRectangle(Brushes.Black, i, 0, 1, YSIZE);
                    cm++;
                    if(cm == mixgroupsize) { cm = 0; fll = false; }
                }

                graphic.FillRectangle(Brushes.Black, i, 0, 1, YSIZE);
                s++;
                nextIn--;
            }
            done = true;
            panel.Cursor = Cursors.Hand;
        }
        #endregion Graphic

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            Start(panel1);
        }


        public bool bworking = true;
        private void button3_Click(object sender, EventArgs e)
        {
            if (bworking)
            {
                BigGraphic BigGraphic = new BigGraphic();
                BigGraphic.form1 = this;
                BigGraphic.Show();
                bworking = false;
            }
        }

        public bool bworkingeditor = true;
        private void button4_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            if (textBox1.Text != "" && File.Exists(textBox1.Text)){
                if (bworkingeditor)
                {
                    BEditor BEditor = new BEditor();
                    if (locked != null)
                        BEditor.locked = locked;

                    BEditor.form1 = this;
                    BEditor.Ini(textBox1.Text, f1);

                    BEditor.Show();
                    bworkingeditor = false;
                }
            }
            this.UseWaitCursor = false;
        }

        private void label18_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/space2a/Rie");
        }
    }

    public class MixedBytes
    {
        public List<int> POS1 = new List<int>();
        public List<int> POS2 = new List<int>();
    }  

}
