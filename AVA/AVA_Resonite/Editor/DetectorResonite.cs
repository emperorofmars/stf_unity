#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using com.squirrelbite.stf_unity.processors;
using System.Collections.Generic;
using System.IO;
using com.squirrelbite.stf_unity.util;
using UnityEditor.Compilation;

namespace com.squirrelbite.stf_unity.ava.resonite
{
	public class ResoniteContextFactory : STF_ApplicationContextDefinition
	{
		public string ContextId => DetectorResonite.STF_RESONITE_AVATAR_CONTEXT;

		public string DisplayName => "Resonite Avatar";

		public ProcessorContextBase Create(ProcessorState State)
		{
			return new AVAContext(State);
		}
	}

	[InitializeOnLoad, ExecuteInEditMode]
	public class DetectorResonite
	{
		const string STF_AVA_RESONITE_FOUND = "STF_AVA_RESONITE_FOUND";
		public const string STF_RESONITE_AVATAR_CONTEXT = "resonite_avatar";

		public static readonly List<System.Type> Ignores = new() {};

		static DetectorResonite()
		{
			if(Directory.GetFiles(Path.GetDirectoryName(Application.dataPath), "BipedAvatarDescriptor.cs", SearchOption.AllDirectories).Length > 0)
			{
				Debug.Log("AVA: Found Resonite SDK");
				if (!ScriptDefinesManager.IsDefined(STF_AVA_RESONITE_FOUND))
				{
					ScriptDefinesManager.AddDefinesIfMissing(BuildTargetGroup.Standalone, STF_AVA_RESONITE_FOUND);
					CompilationPipeline.RequestScriptCompilation();
				}
				else
				{
					STF_Processor_Registry.RegisterContext(new ResoniteContextFactory());

					foreach ((var _, var processor) in STF_Processor_Registry.GetProcessors("default"))
						if(!Ignores.Contains(processor.TargetType))
							STF_Processor_Registry.RegisterProcessor(STF_RESONITE_AVATAR_CONTEXT, processor);
				}
			}
			else
			{
				if (ScriptDefinesManager.IsDefined(STF_AVA_RESONITE_FOUND))
				{
					ScriptDefinesManager.RemoveDefines(STF_AVA_RESONITE_FOUND);
					CompilationPipeline.RequestScriptCompilation();
				}
			}
		}
	}
}

#endif
