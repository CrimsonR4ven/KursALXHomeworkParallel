using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hw_AsyncCarPlates
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Plates plates = new Plates();
			WebClient wb = new WebClient();
			plates.DownloadJpgsParallel();
			Console.WriteLine(plates);
			Console.ReadKey();
		}
	}
}
