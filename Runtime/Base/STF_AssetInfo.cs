using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity
{
	[System.Serializable]
	public class AssetInfo
	{
		public string AssetName;
		public string Version;
		public string URL;
		public string Author;
		public string License;
		public string LicenseURL;
		public string DocumentationURL;


		public AssetInfo() {}

		public AssetInfo(JObject JsonAssetInfo)
		{
			foreach((var key, var value) in JsonAssetInfo)
			{
				switch(key)
				{
					case "asset_name": AssetName = (string)value; break;
					case "version": Version = (string)value; break;
					case "url": URL = (string)value; break;
					case "author": Author = (string)value; break;
					case "license": License = (string)value; break;
					case "license_url": LicenseURL = (string)value; break;
					case "documentation_url": DocumentationURL = (string)value; break;
				}
			}
		}

		public JObject ToJson()
		{
			var ret = new JObject();
			if(!string.IsNullOrWhiteSpace(AssetName)) ret.Add("asset_name", AssetName);
			if(!string.IsNullOrWhiteSpace(Version)) ret.Add("version", Version);
			if(!string.IsNullOrWhiteSpace(URL)) ret.Add("url", URL);
			if(!string.IsNullOrWhiteSpace(Author)) ret.Add("author", Author);
			if(!string.IsNullOrWhiteSpace(License)) ret.Add("license", License);
			if(!string.IsNullOrWhiteSpace(LicenseURL)) ret.Add("license_url", LicenseURL);
			if(!string.IsNullOrWhiteSpace(DocumentationURL)) ret.Add("documentation_url", DocumentationURL);
			return ret;
		}
	}
}

