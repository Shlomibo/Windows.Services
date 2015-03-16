using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Interop;

namespace Windows.Services
{
	/// <summary>
	/// Represents service's trigger
	/// </summary>
	public sealed class Trigger
	{
		#region Properties

		/// <summary>
		/// Gets the trigger event type.
		/// </summary>
		public TriggerType Type { get; }

		/// <summary>
		/// Gets the action to take when the specified trigger event occurs.
		/// </summary>
		public TriggerAction Action { get; }

		/// <summary>
		/// Gets the Guid that identifies the trigger event subtype.
		/// </summary>
		public Guid Subtype { get; }

		/// <summary>
		/// Gets a collection of items that contain trigger-specific data.
		/// </summary>
		public ReadOnlyCollection<TriggerData> DataItems { get; }
		#endregion

		#region Ctor

		/// <summary>
		/// Create new Trigger instance
		/// </summary>
		/// <param name="type">The trigger event type.</param>
		/// <param name="action">The action to take when the specified trigger event occurs.</param>
		/// <param name="subType">The Guid that identifies the trigger event subtype.</param>
		/// <param name="dataItems">A collection of items that contain trigger-specific data.</param>
		public Trigger(TriggerType type, TriggerAction action, Guid subType, IList<TriggerData> dataItems)
		{
			this.Type = type;
			this.Action = action;
			this.Subtype = subType;

			if (dataItems is ReadOnlyCollection<TriggerData>)
			{
				this.DataItems = (ReadOnlyCollection<TriggerData>)dataItems;
			}
			else if (dataItems is IList<TriggerData>)
			{
				this.DataItems = new ReadOnlyCollection<TriggerData>(dataItems as IList<TriggerData>);
			}
			else if (dataItems != null)
			{
				this.DataItems = Array.AsReadOnly(dataItems.ToArray());
			}
			else
			{
				this.DataItems = null;
			}
		}

		internal unsafe Trigger(ref ServiceTrigger trigger)
		{
			var dataItems = new TriggerData[trigger.dataItemsCount];

			for (int i = 0; i < dataItems.Length; i++)
			{
				dataItems[i] = TriggerData.Create(trigger.dataItems[i]);
			}

			this.DataItems = Array.AsReadOnly(dataItems);
		}
		#endregion

		#region Methods

		internal unsafe void ToUnmanaged(ref ServiceTrigger unmanaged)
		{
			unmanaged.action = this.Action;
			unmanaged.triggerType = this.Type;
			unmanaged.dataItemsCount = (uint)this.DataItems.Count;

			unmanaged.dataItems = (TriggerSpecificDataItem*)Marshal.AllocHGlobal(
				sizeof(TriggerSpecificDataItem) * this.DataItems.Count);

			if (unmanaged.dataItems == null)
			{
				throw new OutOfMemoryException();
			}

			for (int i = 0; i < this.DataItems.Count; i++)
			{
				this.DataItems[i].ToUnmanaged(ref unmanaged.dataItems[i]);
			}
			
			unmanaged.triggerSubType = (Guid*)Marshal.AllocHGlobal(sizeof(Guid));
			Marshal.StructureToPtr(this.Subtype, (IntPtr)unmanaged.triggerSubType, false);
		}

		internal static unsafe void FreeUnmanaged(ref ServiceTrigger unmanaged)
		{
			for (int i = 0; i < unmanaged.dataItemsCount; i++)
			{
				TriggerData.FreeUnmanaged(ref unmanaged.dataItems[i]);
			}

			Marshal.FreeHGlobal((IntPtr)unmanaged.dataItems);
			unmanaged.dataItems = null;

			Marshal.FreeHGlobal((IntPtr)unmanaged.triggerSubType);
			unmanaged.triggerSubType = null;
		}
		#endregion
	}
}
