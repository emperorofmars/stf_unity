using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public enum ErrorSeverity
	{
		INFO = 0, WARNING = 1, ERROR = 2, FATAL_ERROR = 3
	}

	[System.Serializable]
	public class STFReport
	{
		public string Message;
		public ErrorSeverity Severity;
		public string ResourceType;
		public string ResourceID;
		public Object Node;
		public System.Exception Exception;

		public STFReport(string Message, ErrorSeverity Severity = ErrorSeverity.ERROR, string ModuleType = null, string ResourceID = null, UnityEngine.Object Node = null, System.Exception Exception = null)
		{
			this.Message = Message;
			this.Severity = Severity;
			this.ResourceType = ModuleType;
			this.ResourceID = ResourceID;
			this.Node = Node;
			this.Exception = Exception;
		}

		public override string ToString()
		{
			return $"STF {Severity}\n{Message}\nType: {ResourceType}\nNode: {Node}\nException: {Exception?.Message}";
		}
	}

	public class STFException : System.Exception
	{
		public STFException(STFReport Report)
			 : base(Report.Message, Report.Exception)
		{
			this.Report = Report;
		}
		public STFException(string Message, ErrorSeverity Severity = ErrorSeverity.ERROR, string ModuleType = null, string ResourceID = null, UnityEngine.Object Node = null, System.Exception Exception = null)
			 : base(Message, Exception)
		{
			this.Report = new STFReport(Message, Severity, ModuleType, ResourceID, Node, Exception);
		}

		public readonly STFReport Report;
	}
}
