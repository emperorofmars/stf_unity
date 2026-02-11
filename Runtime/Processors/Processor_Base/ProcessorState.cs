using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class ProcessorState
	{
		public readonly ImportState State;
		public readonly Dictionary<System.Type, ISTF_Processor> Processors;
		public readonly Dictionary<System.Type, ISTF_GlobalProcessor> GlobalProcessors;
		public readonly GameObject Root;

		public readonly List<ISTF_Resource> ResourcesToProcess = new();
		public readonly Dictionary<string, Dictionary<System.Type, List<ISTF_Resource>>> ExclusionGroups = new();
		public readonly Dictionary<string, List<ISTF_Processor>> GroupWinners = new(); // Group Name -> STF_Type
		public readonly Dictionary<string, string> ExcludedComponents = new(); // Component ID -> Group Name

		public readonly Dictionary<System.Type, List<ISTF_Resource>> ResourcesByType = new();

		public readonly Dictionary<uint, List<Task>> ProcessOrderMap = new();
		public List<Task> Tasks = new();

		public STF_ApplicationContextDefinition ApplicationContextFactory;

		public ProcessorState(
			ImportState State,
			GameObject Root,
			Dictionary<System.Type, ISTF_Processor> Processors = null,
			Dictionary<System.Type, ISTF_GlobalProcessor> GlobalProcessors = null,
			STF_ApplicationContextDefinition ApplicationContextFactory = null
		)
		{
			this.State = State;
			this.Root = Root;
			this.Processors = Processors ?? STF_Processor_Registry.GetProcessors(State.ImportConfig.SelectedApplication);
			this.GlobalProcessors = GlobalProcessors ?? STF_Processor_Registry.GetGlobalProcessors(State.ImportConfig.SelectedApplication);
			this.ApplicationContextFactory = ApplicationContextFactory ?? STF_Processor_Registry.GetApplicationContextDefinition(State.ImportConfig.SelectedApplication);

			Init();
		}

		private void Init()
		{
			var tmpResources = new HashSet<ISTF_Resource>();
			var armatureResources = new HashSet<ISTF_Resource>();

			// Find all processable resources
			foreach ((var stfId, var objectToRegister) in State.ImportedObjects)
			{
				// Ignore all resources that are children of an armature resource, except for components directly on the armature resource itself
				if (objectToRegister is STF_Armature armature)
				{
					foreach (var resourceOnArmature in armature.GetComponentsInChildren<ISTF_Resource>())
						if (!armatureResources.Contains(resourceOnArmature)) armatureResources.Add(resourceOnArmature);
					foreach (var resourceOnArmatureItself in armature.GetComponents<ISTF_Resource>())
						if (armatureResources.Contains(resourceOnArmatureItself)) armatureResources.Remove(resourceOnArmatureItself);
				}

				// Register all stf resources
				if (objectToRegister != null && !tmpResources.Contains(objectToRegister) && GetProcessor(objectToRegister) is var processor && processor != null)
					tmpResources.Add(objectToRegister);

				// Register all resources that have been instantiated from an armature
				if (objectToRegister is STF_Prefab go)
					foreach (var resourceOnObject in go.GetComponentsInChildren<ISTF_Resource>())
						if (!tmpResources.Contains(resourceOnObject) && GetProcessor(resourceOnObject) is var processorOnObject && processorOnObject != null)
							tmpResources.Add(resourceOnObject);
			}

			foreach ((var stfId, var resource) in State.ImportedObjects)
			{
				if(ResourcesByType.ContainsKey(resource.GetType()))
					ResourcesByType[resource.GetType()].Add(resource);
				else
					ResourcesByType.Add(resource.GetType(), new() { resource });
			}

			// Handle exclusion-groups
			foreach (var resource in tmpResources)
			{
				if (resource is ISTF_ComponentResource component && !string.IsNullOrWhiteSpace(component.ExclusionGroup))
				{
					if(!ExclusionGroups.ContainsKey(component.ExclusionGroup))
						ExclusionGroups.Add(component.ExclusionGroup, new() {{component.GetType(), new() {component as ISTF_Resource}}});
					else if(!ExclusionGroups[component.ExclusionGroup].ContainsKey(component.GetType()))
						ExclusionGroups[component.ExclusionGroup].Add(component.GetType(), new() {component as ISTF_Resource});
					else
						ExclusionGroups[component.ExclusionGroup][component.GetType()].Add(component as ISTF_Resource);
				}
			}

			foreach((var group, var types) in ExclusionGroups)
			{
				ISTF_Processor selectedProcessor = null;
				foreach(var k in types.Keys)
					if(GetProcessor(k) is var processor && processor != null)
						if(selectedProcessor == null || processor.Priority > selectedProcessor.Priority)
							selectedProcessor = processor;

				if(!GroupWinners.ContainsKey(group)) GroupWinners.Add(group, new () {selectedProcessor});
				else GroupWinners[group].Add(selectedProcessor);

				// TODO also add exclusion whitelist from winner processor to GroupWinners

				foreach((var k, var components) in types)
					if(GetProcessor(k) != selectedProcessor)
						foreach(var c in components)
							if(!ExcludedComponents.ContainsKey(c.STF_Id))
								ExcludedComponents.Add(c.STF_Id, group);
			}

			foreach (var resource in tmpResources)
				if (!ExcludedComponents.ContainsKey(resource.STF_Id) && !armatureResources.Contains(resource))
					ResourcesToProcess.Add(resource);
		}

		public ISTF_Processor GetProcessor(ISTF_Resource Resource)
		{
			return GetProcessor(Resource.GetType());
		}

		public ISTF_Processor GetProcessor(System.Type ResourceType)
		{
			if (Processors.ContainsKey(ResourceType))
				return Processors[ResourceType];
			else
				return null;
		}

		public List<ISTF_Resource> GetResourceByType(System.Type Type)
		{
			if (ResourcesByType.ContainsKey(Type))
				return ResourcesByType[Type];
			else
				return null;
		}

		public bool IsOverridden(string STF_Id)
		{
			return ExcludedComponents.ContainsKey(STF_Id);
		}

		public void RegisterResult(List<Object> ApplicationObjects)
		{
			if (ApplicationObjects != null && ApplicationObjects.Count > 0)
				foreach (var ApplicationObject in ApplicationObjects)
					if (ApplicationObject is Object @object)
						State.ObjectToRegister.Add(@object);
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
