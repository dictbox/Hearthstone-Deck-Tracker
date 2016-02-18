﻿#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Controls.Error;
using Hearthstone_Deck_Tracker.Stats;

#endregion

namespace Hearthstone_Deck_Tracker.HsReplay
{
	public class HsReplayUploader
	{
		public static void UploadRawPowerLog(List<string> log)
		{
			//TODO
		}

		public static async Task<string> UploadXml(string filePath)
		{
			string content;
			using(var sr = new StreamReader(filePath))
				content = sr.ReadToEnd();
			var location = await PostAsync("http://hsreplayarchive.org/api/v1/replay/upload", content);
			var id = location.Split('/').Last();
			File.Move(filePath, Path.Combine(HsReplayConstants.TmpDirPath, $"{id}.xml"));
			return id;
		}

		private static async Task<string> PostAsync(string url, string data) => await PostAsync(url, Encoding.UTF8.GetBytes(data));

		private static async Task<string> PostAsync(string url, byte[] data)
		{
			try
			{
				var request = CreateRequest(url, "POST");
				using(var stream = await request.GetRequestStreamAsync())
					stream.Write(data, 0, data.Length);
				var webResponse = await request.GetResponseAsync();
				var location = webResponse.Headers["Location"];
				Logger.WriteLine("< " + location, "HsReplayAPI");
				return location;
			}
			catch(WebException e)
			{
				if(Core.MainWindow != null)
					ErrorManager.AddError(new Error("HsReplayAPI", e.Message));
				throw;
			}
		}

		private static HttpWebRequest CreateRequest(string url, string method)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.ContentType = "application/json";
			request.Accept = "application/json";
			request.Method = method;
			return request;
		}
	}
}