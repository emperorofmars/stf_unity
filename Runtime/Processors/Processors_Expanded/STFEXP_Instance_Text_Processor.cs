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

			var textGo = new GameObject("STF Text");
			textGo.transform.SetParent(stfTextInstance.transform, false);
			textGo.transform.Rotate(Vector3.right, -90);
			textGo.transform.Rotate(Vector3.up, 180);

#if STF_TEXTMESHPRO_FOUND
			var text = textGo.AddComponent<TextMeshPro>();
			text.text = stfTextInstance.Text.text;

			var rect = textGo.GetComponent<RectTransform>();
			rect.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
#else
			var text = textGo.AddComponent<Text>();
			text.text = stfTextInstance.Text.text;
#endif

			//canvas.enabled = stfTextInstance.enabled;

			return (new() { textGo }, null);
		}
	}
}
