
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	[Serializable]
	public class STF_Meta
	{
		[Serializable]
		public class AssetInfo
		{
			[Serializable]
			public class AssetInfoProperty {public string Name; public string Value;}

			public string AssetName;
			public string Version;
			public string URL;
			public string Author;
			public string License;
			public string LicenseURL;
			public string DocumentationURL;

			public List<AssetInfoProperty> CustomProperties = new();
		}

		public uint DefinitionVersionMajor = 0;
		public uint DefinitionVersionMinor = 0;
		public string Timestamp = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern);
		public string Generator = "stf_unity";
		public string Root;
		public double MetricMultiplier = 1;
		public List<string> Profiles = new();
		public AssetInfo STFAssetInfo = new();


		public STF_Meta() {}

		public STF_Meta(JObject JsonMeta)
		{
			DefinitionVersionMajor = (uint)JsonMeta.GetValue("version_major");
			DefinitionVersionMinor = (uint)JsonMeta.GetValue("version_minor");
			Timestamp = (string)JsonMeta.GetValue("timestamp");
			Generator = (string)JsonMeta.GetValue("generator");
			Root = (string)JsonMeta.GetValue("root");
			MetricMultiplier = (double)JsonMeta.GetValue("metric_multiplier");
			Profiles = JsonMeta.GetValue("profiles").ToObject<List<string>>();
			if(JsonMeta.GetValue("asset_info") is JObject assetInfo)
			{
				STFAssetInfo.AssetName = (string)assetInfo.GetValue("asset_name");
				STFAssetInfo.Version = (string)assetInfo.GetValue("version");
				STFAssetInfo.URL = (string)assetInfo.GetValue("url");
				STFAssetInfo.Author = (string)assetInfo.GetValue("author");
				STFAssetInfo.License = (string)assetInfo.GetValue("license");
				STFAssetInfo.LicenseURL = (string)assetInfo.GetValue("license_url");
				STFAssetInfo.DocumentationURL = (string)assetInfo.GetValue("documentation_url");
				foreach((var key, var value) in assetInfo)
				{
					switch(key)
					{
						case "asset_name":
							STFAssetInfo.AssetName = (string)value;
							break;
						case "version":
							STFAssetInfo.Version = (string)value;
							break;
						case "url":
							STFAssetInfo.URL = (string)value;
							break;
						case "author":
							STFAssetInfo.Author = (string)value;
							break;
						case "license":
							STFAssetInfo.License = (string)value;
							break;
						case "license_url":
							STFAssetInfo.LicenseURL = (string)value;
							break;
						case "documentation_url":
							STFAssetInfo.DocumentationURL = (string)value;
							break;
						default:
							STFAssetInfo.CustomProperties.Add(new AssetInfo.AssetInfoProperty {Name=key, Value=(string)value});
							break;
					}
				}
			}
		}
	}
}

