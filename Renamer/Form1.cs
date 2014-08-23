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

            String newName;
            VFolder temp;
            foreach (System.IO.DirectoryInfo folder in directories)
            {
                newName = generateName();
                temp = renameContent(folder.FullName);
                temp.NewName = newName;
                actFolder.SubFolders.Add(temp);
                System.IO.Directory.Move(folder.FullName, folder.Parent.FullName + "\\" + newName);
            }

            return actFolder;
        }

        private String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private Random random = new Random();
        private String generateName()
        {
            return new String(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dg = new OpenFileDialog();
            dg.FileName = "Names.xml";
            dg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            dg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (dg.ShowDialog() == DialogResult.OK)
            {
                System.IO.TextReader reader = new System.IO.StreamReader(dg.FileName);
                VFolder root = new VFolder();
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(root.GetType());
                root = (VFolder) x.Deserialize(reader);
                restoreName(textBox1.Text, root);

                MessageBox.Show("Names restored", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void restoreName(String path, VFolder actFolder)
        {
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(path);

            foreach (VFolder act in actFolder.SubFolders)
            {
                System.IO.Directory.Move(path + "\\" + act.NewName, path + "\\" + act.Name);
                restoreName(path + "\\" + act.Name, act);
            }
        }
    }
}
