using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	[System.Serializable]
	public class ImportOptions
	{
		public bool AbortOnException = false;
		public bool AuthoringImport = false;
		public string SelectedApplication = "default";

		public bool IsFirstImport = true;


		[System.Serializable]
		public class ModuleImportOption
		{
			public string Module;
			public string Json;
		}
		public List<ModuleImportOption> ModuleImportOptions = new();

		public JObject GetModuleImportOptions(string Module)
		{
			foreach(var opt in ModuleImportOptions)
			{
				try {
					if(opt.Module == Module) return JObject.Parse(opt.Json);
				}
				catch
				{
					break;
				}
			}
			return new JObject();
		}

		public void SetModuleImportOptions(string Module, JObject Options)
		{
			foreach(var opt in ModuleImportOptions)
			{
				if(opt.Module == Module)
				{
					opt.Json = Options.ToString();
					return;
				}
			}
			ModuleImportOptions.Add(new () { Module = Module, Json = Options.ToString() });
		}

		[System.Serializable]
		public class ResourceImportOption
		{
			public string Module;
			public string STF_Id;
			public string DisplayName;
			public string Json;
		}
		public List<ResourceImportOption> ResourceImportOptions = new();
		public List<ResourceImportOption> ResourceImportOptionsConfirm = null;

		public JObject GetResourceImportOptions(string Module, string STF_Id)
		{
			foreach(var opt in ResourceImportOptions)
			{
				try {
					if(opt.Module == Module && opt.STF_Id == STF_Id) return JObject.Parse(opt.Json);
				}
				catch
				{
					break;
				}
			}
			return new JObject();
		}

		public void ConfirmResourceImportOptions(string Module, string STF_Id, JObject Options, string DisplayName = null)
		{
			foreach(var opt in ResourceImportOptionsConfirm)
			{
				if(opt.STF_Id == STF_Id)
				{
					opt.Module = Module;
					opt.Json = Options.ToString();
					opt.DisplayName = DisplayName;
					return;
				}
			}
			ResourceImportOptionsConfirm.Add(new () { Module = Module, STF_Id = STF_Id, Json = Options.ToString(), DisplayName = DisplayName });
		}


		//! TODO Remove everything below
		[System.Serializable]
		public class MaterialMapping
		{
			public string ID;
			public string MaterialName;
			public string TargetShader;
		}
		public List<MaterialMapping> MaterialMappings = new();

		public bool ImportVertexColors = true;
		public int MaxWeights = 4;
	}
}
