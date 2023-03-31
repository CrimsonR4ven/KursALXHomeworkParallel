using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hw_AsyncCarPlates
{
	public class Plates
	{
		public string[] PlateFiles;
		private string Address;
		private string FolderPath;

		public int DownloadedBytes { get; private set; }

		public Plates()
		{
			DownloadedBytes = 0;
			FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Car Plates");
			Address = "http://51.91.120.89/TABLICE/";
			PlateFiles = GetFilesList();
		}

		public string[] GetFilesList()
		{
			try
			{
				WebClient wb = new WebClient();
				byte[] plateBytes = wb.DownloadData(Address);
				DownloadedBytes += plateBytes.Length;
				Console.WriteLine($"{DownloadedBytes}B\n");
				if (!Directory.Exists(FolderPath))
				{
					Directory.CreateDirectory(FolderPath);
				}
				return Encoding.Default.GetString(plateBytes).Split('\n');
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error while downloading file list \n");
			}
			return new string[0];
		}
		public void DownloadJpgsParallel()
		{
			Parallel.For(0, PlateFiles.Length - 1, i =>
			{
				try
				{
					WebClient wb = new WebClient();
					byte[] temp = wb.DownloadData(Address + PlateFiles[i]);
					DownloadedBytes += temp.Length;
					LogDataDownloaded(temp.Length, PlateFiles[i]);
					File.WriteAllBytes(FolderPath + "/" + PlateFiles[i].TrimEnd('\r'), temp);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error while downloading file {PlateFiles[i]}\n");
				}
			});
			if(PlateFiles.Length != 0)
			{
				Process.Start("explorer.exe", FolderPath);
			}
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
            Console.WriteLine($"File name:         {fileName}\n" +
							  $"File size:         {DataInfo(bytesNumber)}\n" +
							  $"Downloaded data:   {DataInfo(DownloadedBytes)}\n");
		}
		public override string ToString()
		{
			return $"Downloaded data - {DataInfo(DownloadedBytes)}";
		}
	}
}
