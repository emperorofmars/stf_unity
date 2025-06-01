
using System;
using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Instance_Mesh_Converter : ISTF_PropertyConverter
	{
		public (string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(ISTF_Resource STFResource, List<string> STFPath)
		{
			var convert = new System.Func<List<float>, List<float>>(Values =>
			{
				Values[0] *= 100;
				return Values;
			});

			if (STFPath.Count == 3 && STFPath[0] == "blendshape" && STFPath[2] == "value")
			{
				return ("", typeof(SkinnedMeshRenderer), new() { "blendShape." + STFPath[1] }, convert);
			}
			else return ("", null, null, null);
		}
	}

	public class STF_Instance_Mesh_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(STF_Instance_Mesh);
		public uint Order => 100;
		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContext Context, ISTF_Resource STFResource)
		{
			var meshInstance = STFResource as STF_Instance_Mesh;
			meshInstance.PropertyConverter = new STF_Instance_Mesh_Converter();
			
			var processedUnityMesh = (Mesh)(meshInstance.Mesh.ProcessedObjects != null && meshInstance.Mesh.ProcessedObjects.Count == 1 ? meshInstance.Mesh.ProcessedObjects[0] : null);
			if (processedUnityMesh != null)
			{
				Renderer renderer;
				if (meshInstance.Mesh.weights != null && meshInstance.Mesh.weights.Count > 0 || meshInstance.Mesh.blendshapes != null && meshInstance.Mesh.blendshapes.Count > 0)
				{
					var smr = meshInstance.gameObject.AddComponent<SkinnedMeshRenderer>();
					renderer = smr;
					smr.sharedMesh = processedUnityMesh;
					smr.materials = new Material[meshInstance.Mesh.material_slots.Count];
					if (meshInstance.ArmatureInstance)
					{
						var instance = meshInstance.ArmatureInstance.GetComponent<STF_Instance_Armature>();
						smr.rootBone = meshInstance.ArmatureInstance.transform;
						var bones = new Transform[meshInstance.Mesh.bones.Count];
						for (int i = 0; i < meshInstance.Mesh.bones.Count; i++)
						{
							var id = meshInstance.Mesh.bones[i];
							var bone = meshInstance.ArmatureInstance.GetComponentsInChildren<STF_NodeResource>().FirstOrDefault(bone => bone.STF_Id == id && bone.STF_Owner == instance);
							bones[i] = bone ? bone.transform : instance.transform;
						}
						smr.bones = bones;
					}

					for (int blenshapeIdx = 0; blenshapeIdx < meshInstance.Mesh.blendshapes.Count; blenshapeIdx++)
					{
						smr.SetBlendShapeWeight(blenshapeIdx, meshInstance.Mesh.blendshapes[blenshapeIdx].default_value);
						if (meshInstance.BlendshapeValues[blenshapeIdx].Item1) smr.SetBlendShapeWeight(blenshapeIdx, meshInstance.BlendshapeValues[blenshapeIdx].Item2);
					}
				}
				else
				{
					var meshFilter = meshInstance.gameObject.AddComponent<MeshFilter>();
					meshFilter.sharedMesh = processedUnityMesh;
					renderer = meshInstance.gameObject.AddComponent<MeshRenderer>();
					renderer.materials = new Material[meshInstance.Mesh.material_slots.Count];
				}

				meshInstance.ProcessedObjects.Add(renderer);

				var rendererMaterials = new Material[Math.Max(meshInstance.Mesh.material_slots.Count, meshInstance.Materials.Count)];
				for (int matIdx = 0; matIdx < rendererMaterials.Length; matIdx++)
				{
					if (matIdx < meshInstance.Materials.Count && meshInstance.Materials[matIdx] != null)
					{
						rendererMaterials[matIdx] = STFUtil.GetProcessed<Material>(meshInstance.Materials[matIdx]);
					}
					else if (matIdx < meshInstance.Mesh.material_slots.Count && meshInstance.Mesh.material_slots[matIdx] != null)
					{
						rendererMaterials[matIdx] = STFUtil.GetProcessed<Material>(meshInstance.Mesh.material_slots[matIdx]);
					}
				}
				renderer.materials = rendererMaterials;
				return new List<UnityEngine.Object>() { renderer };
			}
			return null;
		}
	}
}