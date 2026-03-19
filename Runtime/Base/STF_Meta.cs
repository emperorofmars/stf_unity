using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	[System.Serializable]
	public class STF_Meta
	{
		public const uint _DefinitionVersionMajor = 0;
		public const uint _DefinitionVersionMinor = 0;
		public const string _Generator = "stf_unity";
		public const string _GeneratorVersion = "0.0.0";

		public uint DefinitionVersionMajor = _DefinitionVersionMajor;
		public uint DefinitionVersionMinor = _DefinitionVersionMinor;
		public string Timestamp = System.DateTime.UtcNow.ToString(CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern);
		public string Generator = _Generator;
		public string GeneratorVersion = _GeneratorVersion;
		public string Root;
		public double MetricMultiplier = 1;
		public AssetInfo STFAssetInfo = new();

		[System.Serializable]
		public class AssetInfoProperty {public string Name; public string Value;}
		public List<AssetInfoProperty> AssetProperties = new();


		public STF_Meta() {}

		public STF_Meta(JObject JsonMeta)
		{
			DefinitionVersionMajor = (uint)JsonMeta.GetValue("version_major");
			DefinitionVersionMinor = (uint)JsonMeta.GetValue("version_minor");
			Timestamp = (string)JsonMeta.GetValue("timestamp");
			Generator = (string)JsonMeta.GetValue("generator");
			GeneratorVersion = (string)JsonMeta.GetValue("generator_version");
			Root = (string)JsonMeta.GetValue("root");
			MetricMultiplier = (double)JsonMeta.GetValue("metric_multiplier");

			if(JsonMeta.GetValue("asset_info") is JObject assetInfo)
				STFAssetInfo = new AssetInfo(assetInfo);

			Debug.Log(JsonMeta.GetValue("asset_properties"));
			if(JsonMeta.GetValue("asset_properties") is JObject assetProperties)
				foreach((var key, var value) in assetProperties)
					AssetProperties.Add(new AssetInfoProperty {Name=key, Value=(string)value});
		}

		public JObject ToJson()
		{
			var assetProperties = new JObject();
			var ret = new JObject {
				{"version_major", _DefinitionVersionMajor},
				{"version_minor", _DefinitionVersionMinor},
				{"timestamp", System.DateTime.UtcNow.ToString(CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern)},
				{"generator", _Generator},
				{"generator_version", _GeneratorVersion},
				{"root", Root},
				{"metric_multiplier", MetricMultiplier},
				{"asset_info", STFAssetInfo.ToJson()},
				{"asset_properties", assetProperties},
			};

			foreach(var customProperty in AssetProperties)
				if(!string.IsNullOrWhiteSpace(customProperty.Name))
					assetProperties.Add(customProperty.Name, customProperty.Value);

			return ret;
		}
	}
}

