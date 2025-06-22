using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STFEXP_LightprobeAnchor_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(STFEXP_LightprobeAnchor);

		public uint Order => STF_Instance_Mesh_Processor._Order + 10;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfAnchor = STFResource as STFEXP_LightprobeAnchor;
			foreach (var renderer in stfAnchor.GetComponents<Renderer>())
			{
				renderer.probeAnchor = stfAnchor.TargetGo?.transform;
			}
			return (null, null);
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	public class Register_STFEXP_LightprobeAnchor_Processor
	{
		static Register_STFEXP_LightprobeAnchor_Processor()
		{
			STF_Processor_Registry.RegisterProcessor("default", new STFEXP_LightprobeAnchor_Processor());
		}
	}
#endif
}
