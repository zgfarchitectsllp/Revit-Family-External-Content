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

namespace WatcherApp
{
    public partial class Form1 : Form
    {
        delegate void logMsgCallback(string msgString);
        FileSystemWatcher fsw = new FileSystemWatcher();

        private void SetMessage(string msgString)
        {
            if (this.textBox1.InvokeRequired)
            {
                logMsgCallback d = new logMsgCallback(SetMessage);
                this.Invoke(d, new object[] { msgString });
            }
            else
            {
                this.textBox1.Text += msgString;
            }
        }
        

        public Form1()
        {
            InitializeComponent();

           
            
            fsw.Path = @"Z:\PDX\Projects\23398.vanc";
            fsw.Filter = "*.*";
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;
            fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            //fsw.WaitForChanged(WatcherChangeTypes.All);

            
            fsw.Changed += new FileSystemEventHandler(OnChanged);
            fsw.Renamed += new RenamedEventHandler(OnRenamed);
            fsw.Error += new ErrorEventHandler(OnError);
            fsw.Created += new FileSystemEventHandler(OnCreated);
            fsw.Deleted += new FileSystemEventHandler(OnDeleted);
            
            
        }

       

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            SetMessage(e.ChangeType.ToString() + ": " + e.FullPath + "\r\n");
            
        
        }

        private void OnRenamed(object source, FileSystemEventArgs e)
        {
            SetMessage(e.ChangeType.ToString() + ": " + e.FullPath + "\r\n");
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            SetMessage(e.ChangeType.ToString() + ": " + e.FullPath + "\r\n");
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            SetMessage(e.ChangeType.ToString() + ": " + e.FullPath + "\r\n");
        }

        

        private void OnError(object source, ErrorEventArgs e)
        {
            SetMessage("ERROR: " + e.ToString() + "\r\n");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Clear();
            //this.Close();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            fsw.Dispose();
        }
    }
}
