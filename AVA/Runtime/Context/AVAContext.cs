
using System.Collections.Generic;
using System.Threading.Tasks;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.processors;
using UnityEngine;

namespace com.squirrelbite.stf_unity.ava
{
	public class AVAContext : ProcessorContextBase
	{
		public readonly Dictionary<string, Dictionary<string, Task>> Tasks = new();
		public readonly Dictionary<string, string> Preferred = new();

		public readonly Dictionary<string, object> Messages = new();
		protected AVA_Avatar _AVA_Avatar_Resource;


		public AVAContext(ProcessorState State) : base(State)
		{
			var avatars = State.GetResourceByType(typeof(AVA_Avatar));
			if (avatars.Count == 1)
			{
				_AVA_Avatar_Resource = avatars[0] as AVA_Avatar;
			}
			else
			{
				Report(new STFReport("No Avatar Component on Root found! Attempting to automap.", ErrorSeverity.WARNING));
				Automap();
			}
		}

		public AVA_Avatar AVA_Avatar_Resource => _AVA_Avatar_Resource;
		public STF_Instance_Armature PrimaryArmatureInstance => AVA_Avatar_Resource.PrimaryArmatureInstance;
		public STF_Instance_Mesh PrimaryMeshInstance => AVA_Avatar_Resource.PrimaryMeshInstance;

		public void AddMessage(string MessageId, object Message)
		{
			if (!string.IsNullOrWhiteSpace(MessageId))
			{
				if (Messages.ContainsKey(MessageId)) Messages[MessageId] = Message;
				else Messages.Add(MessageId, Message);
			}
		}
		public bool HasMessage(string MessageId)
		{
			return Messages.ContainsKey(MessageId);
		}
		public T GetMessage<T>(string MessageId)
		{
			if (Messages.ContainsKey(MessageId) && Messages[MessageId].GetType() == typeof(T))
				return (T)Messages[MessageId];
			else
				return default;
		}
		public object GetMessage(string MessageId)
		{
			return Messages.ContainsKey(MessageId) ? Messages[MessageId] : null;
		}

		protected virtual void Automap()
		{
			_AVA_Avatar_Resource = Root.AddComponent<AVA_Avatar>();

			var processor = State.GetProcessor(_AVA_Avatar_Resource);
			State.AddProcessorTask(processor.Order, new Task(() =>
			{
				var results = processor.Process(this, _AVA_Avatar_Resource);
				if (results != null) _AVA_Avatar_Resource.ProcessedObjects.AddRange(results);
				State.RegisterResult(results);
			}));

			foreach (var t in Root.GetComponentsInChildren<Transform>())
			{
				if (t.name.ToLower() == "armature" && t.gameObject.GetComponent<STF_Instance_Armature>() is var armatureInstance)
					_AVA_Avatar_Resource.PrimaryArmatureInstance = armatureInstance;
				if (t.name.ToLower() == "body" && t.gameObject.GetComponent<STF_Instance_Mesh>() is var meshInstance)
					_AVA_Avatar_Resource.PrimaryMeshInstance = meshInstance;
			}
		}
	}
}
