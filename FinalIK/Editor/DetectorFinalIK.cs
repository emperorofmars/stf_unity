#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;
using com.squirrelbite.stf_unity.util;


namespace com.squirrelbite.stf_unity.processors.finalik
{
	[InitializeOnLoad, ExecuteInEditMode]
	public class DetectorFinalIK
	{
		const string STF_FINALIK_FOUND = "STF_FINALIK_FOUND";

		static DetectorFinalIK()
		{
			var finalIKPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Assets/Plugins/RootMotion");
			if(Directory.Exists(finalIKPath) && Directory.GetFiles(finalIKPath, "ArmIK.cs", SearchOption.AllDirectories).Length > 0)
			{
				Debug.Log("STF: Found FinalIK");
				if (!ScriptDefinesManager.IsDefined(STF_FINALIK_FOUND))
				{
					ScriptDefinesManager.AddDefinesIfMissing(BuildTargetGroup.Standalone, STF_FINALIK_FOUND);
					CompilationPipeline.RequestScriptCompilation();
				}

				STF_Processor_Registry.RegisterProcessor("default", new FinalIK_STFEXP_Constraint_IK_Processor());
			}
			else
			{
				if (ScriptDefinesManager.IsDefined(STF_FINALIK_FOUND))
				{
					ScriptDefinesManager.RemoveDefines(STF_FINALIK_FOUND);
					CompilationPipeline.RequestScriptCompilation();
				}
			}
		}
	}
}

#endif
