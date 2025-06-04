
using System.Collections.Generic;
using System.Threading.Tasks;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.processors;

namespace com.squirrelbite.stf_unity.ava
{
	public class AVAContext : ProcessorContextBase
	{
		public readonly Dictionary<string, Dictionary<string, Task>> Tasks = new();
		public readonly Dictionary<string, string> Preferred = new();

		public readonly Dictionary<string, object> Messages = new();
		private readonly AVA_Avatar _AVA_Avatar_Resource;


		public AVAContext(ProcessorState State) : base(State)
		{
			var avatars = State.GetResourceByType(typeof(AVA_Avatar));
			if (avatars.Count == 1)
			{
				_AVA_Avatar_Resource = avatars[0] as AVA_Avatar;
			}
			else
			{
				Report(new STFReport("Avatar Component on Root required!", ErrorSeverity.FATAL_ERROR));
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
		public bool HasMessage(string MessageId) {
			return Messages.ContainsKey(MessageId);
		}
		public T GetMessage<T>(string MessageId)
		{
			if (Messages.ContainsKey(MessageId) && Messages[MessageId].GetType() == typeof(T))
				return (T)Messages[MessageId];
			else
				return default;
		}
		public object GetMessage(string MessageId) {
			return Messages.ContainsKey(MessageId) ? Messages[MessageId] : null;
		}
	}
}
