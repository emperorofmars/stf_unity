
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class ProcessorContextBase
	{
		protected readonly ProcessorState State;

		public ProcessorContextBase(ProcessorState State)
		{
			this.State = State;
			Run();
		}

		public ImportOptions ImportConfig => State.State.ImportConfig;

		public void AddUnityObject(ISTF_Resource STFResource, Object UnityObject)
		{
			STFResource.ProcessedObjects.Add(UnityObject);
			State.State.AddUnityObject(UnityObject);
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

		public void AddTask(Task Task) { State.Tasks.Add(Task); }
		public void AddTrash(Object Trash) { State.AddTrash(Trash); }
		public void AddTrash(IEnumerable<Object> Trash) { State.AddTrash(Trash); }

		protected virtual void Execute()
		{
		}

		private void Run()
		{
			var resources = new HashSet<ISTF_Resource>();

			// Find all processable resources
			foreach ((var stfId, var objectToRegister) in State.State.ImportedObjects)
			{
				if (objectToRegister is ISTF_Resource resource && !resources.Contains(resource) && State.GetProcessor(resource) is var processor && processor != null)
					resources.Add(resource);
				if (objectToRegister is STF_PrefabResource go)
					foreach (var resourceOnObject in go.GetComponentsInChildren<ISTF_Resource>())
						if (!resources.Contains(resourceOnObject) && State.GetProcessor(resourceOnObject) is var processorOnObject && processorOnObject != null)
							resources.Add(resourceOnObject);
			}

			// Figure out overrides
			var overrides = new HashSet<string>();
			foreach (var resource in resources)
			{
				if (resource is STF_DataComponentResource dataComponent)
					foreach (var o in dataComponent.Overrides)
						if (!overrides.Contains(o)) overrides.Add(o);
				if (resource is STF_NodeComponentResource nodeComponent)
					foreach (var o in nodeComponent.Overrides)
						if (!overrides.Contains(o)) overrides.Add(o);
			}

			// Create processing tasks
			foreach (var resource in resources)
			{
				if (!overrides.Contains(resource.STF_Id))
				{
					var processor = State.GetProcessor(resource);
					State.AddProcessorTask(processor.Order, new Task(() =>
					{
						var results = processor.Process(this, resource);
						if (results != null) resource.ProcessedObjects.AddRange(results);
						State.RegisterResult(results);
					}));
				}
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
	}
}
