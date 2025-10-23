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

		public T GetAndConfirmImportOption<T>(string Module, string STF_Id, string DisplayName, string Option, T Default = default)
		{
			var ret = GetImportOption(Module, STF_Id, Option, Default);
			ConfirmImportOption(Module, STF_Id, DisplayName, Option, ret);
			return ret;
		}

		public T GetImportOption<T>(string Module, string STF_Id, string Option, T Default = default)
		{
			foreach(var opt in ResourceImportOptions)
			{
				try {
					if(opt.Module == Module && opt.STF_Id == STF_Id)
					{
						var settings = JObject.Parse(opt.Json);
						if(settings.ContainsKey(Option))
						{
							return settings.Value<T>(Option);
						}
					}
				}
				catch
				{
					break;
				}
			}
			return Default;
		}
		public void ConfirmImportOption<T>(string Module, string STF_Id, string DisplayName, string Option, T Value)
		{
			try {
				foreach(var opt in ResourceImportOptionsConfirm)
				{
					if(opt.STF_Id == STF_Id)
					{
						opt.Module = Module;
						opt.DisplayName = DisplayName;
						var settings = JObject.Parse(opt.Json);
						settings[Option] = JToken.FromObject(Value);
						opt.Json = settings.ToString();
						return;
					}
				}
				ResourceImportOptionsConfirm.Add(new () { Module = Module, STF_Id = STF_Id, DisplayName = DisplayName, Json = new JObject() {{ Option, JToken.FromObject(Value) }}.ToString()});
			}
			catch
			{
			}
		}
	}
}
