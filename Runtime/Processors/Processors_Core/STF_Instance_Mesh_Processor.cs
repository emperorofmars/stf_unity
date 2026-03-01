using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Instance_Mesh_Converter : ISTF_PropertyConverter
	{
		public ImportPropertyPathPart ConvertPropertyPath(ISTF_Resource STFResource, List<string> STFPath)
		{
			var meshInstance = STFResource as STF_Instance_Mesh;
			var convert = new System.Func<List<float>, List<float>>(Values =>
			{
				Values[0] *= 100;
				return Values;
			});

			if (STFPath.Count == 3 && STFPath[0] == "blendshape" && STFPath[2] == "value")
			{
				return new ImportPropertyPathPart(typeof(SkinnedMeshRenderer), new() { "blendShape." + STFPath[1] }, convert);
			}

			if(STFPath.Count > 2 && STFPath[0] == "material" && int.TryParse(STFPath[1], out int materialIndex) && meshInstance.Materials.Count > materialIndex)
			{
				var material = meshInstance.Materials[materialIndex] ? meshInstance.Materials[materialIndex] : meshInstance.Mesh.material_slots.Count > materialIndex ? meshInstance.Mesh.material_slots[materialIndex] : null;
				if(material)
				{
					return ImportPropertyPathPart.FromTargetType(typeof(SkinnedMeshRenderer)) + material.PropertyConverter.ConvertPropertyPath(material, STFPath.GetRange(2, STFPath.Count - 2));
				}
			}
			return null;
		}
	}

	public class STF_Instance_Mesh_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Instance_Mesh);
		public const uint _Order = 100;
		public uint Order => _Order;
		public int Priority => 1;

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var meshInstance = STFResource as STF_Instance_Mesh;
			meshInstance.PropertyConverter = new STF_Instance_Mesh_Converter();

			var processedUnityMesh = (Mesh)(meshInstance.Mesh.ProcessedObjects != null && meshInstance.Mesh.ProcessedObjects.Count == 1 ? meshInstance.Mesh.ProcessedObjects[0] : null);
			if (processedUnityMesh != null)
			{
				if(meshInstance.Mesh.weights != null && meshInstance.transform.parent && meshInstance.transform.parent.parent)
				{
					var reparentSkinned = Context.ImportConfig.GetAndConfirmImportOption(STF_Animation.STF_TYPE, meshInstance.STF_Id, meshInstance.STF_Name, "reparent_skinned", true);
					if(reparentSkinned)
					{
						meshInstance.transform.SetParent(meshInstance.transform.parent.parent, true);
					}
				}

				Renderer renderer;
				if (meshInstance.Mesh.weights != null  || meshInstance.Mesh.blendshapes != null && meshInstance.Mesh.blendshapes.Count > 0)
				{
					var smr = meshInstance.gameObject.AddComponent<SkinnedMeshRenderer>();
					renderer = smr;
					smr.sharedMesh = processedUnityMesh;
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
						smr.SetBlendShapeWeight(blenshapeIdx, meshInstance.Mesh.blendshapes[blenshapeIdx].default_value * 100);
						if (meshInstance.BlendshapeValues.Count > blenshapeIdx && meshInstance.BlendshapeValues[blenshapeIdx].Override) smr.SetBlendShapeWeight(blenshapeIdx, meshInstance.BlendshapeValues[blenshapeIdx].Value * 100);
					}
				}
				else
				{
					var meshFilter = meshInstance.gameObject.AddComponent<MeshFilter>();
					meshFilter.sharedMesh = processedUnityMesh;
					renderer = meshInstance.gameObject.AddComponent<MeshRenderer>();
				}

				meshInstance.ProcessedObjects.Add(renderer);

				var materialCount = System.Math.Max(meshInstance.Mesh.material_slots.Count, renderer.sharedMaterials.Length);
				var rendererMaterials = new Material[materialCount];
				for (int matIdx = 0; matIdx < materialCount; matIdx++)
				{
					if (matIdx < meshInstance.Materials.Count && meshInstance.Materials[matIdx] != null)
						rendererMaterials[matIdx] = STFUtil.GetProcessed<Material>(meshInstance.Materials[matIdx]);
					else if (matIdx < meshInstance.Mesh.material_slots.Count && meshInstance.Mesh.material_slots[matIdx] != null)
						rendererMaterials[matIdx] = STFUtil.GetProcessed<Material>(meshInstance.Mesh.material_slots[matIdx]);
					else
						rendererMaterials[matIdx] = Context.GetDefaultMaterial();
				}
				if(rendererMaterials.Length > 0) renderer.sharedMaterials = rendererMaterials;
				return (new List<Object>() { renderer }, null);
			}
			return (null, null);
		}
	}
}
