#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND
#if AVA_BASE_SETUP_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.resources;
using UnityEngine;
using com.squirrelbite.ava_base_setup.vrchat;
using com.squirrelbite.ava_base_setup;
using VRC.SDK3.Avatars.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_AVA_Expressions_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(AVA_Expressions);

		public const uint _Order = STF_Animation_Processor._Order + 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avaExpressions = STFResource as AVA_Expressions;
			var baseSetup = InitAvatarBaseSetupVRChat.Init(Context.Root.GetComponent<VRCAvatarDescriptor>());

			var expressionsGo = new GameObject("Expressions");
			expressionsGo.transform.SetParent(baseSetup.transform);

			var expressionsSetup = expressionsGo.AddComponent<AVAExpressionsVRC>();
			var bindingsSetup = expressionsGo.AddComponent<AVAExpressionBindingsProducerVRC>();

			baseSetup.LayerManualExpressions.Add(new() { ProducerComponent = bindingsSetup });

			foreach (var expression in avaExpressions.expressions)
			{
				if (expression.animation.ProcessedObjects.Count == 1 && expression.animation.ProcessedObjects[0] is AnimationClip animationClip)
				{
					expressionsSetup.Expressions.Add(new AvatarExpressionOld() {
						Expression = expression.meaning,
						Animation = animationClip,
					});
				}
			}
			bindingsSetup.InitBindings();
			while(UnityEditorInternal.ComponentUtility.MoveComponentUp(bindingsSetup));

			return (new() { baseSetup, expressionsSetup, bindingsSetup }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_AVA_Expressions_Processor
	{
		static Register_VRC_AVA_Expressions_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_AVA_Expressions_Processor());
		}
	}
}

#endif
#endif
#endif
