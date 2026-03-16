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
using com.squirrelbite.stf_unity.squirrelbite;
using System.Linq;
using UnityEditor.Animations;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_Squirrelbite_AvatarSetup_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(Squirrelbite_AvatarSetup);

		public const uint _Order = STF_Animation_Processor._Order + 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avatarSetup = STFResource as Squirrelbite_AvatarSetup;
			var baseSetup = InitAvatarBaseSetupVRChat.Init(Context.Root.GetComponent<VRCAvatarDescriptor>());

			var controlsGo = new GameObject("Controls");
			controlsGo.transform.SetParent(baseSetup.transform);

			foreach(var toggle in avatarSetup.TogglesPre)
			{
				var behaviour = controlsGo.AddComponent<AnimationToggleVRC>();
				behaviour.Name = toggle.Name;
				behaviour.IsOverridable = true;
				if(toggle.On && toggle.On.ProcessedObjects.Find(o => o is AnimationClip) is AnimationClip clipOn)
					behaviour.On = clipOn;
				if(toggle.Off && toggle.Off.ProcessedObjects.Find(o => o is AnimationClip) is AnimationClip clipOff)
					behaviour.On = clipOff;
			}

			foreach(var puppet in avatarSetup.PersistentPuppetsPre)
			{
				var behaviour = controlsGo.AddComponent<PuppetVRC>();
				behaviour.Name = puppet.Name;
				behaviour.IsOverridable = true;
				behaviour.IsPersistent = true;
				if(puppet.Blendtree && puppet.Blendtree.ProcessedObjects.Find(o => o is BlendTree) is BlendTree blendtree)
				{
					foreach(var mapping in blendtree.children)
					{
						behaviour.Blendtree.Add(new () { Animation = (AnimationClip)mapping.motion, Position = mapping.position });
					}
				}
			}

			foreach(var toggle in avatarSetup.Toggles)
			{
				var behaviour = controlsGo.AddComponent<AnimationToggleVRC>();
				behaviour.Name = toggle.Name;
				behaviour.IsOverridable = false;
				if(toggle.On && toggle.On.ProcessedObjects.Find(o => o is AnimationClip) is AnimationClip clipOn)
					behaviour.On = clipOn;
				if(toggle.Off && toggle.Off.ProcessedObjects.Find(o => o is AnimationClip) is AnimationClip clipOff)
					behaviour.On = clipOff;
			}

			foreach(var puppet in avatarSetup.Puppets)
			{
				var behaviour = controlsGo.AddComponent<PuppetVRC>();
				behaviour.Name = puppet.Name;
				behaviour.IsOverridable = false;
				behaviour.IsPersistent = false;
				if(puppet.Blendtree && puppet.Blendtree.ProcessedObjects.Find(o => o is BlendTree) is BlendTree blendtree)
				{
					foreach(var mapping in blendtree.children)
					{
						behaviour.Blendtree.Add(new () { Animation = (AnimationClip)mapping.motion, Position = mapping.position });
					}
				}
			}

			return (new() { baseSetup }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_Squirrelbite_AvatarSetup_Processor
	{
		static Register_VRC_Squirrelbite_AvatarSetup_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_Squirrelbite_AvatarSetup_Processor());
		}
	}
}

#endif
#endif
#endif
