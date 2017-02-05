using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace AppCentralWebService
{
	/// <summary>
	/// Summary description for AppCentral.
	/// </summary>
	public class AppCentral : IDataAnalyser
	{
		public AppCentral()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region IDataAnalyser Members

		public string Name()
		{
			return "AppCentral";
		}

		public string Post(string query, string data)
		{
			return "AppCentral Post";
		}

		public string Get(string query, string data)
		{
			switch (query.ToUpper())
			{
				default:
				{
					return "AppCentral Get";
				}
			}

		}

		public string Support(string query, string data)
		{
			return "AppCentral Support";
		}

		public string List(string query, string data)
		{
			switch (query.ToUpper())
			{
				case "FILES":
					{
						data = "<Query>" + data + "</Query>";
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(data);
						string folder = xmlDocument.SelectSingleNode("//folder").Value;
						string filter = xmlDocument.SelectSingleNode("//filter").Value;

						return listFiles(folder, filter);
					}
				case "TASKS":
					{
						return listTasks(data);
					}
				case "PROFILES":
					{
						return listProfiles();
					}
				default:
					{
						return "AppCentral List";
					}
			}
		}

		#endregion

		public string listFiles(string folder, string filter)
		{
			if (filter == null || filter == "")
				filter = "*";

			ListItems fileList = new ListItems();
			fileList.Folder = folder;

			try
			{
				string[] directoryEntries =
					System.IO.Directory.GetFileSystemEntries(folder, filter);

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
			StringWriter sw = new StringWriter();
			XmlSerializer serializer = new XmlSerializer(typeof(ListItems));
			serializer.Serialize(sw, fileList);
			return sw.ToString();
		}
		public string listTasks(string profile)
		{
			if (profile.IndexOf("webApps/") == -1)
				profile = "webApps/" + profile;
			string[] directoryEntries = System.IO.Directory.GetFileSystemEntries(HttpContext.Current.Server.MapPath("~/" + profile));
			ModuleList Modules = new ModuleList();
			ModuleFolder[] folderList = new ModuleFolder[directoryEntries.Length];
			int i = 0;

			foreach (string str in directoryEntries)
			{
				if (!File.Exists(str))	//check to see if it's a folder
				{
					ModuleFolder currFolder = new ModuleFolder();
					currFolder.Name = getFileName(str.ToString());

					string[] moduleEntries = System.IO.Directory.GetFileSystemEntries(str);
					Module[] moduleList = new Module[moduleEntries.Length];
					int j = 0;
					foreach (string mod in moduleEntries)
					{
						if (File.Exists(mod + "//module.xml"))
						{
							Module currModule = new Module();
							XmlDocument xd = new XmlDocument();
							xd.Load(mod + "//module.xml");
							XmlNode identity = xd.SelectSingleNode("//module/identity");
							currModule.Name = identity.Attributes.GetNamedItem("name").InnerText;
							currModule.Description = identity.Attributes.GetNamedItem("description").InnerText;
							currModule.largeIcon = identity.Attributes.GetNamedItem("largeIcon").InnerText;
							currModule.smallIcon = identity.Attributes.GetNamedItem("smallIcon").InnerText;
							currModule.Src = identity.Attributes.GetNamedItem("src").InnerText;
							currModule.Folder = profile + "/" + currFolder.Name + "/" + getFileName(mod);
							moduleList[j] = currModule;
						}
						j++;
					}
                    if (j > 0)
                    {
                        currFolder.Modules = moduleList;
                        folderList[i] = currFolder;
                    }
				}
				i++;
			}
			Modules.ModuleFolders = folderList;
			StringWriter sw = new StringWriter();
			XmlSerializer serializer = new XmlSerializer(typeof(ModuleList));
			serializer.Serialize(sw, Modules);
			return sw.ToString();
		}
		public string listProfiles()
		{
			string[] directoryEntries = System.IO.Directory.GetDirectories(HttpContext.Current.Server.MapPath("~/webApps"));
			ProfileList ProfileList = new ProfileList();
			Profile[] Profiles = new Profile[directoryEntries.Length];
			int j = 0;
			foreach (string str in directoryEntries)
			{
				if (!File.Exists(str))	//check to see if it's a folder
				{
					if (File.Exists(str + "//profile.xml"))
					{
						Profile currProfile = new Profile();
						XmlDocument xd = new XmlDocument();
						xd.Load(str + "//profile.xml");
						XmlNode identity = xd.SelectSingleNode("//profile/identity");
						currProfile.Name = identity.Attributes.GetNamedItem("name").InnerText;
						currProfile.Folder = "webApps/" + getFileName(str);
						currProfile.Description = identity.Attributes.GetNamedItem("description").InnerText;
						currProfile.largeIcon = identity.Attributes.GetNamedItem("largeIcon").InnerText;
						currProfile.smallIcon = identity.Attributes.GetNamedItem("smallIcon").InnerText;
						currProfile.Version = identity.Attributes.GetNamedItem("version").InnerText;
						Profiles[j] = currProfile;
						j++;
					}
				}
			}
			ProfileList.Profiles = Profiles;
			StringWriter sw = new StringWriter();
			XmlSerializer serializer = new XmlSerializer(typeof(ProfileList));
			serializer.Serialize(sw, ProfileList);
			return sw.ToString();
		}
		
		#region private functions

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
		#endregion
	}

	#region Reply Classes
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

	[System.Xml.Serialization.XmlTypeAttribute()]
	public class ProfileList
	{
		[System.Xml.Serialization.XmlElementAttribute("Profile")]
		public Profile[] Profiles;
	}

	[System.Xml.Serialization.XmlTypeAttribute()]
	public class Profile
	{
		[System.Xml.Serialization.XmlAttributeAttribute("Name")]
		public string Name;

		[System.Xml.Serialization.XmlAttributeAttribute("Folder")]
		public string Folder;

		[System.Xml.Serialization.XmlAttributeAttribute("largeIcon")]
		public string largeIcon;

		[System.Xml.Serialization.XmlAttributeAttribute("smallIcon")]
		public string smallIcon;

		[System.Xml.Serialization.XmlAttributeAttribute("Description")]
		public string Description;

		[System.Xml.Serialization.XmlAttributeAttribute("Date")]
		public string Date;

		[System.Xml.Serialization.XmlAttributeAttribute("Version")]
		public string Version;
	}

	[System.Xml.Serialization.XmlTypeAttribute()]
	public class ModuleList
	{
		[System.Xml.Serialization.XmlElementAttribute("Folders")]
		public ModuleFolder[] ModuleFolders;
	}

	[System.Xml.Serialization.XmlTypeAttribute()]
	public class ModuleFolder
	{
		[System.Xml.Serialization.XmlAttributeAttribute("Name")]
		public string Name;

		[System.Xml.Serialization.XmlElementAttribute("Modules")]
		public Module[] Modules;
	}

	[System.Xml.Serialization.XmlTypeAttribute()]
	public class Module
	{
		[System.Xml.Serialization.XmlAttributeAttribute("Name")]
		public string Name;

		[System.Xml.Serialization.XmlAttributeAttribute("largeIcon")]
		public string largeIcon;

		[System.Xml.Serialization.XmlAttributeAttribute("smallIcon")]
		public string smallIcon;

		[System.Xml.Serialization.XmlAttributeAttribute("Description")]
		public string Description;

		[System.Xml.Serialization.XmlAttributeAttribute("Src")]
		public string Src;

		[System.Xml.Serialization.XmlAttributeAttribute("Folder")]
		public string Folder;

		[System.Xml.Serialization.XmlAttributeAttribute("Date")]
		public string Date;

		[System.Xml.Serialization.XmlAttributeAttribute("Version")]
		public string Version;
	}
	#endregion

}
