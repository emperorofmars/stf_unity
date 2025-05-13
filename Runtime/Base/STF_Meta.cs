
using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	[Serializable]
	public class STF_Meta
	{
		public const uint _DefinitionVersionMajor = 0;
		public const uint _DefinitionVersionMinor = 0;
		public const string _Generator = "stf_unity";

		public uint DefinitionVersionMajor = _DefinitionVersionMajor;
		public uint DefinitionVersionMinor = _DefinitionVersionMinor;
		public string Timestamp = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern);
		public string Generator = _Generator;
		public string Root;
		public double MetricMultiplier = 1;
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

			if(JsonMeta.GetValue("asset_info") is JObject assetInfo)
				STFAssetInfo = new AssetInfo(assetInfo);
		}

		public JObject ToJson()
		{
			var ret = new JObject {
				{"version_major", _DefinitionVersionMajor},
				{"version_minor", _DefinitionVersionMinor},
				{"timestamp", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern)},
				{"generator", _Generator},
				{"root", Root},
				{"metric_multiplier", MetricMultiplier},
				{"asset_info", STFAssetInfo.ToJson()},
			};

			return ret;
		}
	}
}

