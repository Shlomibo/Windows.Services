using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	/// <summary>
	/// Contains trigger-specific data for a service trigger event. 
	/// </summary>
	public abstract class TriggerData
	{
		#region Properties

		/// <summary>
		/// Gets the data type of the trigger-specific data.
		/// </summary>
		public DataItemType DataType { get; protected set; }

		/// <summary>
		/// Gets the trigger-specific data for the service trigger event.
		/// The trigger-specific data depends on the trigger event type
		/// </summary>
		public abstract object Data { get; } 
		#endregion

		#region Methods

		/// <summary>
		/// Creates new TriggerData object
		/// </summary>
		/// <param name="data">The data for the TriggerData object</param>
		/// <returns>A new TriggerData object that contains the specified data</returns>
		/// <exception cref="ArgumentException">The data was in an incompatible format</exception>
		public static TriggerData Create(object data)
		{
			if (data is string)
			{
				return new TriggerStringData((string)data);
			}
			else if (data is byte[])
			{
				return new TriggerBinaryData((byte[])data);
			}
			else
			{
				throw new ArgumentException("Invalid data type for data", "data");
			}
		}

		internal unsafe static TriggerData Create(ref TriggerSpecificDataItem dataItem)
		{
			TriggerData data = null;

			switch (dataItem.type)
			{
				case DataItemType.Binary:

					var binaryData = new byte[dataItem.bytesCount];
					Marshal.Copy(
						(IntPtr)dataItem.data,
						binaryData,
						0,
						binaryData.Length);
					data = new TriggerBinaryData(binaryData);
					
					break;

				case DataItemType.String:

					var str = new string(
						(char*)dataItem.data,
						0,
						(int)dataItem.bytesCount / sizeof(char));
					data = new TriggerStringData(str);

					break;

				default:

					throw new NotImplementedException("The given trigger data item type " +
						"is not implemented");
			}

			return data;
		}

		internal abstract unsafe void ToUnmanaged(ref TriggerSpecificDataItem unmanaged);

		internal static unsafe void FreeUnmanaged(ref TriggerSpecificDataItem unmanaged)
		{
			Marshal.FreeHGlobal((IntPtr)unmanaged.data);
			unmanaged.data = null;
		}
		#endregion
	}

	/// <summary>
	/// Contains trigger-specific string data for a service trigger event.
	/// </summary>
	public class TriggerStringData : TriggerData
	{
		#region Properties

		/// <summary>
		/// Gets the string data for a service trigger event.
		/// </summary>
		public string StringData { get; private set; }

		/// <summary>
		/// Gets the trigger-specific data for the service trigger event.
		/// </summary>
		public override object Data
		{
			get { return this.StringData; }
		} 
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new TriggerData that contains string data.
		/// </summary>
		/// <param name="data">The string data for a service trigger event.</param>
		public TriggerStringData(string data)
		{
			this.DataType = DataItemType.String;
			this.StringData = data;
		} 
		#endregion

		#region Methods

		internal override unsafe void ToUnmanaged(ref TriggerSpecificDataItem unmanaged)
		{
			unmanaged.bytesCount = (uint)(sizeof(char) * (this.StringData.Length + 1));
			unmanaged.data = (byte*)Marshal.StringToHGlobalUni(this.StringData);
			unmanaged.type = DataItemType.String;
		}
		#endregion
	}

	/// <summary>
	/// Contains trigger-specific binary data for a service trigger event.
	/// </summary>
	public class TriggerBinaryData : TriggerData
	{
		#region Properties

		/// <summary>
		/// Gets the binary data for a service trigger event.
		/// </summary>
		public byte[] BinaryData { get; private set; }

		/// <summary>
		/// Gets the trigger-specific data for the service trigger event.
		/// </summary>
		public override object Data
		{
			get { return this.BinaryData; }
		}
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new TriggerData that contains binary data.
		/// </summary>
		/// <param name="data">The binary data for a service trigger event.</param>
		public TriggerBinaryData(byte[] data)
		{
			this.DataType = DataItemType.Binary;
			this.BinaryData = data;
		}
		#endregion

		#region Methods

		internal override unsafe void ToUnmanaged(ref TriggerSpecificDataItem unmanaged)
		{
			unmanaged.bytesCount = (uint)this.BinaryData.Length;
			unmanaged.data = (byte*)Marshal.AllocHGlobal(this.BinaryData.Length);
			Marshal.Copy(this.BinaryData, 0, (IntPtr)unmanaged.data, this.BinaryData.Length);
			unmanaged.type = DataItemType.Binary;
		}
		#endregion
	}
}
