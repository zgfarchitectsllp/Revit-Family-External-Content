using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZGF.Revit;

namespace RevitContentWatcher
{
    public partial class ContentTester : Form
    {
        public ContentTester()
        {
            InitializeComponent();

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RfaFileWrapper.WriteFamilyDataFlatFile(new DirectoryInfo(@"Z:\ZGFResources\Software_Content_Libraries\Revit\2017"), new FileInfo(@"c:\users\trevor.taylor\desktop\FlatNoTypes.txt"), true, false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime startTime = DateTime.Now;
            TimeSpan duration;

            string contentFolder = @"Z:\ZGFResources\Software_Content_Libraries\Revit\2017";
            MySortableBindingList<RfaFileWrapper> _FamilyList = new MySortableBindingList<RfaFileWrapper>();

            // Collect Content data:
            DirectoryInfo contentDir = new DirectoryInfo(contentFolder);

            textBox1.AppendText("Collecting family files... ");
            startTime = DateTime.Now;
            FileInfo[] files = contentDir.GetFiles("*.xml", SearchOption.AllDirectories);
            duration = DateTime.Now - startTime;
            textBox1.AppendText("Times: " + duration.TotalSeconds.ToString() + " secs\r\n");

            textBox1.AppendText("Preparing library. " + files.Count().ToString() + " files. " + duration.TotalSeconds.ToString() + " secs\r\n");
            startTime = DateTime.Now;
            foreach (FileInfo f in files)
            {
                try
                {
                    _FamilyList.Add(new RfaFileWrapper(f));
                }
                catch (Exception ex) { textBox1.AppendText("Failed: " + f.FullName + ", Exception: " + ex.Message + "\r\n"); }
            }
            duration = DateTime.Now - startTime;
            textBox1.AppendText("Time: " + duration.TotalSeconds.ToString() + " secs\r\n");
            contentDir = null;
            textBox1.AppendText(_FamilyList.Count.ToString() + " families collected\r\n");

            startTime = DateTime.Now;
            textBox1.AppendText("Loading datagrid: " + duration.TotalSeconds.ToString() + " secs\r\n");
            this.dataGridView1.DataSource = _FamilyList;
            duration = DateTime.Now - startTime;
            textBox1.AppendText("Time: " + duration.TotalSeconds.ToString() + " secs\r\n");
        }

        private void buttonSaveXMLs_Click(object sender, EventArgs e)
        {
            RfaFileWrapper.WriteXmlFilesFromRfaContentLibrary(new DirectoryInfo(@"Z:\ZGFResources\Software_Content_Libraries\Revit Family Library"));
        }
    }
}
