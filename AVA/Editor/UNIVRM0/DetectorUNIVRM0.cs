#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;
using com.squirrelbite.stf_unity.processors.stfexp;
using com.squirrelbite.stf_unity.processors.ava.util;

namespace com.squirrelbite.stf_unity.ava.univrm0
{
	[InitializeOnLoad, ExecuteInEditMode]
	public class DetectorUNIVRM0
	{
		const string STF_AVA_UNIVRM0_FOUND = "STF_AVA_UNIVRM0_FOUND";
		public const string STF_UNIVRM0_AVATAR_CONTEXT = "univrm0";

		static DetectorUNIVRM0()
		{
			//if(AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.StartsWith("VRC.SDK3.Avatars")))
			if(Directory.GetFiles(Path.GetDirectoryName(Application.dataPath), "IVRMComponent.cs", SearchOption.AllDirectories).Length > 0)
			{
				Debug.Log("AVA: Found UNIVRM0 SDK");
				if (!ScriptDefinesManager.IsDefined(STF_AVA_UNIVRM0_FOUND))
				{
					ScriptDefinesManager.AddDefinesIfMissing(BuildTargetGroup.Standalone, STF_AVA_UNIVRM0_FOUND);
					CompilationPipeline.RequestScriptCompilation();
				}
				else
				{
					STF_Processor_Registry.RegisterContextFactory(STF_UNIVRM0_AVATAR_CONTEXT, new AVAContextFactory());

					STF_Processor_Registry.ContextDisplayNames.Add(STF_UNIVRM0_AVATAR_CONTEXT, "VRM 0 Avatar");
					foreach (var processor in STF_Processor_Registry.DefaultProcessors["default"])
						STF_Processor_Registry.RegisterProcessor(STF_UNIVRM0_AVATAR_CONTEXT, processor);

					STF_Processor_Registry.RegisterProcessor(STF_UNIVRM0_AVATAR_CONTEXT, new STFEXP_Humanoid_Armature_Processor());
					STF_Processor_Registry.RegisterProcessor(STF_UNIVRM0_AVATAR_CONTEXT, new STFEXP_Constraint_Twist_Processor());
				}
			}
			else
			{
				Debug.Log("AVA: Didn't find UNIVRM0 SDK");
				if (ScriptDefinesManager.IsDefined(STF_UNIVRM0_AVATAR_CONTEXT))
				{
					ScriptDefinesManager.RemoveDefines(STF_UNIVRM0_AVATAR_CONTEXT);
					CompilationPipeline.RequestScriptCompilation();
				}
			}
		}
	}
}

#endif