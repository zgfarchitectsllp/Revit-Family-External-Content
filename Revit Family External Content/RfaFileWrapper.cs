using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace RevitContentWatcher
{
    public class RfaFileWrapper
    {
        // M E M B E R S
        private XDocument _xmlDoc;

        private XNamespace _xmlNs = "http://www.w3.org/2005/Atom";
        private XNamespace _xmlNsA = "urn:schemas-autodesk-com:partatom";

        private FileInfo _pathToXmlFile;

        // https://stackoverflow.com/questions/2340411/use-linq-to-xml-with-xml-namespaces
        // http://www.codearsenal.net/2012/07/c-sharp-load-xml-using-xlinq.html#.WeFbs4jythE


        public RfaFileWrapper(FileInfo pathTo_Rfa_File)
        {
            theFileInfo = pathTo_Rfa_File;

            // Get the Part Atom and parse XML...
            if (!pathTo_Rfa_File.Exists) throw new FileNotFoundException();

            _pathToXmlFile = pathTo_Rfa_File;
            string XmlData = string.Empty;
            try
            {
                XmlData = RfaFileWrapper.GetPartAtomFromRfa(pathTo_Rfa_File, "entry", 2048, 1000);
            }
            catch { }

            try
            {
                _xmlDoc = XDocument.Parse(XmlData);
            }
            catch
            {
                // Bad XML Part Atom - Waaah...
                throw new FormatException("Malformed XML string");
            } // What if the file is too big?

            // Initialize Class
            XElement _root = _xmlDoc.Root;

            this.FamilyName = Path.GetFileNameWithoutExtension(pathTo_Rfa_File.Name);   // _root.Elements().Where(n => n.Name == _xmlNs + "title").Single().Value;

            var categoryNodes = _root.Descendants(_xmlNs + "category");

            foreach (XElement x in categoryNodes)
            {
                XElement term = (XElement)x.Element(_xmlNs + "term");
                XElement scheme = (XElement)x.Element(_xmlNs + "scheme");
                if (scheme.Value.Equals("adsk:revit:grouping"))
                {
                    this.Category = term.Value;
                    break;
                }

            }


            this.LastUpdate = Convert.ToDateTime(_root.Elements().Where(n => n.Name == _xmlNs + "updated").Single().Value);

            this.RfaPathOnDisk = Path.Combine(_pathToXmlFile.Directory.FullName, this.FamilyName + ".rfa");

            this.RevitVersion = _root.Elements(_xmlNs + "link").Elements()
                .Elements()
                .Where(n => n.Name == _xmlNsA + "product-version").Single().Value;

            var family = _root.Elements(_xmlNsA + "family").Elements();
            try
            {
                this.HostingMethod = family.Where(n => n.Name == _xmlNs + "Host").Single().Value;
            }
            catch { this.HostingMethod = string.Empty; }

            // type names
            this.TypeNames = family
                .Where(n => n.Name == _xmlNsA + "part").Elements()
                .Where(n => n.Name == _xmlNs + "title")
                .Select(n => n.Value)
                .ToList<string>();
        }

        // P R O P E R T I E S

        public string Category { get; private set; }

        public string FamilyName { get; private set; }

        public DateTime LastUpdate { get; private set; }

        public string RfaPathOnDisk { get; private set; }

        public List<string> TypeNames { get; private set; }

        public string HostingMethod { get; private set; }

        public string RevitVersion { get; private set; }

        public FileInfo theFileInfo { get; private set; }


        public int? DatabaseID { get; set; } = null;

        public string LineEntryDelimited(char delimiter, bool includeTypeNames)
        {
            StringBuilder output = new StringBuilder();

            string prefix = this.FamilyName + delimiter + this.Category + delimiter + this.RfaPathOnDisk + delimiter + this.HostingMethod + delimiter + this.RevitVersion;

            if (includeTypeNames && this.TypeNames.Count > 0)
            {
                foreach (string t in this.TypeNames)
                {
                    output.Append(prefix + delimiter + t + "\r\n");
                }
            }
            else
            {
                output.Append(prefix + delimiter + delimiter + "\r\n");
            }

            return output.ToString();
        }

        // S T A T I C   H E L P E R S


        public static void WriteFamilyDataFlatFile(DirectoryInfo source, FileInfo target, bool useExistingXML, bool includeTypeNames)
        {

            StringBuilder outputData = new StringBuilder();

            string fileFilter = useExistingXML ? "*.xml" : "*.rfa";

            FileInfo[] dataFiles = source.GetFiles(fileFilter, SearchOption.AllDirectories);


            if (useExistingXML)
            {
                foreach (FileInfo f in dataFiles)
                {
                    try
                    {
                        outputData.Append(new RfaFileWrapper(f).LineEntryDelimited('\t', includeTypeNames));
                    }
                    catch { continue; }
                }
            }
            else // RFA Files
            {
                Regex backupRfaMatcher = new Regex(@".+\.\d\d\d\d\.rfa", RegexOptions.IgnoreCase);

                foreach (FileInfo f in dataFiles)
                {
                    if (backupRfaMatcher.IsMatch(f.Name)) continue;

                    try
                    {
                        string xmlData = RfaFileWrapper.GetPartAtomFromRfa(f, "entry", 2048, 1000);
                        //outputData.Append(new RfaFileWrapper(xmlData, f).LineEntryDelimited('\t', includeTypeNames));
                    }
                    catch (Exception ex)
                    {
                        continue;
                        //Console.WriteLine("FAILURE: " + f.FullName + "\r\n\t" + ex.Message);
                    }

                }
            }

            // Output File
            using (StreamWriter writer = new StreamWriter(target.FullName))
            {
                writer.Write(outputData.ToString());
                writer.Close();
            }

        }


        /// <summary>
        /// Creates XML files for each valid RFA file containing searchable date such as Category, Types, modified date.
        /// </summary>
        /// <param name="source"></param>
        public static void WriteXmlFilesFromRfaContentLibrary(DirectoryInfo source)
        {
            Regex backupRfaMatcher = new Regex(@".+\.\d\d\d\d\.rfa", RegexOptions.IgnoreCase);

            FileInfo[] rfaFiles = source.GetFiles("*.rfa", SearchOption.AllDirectories);

            foreach (FileInfo f in rfaFiles)
            {
                if (backupRfaMatcher.IsMatch(f.Name)) continue;

                string xmlFilename = Path.Combine(f.DirectoryName, Path.GetFileNameWithoutExtension(f.Name) + ".xml");

                try
                {
                    string xmlData = RfaFileWrapper.GetPartAtomFromRfa(f, "entry", 2048, 1000);
                    using (StreamWriter writer = new StreamWriter(xmlFilename))
                    {
                        writer.Write(xmlData);
                        writer.Close();
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("FAILURE: " + f.FullName + "\r\n\t" + ex.Message);
                }

            }
        }

        /// <summary>
        /// Recursively deletes XML files in specified directory
        /// </summary>
        /// <param name="contentFolder">Top-level directory</param>
        /// <param name="DeleteAll">If true, all XML files are deleted, else only those without a corresponding RFA file</param>
        /// <returns>List of file paths that could not be successfully deleted</returns>
        public static List<string> CleanupRfaXmlFiles(DirectoryInfo contentFolder, bool DeleteAll)
        {
            if (!contentFolder.Exists) throw new DirectoryNotFoundException();

            List<string> _filesUnableToDelete = new List<string>();

            string[] xmlFiles = Directory.GetFiles(contentFolder.FullName, "*.xml", SearchOption.AllDirectories);

            foreach (string f in xmlFiles)
            {
                try
                {
                    if (!DeleteAll)
                    {
                        FileInfo rfaFile = new FileInfo(Path.Combine(Path.GetDirectoryName(f), Path.GetFileNameWithoutExtension(f) + ".rfa"));
                        if (rfaFile.Exists)
                            continue;
                    }

                    File.Delete(f);
                }
                catch (Exception ex)
                {
                    _filesUnableToDelete.Add(f);
                }
            }

            return _filesUnableToDelete;
        }


        /// <summary>
        /// Helper function to extract the XML part atom from a Revit family file.
        /// </summary>
        /// <param name="rfaPath">Path to the Revit Family file</param>
        /// <param name="rootTagName">Root XML Tag. Should be "entry" for a Revit family file</param>
        /// <param name="buffersize">The size of the file chunk in bytes in which to hunt for the XML part atom. Smaller is better, but if too small then it may take a lot of chunks to find all of the XML</param>
        /// <param name="MaxNumberOfTries">How many chunks of the file to process before giving up.</param>
        /// <returns>A string of XML data</returns>
        public static string GetPartAtomFromRfa(FileInfo rfaPath, string rootTagName, int buffersize, int MaxNumberOfTries)
        {
            if (!rfaPath.Exists) throw new FileNotFoundException();
            if (!rfaPath.Extension.Equals(".rfa", StringComparison.CurrentCultureIgnoreCase)) throw new Exception("File extension must be .RFA.");

            string _startTag = "<" + rootTagName;
            string _endTag = "</" + rootTagName + ">";
            StringBuilder returnStringBuilder = new StringBuilder();

            using (TextReader tr = File.OpenText(rfaPath.FullName))
            {
                int maxNumberofTries = MaxNumberOfTries;
                char[] buffer = new char[buffersize];

                while (!returnStringBuilder.ToString().Contains(_endTag))
                {
                    if (maxNumberofTries < 1)
                    {
                        tr.Close();
                        throw new Exception("Could not locate the XML root ending tag in the file. TAG: " + _endTag + " FILE: " + rfaPath.FullName);
                    }

                    maxNumberofTries--;

                    tr.ReadBlock(buffer, 0, buffersize);
                    returnStringBuilder.Append(new string(buffer));
                }
                tr.Close();
            }

            // This is a chunk of the RFA that contains the XML part atom:
            string returnXML = returnStringBuilder.ToString();
            // The index in the chunk where the part atom XML begins:
            int x = returnXML.IndexOf(_startTag, 0);
            // Trims the bytes occurring BEFORE the index of the XML data
            returnXML = returnXML.Substring(x);
            // The index in the chunk where the part atom XML ends:
            x = returnXML.IndexOf(_endTag);
            // Trims the bytes occurring AFTER the index of the XML data
            returnXML = returnXML.Substring(0, x + _endTag.Length);

            return returnXML;
        }

    }


}
