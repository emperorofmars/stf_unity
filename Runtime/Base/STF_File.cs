using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Buffers;
using System.Buffers.Binary;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	/// <summary>
	/// Holds the raw data from an STF file.
	/// Provides methods to read and write STF files.
	/// </summary>
	[System.Serializable]
	public class STF_File
	{
		public const string _MAGIC = "STF0";

		public uint Version = 0;
		public uint PaddingFutureUse = 0;
		public string Json;

		[HideInInspector, SerializeField]
		public List<byte[]> Buffers = new();
		public int BufferCount => Buffers.Count;
		public string OriginalFileName;

		public STF_File(string Json, List<byte[]> Buffers)
		{
			this.Json = Json;
			this.Buffers = Buffers;
		}

		public STF_File(string ImportPath) : this(File.ReadAllBytes(ImportPath), Path.GetFileNameWithoutExtension(ImportPath)) {}

		public STF_File(byte[] ByteArray, string OriginalFileName = null)
		{
			this.OriginalFileName = OriginalFileName;

			// Minimal file size = minimal binary header size (20) + length of minimal JSON definition
			if(ByteArray.Length < (20 + "{\"stf\"{\"version\":[0,1],\"root\":\"\"}}".Length))
				throw new System.Exception("Invalid buffer!");

			var bufferReader = new SequenceReader<byte>(new ReadOnlySequence<byte>(ByteArray));

			// Magic Number
			var magic = Encoding.UTF8.GetString(bufferReader.UnreadSpan[..4]); bufferReader.Advance(4);
			if(magic != _MAGIC)
				throw new System.Exception("Not an STF file, invalid magic number.");

			// Version
			Version = BinaryPrimitives.ReadUInt32LittleEndian(bufferReader.UnreadSpan);
			bufferReader.Advance(4);
			if(Version != 0)
				throw new System.Exception("Unsupported STF version!");
			PaddingFutureUse = BinaryPrimitives.ReadUInt32LittleEndian(bufferReader.UnreadSpan);
			bufferReader.Advance(4);

			// Number of all buffers, including the Json definition
			var bufferCount = BinaryPrimitives.ReadUInt32LittleEndian(bufferReader.UnreadSpan);
			bufferReader.Advance(4);

			// Length of each buffer
			var buffer_lengths = new ulong[bufferCount];
			for(uint i = 0; i < bufferCount; i++)
			{
				buffer_lengths[i] = BinaryPrimitives.ReadUInt64LittleEndian(bufferReader.UnreadSpan);
				bufferReader.Advance(8);
			}

			// Read the Json definition
			Json = Encoding.UTF8.GetString(ReadBytes(bufferReader, buffer_lengths[0])); bufferReader.Advance((long)buffer_lengths[0]);

			// Read each subsequent buffer
			Buffers = new List<byte[]>();
			for(uint i = 1; i < bufferCount; i++)
			{
				Buffers.Add(ReadBytes(bufferReader, buffer_lengths[i])); bufferReader.Advance((long)buffer_lengths[i]);
			}

			//BufferCount = Buffers.Count();
		}

		private byte[] ReadBytes(SequenceReader<byte> Reader, ulong Length)
		{
			var ret = new byte[Length];
			for(ulong i = 0; i < Length; i++)
			{
				Reader.TryRead(out ret[i]);
			}
			return ret;
		}


		public System.ReadOnlyMemory<byte> CreateSTFBinary()
		{
			var bufferWriter = new ArrayBufferWriter<byte>();

			// Magic Number
			bufferWriter.Write(Encoding.UTF8.GetBytes(_MAGIC));

			// Version
			BinaryPrimitives.WriteUInt32LittleEndian(bufferWriter.GetSpan(4), Version);
			bufferWriter.Advance(4);
			BinaryPrimitives.WriteUInt32LittleEndian(bufferWriter.GetSpan(4), PaddingFutureUse);
			bufferWriter.Advance(4);

			// Number of all buffers, including the Json definition
			BinaryPrimitives.WriteUInt32LittleEndian(bufferWriter.GetSpan(4), (uint)(Buffers.Count() + 1));
			bufferWriter.Advance(4);

			byte[] jsonUtf8 = Encoding.UTF8.GetBytes(this.Json);
			// Json definition length
			BinaryPrimitives.WriteUInt64LittleEndian(bufferWriter.GetSpan(8), (ulong)jsonUtf8.LongLength);
			bufferWriter.Advance(8);
			// Length of each subsequent buffer
			foreach(var buffer in Buffers)
			{
				BinaryPrimitives.WriteUInt64LittleEndian(bufferWriter.GetSpan(8), (ulong)buffer.LongLength);
				bufferWriter.Advance(8);
			}

			// Write the Json definition
			bufferWriter.Write(Encoding.UTF8.GetBytes(this.Json));
			// Write each subsequent buffer
			foreach(var buffer in Buffers)
			{
				bufferWriter.Write(buffer);
			}

			return bufferWriter.WrittenMemory;
		}
	}
}
