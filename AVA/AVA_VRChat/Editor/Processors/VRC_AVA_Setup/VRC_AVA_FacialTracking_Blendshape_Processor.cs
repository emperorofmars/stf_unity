#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND
#if AVA_BASE_SETUP_FOUND

using UnityEngine;
using VRC.SDK3.Avatars.Components;
using UnityEditor;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using System.Linq;
using com.squirrelbite.ava_base_setup.vrchat;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_AVA_FacialTracking_Blendshape_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(AVA_FacialTracking_Blendshape);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avaFT = STFResource as AVA_FacialTracking_Blendshape;

			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
			if (!avatar) Context.Report(new STFReport("No Avatar Component created!", ErrorSeverity.FATAL_ERROR, AVA_Eyelids_Blendshape._STF_Type));

			var baseSetup = Context.Root.GetComponent<AVABaseSetupVRC>();
			if(!baseSetup)
				baseSetup = Context.Root.AddComponent<AVABaseSetupVRC>();

			var FTSetup = Context.Root.AddComponent<AVASetupVRCFTController>();

			FTSetup.FTMesh = (Context as AVAContext).PrimaryMeshInstance.ProcessedObjects.Find(po => po is SkinnedMeshRenderer) as SkinnedMeshRenderer;
			FTSetup.Type = avaFT.ft_type switch
			{
				"unified_expressions" => FT_Type.UnifiedExpressions,
				"arkit" => FT_Type.ARKit,
				"sranipal" => FT_Type.SRanipal,
				_ => FT_Type.Automatic,
			};
			baseSetup.LayerFT.Add(new () { ProducerComponent = FTSetup });

			var stfMeshInstance = (Context as AVAContext).PrimaryMeshInstance;

			return (new() { baseSetup, FTSetup }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_AVA_FacialTracking_Blendshape_Processor
	{
		static Register_VRC_AVA_FacialTracking_Blendshape_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_AVA_FacialTracking_Blendshape_Processor());
		}
	}
}

#endif
#endif
#endif
