#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;
using com.squirrelbite.stf_unity.processors.stfexp;
using com.squirrelbite.stf_unity.processors.ava.util;
using com.squirrelbite.stf_unity.processors;

namespace com.squirrelbite.stf_unity.ava.vrchat
{
	public class VRCContextFactory : STF_ApplicationContextDefinition
	{
		public string ContextId => DetectorVRC.STF_VRC_AVATAR_CONTEXT;

		public string DisplayName => "VRChat Avatar";

		public ProcessorContextBase Create(ProcessorState State)
		{
			return new AVAContext(State);
		}
	}

	[InitializeOnLoad, ExecuteInEditMode]
	public class DetectorVRC
	{
		const string STF_AVA_VRCSDK3_FOUND = "STF_AVA_VRCSDK3_FOUND";
		public const string STF_VRC_AVATAR_CONTEXT = "vrchat_avatar3";

		static DetectorVRC()
		{
			//if(AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.StartsWith("VRC.SDK3.Avatars")))
			if (Directory.GetFiles(Path.GetDirectoryName(Application.dataPath), "VRCAvatarDescriptorEditor3.cs", SearchOption.AllDirectories).Length > 0)
			{
				Debug.Log("AVA: Found VRC SDK 3");
				if (!ScriptDefinesManager.IsDefined(STF_AVA_VRCSDK3_FOUND))
				{
					ScriptDefinesManager.AddDefinesIfMissing(BuildTargetGroup.Standalone, STF_AVA_VRCSDK3_FOUND);
					CompilationPipeline.RequestScriptCompilation();
				}
				else
				{
					STF_Processor_Registry.RegisterContext(new VRCContextFactory());

					foreach (var processor in STF_Processor_Registry.DefaultProcessors["default"])
						STF_Processor_Registry.RegisterProcessor(STF_VRC_AVATAR_CONTEXT, processor);

					STF_Processor_Registry.RegisterProcessor(STF_VRC_AVATAR_CONTEXT, new STFEXP_Humanoid_Armature_Processor());
					STF_Processor_Registry.RegisterProcessor(STF_VRC_AVATAR_CONTEXT, new STFEXP_Constraint_Twist_Processor());
				}
			}
			else
			{
				Debug.Log("AVA: Didn't find VRC SDK 3");
				if (ScriptDefinesManager.IsDefined(STF_AVA_VRCSDK3_FOUND))
				{
					ScriptDefinesManager.RemoveDefines(STF_AVA_VRCSDK3_FOUND);
					CompilationPipeline.RequestScriptCompilation();
				}
			}
		}
	}
}

#endif
