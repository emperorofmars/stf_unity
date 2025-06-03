using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class ProcessorState
	{
		public readonly ImportState State;
		public readonly Dictionary<System.Type, ISTF_Processor> Processors;
		public readonly GameObject Root;

		public readonly Dictionary<uint, List<Task>> ProcessOrderMap = new();
		public List<Task> Tasks = new();
		public readonly List<Object> Trash = new();

		public STF_ApplicationContextDefinition ApplicationContextFactory;

		public ProcessorState(
			ImportState State,
			GameObject Root,
			Dictionary<System.Type, ISTF_Processor> Processors = null,
			STF_ApplicationContextDefinition ApplicationContextFactory = null
		)
		{
			this.State = State;
			this.Root = Root;
			this.Processors = Processors ?? STF_Processor_Registry.GetProcessors(State.ImportConfig.SelectedApplication);
			this.ApplicationContextFactory = ApplicationContextFactory ?? STF_Processor_Registry.GetApplicationContextDefinition(State.ImportConfig.SelectedApplication);
		}

		public ISTF_Processor GetProcessor(ISTF_Resource Resource)
		{
			if (Processors.ContainsKey(Resource.GetType()))
				return Processors[Resource.GetType()];
			else
				return null;
		}

		public void RegisterResult(List<Object> ApplicationObjects)
		{
			if(ApplicationObjects != null && ApplicationObjects.Count > 0)
			{
				foreach(var ApplicationObject in ApplicationObjects)
				{
					if (ApplicationObject is Object @object)
						State.ObjectToRegister.Add(@object);
				}
			}
		}

		public object GetImportedResource(string STF_Id)
		{
			if(State.ImportedObjects.ContainsKey(STF_Id))
				return State.ImportedObjects[STF_Id];
			else
				return null;
		}

		public void AddProcessorTask(uint Order, Task Task)
		{
			if (ProcessOrderMap.ContainsKey(Order)) ProcessOrderMap[Order].Add(Task);
			else ProcessOrderMap.Add(Order, new List<Task> { Task });
		}

		public void Report(STFReport Report) { State.Report(Report); }
		public void AddTrash(Object Trash) { State.AddTrash(Trash); }
		public void AddTrash(IEnumerable<Object> Trash) { State.AddTrash(Trash); }
	}
}
