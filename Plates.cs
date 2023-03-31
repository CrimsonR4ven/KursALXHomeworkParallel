using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hw_AsyncCarPlates
{
	public class Plates
	{
		public string[] PlateFiles;
		public string address;
		public string FolderPath;

		public int DownloadedBytes { get; private set; }

		public Plates()
		{
			DownloadedBytes = 0;
			FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Car Plates");
			GetFilesList();
		}

		public void GetFilesList()
		{
			WebClient wb = new WebClient();
			byte[] plateBytes = wb.DownloadData("http://51.91.120.89/TABLICE/");
			DownloadedBytes += plateBytes.Length;
			PlateFiles = Encoding.Default.GetString(plateBytes).Split('\n');
			Console.WriteLine($"{DownloadedBytes}B\n");
			Directory.CreateDirectory(FolderPath);
		}
		public void DownloadJpgsParallel()
		{
			Parallel.For(0, PlateFiles.Length - 1, i =>
			{
				WebClient wb = new WebClient();
				byte[] temp = wb.DownloadData("http://51.91.120.89/TABLICE/" + PlateFiles[i]);
				DownloadedBytes += temp.Length;
				LogDataDownloaded(temp.Length, PlateFiles[i]);
				File.WriteAllBytes(FolderPath + "/" + PlateFiles[i].Split('.')[0] + ".jpg", temp);
			});
		}
		private string DataInfo(int amountBytes)
		{
			if (amountBytes == 0)
			{
				return "none";
			}
			else if (amountBytes < 1024)
			{
				return $"{amountBytes}B";
			}
			else if (amountBytes < 1048576)
			{
				return $"{amountBytes / 1024}KB {amountBytes % 1024}B";
			}
			else
			{
				return $"{amountBytes / 1048576}MB {(amountBytes % 1048576) / 1024}KB {(amountBytes % 1048576) % 1024}B";
			}
		}
		private void LogDataDownloaded(int bytesNumber, string fileName)
		{
            Console.WriteLine("" +
							 $"File name: {fileName}\n" +
							 $"File size: {DataInfo(bytesNumber)}\n" +
							 $"Overall downloaded data: {DataInfo(DownloadedBytes)}\n");
		}
		private void WriteColor(ConsoleColor color, string writeThis)
		{
			Console.ForegroundColor = color;
			Console.Write(writeThis);
			Console.ResetColor();
		}

		public override string ToString()
		{
			return $"Downloaded data - {DataInfo(DownloadedBytes)}";
		}
	}
}
