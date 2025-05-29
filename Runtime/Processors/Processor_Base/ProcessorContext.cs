
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class ProcessorContext
	{
		public ProcessorState State;

		public ProcessorContext(ProcessorState State)
		{
			this.State = State;
			Run();
		}

		public ImportOptions ImportConfig => State.State.ImportConfig;

		public void Report(STFReport Report)
		{
			State.Report(Report);
		}

		public GameObject Root => State.Root;

		public void AddTask(Task Task) { State.Tasks.Add(Task); }
		public void AddTrash(Transform Trash) { State.Trash.Add(Trash); }
		public void AddTrash(IEnumerable<Transform> Trash) { State.Trash.AddRange(Trash); }

		private void Run()
		{
			var registeredResources = new HashSet<ISTF_Resource>();
			foreach (var objectToRegister in State.State.ObjectToRegister)
			{
				if (objectToRegister is ISTF_Resource resource && !registeredResources.Contains(resource) && State.GetProcessor(resource) is var processor && processor != null)
				{
					registeredResources.Add(resource);
					State.AddProcessorTask(processor.Order, new Task(() =>
					{
						var results = processor.Process(this, resource);
						if (results != null) resource.ProcessedObjects.AddRange(results);
						State.RegisterResult(results);
					}));
				}
				else if (objectToRegister is GameObject go)
				{
					foreach (var resourceOnObject in go.GetComponentsInChildren<ISTF_Resource>())
					{
						if (!registeredResources.Contains(resourceOnObject) && State.GetProcessor(resourceOnObject) is var processorOnObject && processorOnObject != null)
						{
							State.AddProcessorTask(processorOnObject.Order, new Task(() =>
							{
								var results = processorOnObject.Process(this, resourceOnObject);
								if (results != null) resourceOnObject.ProcessedObjects.AddRange(results);
								State.RegisterResult(results);
							}));
						}
					}
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
