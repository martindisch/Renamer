using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Renamer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dg = new FolderBrowserDialog();
            dg.ShowNewFolderButton = false;
            dg.RootFolder = System.Environment.SpecialFolder.Desktop;
            if (dg.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dg.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            VFolder root = renameContent(textBox1.Text);
            SaveFileDialog dg = new SaveFileDialog();
            dg.FileName = "Names.xml";
            dg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            dg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (dg.ShowDialog() == DialogResult.OK)
            {
                System.IO.TextWriter writer = new System.IO.StreamWriter(dg.FileName);
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(root.GetType());
                x.Serialize(writer, root);
            }
        }

        private VFolder renameContent(String path)
        {
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(path);
            System.IO.DirectoryInfo[] directories = directory.GetDirectories();
            VFolder actFolder = new VFolder();
            actFolder.Name = directory.Name;

            foreach (System.IO.DirectoryInfo folder in directories)
            {
                actFolder.SubFolders.Add(renameContent(folder.FullName));
                System.IO.Directory.Move(folder.FullName, folder.Parent.FullName + "\\" + generateName());
            }

            return actFolder;
        }

        private String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private Random random = new Random();
        private String generateName()
        {
            return new String(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
