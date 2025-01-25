
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public class STF_Definition : ScriptableObject
	{
		public STF_File File;
		public STF_Meta Meta;
		public JObject JsonResources;
		public JObject JsonBuffers;

		public void Init(STF_File File)
		{
			this.File = File;
			var json = JObject.Parse(File.Json);
			Meta = new STF_Meta();
			Meta.Init(json["stf"] as JObject);
			JsonResources = json["resources"] as JObject;
			JsonBuffers = json["buffers"] as JObject;
		}
	}
}
