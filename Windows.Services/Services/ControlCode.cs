using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.Services
{
	/// <summary>
	/// A service control code
	/// </summary>
	public struct ControlCode : IEquatable<ControlCode>
	{
		#region Consts

		private const uint DEVICE_EVENT = 0x0000000B;
		private const uint HARDWARE_PROFILE_CHANGE = 0x0000000C;
		private const uint POWER_EVENT = 0x0000000D;
		private const uint SESSION_CHANGE = 0x0000000E;
		private const uint TIME_CHANGE = 0x00000010;
		private const uint TRIGGER_EVENT = 0x00000020;
		private const uint USER_MODE_REBOOT = 0x00000040;

		private const uint MIN_USER_EVENT = 128;
		private const uint MAX_USER_EVENT = 255;

		private const string USER_DEFINED_CONTROL_NAME = "UserControl";
		private const string DEVICE_EVENT_NAME = "DeviceEvent";
		private const string HARDWARE_PROFILE_CHANGE_NAME = "HardwareProfileChanged";
		private const string POWER_EVENT_NAME = "PowerEvent";
		private const string SESSION_CHANGE_NAME = "SessionChange";
		private const string TIME_CHANGE_NAME = "TimeChange";
		private const string TRIGGER_EVENT_NAME = "TriggerEvent";
		private const string USER_MODE_REBOOT_NAME = "UserModeReboot";

		#region Public

		#region Device events

		/// <summary>
		/// The system broadcasts the DE_DEVICE_ARRIVAL device event when a device or piece 
		/// of media has been inserted and becomes available
		/// </summary>
		public const uint DE_DEVICE_ARRIVAL = 0x8000;
		/// <summary>
		/// The system broadcasts the DE_DEVICE_QUERY_REMOVE device event when a device or 
		/// piece of media has been physically removed.
		/// </summary>
		public const uint DE_DEVICE_QUERY_REMOVE = 0x8001;
		/// <summary>
		/// The system broadcasts the DE_DEVICE_QUERY_REMOVE_FAILED device event to request permission to 
		/// remove a device or piece of media. 
		/// This message is the last chance for applications and drivers to prepare for this removal. 
		/// However, any application can deny this request and cancel the operation.
		/// </summary>
		public const uint DE_DEVICE_QUERY_REMOVE_FAILED = 0x8002;
		/// <summary>
		/// The system broadcasts the DE_DEVICE_REMOVE_PENDING device event when a request to remove a device or 
		/// piece of media has been canceled.
		/// </summary>
		public const uint DE_DEVICE_REMOVE_PENDING = 0x8003;
		/// <summary>
		/// The system broadcasts the DE_DEVICE_REMOVE_COMPLETE device event when a device or 
		/// piece of media is being removed and is no longer available for use.
		/// </summary>
		public const uint DE_DEVICE_REMOVE_COMPLETE = 0x8004;
		/// <summary>
		/// The system sends the DE_CUSTOM_EVENT device event when a driver-defined custom event has occurred.
		/// </summary>
		public const uint DE_CUSTOM_EVENT = 0x8006; 
		#endregion

		#region Hardware profile change events

		/// <summary>
		/// The system broadcasts the HPC_QUERY_CHANGE_CONFIG device event to request permission to change 
		/// the current configuration (dock or undock). Any application can deny this request and cancel the change.
		/// </summary>
		public const uint HPC_QUERY_CHANGE_CONFIG = 0x0017;
		/// <summary>
		/// The system broadcasts the DBT_CONFIGCHANGED device event to indicate that 
		/// the current configuration has changed, due to a dock or undock. 
		/// An application or driver that stores data in the registry under the HKEY_CURRENT_CONFIG key 
		/// should update the data.
		/// </summary>
		public const uint HPC_CONFIG_CHANGED = 0x0018;
		/// <summary>
		/// The system broadcasts the DBT_CONFIGCHANGECANCELED device event when a request to change 
		/// the current configuration (dock or undock) has been canceled.
		/// </summary>
		public const uint HPC_CONFIG_CHANGE_CANCELED = 0x0019; 
		#endregion
		#endregion
		#endregion

		#region Fields

		private static readonly ICollection<uint> ValidNotUserEvents = new HashSet<uint>(new[]
		{
			DEVICE_EVENT,
			HARDWARE_PROFILE_CHANGE,
			POWER_EVENT,
			SESSION_CHANGE,
			TIME_CHANGE,
			TRIGGER_EVENT,
			USER_MODE_REBOOT,
		});

		private static readonly IDictionary<uint, string> nameByControl = new Dictionary<uint, string>
		{
			[DEVICE_EVENT] = DEVICE_EVENT_NAME,
			[HARDWARE_PROFILE_CHANGE] = HARDWARE_PROFILE_CHANGE_NAME,
			[POWER_EVENT] = POWER_EVENT_NAME,
			[SESSION_CHANGE] = SESSION_CHANGE_NAME,
			[TIME_CHANGE] = TIME_CHANGE_NAME,
			[TRIGGER_EVENT] = TRIGGER_EVENT_NAME,
			[USER_MODE_REBOOT] = USER_MODE_REBOOT_NAME,
		};

		#region Public readonly fields

		/// <summary>
		/// Notifies a service that it should stop.
		/// </summary>
		public static readonly ControlCode Stop = new ControlCode(ApplicationControl.Stop);

		/// <summary>
		/// Notifies a service that it should pause.
		/// </summary>
		public static readonly ControlCode Pause = new ControlCode(ApplicationControl.Pause);

		/// <summary>
		/// Notifies a paused service that it should resume.
		/// </summary>
		public static readonly ControlCode Continue = new ControlCode(ApplicationControl.Continue);

		/// <summary>
		/// Notifies a service to report its current status information to the service control manager.
		/// 
		/// The handler should simply return NO_ERROR; the SCM is aware of the current state of the service.
		/// </summary>
		public static readonly ControlCode Interrogate = new ControlCode(ApplicationControl.Interrogate);
		
		// Disabling obsolete warnings
#pragma warning disable 612, 618

		/// <summary>
		/// Notifies a service that service-specific startup parameters have changed. 
		/// The service should reread its startup parameters.
		/// </summary>
		[Obsolete]
		public static readonly ControlCode ParamChange = new ControlCode(ApplicationControl.ParamChange);

		/// <summary>
		/// Notifies a network service that there is a new component for binding. 
		/// The service should bind to the new component.
		/// 
		/// Applications should use Plug and Play functionality instead.
		/// </summary>
		[Obsolete]
		public static readonly ControlCode NetBindAdd = new ControlCode(ApplicationControl.NetBindAdd);

		/// <summary>
		/// Notifies a network service that a component for binding has been removed. 
		/// The service should reread its binding information and unbind from the removed component.
		/// 
		/// Applications should use Plug and Play functionality instead.
		/// </summary>
		[Obsolete]
		public static readonly ControlCode NetBindRemove = new ControlCode(ApplicationControl.NetBindRemove);

		/// <summary>
		/// Notifies a network service that a disabled binding has been enabled. 
		/// The service should reread its binding information and add the new binding.
		/// 
		/// Applications should use Plug and Play functionality instead.
		/// </summary>
		[Obsolete]
		public static readonly ControlCode NetBindEnable = new ControlCode(ApplicationControl.NetBindEnable);

		/// <summary>
		/// Notifies a network service that one of its bindings has been disabled. 
		/// The service should reread its binding information and remove the binding.
		/// 
		/// Applications should use Plug and Play functionality instead.
		/// </summary>
		[Obsolete]
		public static readonly ControlCode NetBindDisable = new ControlCode(ApplicationControl.NetBindDisable);

#pragma warning restore 612, 618

		/// <summary>
		/// Notifies a service of device events. 
		/// (The service must have registered to receive these notifications using the RegisterDeviceNotification function.) 
		/// The dwEventType and lpEventData parameters contain additional information.
		/// </summary>
		public static readonly ControlCode DeviceEvent = new ControlCode(DEVICE_EVENT);

		/// <summary>
		/// Notifies a service that the computer's hardware profile has changed. 
		/// The dwEventType parameter contains additional information.
		/// </summary>
		public static readonly ControlCode HardwareProfileChanged = new ControlCode(HARDWARE_PROFILE_CHANGE);

		/// <summary>
		/// Notifies a service of system power events.
		/// </summary>
		public static readonly ControlCode PowerEvent = new ControlCode(POWER_EVENT);

		/// <summary>
		/// Notifies a service of session change events.
		/// </summary>
		public static readonly ControlCode SessionChange = new ControlCode(SESSION_CHANGE);

		/// <summary>
		/// Notifies a service that the system time has changed.
		/// </summary>
		public static readonly ControlCode TimeChange = new ControlCode(TIME_CHANGE);

		/// <summary>
		/// Notifies a service registered for a service trigger event that the event has occurred.
		/// </summary>
		public static readonly ControlCode TriggerEvent = new ControlCode(TRIGGER_EVENT);

		/// <summary>
		/// Notifies a service that the user has initiated a reboot.
		/// </summary>
		public static readonly ControlCode UserModeReboot = new ControlCode(USER_MODE_REBOOT);
		#endregion
		#endregion

		#region Properties

		/// <summary>
		/// Gets the control code
		/// </summary>
		public uint Code { get; }

		/// <summary>
		/// Gets a name for the control code
		/// </summary>
		public string Name => ControlCode.GetName(this.Code);
		#endregion

		#region Ctor

		/// <summary>
		/// Initialize new instance with the given control code.
		/// Be aware that only valid control codes are supported.
		/// </summary>
		/// <param name="control">The control code.</param>
		public ControlCode(uint control)
			: this()
		{
			if (!IsLegal(control))
			{
				throw new ArgumentException("Invalid value for eventType", nameof(control));
			}

			this.Code = control;
		}

		/// <summary>
		/// Initialize new instance with the given control code.
		/// Be aware that only valid control codes are supported.
		/// </summary>
		/// <param name="control">The control code.</param>
		public ControlCode(ApplicationControl control) : this((uint)control) { }
		#endregion

		#region Methods

		private static string GetName(uint control)
		{
			string name;

			if (Enum.IsDefined(typeof(ApplicationControl), control))
			{
				name = ((ApplicationControl)control).ToString();
			}
			else if (!nameByControl.TryGetValue(control, out name))
			{
				name = USER_DEFINED_CONTROL_NAME;
			}

			return name;
		}

		private bool IsLegal(uint control) =>
			((control >= MIN_USER_EVENT) && (control <= MAX_USER_EVENT)) ||
			ControlCode.ValidNotUserEvents.Contains(control) ||
			(Enum.IsDefined(typeof(ApplicationControl), control) && (control != 0));

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(ControlCode other) =>
			this.Code == other.Code;

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			// TODO: write your implementation of Equals() here
			return Equals((ControlCode)obj);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() =>
			this.Code.GetHashCode();

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString() =>
			$"{this.Name}: {this.Code}";
		#endregion

		#region Operators

		/// <summary></summary><param name="left"></param><param name="right"></param><returns></returns>
		public static bool operator ==(ControlCode left, ControlCode right) =>
			left.Equals(right);

		/// <summary></summary><param name="left"></param><param name="right"></param><returns></returns>
		public static bool operator !=(ControlCode left, ControlCode right) =>
			!(left == right);

		/// <summary></summary><param name="value"></param><returns></returns>
		public static explicit operator ControlCode(uint value) =>
			new ControlCode(value);

		/// <summary></summary><param name="value"></param><returns></returns>
		public static implicit operator uint(ControlCode value) =>
			value.Code;

		/// <summary></summary><param name="value"></param><returns></returns>
		public static implicit operator ControlCode(ApplicationControl value) =>
			new ControlCode(value);

		/// <summary></summary><param name="value"></param><returns></returns>
		public static explicit operator ApplicationControl(ControlCode value)
		{
			if (!Enum.IsDefined(typeof(ApplicationControl), value.Code))
			{
				throw new InvalidCastException("The control code is not defined by the controls enum");
			}

			return (ApplicationControl)value.Code;
		}
		#endregion
	}
}
