
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.squirrelbite.stf_unity.processors;

namespace com.squirrelbite.stf_unity.ava
{
	public class AVAContextFactory : ISTF_ApplicationContextFactory
	{
		public ProcessorContextBase Create(ProcessorState State)
		{
			return new AVAContext(State);
		}
	}

	public class AVAContext : ProcessorContextBase
	{
		public readonly Dictionary<string, Dictionary<string, Task>> Tasks = new();
		public readonly Dictionary<string, string> Preferred = new();

		public AVAContext(ProcessorState State) : base(State)
		{
		}

		public void AddTask(string Concept, string STF_Id, Task Task)
		{
			if (!Tasks.ContainsKey(Concept))
				Tasks.Add(Concept, new Dictionary<string, Task>());

			if (!Tasks[Concept].ContainsKey(STF_Id))
				Tasks[Concept].Add(STF_Id, Task);
			else
				Tasks[Concept][STF_Id] = Task;
		}

		public void SetPreferred(string Concept, string STF_Id)
		{
			if (Preferred.ContainsKey(STF_Id))
				Preferred.Add(Concept, STF_Id);
			else
				Preferred[Concept] = STF_Id;
		}

		protected override void Execute()
		{
			foreach ((var concept, var taskSet) in Tasks)
			{
				if (taskSet.Count == 1)
					RunTask(taskSet.First().Value);
				else if (taskSet.Count > 1 && Preferred.ContainsKey(concept))
					RunTask(taskSet.First(e => e.Key == Preferred[concept]).Value);
				else
					Report(new STFReport($"Invalid Taskset: {concept}", ErrorSeverity.ERROR));
			}
		}

		private void RunTask(Task Task)
		{
			Task.RunSynchronously();
			if (Task.Exception != null)
			{
				HandleTaskException(Task.Exception);
			}
		}

		private void HandleTaskException(System.AggregateException Exception)
		{
			foreach (var e in Exception.InnerExceptions)
				if (e is STFException stfError)
					Report(stfError.Report);
				else
					Report(new STFReport(e.Message, ErrorSeverity.FATAL_ERROR, null, null, e));
		}
	}
}
