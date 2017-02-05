using System;

namespace AppCentralWebService
{
	public interface IDataAnalyser
	{
		string Name();
		string Post(string query, string data);
		string Get(string query, string data);
		string Support(string query, string data);
		string List(string query, string data);
	}
}
