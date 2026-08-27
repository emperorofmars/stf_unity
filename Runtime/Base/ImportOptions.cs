using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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
		public class ContextImportOption
		{
			public string ContextID;
			public string Json;
		}
		public List<ContextImportOption> ContextImportOptions = new();

		public JObject GetContextImportOptions(string ContextID)
		{
			foreach(var opt in ContextImportOptions)
			{
				try {
					if(opt.ContextID == ContextID) return JObject.Parse(opt.Json);
				}
				catch
				{
					break;
				}
			}
			return new JObject();
		}

		public void SetContextImportOptions(string ContextID, JObject Options)
		{
			foreach(var opt in ContextImportOptions)
			{
				if(opt.ContextID == ContextID)
				{
					opt.Json = Options.ToString();
					return;
				}
			}
			ContextImportOptions.Add(new () { ContextID = ContextID, Json = Options.ToString() });
		}


		/*[System.Serializable]
		public class HandlerImportOption
		{
			public string STF_Type;
			public string Json;
		}
		public List<HandlerImportOption> HandlerImportOptions = new();

		public JObject GetHandlerImportOptions(string STF_Type)
		{
			foreach(var opt in HandlerImportOptions)
			{
				try {
					if(opt.STF_Type == STF_Type) return JObject.Parse(opt.Json);
				}
				catch
				{
					break;
				}
			}
			return new JObject();
		}

		public void SetHandlerImportOptions(string STF_Type, JObject Options)
		{
			foreach(var opt in HandlerImportOptions)
			{
				if(opt.STF_Type == STF_Type)
				{
					opt.Json = Options.ToString();
					return;
				}
			}
			HandlerImportOptions.Add(new () { STF_Type = STF_Type, Json = Options.ToString() });
		}*/

		[System.Serializable]
		public class ResourceImportOption
		{
			public string STF_Type;
			public string STF_Id;
			public string DisplayName;
			public string Json;
		}
		public List<ResourceImportOption> ResourceImportOptions = new();
		public List<ResourceImportOption> ResourceImportOptionsConfirm = null;

		public T GetAndConfirmImportOption<T>(string STF_Type, string STF_Id, string DisplayName, string Option, T Default = default)
		{
			var ret = GetImportOption(STF_Type, STF_Id, Option, Default);
			ConfirmImportOption(STF_Type, STF_Id, DisplayName, Option, ret);
			return ret;
		}

		public T GetImportOption<T>(string STF_Type, string STF_Id, string Option, T Default = default)
		{
			foreach(var opt in ResourceImportOptions)
			{
				try {
					if(opt.STF_Type == STF_Type && opt.STF_Id == STF_Id)
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
		public void ConfirmImportOption<T>(string STF_Type, string STF_Id, string DisplayName, string Option, T Value)
		{
			try {
				foreach(var opt in ResourceImportOptionsConfirm)
				{
					if(opt.STF_Id == STF_Id)
					{
						opt.STF_Type = STF_Type;
						opt.DisplayName = DisplayName;
						var settings = JObject.Parse(opt.Json);
						settings[Option] = JToken.FromObject(Value);
						opt.Json = settings.ToString();
						return;
					}
				}
				ResourceImportOptionsConfirm.Add(new () { STF_Type = STF_Type, STF_Id = STF_Id, DisplayName = DisplayName, Json = new JObject() {{ Option, JToken.FromObject(Value) }}.ToString()});
			}
			catch
			{
			}
		}
	}
}
