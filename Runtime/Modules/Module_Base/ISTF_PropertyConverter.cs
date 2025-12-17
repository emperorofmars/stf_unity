using System.Collections.Generic;
using System.Linq;

namespace com.squirrelbite.stf_unity.modules
{
	public class ImportPropertyPathPart
	{
		public string RelativePath = null;
		public System.Type TargetType;
		public List<string> PropertyNames = new();
		public System.Func<List<float>, List<float>> ConvertValueFunc;

		public ImportPropertyPathPart(string RelativePath, System.Type TargetType, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc = null)
		{
			this.RelativePath = RelativePath;
			this.TargetType = TargetType;
			this.PropertyNames = PropertyNames;
			this.ConvertValueFunc = ConvertValueFunc;
		}

		public ImportPropertyPathPart(System.Type TargetType, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc = null)
		{
			this.TargetType = TargetType;
			this.PropertyNames = PropertyNames;
			this.ConvertValueFunc = ConvertValueFunc;
		}

		public ImportPropertyPathPart(List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc = null)
		{
			this.PropertyNames = PropertyNames;
			this.ConvertValueFunc = ConvertValueFunc;
		}

		public ImportPropertyPathPart(string RelativePath)
		{
			this.RelativePath = RelativePath;
		}

		public static ImportPropertyPathPart FromTargetType(System.Type TargetType)
		{
			return new ImportPropertyPathPart(null, TargetType, null, null);
		}

		public static ImportPropertyPathPart operator +(ImportPropertyPathPart a, ImportPropertyPathPart b)
		{
			if(a == null || b == null) return null;

			string path = null;
			if(!string.IsNullOrEmpty(a.RelativePath))
			{
				path = a.RelativePath;
				if(!string.IsNullOrEmpty(b.RelativePath))
				{
					if(!a.RelativePath.EndsWith("/") && !b.RelativePath.StartsWith("/"))
						path += "/";
				}
				path += b.RelativePath;
			}
			else if(!string.IsNullOrEmpty(b.RelativePath))
			{
				path = b.RelativePath;
			}
			return new ImportPropertyPathPart(path, b.TargetType ?? a.TargetType, b.PropertyNames ?? a.PropertyNames, b.ConvertValueFunc ?? a.ConvertValueFunc);
		}

		public bool IsValid()
		{
			return !string.IsNullOrWhiteSpace(this.RelativePath) && this.TargetType != null && this.PropertyNames != null && this.PropertyNames.Count > 0;
		}

		public override string ToString()
		{
			if(IsValid())
				return this.TargetType.ToString() + "_" + this.RelativePath + "_" + this.PropertyNames.Aggregate((a, b) => a + b);
			else
				return"Invalid";
		}
	}

	/// <summary>
	/// Converts animation paths from STF to Unity's animation system.
	/// </summary>
	public interface ISTF_PropertyConverter
	{
		ImportPropertyPathPart ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath);
	}
}
