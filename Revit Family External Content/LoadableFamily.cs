using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileWatcher;

namespace RevitContentWatcher
{
    class SyncLoadableFamilyRecords
    {
        // P R I V A T E   M E M B E R S
        DirectoryInfo _rootContentPath;
        List<RfaFileWrapper> _rfaFileWrapperObjects; // = new List<RfaFileWrapper>();
        FileInfo _errorLogFile;


#if DEBUG
        //const string ROOT_PATH = @"\\zgf.local\data\ZGFResources\Software_Content_Libraries\Revit Family Library";
        //const string ROOT_PATH = @"\\zgf.local\data\ZGFResources\Software_Content_Libraries\Revit Family Library\Presentation";
        const string ROOT_PATH = @"\\zgf.local\data\ZGFResources\Software_Content_Libraries\Revit Family Library\USA\Detail\Concrete";
#else        
        const string ROOT_PATH = @"\\zgf.local\data\ZGFResources\Software_Content_Libraries\Revit Family Library";
#endif
        // C O N S T R U C T O R


        public SyncLoadableFamilyRecords() // string RootContentFolderPath)
        {

            // Get the ROOT Content folder path:
            _rootContentPath = new DirectoryInfo(ROOT_PATH);  //RootContentFolderPath);

            // #Log file path:
            DirectoryInfo tmpPath = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp\\ZGF_Revit_Content_Sync_Logs"));
            if (!tmpPath.Exists) tmpPath.Create();
            _errorLogFile = new FileInfo(Path.Combine(tmpPath.FullName, "SyncLog_" + Guid.NewGuid().ToString() + ".log"));  
            
            // #TODO: Delete log files older than X days:


            // Get the RFA files:
            _rfaFileWrapperObjects = GetRfaFiles_asWrapperClass();

        }

        // P R O P E R T I E S
        public List<RfaFileWrapper> RfaFiles { get { return _rfaFileWrapperObjects; } }

        // M E T H O D S

        /// <summary>
        /// Iterate Content DB records of file type "Loadable FAmily." If there is a matching file, update the record properties. If no file, then delete the record.
        /// </summary>
        public void DeleteRfaDataRecordsWithoutFile()
        {
            // https://www.codeproject.com/Articles/215712/LINQ-to-SQL-Basic-Concepts-and-Features
            // Get the database:            
            RevitContentDataContext rfaDB = new RevitContentDataContext();

            // Retrieve RFA (Loadable Families) records in the DB:
            IEnumerable<RevitContentMain> orphanedRecords = rfaDB.RevitContentMains
                .Where(r => r.FileType.Equals("Loadable Family"));

            IEnumerable<RevitContentMain> rec = orphanedRecords.Where(r => r.ID == 16951);

            //int x = orphanedRecords.Count();

            // Filter out those records that have a matching file in the Library:
            orphanedRecords = orphanedRecords.Where(r => !File.Exists(r.FullPathName));

            Debug.Assert(orphanedRecords.Count() == 0, "No orphaned records.");

            // #This deletes any DB records that don't have a matching file, so BEWARE if you're debugging
            // against a truncated directory list:
            if (0 < orphanedRecords.Count() )
            {
                rfaDB.RevitContentMains.DeleteAllOnSubmit(orphanedRecords);
#if !DEBUG
                rfaDB.SubmitChanges();
#endif
            }

            //x = orphanedRecords.Count();
            // Delete orphaned records:
            //if (orphanedRecords.Count > 0)
            //{
            //    int x = orphanedRecords.Count + 1;
            //}

        }

        public void UpdateDataRfaRecords()
        {
            // Get the database:
            RevitContentDataContext rfaDB = new RevitContentDataContext();
            // Get_RevitContentDataConnection_Service("PDX-SQL-3", "ZGF_Revit_Content"));

            //bool exsts = rfaDB.DatabaseExists();

            using (StreamWriter _errorLogger = new StreamWriter(_errorLogFile.FullName, true))
            {
                _errorLogger.WriteLine("\r\n\r\nBegin Updating Loadable Family Data Records:");
                _errorLogger.WriteLine(_rfaFileWrapperObjects.Count.ToString() + " RFA File Wrapper objects");

                foreach (RfaFileWrapper rfw in _rfaFileWrapperObjects)
                {
                    // Get the record associated with the content path
                    int recordCount = rfaDB.RevitContentMains
                        .Where(p => p.FullPathName == rfw.RfaPathOnDisk)
                        .Count();

                    if (recordCount > 1)
                    {
                        // TODO: More than 1 record, delete all records...


                        // Recount
                        recordCount = rfaDB.RevitContentMains
                        .Where(p => p.FullPathName.Equals(rfw.RfaPathOnDisk, StringComparison.CurrentCultureIgnoreCase))
                        .Count();
                    }

                    Debug.Assert(recordCount > 0, "RecordCound = " + recordCount.ToString());

                    switch (recordCount)
                    {
                        case 0: // No Record, add it.
                            RevitContentMain rc = new RevitContentMain()
                            {
                                FileName = rfw.FamilyName,
                                FullPathName = rfw.RfaPathOnDisk,
                                FileVersion = Convert.ToInt32(rfw.RevitVersion),
                                FileType = "Loadable Family",
                                CategoryName = rfw.Category,
                                FileSize = rfw.theFileInfo.Length,
                                LastUpdate = rfw.LastUpdate,
                                FolderSearchTerm = ExtractFolderSearchTerm(rfw.RfaPathOnDisk)
                            };
                            rfaDB.RevitContentMains.InsertOnSubmit(rc);
                            rfaDB.SubmitChanges();
                            break;
                        case 1: // Found a record, update it...
                            IEnumerable<RevitContentMain> rfaDbItem =
                                from r in rfaDB.RevitContentMains
                                where r.FullPathName == rfw.RfaPathOnDisk
                                select r;

                            foreach (var rec in rfaDbItem)
                            {
                                if (null == rec.LastUpdate) rec.LastUpdate = DateTime.MinValue;

                                if (rfw.LastUpdate > rec.LastUpdate)
                                {
                                    rec.FileName = rfw.FamilyName;
                                    rec.FullPathName = rfw.RfaPathOnDisk;
                                    rec.FileVersion = Convert.ToInt32(rfw.RevitVersion);
                                    rec.FileType = "Loadable Family";
                                    rec.CategoryName = rfw.Category;
                                    rec.FileSize = rfw.theFileInfo.Length;
                                    rec.LastUpdate = rfw.LastUpdate;
                                    rec.FolderSearchTerm = ExtractFolderSearchTerm(rfw.RfaPathOnDisk);
                                    rfaDB.SubmitChanges();
                                }
                            }

                            break;
                    }


                }


                // Iterate the records. If no file, then delete the record.

                _errorLogger.Close();
            }

        }

        /// <summary>
        /// Recursively iterate all files in the Content folder. If there is a matching record, then update it. If no record, then create a new one. 
        /// </summary>
        public List<RfaFileWrapper> GetRfaFiles_asWrapperClass()
        {
            if (!_rootContentPath.Exists) throw new DirectoryNotFoundException();

            int fileCounterSuccess = 0;
            int fileCounterFailure = 0;

            DateTime startLoggingFiles = DateTime.Now;

            List<FileInfo> _rfaFiles = _rootContentPath.GetFiles("*.rfa", SearchOption.AllDirectories).ToList<FileInfo>();
            // Try to get the Part Atom from the RFA. If can't get it, then log an error and just enter the basics. 

            List<RfaFileWrapper> _rfwList = new List<RfaFileWrapper>();

            // Turn all of the RFA files into RfafileWrappers:

            using (StreamWriter _errorLogger = new StreamWriter(_errorLogFile.FullName, true))
            {
                
                foreach (FileInfo fi in _rfaFiles)
                {
                    try
                    {                        
                        // TODO: What if this fails...?
                        _rfwList.Add(new RfaFileWrapper(fi));
                        fileCounterSuccess++;
                    }
                    catch (FormatException ex)
                    {
                        // log this....
                        _errorLogger.WriteLine(fi.FullName + "\t" + ex.Message);
                        fileCounterFailure++;
                    }                    
                }

                _errorLogger.WriteLine("\n" + fileCounterSuccess.ToString() + " files successfully cataloged.");
                _errorLogger.WriteLine(fileCounterFailure.ToString() + " files could not be cataloged.");
                TimeSpan duration = (DateTime.Now - startLoggingFiles);
                _errorLogger.WriteLine("\nTotal Duration: " + duration.Hours.ToString() + " hrs " + duration.Minutes.ToString() + " mins " + duration.Seconds.ToString() + " secs");

                _errorLogger.Close();
            }

            return _rfwList;

        }


        // Constructs a series of search words by removing the path root
        private string ExtractFolderSearchTerm(string filePath)
        {
            // \\zgf.local\data\ZGFResources\Software_Content_Libraries\Revit Family Library\Presentation\2017\Specialty Equipment\meqp - Refrigerator Domestic With Freezer (Presentation).rfa
            return Path.GetDirectoryName(filePath)
                .Replace(ROOT_PATH, string.Empty)
                .Replace("\\", " ")
                .Trim();
        }

        /// <summary>
        /// Connection string for read-write access to Revit Content Database
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static string Get_RevitContentDataConnection_Service(string serverName, string databaseName)
        {
            return "Data Source=" + serverName + ";" +
                "Initial Catalog=" + databaseName + ";" +
                "Persist Security Info=True;" +
                "User ID=RevitContentManager;" +
                "Password=8jia6clip";
        }


    }
}
