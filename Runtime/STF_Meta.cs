
using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	[Serializable]
	public class STF_Meta
	{
		public class AssetInfoProperty {public string Name; public string Value;}

		public uint DefinitionVersionMajor = 0;
		public uint DefinitionVersionMinor = 0;
		public string Generator = "stf_unity";
		public string Timestamp = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern);
		public string Root;
		public double MetricMultiplier = 1;
		public List<string> Profiles = new();
		public List<AssetInfoProperty> AssetInfo = new();

		public STF_Meta(JObject JsonMeta)
		{
			Root = (string)JsonMeta.GetValue("root");
		}
	}
}

