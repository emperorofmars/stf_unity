using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public enum ErrorSeverity
	{
		INFO, WARNING, ERROR, FATAL_ERROR
	}

	[System.Serializable]
	public class STFReport
	{
		public string Message;
		public ErrorSeverity Severity;
		public string ModuleType;
		public Object Node;
		public System.Exception Exception;

		public STFReport(string Message, ErrorSeverity Severity = ErrorSeverity.ERROR, string ModuleType = null, UnityEngine.Object Node = null, System.Exception Exception = null)
		{
			this.Message = Message;
			this.Severity = Severity;
			this.ModuleType = ModuleType;
			this.Node = Node;
			this.Exception = Exception;
		}

		public override string ToString()
		{
			return $"STF {Severity}\n{Message}\nType: {ModuleType}\nNode: {Node}\nException: {Exception?.Message}";
		}
	}

	public class STFException : System.Exception
	{
		public STFException(STFReport Report)
			 : base(Report.Message, Report.Exception)
		{
			this.Report = Report;
		}
		public STFException(string Message, ErrorSeverity Severity = ErrorSeverity.ERROR, string ModuleType = null, UnityEngine.Object Node = null, System.Exception Exception = null)
			 : base(Message, Exception)
		{
			this.Report = new STFReport(Message, Severity, ModuleType, Node, Exception);
		}

		public readonly STFReport Report;
	}
}
