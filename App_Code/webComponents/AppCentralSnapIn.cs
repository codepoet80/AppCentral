using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


namespace AppCentralService.webComponents
{
    /// <summary>
    /// Summary description for AppCentralSnapIn.
    /// </summary>
    public class AppCentralSnapIn
    {
        public AppCentralSnapIn()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string urlDecode(string param)
        {
            string decodeVal;
            decodeVal = param;
            decodeVal = decodeVal.Replace("%25", "%");
            decodeVal = decodeVal.Replace("%26", "&");
            decodeVal = decodeVal.Replace("%20", " ");
            decodeVal = decodeVal.Replace("%23", "#");
            decodeVal = decodeVal.Replace("%2B", "+");
            decodeVal = decodeVal.Replace("%27", "'");
            decodeVal = decodeVal.Replace("%2C", ",");
            decodeVal = decodeVal.Replace("\\", "/");
            return decodeVal;
        }

        public string getFileName(string fpath)
        {
            string[] pathParts = fpath.Split('\\');
            string fileName = pathParts[pathParts.Length - 1];
            pathParts = fileName.Split('.');
            if (pathParts.Length > 1)
            {
                fileName = "";
                for (int f = 0; f <= pathParts.Length - 1; f++)
                {
                    if (f < pathParts.Length - 1)
                    {
                        fileName += pathParts[f];
                    }
                }
            }
            return fileName;
        }

        public string getType(string fpath)
        {
            string[] pathParts = fpath.Split('.');
            string fileType = pathParts[pathParts.Length - 1];
            return fileType.ToLower();
        }

        public string calculateSize(long fsize)
        {
            string fileSize = fsize.ToString();
            if (fsize < 1024)
            {
                fileSize = fsize.ToString() + " Bytes";
            }
            if (fsize >= 1024 && fsize < 1024000)
            {
                fsize = fsize / 1024;
                fileSize = fsize.ToString() + " KB";
            }
            if (fsize >= 1024000 && fsize < 10240000)
            {
                fsize = (fsize / 1024) / 1024;
                fileSize = fsize.ToString() + " MB";
            }
            if (fsize > 10240000)
            {
                fsize = ((fsize / 1024) / 1024) / 1024;
                fileSize = fsize.ToString() + " GB";
            }
            return fileSize;
        }

        public string ListFiles(string Folder, string Filter)
        {
            if (Filter == null || Filter == "")
                Filter = "*";

            ListItems fileList = new ListItems();
            fileList.Folder = Folder;

            try
            {
                string[] directoryEntries =
                    System.IO.Directory.GetFileSystemEntries(Folder, Filter);

                ListItem[] Items = new ListItem[directoryEntries.Length];
                int i = 0;

                foreach (string str in directoryEntries)
                {
                    ListItem currItem = new ListItem();
                    if (File.Exists(str))	//check to see if it's a file
                    {
                        currItem.Path = str.ToString();
                        currItem.Name = getFileName(str.ToString());
                        FileInfo fi = new FileInfo(str);
                        currItem.Type = fi.Extension.ToLower();
                        currItem.Type = currItem.Type.Replace(".", "");
                        currItem.Size = calculateSize(fi.Length);
                        currItem.Date = fi.CreationTime.ToString();
                        Items[i] = new ListItem();
                        Items[i] = currItem;
                    }
                    else	//it's a subfolder
                    {
                        currItem.Path = str.ToString();
                        currItem.Name = getFileName(str.ToString());
                        currItem.Type = "folder";
                        FileInfo fi = new FileInfo(str);
                        currItem.Date = fi.CreationTime.ToString();
                        Items[i] = new ListItem();
                        Items[i] = currItem;
                    }
                    currItem = null;
                    i++;
                }
                fileList.Item = Items;
                fileList.Result = "S_OK";
            }
            catch (Exception ex)
            {
                fileList.Result = ex.Message;
            }
            XmlDocument xd = new XmlDocument();
            XmlTextWriter writer = new XmlTextWriter("C:\\Build 33 - JW\\output.xml", null);
            StringWriter sw = new StringWriter();

            XmlSerializer serializer = new XmlSerializer(typeof(ListItems));
            serializer.Serialize(sw, fileList);
            return sw.ToString();
        }
    }


    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.jonandnic.com/webservice/photoshare/v1.0")]
    public class ListItems
    {
        [System.Xml.Serialization.XmlElementAttribute("Result")]
        public string Result;

        [System.Xml.Serialization.XmlElementAttribute("Item")]
        public ListItem[] Item;

        [System.Xml.Serialization.XmlElementAttribute("Description")]
        public string Description;

        [System.Xml.Serialization.XmlElementAttribute("Date")]
        public string Date;

        [System.Xml.Serialization.XmlElementAttribute("Author")]
        public string Author;

        [System.Xml.Serialization.XmlAttributeAttribute("Folder")]
        public string Folder;

        [System.Xml.Serialization.XmlAttributeAttribute("Filter")]
        public string Filter;
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.jonandnic.com/webservice/photoshare/v1.0")]
    public class ListItem
    {
        [System.Xml.Serialization.XmlAttributeAttribute("Name")]
        public string Name;

        [System.Xml.Serialization.XmlAttributeAttribute("Path")]
        public string Path;

        [System.Xml.Serialization.XmlAttributeAttribute("Size")]
        public string Size;

        [System.Xml.Serialization.XmlAttributeAttribute("Description")]
        public string Description;

        [System.Xml.Serialization.XmlAttributeAttribute("Type")]
        public string Type;

        [System.Xml.Serialization.XmlAttributeAttribute("Date")]
        public string Date;
    }
}
