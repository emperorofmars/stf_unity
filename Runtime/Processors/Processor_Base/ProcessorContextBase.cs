
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stf_material;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class ProcessorContextBase
	{
		protected readonly ProcessorState State;

		public ProcessorContextBase(ProcessorState State)
		{
			this.State = State;
		}

		public ImportOptions ImportConfig => State.State.ImportConfig;

		public void RegisterProcessedResult(ISTF_Resource STFResource, object Result, bool AddAsUnityObject = true)
		{
			STFResource.ProcessedObjects.Add(Result);

			// Also register to all excluded components
			if(STFResource is ISTF_ComponentResource componentResource && !string.IsNullOrWhiteSpace(componentResource.ExclusionGroup))
				foreach((var k, var components) in State.ExclusionGroups[componentResource.ExclusionGroup])
					if(!State.GroupWinners[componentResource.ExclusionGroup].Contains(State.GetProcessor(k)))
						foreach(var c in components)
							c.ProcessedObjects.Add(Result);

			if(AddAsUnityObject && Result is UnityEngine.Object @object)
				State.RegisterResult(new List<Object> {@object});
		}

		public List<ISTF_Resource> GetResourceByType(System.Type Type)
		{
			return State.GetResourceByType(Type);
		}

		public AssetInfo GetMeta()
		{
			return State.State.Meta.STFAssetInfo;
		}
		public string GetMetaCustomValue(string Key)
		{
			return State.State.Meta.STFAssetInfo.CustomProperties.FirstOrDefault(e => e.Name == Key)?.Value;
		}

		public void Report(STFReport Report)
		{
			State.Report(Report);
		}

		public GameObject Root => State.Root;

		private Material DefaultMaterial = null;

		public void AddTask(Task Task) { State.Tasks.Add(Task); }
		public void AddTrash(Object Trash) { State.AddTrash(Trash); }
		public void AddTrash(IEnumerable<Object> Trash) { State.AddTrash(Trash); }

		protected virtual void Execute()
		{
		}

		public void Run()
		{
			// Create processing tasks
			foreach (var resource in State.ResourcesToProcess)
			{
				var processor = State.GetProcessor(resource);
				State.AddProcessorTask(processor.Order, new Task(() =>
				{
					(var ProcessedObjects, var ObjectsToRegister) = processor.Process(this, resource);
					if (ProcessedObjects != null && ProcessedObjects.Count > 0)
					{
						resource.ProcessedObjects.AddRange(ProcessedObjects);

						// Also register to all excluded components
						if(resource is ISTF_ComponentResource componentResource && !string.IsNullOrWhiteSpace(componentResource.ExclusionGroup))
							foreach((var k, var components) in State.ExclusionGroups[componentResource.ExclusionGroup])
								if(!State.GroupWinners[componentResource.ExclusionGroup].Contains(State.GetProcessor(k)))
									foreach(var c in components)
										c.ProcessedObjects.AddRange(ProcessedObjects);
					}
					State.RegisterResult(ObjectsToRegister);
				}));
			}
			foreach ((var type, var globalProcessor) in State.GlobalProcessors)
			{
				State.AddProcessorTask(globalProcessor.Order, new Task(() =>
				{
					var results = globalProcessor.Process(this);
					State.RegisterResult(results);
				}));
			}

			//Execute processor tasks in their defined order
			foreach (var (order, taskList) in State.ProcessOrderMap.OrderBy(e => e.Key))
			{
				foreach (var task in taskList)
				{
					task.RunSynchronously();
					if (task.Exception != null)
					{
						HandleTaskException(task.Exception);
					}
				}
			}

			Execute();

			// Run any Tasks added to the State during the processor execution
			var maxDepth = 100;
			while (State.Tasks.Count > 0)
			{
				var taskset = State.Tasks;
				State.Tasks = new List<Task>();
				foreach (var task in taskset)
				{
					task.RunSynchronously();
					if (task.Exception != null)
					{
						HandleTaskException(task.Exception);
					}
				}

				maxDepth--;
				if (maxDepth <= 0)
				{
					Debug.LogWarning("Maximum recursion depth reached!");
					break;
				}
			}
		}

		private void HandleTaskException(System.AggregateException Exception)
		{
			foreach(var e in Exception.InnerExceptions)
				if(e is STFException nnaError)
					State.Report(nnaError.Report);
				else
					State.Report(new STFReport(e.Message, ErrorSeverity.FATAL_ERROR, null, null, e));
		}

		public Material GetDefaultMaterial()
		{
			if(!DefaultMaterial)
			{
				DefaultMaterial = new Material(Shader.Find(STF_Material_Converter_Registry.DefaultShader)) {
					name = "Default"
				};
				State.RegisterResult(new () {DefaultMaterial});
			}
			return DefaultMaterial;
		}
	}
}
