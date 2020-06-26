using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ZGF.Revit
{
    class RevitPartAtomReader
    {
        FileInfo _revitFamilyFile = null;

        public RevitPartAtomReader(FileInfo revitFamilyFile)
        {
            if (revitFamilyFile.Exists)
            {

            }
            else
                throw new FileNotFoundException();
        }


        // H E L P E R S 

        

        private string GetPartString(FileInfo revitFamilyFile)
        {
            string startTagVal = "<entry ";
            int startTagIndex = 0;

            bool endtagOpen = true;
            StringBuilder outString = new StringBuilder();
            StringBuilder startTag = new StringBuilder();
            StringBuilder endTag = new StringBuilder();

            using (TextReader sr = File.OpenText(revitFamilyFile.FullName))
            {
                int ic = sr.Read();

                while (ic != -1 & endtagOpen)
                {


                };


                sr.Close();
            }


                return outString.ToString();
        }


    }
}
