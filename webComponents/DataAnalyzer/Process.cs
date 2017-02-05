using System;

namespace AppCentralWebService
{
	/// <summary>
	/// Summary description for Process.
	/// </summary>
	public class Process:IDataAnalyser
	{
		public Process()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region IDataAnalyser Members

		public string Name()
		{
			return "Process";
		}

		public string Post(string query, string data)
		{
			// TODO:  Add Process.Post implementation
			return "Process Post";
		}

		public string Get(string query, string data)
		{
			// TODO:  Add Process.Get implementation
			return "Process Get";
		}

		public string Support(string query, string data)
		{
			// TODO:  Add Process.Put implementation
			return "Process Support";
		}

		public string List(string query, string data)
		{
			// TODO:  Add Process.Put implementation
			return "Process List";
		}

		#endregion
	}
}
