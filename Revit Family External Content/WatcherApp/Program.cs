using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WatcherApp
{
    static class Program
    {

        const string _xmlHeader = "<? xml version = \"1.0\" encoding=\"UTF-8\"?>";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (TextReader tr = File.OpenText(@"C:\Temp\tag - Room-Fire Life Safety.rfa"))
            {
                
                int index = 0;
                int maxNumberofTries = 100;
                int currentChar = 0;
                char[] buffer = new char[256];

                string _endTag = "</entry>";
                
                StringBuilder sb = new StringBuilder();
                string _thePartAtom = sb.ToString();

                while (! _thePartAtom.Contains(_endTag))
                {
                    if (maxNumberofTries < 1) break;
                    maxNumberofTries--;

                    tr.ReadBlock(buffer, 0, 256);
                    sb.Append(new string(buffer));

                }




                bool gotLamb = false;

                while (currentChar > -1)
                {
                    currentChar = tr.Read();
                    index++;

                    if (currentChar > -1)
                    {
                        char c = (char)currentChar;
                        if (c.Equals('<'))
                        {
                             // new char[]; // = char[3];
                            int len = tr.ReadBlock(buffer, 0, 6);
                            StringBuilder sb = new StringBuilder(c.ToString());
                            foreach (char chr in buffer)
                            {
                                sb.Append(chr.ToString()); 
                            }
                            string s = sb.ToString();
                            if (s.Equals("<entry "))
                                break;
                            else
                                sb.Clear();

                        }
                    }
                    

                }


                tr.Close();
            }
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
