
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.squirrelbite.stf_unity.modules;
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

		public void AddUnityObject(ISTF_Resource STFResource, Object UnityObject)
		{
			STFResource.ProcessedObjects.Add(UnityObject);
			State.State.AddUnityObject(UnityObject);
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
					var results = processor.Process(this, resource);
					if (results != null) resource.ProcessedObjects.AddRange(results);
					State.RegisterResult(results);
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
	}
}
