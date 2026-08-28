#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.resources.stfexp;
using System.Collections.Generic;
using UnityEngine.UIElements;

#if STF_FINALIK_FOUND
using com.squirrelbite.stf_unity.processors.finalik;
#endif

namespace com.squirrelbite.stf_unity.ava.vrchat
{
	public class VRCContext : AVAContext
	{
		public readonly bool physics_separate = true;
		public readonly GameObject physics_parent = null;

		public VRCContext(ProcessorState State) : base(State)
		{
			var options = State.ImportConfig.GetContextImportOptions(DetectorVRC.STF_VRC_AVATAR_CONTEXT);
			this.physics_separate = options.ContainsKey("physics_separate") ? options.Value<bool>("physics_separate") : true;

			if(this.physics_separate)
			{
				this.physics_parent = new GameObject("Physics");
				this.physics_parent.transform.parent = State.Root.transform;
			}
		}
	}

	public class VRCContextFactory : STF_ApplicationContextDefinition
	{
		public string ContextId => DetectorVRC.STF_VRC_AVATAR_CONTEXT;

		public string DisplayName => "VRChat Avatar";

		public ProcessorContextBase Create(ProcessorState State)
		{
			return new VRCContext(State);
		}

		public VisualElement CreateSettingsGUI(ImportOptions Options, System.Action EmitChange) {
			var ret = new VisualElement();
			var option = Options.GetContextImportOptions(this.ContextId);
			var value = option.ContainsKey("physics_separate") ? option.Value<bool>("physics_separate") : true;
			var toggle = new Toggle("Place Physics Under Root") { value = value };
			toggle.RegisterValueChangedCallback(e => {
				var option = Options.GetContextImportOptions(this.ContextId);
				option["physics_separate"] = e.newValue;
				Options.SetContextImportOptions(this.ContextId, option);
				EmitChange();
			});
			ret.Add(toggle);
			return ret;
		}
	}

	[InitializeOnLoad, ExecuteInEditMode]
	public class DetectorVRC
	{
		public const string STF_VRC_AVATAR_CONTEXT = "vrchat_avatar3";

		public static readonly List<System.Type> Ignores = new() { typeof(STFEXP_Collider_Sphere), typeof(STFEXP_Collider_Capsule), typeof(STFEXP_Collider_Plane), typeof(STFEXP_Constraint_Twist), typeof(STFEXP_Constraint_Rotation), typeof(STFEXP_Constraint_Parent), typeof(STFEXP_Constraint_IK), };

		static DetectorVRC()
		{
#if STF_AVA_VRCSDK3_FOUND
			Debug.Log("AVA: Found VRC SDK 3");
			STF_Processor_Registry.RegisterContext(new VRCContextFactory());

			foreach ((var _, var processor) in STF_Processor_Registry.GetProcessors("default"))
				if(!Ignores.Contains(processor.TargetType))
					STF_Processor_Registry.RegisterProcessor(STF_VRC_AVATAR_CONTEXT, processor);
#if STF_FINALIK_FOUND
			STF_Processor_Registry.RegisterProcessor(STF_VRC_AVATAR_CONTEXT, new FinalIK_STFEXP_Constraint_IK_Processor());
#endif
#endif
		}
	}
}

#endif
