using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rie
{
    public partial class BEditor : Form
    {
        public Form1 form1;
        List<byte> f1 = new List<byte>();
        public List<bool> locked = null;
        int start = 0;
        int end = 0;

        int startlist = 0;

        int maxperpage = 9999;

        public void Ini(string path, byte[] f1x)
        {
            label8.Hide();

            foreach (var item in f1x)
            {
                f1.Add(item);
            }

            toolStripTextBox1.Text = maxperpage.ToString();
            HideAll();
            selectionpanel.Hide();
            bsizepanel.Hide();
            if (f1.Count > 10000)
            {
                int x = 0;
                for (int i = 0; i < 10000; i++)
                {
                    if (x == 8)
                        x = 0;
                    x++;
                    Color color = Color.WhiteSmoke;
                    if (x > 4)
                        color = Color.Gainsboro;
                    listView1.Items.Add(new ListViewItem(new[] { i.ToString("0000"), f1[i].ToString(), f1[i].ToString("X2"), Encoding.Default.GetString(new[] { f1[i] }) }, 0) { BackColor = color });
                }
                startlist = maxperpage;
            }
            else
            {
                int x = 0;
                for (int i = 0; i < f1.Count -1 ; i++)
                {
                    if (x == 8)
                        x = 0;
                    x++;
                    Color color = Color.WhiteSmoke;
                    if (x > 4)
                        color = Color.Gainsboro;
                    listView1.Items.Add(new ListViewItem(new[] { i.ToString("0000"), f1[i].ToString(), f1[i].ToString("X2"), Encoding.Default.GetString(new[] { f1[i] }) }, 0) { BackColor = color });
                }
                label8.Hide();
                label2.Hide();
            }
        }

        void HideAll()
        {
            groupBox1.Hide();
            label3.Hide();
            button1.Hide();
            button2.Hide();
            pleaseselectlabel.Show();
        }

        void ShowAll()
        {
            groupBox1.Show();
            label3.Show();
            button1.Show();
            button2.Show();
            pleaseselectlabel.Show();

            textBox1.ReadOnly = true;
        }

        public BEditor()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            { HideAll(); return; }
            if (listView1.SelectedItems.Count > 1)
            { start = int.Parse(listView1.SelectedItems[0].Text); end = int.Parse(listView1.SelectedItems[listView1.SelectedItems.Count - 1].Text); }
            else { start = int.Parse(listView1.SelectedItems[0].Text); end = int.Parse(listView1.SelectedItems[0].Text); }

            WorkSelection();
        }


        void WorkSelection()
        {
            if (start != end)
            { //Multiples item selected
                button1.Show();
                label3.Show();
                ShowAll();
                label3.Text = "Bytes index : " + start.ToString("0000") + "-" + end.ToString("0000");
                textBox1.Text = "";
                checkBox1.CheckState = CheckState.Indeterminate;
            }
            else
            {
                int selected = start;
                button1.Text = "Edit this byte";
                button2.Text = "Save";
                button2.Enabled = false;
                pleaseselectlabel.Text = "Please select a byte";
                ShowAll();
                
                label3.Text = "Byte index : " + selected.ToString("0000");

                checkBox1.Checked = locked[selected];
                if (label1.Text == "Byte size :")
                    textBox1.Text = f1[selected].ToString();
                else
                    textBox1.Text = Encoding.Default.GetString(new[] { f1[selected] });
        }
        }


        void Erase()
        {
            for (int i = start; i <= end; i++)
            {
                f1[i] = new byte();
            }

            DialogResult dialogResult = MessageBox.Show("Byte(s) erased, do you want to refresh the byte view ?", "Refresh the byte view ?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
                refreshlist();

            listView1.Enabled = true;
            textBox1.ReadOnly = true;

            button3.Enabled = false;
            button4.Enabled = false;
            button2.Enabled = false;
            button8.Enabled = false;
            button1.Text = "Edit this byte";
        }

        void Delete()
        {
            for (int i = start; i <= end; i++)
            {
                f1.RemoveAt(i);
            }

            DialogResult dialogResult = MessageBox.Show("Byte(s) deleted, do you want to refresh the byte view ?", "Refresh the byte view ?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
                refreshlist();

            listView1.Enabled = true;
            textBox1.ReadOnly = true;

            button3.Enabled = false;
            button4.Enabled = false;
            button2.Enabled = false;
            button8.Enabled = false;
            button1.Text = "Edit this byte";
        }

        void refreshlist()
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();

            int x = 0;
            for (int i = startlist - maxperpage; i < startlist; i++)
            {
                if (i < 0)
                    i = 0;
                if (x == 8)
                    x = 0;
                x++;
                Color color = Color.WhiteSmoke;
                if (x > 4)
                    color = Color.Gainsboro;
                listView1.Items.Add(new ListViewItem(new[] { i.ToString("0000"), f1[i].ToString(), f1[i].ToString("X2"), Encoding.Default.GetString(new[] { f1[i] }) }, 0) { BackColor = color });
            }

            listView1.EndUpdate();
        }

        void Cancel()
        {
            button1.Text = "Edit this byte";
            listView1.Enabled = true;
            textBox1.ReadOnly = true;
            checkBox1.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button8.Enabled = false;
        }


        void Edit()
        {
            button1.Text = "Cancel";
            listView1.Enabled = false;
            textBox1.ReadOnly = false;
            checkBox1.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button2.Enabled = true;
            button8.Enabled = true;
        }

        void Save()
        {
            if (textBox1.Text != "")
            {
                if (label1.Text == "Byte size :")
                {
                    if (!gInt(textBox1.Text, false)) { return; }
                    if (int.Parse(textBox1.Text) > 255) { errMsg(); return; }
                    else
                    {
                        for (int i = start; i <= end; i++)
                        {
                            f1[i] = (byte)int.Parse(textBox1.Text);
                        }
                    }
                }
                else if (label1.Text == "Byte content :")
                {
                    for (int i = start; i <= end; i++)
                    {
                        f1[i] = Encoding.Default.GetBytes(textBox1.Text)[0];
                    }
                }
                else
                {
                    int x = 0;
                    if (int.TryParse(textBox1.Text, NumberStyles.HexNumber, null, out x))
                    {
                        for (int i = start; i <= end; i++)
                        {
                            f1[i] = (byte)x;
                        }
                    }
                    else { MessageBox.Show("Invalid hexadecimal characters.", "Invalid hexadecimal characters", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }

            if (checkBox1.CheckState != CheckState.Indeterminate)
            {
                for (int i = start; i <= end; i++)
                {
                    locked[i] = checkBox1.Checked;
                }
            }

            listView1.Enabled = true;
            textBox1.ReadOnly = true;

            button3.Enabled = false;
            button4.Enabled = false;
            button2.Enabled = false;
            button8.Enabled = false;
            checkBox1.Enabled = false;
            button1.Text = "Edit this byte";

            //DialogResult dialogResult = MessageBox.Show("Byte(s) edited, do you want to refresh the byte view ?", "Refresh the byte view ?", MessageBoxButtons.YesNo);
            //if (dialogResult == DialogResult.Yes)
            //    refreshlist();
            //


            List<ListViewItem> litems = new List<ListViewItem>();
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                litems.Add(item);
            }

            for (int i = start; i <= end; i++)
            {
                if (listView1.Items[i] != null)
                {
                    Color color = listView1.Items[i].BackColor;
                    listView1.Items[i] = new ListViewItem(new[] { i.ToString("0000"), f1[i].ToString(), f1[i].ToString("X2"), Encoding.Default.GetString(new[] { f1[i] }) }, 0) { BackColor = color };
                }
            }

            foreach (var item in litems)
            {
                if(item == null || item.Index < 0) { continue; }
                listView1.Items[item.Index].Selected = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Edit this byte")
                Edit();
            else if (button1.Text == "Delete bytes")
                Delete();
            else if (button1.Text == "Cancel")
                Cancel();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Save")
                Save();
            else if (button2.Text == "Erase bytes")
                Erase();
        }

        private void selectBytesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectionpanel.Location = new Point(260, 195);
            selectionpanel.Show();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            selectionpanel.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            listView1.SelectedItems.Clear();
            if (!gInt(textBox3.Text)) { return; }
            if(textBox4.Text == "r") { textBox4.Text = new Random().Next(int.Parse(textBox3.Text), f1.Count - 1).ToString(); }
            if (!gInt(textBox4.Text)) { return; }
            start = int.Parse(textBox3.Text);
            end = int.Parse(textBox4.Text);
            if (start < end)
            {
                selectionpanel.Hide();
                WorkSelection();
            }
            else { MessageBox.Show("The start number cannot be bigger than the end number.", "Invalid number", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        bool gInt(string text, bool f = true)
        {
            int o = 0;
            if (int.TryParse(text, out o)) { if (o < 0) { errMsg(); return false; } if (o > f1.Count && f) { textBox4.Text = f1.Count.ToString(); return true; } return true; } else { errMsg(); return false; }
        }

        void errMsg()
        {
            MessageBox.Show("Invalid number.", "Invalid number", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (label1.Text == "Byte size :")
            {
                label1.Text = "Byte content :";
                textBox1.Text = Encoding.Default.GetString(new[] { f1[start] });
                textBox1.Width = 66;
                textBox1.MaxLength = 1;
                textBox1.Location = new Point(80, 37);
                button6.Text = "H";
            }
            else if (label1.Text == "Byte content :") { label1.Text = "Byte hex :"; textBox1.Text = f1[start].ToString("X2"); textBox1.Width = 80; textBox1.MaxLength = 3; textBox1.Location = new Point(67, 37); button6.Text = "S"; }
            else { label1.Text = "Byte size :"; textBox1.Text = f1[start].ToString(); textBox1.Width = 80; textBox1.MaxLength = 3; textBox1.Location = new Point(67, 37); button6.Text = "T"; }



            //final else 
            //label1.Text = "Byte size :"; textBox1.Text = f1[start].ToString(); textBox1.Width = 80; textBox1.MaxLength = 3; textBox1.Location = new Point(67, 37);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();

            int x = 0;
            for (int i = startlist; i < startlist + maxperpage; i++)
            {
                if (x == 8)
                    x = 0;
                x++;
                Color color = Color.WhiteSmoke;
                if (x > 4)
                    color = Color.Gainsboro;
                listView1.Items.Add(new ListViewItem(new[] { i.ToString("0000"), f1[i].ToString(), f1[i].ToString("X2"), Encoding.Default.GetString(new[] { f1[i] }) }, 0) { BackColor = color });
            }
            startlist += maxperpage;
            listView1.EndUpdate();
            label8.Show();
        }

        private void editionToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            int mpp;
            if (int.TryParse(toolStripTextBox1.Text, out mpp))
                maxperpage = mpp;
            else
                toolStripTextBox1.Text = maxperpage.ToString();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            try
            {
                startlist -= maxperpage;
                listView1.BeginUpdate();
                listView1.Items.Clear();

                int x = 0;
                for (int i = startlist - maxperpage; i < startlist; i++)
                {
                    if (x == 8)
                        x = 0;
                    x++;
                    Color color = Color.WhiteSmoke;
                    if (x > 4)
                        color = Color.Gainsboro;
                    listView1.Items.Add(new ListViewItem(new[] { i.ToString("0000"), f1[i].ToString(), f1[i].ToString("X2"), Encoding.Default.GetString(new[] { f1[i] }) }, 0) { BackColor = color });
                }
                listView1.EndUpdate();
            }
            catch (Exception)
            {
                listView1.BeginUpdate();
                listView1.Items.Clear();

                int x = 0;
                for (int i = 0; i < maxperpage; i++)
                {
                    if (x == 8)
                        x = 0;
                    x++;
                    Color color = Color.WhiteSmoke;
                    if (x > 4)
                        color = Color.Gainsboro;
                    listView1.Items.Add(new ListViewItem(new[] { i.ToString("0000"), f1[i].ToString(), f1[i].ToString("X2"), Encoding.Default.GetString(new[] { f1[i] }) }, 0) { BackColor = color });
                }
                listView1.EndUpdate();
            }
            if (startlist == maxperpage)
                label8.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Erase();
        }

        private void BEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            form1.bworkingeditor = true;
            form1.locked = locked;
            form1.edited(f1.ToArray());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == "r") { textBox5.Text = new Random().Next(0, 255).ToString(); }
            if (!gInt(textBox5.Text, false)) { return; }
            if (textBox2.Text == "r") { textBox2.Text = new Random().Next(0, 255).ToString(); }
            if (!gInt(textBox2.Text, false)) { return; }
            if(int.Parse(textBox2.Text) > 255) { errMsg(); return; }

            int rp = int.Parse(textBox5.Text);
            int rpwith = int.Parse(textBox2.Text);

            if (int.Parse(textBox5.Text) > 255) { errMsg(); return; } else 
            {
                int g = 0;
                for (int i = 0; i < f1.Count - 1; i++)
                {
                    if(f1[i] == rp && locked[i] == false)
                    {
                        f1[i] = (byte)rpwith;
                        g++;
                    }
                }
                DialogResult dialogResult = MessageBox.Show(g + " byte(s) edited, do you want to refresh the byte view ?", "Refresh the byte view ?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                    refreshlist();
            }
            bsizepanel.Hide();
        }

        private void replaceBytesSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bsizepanel.Location = new Point(243, 202);
            bsizepanel.Show();
        }

        private void label12_Click(object sender, EventArgs e)
        {
            bsizepanel.Hide();
        }

        private void label13_Click(object sender, EventArgs e)
        {
            selectionpanel.Hide();
        }

        private void lockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.SelectedItems.Count; i++)
            {
                locked[int.Parse(listView1.SelectedItems[i].Text)] = true;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            List<byte> olding = new List<byte>();
            for (int i = start; i <= end; i++)
            {
                olding.Add(f1[i]);
            }

            foreach (var b in olding.AsEnumerable().Reverse())
            {
                f1.Insert(start, b);
            }

            DialogResult dialogResult = MessageBox.Show("Byte(s) duplicated, do you want to refresh the byte view ?", "Refresh the byte view ?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
                refreshlist();

            listView1.Enabled = true;
            textBox1.ReadOnly = true;

            button3.Enabled = false;
            button4.Enabled = false;
            button2.Enabled = false;
            button8.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "Rie: Bytes editor | Amount of bytes: " + f1.Count;
        }

        private void BEditor_Load(object sender, EventArgs e)
        {
            int iii = 0;
            if (locked == null)
            {
                locked = new List<bool>();
                foreach (var item in f1)
                {
                    if (iii <= 255)
                        locked.Add(true);
                    else
                        locked.Add(false);
                    iii++;
                }
            }
        }
    }
}
