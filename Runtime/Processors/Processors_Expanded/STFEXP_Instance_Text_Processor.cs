using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;
using UnityEngine;
using UnityEngine.UI;

#if STF_TEXTMESHPRO_FOUND
using TMPro;
#endif

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STFEXP_Instance_Text_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Instance_Text);
		public const uint _Order = 100;
		public uint Order => _Order;
		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfTextInstance = STFResource as STFEXP_Instance_Text;
			if(!stfTextInstance.Text) return (null, null);

			var go = new GameObject("STF Canvas");
			go.transform.SetParent(stfTextInstance.transform, false);
			go.transform.Rotate(Vector3.right, -90);
			go.transform.Rotate(Vector3.up, 180);
			var canvas = go.AddComponent<Canvas>();

			var textGo = new GameObject(stfTextInstance.Text.STF_Name ?? "STF Text");
			textGo.transform.SetParent(go.transform, false);

#if STF_TEXTMESHPRO_FOUND
			var text = textGo.AddComponent<TextMeshPro>();
			text.text = stfTextInstance.Text.text;
#else
			var text = textGo.AddComponent<Text>();
			text.text = stfTextInstance.Text.text;
#endif

			canvas.enabled = stfTextInstance.enabled;

			return (new() { canvas }, null);
		}
	}
}
