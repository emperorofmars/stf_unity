using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public abstract class STF_ScriptableObject : ScriptableObject, ISTF_Resource
	{
		public abstract string STF_Type {get;}
		public abstract string STF_Kind {get;}

		public string _STF_Id;
		public string STF_Id {get => _STF_Id; set => _STF_Id = value;}

		public string _STF_Name;
		public string STF_Name {get => _STF_Name; set => _STF_Name = value;}

		public bool _Degraded = false;
		public bool Degraded {get => _Degraded; set => _Degraded = value;}

		public readonly List<object> _ProcessedObjects = new();
		public List<object> ProcessedObjects => _ProcessedObjects;

		public ISTF_PropertyConverter _PropertyConverter;
		public ISTF_PropertyConverter PropertyConverter { get { return _PropertyConverter; } set { _PropertyConverter = value; } }

		public virtual void SetFromJson(JObject JsonResource, string STF_Id, string DefaultName = "STF ScriptableObject")
		{
			this.STF_Id = STF_Id;
			this.STF_Name = JsonResource.ContainsKey("name") ? (string)JsonResource["name"] : null;
			this.name = STFUtil.DetermineName(JsonResource, DefaultName);
			this.Degraded = (bool)(JsonResource.GetValue("degraded") ?? false);
		}
	}
}
