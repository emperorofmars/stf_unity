
using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.processors;

namespace com.squirrelbite.stf_unity
{
	public static class STF_Processor_Registry
	{
		public static readonly Dictionary<string, Dictionary<System.Type, ISTF_Processor>> DefaultProcessors = new();

		private static readonly Dictionary<string, Dictionary<System.Type, ISTF_Processor>> RegisteredProcessors = new();

		public static void RegisterProcessor(string Context, ISTF_Processor Processor)
		{
			if(!RegisteredProcessors.ContainsKey(Context))
				RegisteredProcessors[Context].Add(Processor.TargetType, Processor);
			else
				RegisteredProcessors.Add(Context, new Dictionary<System.Type, ISTF_Processor> {{Processor.TargetType, Processor}});
		}

		public static Dictionary<System.Type, ISTF_Processor> GetProcessors(string Context)
		{
			var ret = new Dictionary<System.Type, ISTF_Processor>(RegisteredProcessors.ContainsKey(Context) ? RegisteredProcessors[Context] : null);
			if(DefaultProcessors.ContainsKey(Context))
			{
				foreach(var processor in DefaultProcessors[Context])
				{
					if(!ret.ContainsKey(processor.Key))
						ret.Add(processor.Key, processor.Value);
				}
			}
			return ret;
		}

		public static List<string> GetAvaliableContexts()
		{
			var ret = new HashSet<string>();
			foreach(var entry in RegisteredProcessors)
				if(!ret.Contains(entry.Key))
					ret.Add(entry.Key);
			foreach(var entry in DefaultProcessors)
				if(!ret.Contains(entry.Key))
					ret.Add(entry.Key);
			return ret.ToList();
		}
	}
}
