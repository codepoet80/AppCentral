using System;
using System.IO;
using System.Web;
using System.Diagnostics;
using System.Reflection;
using System.Collections;

namespace AppCentralWebService
{
	/// <summary>
	/// Summary description for DataAnalyserBuilder.
	/// </summary>
	public class DataAnalyserFactory
	{
		private static DataAnalyserFactory instance=null;
		private static Hashtable dataAnalyserList=null;

		private DataAnalyserFactory()
		{
		}

		public static DataAnalyserFactory GetInstance()
		{
			if(instance==null)
			{
				instance = new DataAnalyserFactory();
				Register();
			}
			return instance;
		}

		public IDataAnalyser GetDataAnalyser(string type)
		{
			if(dataAnalyserList[type] is IDataAnalyser)
				return dataAnalyserList[type] as IDataAnalyser;

			// This should not happen, but it's a good idea to have a default class to return an error or something.
			return new DefaultDataAnalyser();
		}

		public static void Register()
		{
			dataAnalyserList = new Hashtable();
			foreach(string file in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/bin"), "*.dll"))
			{
				try
				{
					FileInfo fileInfo = new FileInfo(file);
					string assemblyPath = fileInfo.Name.Replace(fileInfo.Extension, "");
					Assembly asm = AppDomain.CurrentDomain.Load(assemblyPath);
    
					foreach(Type t in asm.GetTypes())
					{
						if(!t.IsInterface && !t.IsAbstract && typeof(IDataAnalyser).IsAssignableFrom(t))
						{
							try
							{
								IDataAnalyser dataAnalyser = (IDataAnalyser)Activator.CreateInstance(t);
								dataAnalyserList.Add(dataAnalyser.Name(), dataAnalyser);
							}
							catch (Exception e)
							{
								Trace.WriteLine("Exception encountered loading type '" + t.FullName +  "': " + e.ToString());
							}
						}
					}
				}
				catch (Exception e)
				{
					Trace.WriteLine("Exception encountered loading control: " + e.ToString());
				}
			}
		}
	}

	public class DefaultDataAnalyser:IDataAnalyser
	{
		public DefaultDataAnalyser()
		{
		}
		#region IDataAnalyser Members

		public string Name()
		{
			return "Default";
		}

		public string Post(string query, string data)
		{
			// TODO:  Add Class1.Post implementation
			return "Default Post";
		}

		public string Get(string query, string data)
		{
			// TODO:  Add Class1.Get implementation
			return "Default Get";
		}

		public string Support(string query, string data)
		{
			// TODO:  Add Class1.Support implementation
			return "Default Support";
		}

		public string List(string query, string data)
		{
			// TODO:  Add Class1.Support implementation
			return "Default List";
		}

		#endregion
	}
}
