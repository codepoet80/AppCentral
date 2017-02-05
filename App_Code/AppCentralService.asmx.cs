using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.IO;

namespace AppCentralWebService
{
	/// <summary>
	/// Summary description for Service1.
	/// </summary>
	[WebService(Namespace="http://www.jonandnic.com/appcentral", Description="AppCentral Web Service")]
	public class AppCentralService : System.Web.Services.WebService
	{
		public AppCentralService()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		[WebMethod]
		public string Post(string Type, string query, string data)
		{
			DataAnalyserFactory factory = DataAnalyserFactory.GetInstance();
			IDataAnalyser obj = factory.GetDataAnalyser(Type);
			return obj.Post(query, htmlDecode(data));
		}
		[WebMethod]
		public string Get(string Type, string query, string data)
		{
			DataAnalyserFactory factory = DataAnalyserFactory.GetInstance();
			IDataAnalyser obj = factory.GetDataAnalyser(Type);
			return obj.Get(query, htmlDecode(data));
		}
		[WebMethod]
		public string Support(string Type, string query, string data)
		{
			DataAnalyserFactory factory = DataAnalyserFactory.GetInstance();
			IDataAnalyser obj = factory.GetDataAnalyser(Type);
			return obj.Support(query, htmlDecode(data));
		}
		[WebMethod]
		public string List(string Type, string query, string data)
		{
			DataAnalyserFactory factory = DataAnalyserFactory.GetInstance();
			IDataAnalyser obj = factory.GetDataAnalyser(Type);
			return obj.List(query, htmlDecode(data));
		}

		#region private functions

		public string htmlDecode(string encodedText)
		{
			//encodedText = encodedText.Replace("&amp;", "&");
			encodedText = encodedText.Replace("&lt;", "<");
			encodedText = encodedText.Replace("&gt;", ">");
			encodedText = encodedText.Replace("&#163;", "#");
            encodedText = encodedText.Replace("&linebreak;", "&#13;&#10;");

			return encodedText;
		}

		public string htmlEncode(string decodedText)
		{
			decodedText = decodedText.Replace("<", "&lt;");
			decodedText = decodedText.Replace(">", "&gt;");
			decodedText = decodedText.Replace("#", "&#163;");
			decodedText = decodedText.Replace("&", "&amp;");
			return decodedText;
		}
		#endregion
	}
}
