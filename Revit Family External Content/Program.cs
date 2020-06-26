using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZGF.Revit;
using Newtonsoft.Json;

//using Newtonsoft.Json.Serialization;

namespace RevitContentWatcher
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            //ProgressReport pr = new ProgressReport();
            //pr.Show();

            //string pth = @"Z:\ZGFResources\Software_Content_Libraries\Revit Family Library";

            ////string uncPth = @"\\zgf.local\data\ZGFResources\Software_Content_Libraries\Revit Family Library";

            //string uncPth = 
            //    @"\\zgf.local\data\ZGFResources\Software_Content_Libraries\Revit Family Library\\USA - Imperial - Model\2016\Medical Equipment";

            //DirectoryInfo diPth = new DirectoryInfo(pth);
            //DirectoryInfo diUnc = new DirectoryInfo(uncPth);


            SyncLoadableFamilyRecords slTester = new SyncLoadableFamilyRecords(); // uncPth);

            slTester.DeleteRfaDataRecordsWithoutFile();

            slTester.UpdateDataRfaRecords();

            

            return;





            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ContentTester());

        }


        //static void TestJson()
        //{
        //    RfaFileWrapper rfa = new RfaFileWrapper();
        //}

        static void WriteSingleTabDelimitedFileFromXmlFiles(string[] args)
        {
            string pth = @"Z:\ZGFResources\Software_Content_Libraries\Revit Family Library";



            DirectoryInfo rvt2016Components = new DirectoryInfo(pth);
            FileInfo[] rfaFiles = rvt2016Components.GetFiles("*.xml", SearchOption.AllDirectories);

            using (StreamWriter sr = new StreamWriter(@"c:\users\trevor.taylor\desktop\2017.txt"))
            {
                sr.WriteLine(pth);
                sr.WriteLine();

                int iCounter = 0;
                foreach (FileInfo f in rfaFiles)
                {
                    iCounter++;
                    try
                    {
                        RfaFileWrapper rf = new RfaFileWrapper(new FileInfo(f.FullName));
                        sr.WriteLine(
                            "\r\n" + iCounter.ToString() + ".\t" + rf.FamilyName +
                            "\r\n\t" + rf.RfaPathOnDisk +
                            "\r\n\t" + rf.Category +
                            "\r\n\t" + rf.RevitVersion +
                            "\r\n\t" + rf.LastUpdate.ToShortDateString() + ", " + rf.LastUpdate.ToShortTimeString() +
                            "\r\n\t" + "Types:");

                        foreach (string s in rf.TypeNames)
                        {
                            sr.Write("\t\t" + s + "\r\n");
                        }
                    }
                    catch (Exception ex) { sr.Write(f.Name + iCounter.ToString() + "\r\n" + f.FullName + "\t" + ex.Message + "\r\n"); }
                }
                sr.Close();
            }


            //List<string> filesNotDeleted = ZGF.Revit.RfaFileWrapper.CleanupRfaXmlFiles(new DirectoryInfo(@"Z:\ZGFResources\Software_Content_Libraries\Revit\2015\Components\Floors"), true);

            //foreach (string s in filesNotDeleted)
            //    Console.WriteLine(s);

            Console.WriteLine("Done!");
            //Console.ReadLine();





            Console.WriteLine("\r\nDone!");

            //FileSystemWatcher fsw = new FileSystemWatcher();
            //fsw.Path = @"c:\temp\WatchedFolder";
            //fsw.Filter = "*.rfa";
            //fsw.IncludeSubdirectories = true;
            //fsw.EnableRaisingEvents = true;
            //fsw.NotifyFilter = NotifyFilters.LastWrite;

            Console.ReadLine();
            //fsw.Dispose();
        }

    }
}
