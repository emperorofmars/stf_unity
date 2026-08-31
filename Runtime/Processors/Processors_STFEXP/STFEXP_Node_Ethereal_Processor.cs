using System.Collections.Generic;
using System.Threading.Tasks;
using com.squirrelbite.stf_unity.resources;
using com.squirrelbite.stf_unity.resources.stfexp;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STFEXP_Node_Ethereal_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Node_Ethereal);

		public uint Order => 1000000000;

		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfNodeEthereal = STFResource as STFEXP_Node_Ethereal;

			var preserveEthereal = Context.ImportConfig.GetAndConfirmImportOption(STFEXP_Node_Ethereal._STF_Type, stfNodeEthereal.STF_Id, UnityUtil.getPath(Context.Root.transform, stfNodeEthereal.transform), "preserve", false);

			if(!Context.ImportConfig.AuthoringImport && !stfNodeEthereal.Preserve && !preserveEthereal)
			{
				Context.AddTask(new Task(() => {
					Context.AddTrash(stfNodeEthereal.gameObject);
				}));
			}

			return (null, null);
		}

		public string SettingsKey => STFEXP_Node_Ethereal._STF_Type;
		public bool HasAdvancedSettings => true;

		public VisualElement CreateAdvancedSettingsGUI(ImportOptions.ResourceImportOption Option, System.Action EmitChange)
		{
			var ret = new VisualElement();
			var options = JObject.Parse(Option.Json);

			if(options.ContainsKey("preserve") && options.Value<bool>("preserve") is bool preserveEthereal)
			{
				var togglePreserveEthereal = new Toggle("Preserve ethereal node") { value = preserveEthereal };
				togglePreserveEthereal.RegisterValueChangedCallback(e => {
					var options = JObject.Parse(Option.Json);
					options["preserve"] = e.newValue;
					Option.Json = options.ToString();
					EmitChange();
				});
				ret.Add(togglePreserveEthereal);
			}

			return ret;
		}
	}
}
