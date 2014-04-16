using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Windows.Interop;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	/// <summary>
	/// Represents a windows service.
	/// </summary>
	public partial class Service : IDisposable, INotificationWaiter
	{
		#region Consts

		/// <summary>
		/// A prefix for dependent group names.
		/// </summary>
		public const char SC_GROUP_IDENTIFIER = QueryServiceConfig.SC_GROUP_IDENTIFIER;
		#endregion

		#region Fields

		private bool isInitialized = false;
		private Lazy<string> serviceName;
		private Lazy<string> displayName;
		private Lazy<ServiceType> type;
		private Lazy<StartType> startType;
		private Lazy<ErrorControl> errorControl;
		private Lazy<string> binaryPath;
		private Lazy<string> loadOrderGroup;
		private Lazy<uint> tag;
		private MultiString dependencies;
		private Lazy<string> accountName;
		private Lazy<bool> isAutoStartDelayed;
		private Lazy<string> description;
		private Lazy<ushort?> preferedNodeId;
		private Lazy<int> preShutdownTimeout;
		private MultiString requiredPrivileges;
		private Lazy<SidType> sidType;
		private Lazy<ReadOnlyCollection<Trigger>> triggers;
		private Lazy<LaunchProtected> launchProtection;
		private ServiceEvents events;
		#endregion

		#region Events

		#region Event handlers

		private Dictionary<Notification, EventHandler<ServiceEventArgs>> handlers =
			new Dictionary<Notification, EventHandler<ServiceEventArgs>>()
			{
				{ Notification.ContinuePending, (s, e) => { } },
				{ Notification.DeletePending, (s, e) => { } },
				{ Notification.PausePending, (s, e) => { } },
				{ Notification.Paused, (s, e) => { } },
				{ Notification.Running, (s, e) => { } },
				{ Notification.StartPending, (s, e) => { } },
				{ Notification.StopPending, (s, e) => { } },
				{ Notification.Stopped, (s, e) => { } },
			};
		#endregion

		#region Events

		/// <summary>
		/// Occues when the service enters to ContinuePending state
		/// </summary>
		public event EventHandler<ServiceEventArgs> ContinuePending
		{
			add
			{
				this.handlers[Notification.ContinuePending] += value;
				HookEvents();
			}
			remove
			{
				this.handlers[Notification.ContinuePending] -= value;
			}
		}

		/// <summary>
		/// Occues when the service enters to DeletePeinding state
		/// </summary>
		public event EventHandler<ServiceEventArgs> DeletePeinding
		{
			add
			{
				this.handlers[Notification.DeletePending] += value;
				HookEvents();
			}
			remove
			{
				this.handlers[Notification.DeletePending] -= value;
			}
		}

		/// <summary>
		/// Occues when the service enters to PausePending state
		/// </summary>
		public event EventHandler<ServiceEventArgs> PausePending
		{
			add
			{
				this.handlers[Notification.PausePending] += value;
				HookEvents();
			}
			remove
			{
				this.handlers[Notification.PausePending] -= value;
			}
		}

		/// <summary>
		/// Occues when the service enters to Paused state
		/// </summary>
		public event EventHandler<ServiceEventArgs> Paused
		{
			add
			{
				this.handlers[Notification.Paused] += value;
				HookEvents();
			}
			remove
			{
				this.handlers[Notification.Paused] -= value;
			}
		}

		/// <summary>
		/// Occues when the service enters to Running state
		/// </summary>
		public event EventHandler<ServiceEventArgs> Running
		{
			add
			{
				this.handlers[Notification.Running] += value;
				HookEvents();
			}
			remove
			{
				this.handlers[Notification.Running] -= value;
			}
		}

		/// <summary>
		/// Occues when the service enters to StartPending state
		/// </summary>
		public event EventHandler<ServiceEventArgs> StartPending
		{
			add
			{
				this.handlers[Notification.StartPending] += value;
				HookEvents();
			}
			remove
			{
				this.handlers[Notification.StartPending] -= value;
			}
		}

		/// <summary>
		/// Occues when the service enters to StopPending state
		/// </summary>
		public event EventHandler<ServiceEventArgs> StopPending
		{
			add
			{
				this.handlers[Notification.StopPending] += value;
				HookEvents();
			}
			remove
			{
				this.handlers[Notification.StopPending] -= value;
			}
		}

		/// <summary>
		/// Occues when the service enters to Stopped state
		/// </summary>
		public event EventHandler<ServiceEventArgs> Stopped
		{
			add
			{
				this.handlers[Notification.Stopped] += value;
				HookEvents();
			}
			remove
			{
				this.handlers[Notification.Stopped] -= value;
			}
		}
		#endregion

		/// <summary>
		/// Fire the requested event.
		/// </summary>
		/// <param name="event">The event to fire.</param>
		/// <param name="status">Service status for the event.</param>
		protected void OnEvent(Notification @event, ServiceStatus status)
		{
			this.handlers[@event](this, new ServiceEventArgs(this, status));
		}
		#endregion

		#region Ctor

		private Service()
		{
			this.serviceName = new Lazy<string>(() => LoadAndGet(() => this.ServiceName));
			this.displayName = new Lazy<string>(() => LoadAndGet(() => this.DisplayName));
			this.type = new Lazy<ServiceType>(() => LoadAndGet(() => this.Type));
			this.startType = new Lazy<StartType>(() => LoadAndGet(() => this.StartType));
			this.errorControl = new Lazy<ErrorControl>(() => LoadAndGet(() => this.ErrorControl));
			this.binaryPath = new Lazy<string>(() => LoadAndGet(() => this.BinaryPath));
			this.loadOrderGroup = new Lazy<string>(() => LoadAndGet(() => this.LoadOrderGroup));
			this.tag = new Lazy<uint>(() => LoadAndGet(() => this.Tag));
			this.accountName = new Lazy<string>(() => LoadAndGet(() => this.AccountName));
			this.isAutoStartDelayed = new Lazy<bool>(GetIsAutoStartDelayed);
			this.description = new Lazy<string>(GetDescription);
			this.FailureActions = new FailureActions(this);
			this.preferedNodeId = new Lazy<ushort?>(GetPreferedNodeId);
			this.preShutdownTimeout = new Lazy<int>(GetPreShutdownTimeout);
			this.sidType = new Lazy<SidType>(GetSidType);
			this.triggers = new Lazy<ReadOnlyCollection<Trigger>>(GetTriggers);
			this.launchProtection = new Lazy<LaunchProtected>(GetLaunchProtection);
			this.DependentServices = new DependentServicesCollection(this);
		}

		internal Service(
			ServiceControlManager scm,
			IntPtr? handle,
			string name = null,
			string displayName = null,
			StartType? startType = null,
			ErrorControl? errorControl = null,
			string binaryPath = null,
			string loadOrderGroup = null,
			uint? tag = null,
			ICollection<string> dependencies = null,
			string accountName = null,
			string password = null,
			bool? isAutoStartDelayed = null,
			string description = null,
			FailureActions failureActions = null,
			ushort? preferedNodeId = null,
			int? preShutdownTimeout = null,
			ICollection<string> requiredPrivileges = null,
			SidType? sidType = null,
			ICollection<Trigger> triggers = null,
			LaunchProtected? launchProtection = null)
			: this()
		{
			if (scm == null)
			{
				throw new ArgumentNullException("scm");
			}

			if ((handle == null) && (name == null) && (displayName == null))
			{
				throw new ArgumentNullException(
					"handle, name, displayName",
					"'handle', 'name' and 'displayName' cannot be all null");
			}

			if (handle != null)
			{
				this.Handle = handle.Value;
			}
			else
			{
				if (name == null)
				{
					name = scm.GetServiceName(displayName);
				}

				handle = scm.GetServiceHandle(name);
			}

			this.Scm = scm;

			if (name != null)
			{
				this.ServiceName = name;
			}

			if (displayName != null)
			{
				this.DisplayName = displayName;
			}

			if (startType != null)
			{
				this.StartType = startType.Value;
			}

			if (errorControl != null)
			{
				this.ErrorControl = errorControl.Value;
			}

			if (binaryPath != null)
			{
				this.BinaryPath = binaryPath;
			}

			if (loadOrderGroup != null)
			{
				this.LoadOrderGroup = null;
			}

			if (tag != null)
			{
				this.Tag = tag.Value;
			}

			if (dependencies != null)
			{
				this.Dependencies = dependencies;
			}

			if (accountName != null)
			{
				this.AccountName = accountName;
				this.Password = password;
			}

			if (isAutoStartDelayed != null)
			{
				this.IsAutoStartDelayed = isAutoStartDelayed.Value;
			}

			if (description != null)
			{
				this.Description = description;
			}

			if (failureActions != null)
			{
				this.FailureActions = failureActions;
			}

			if (preferedNodeId != null)
			{
				this.PreferedNodeId = preferedNodeId.Value;
			}

			if (preShutdownTimeout != null)
			{
				this.PreShutdownTimeout = preShutdownTimeout.Value;
			}

			if (requiredPrivileges != null)
			{
				this.RequiredPrivileges = requiredPrivileges;
			}

			if (sidType != null)
			{
				this.SidType = sidType.Value;
			}

			if (triggers != null)
			{
				this.Triggers = Array.AsReadOnly((triggers as Trigger[]) ?? triggers.ToArray());
			}

			if (launchProtection != null)
			{
				this.LaunchProtection = launchProtection.Value;
			}

			this.isInitialized = true;
		}

		/// <summary>
		/// Release unmanaged resources.
		/// </summary>
		~Service()
		{
			Dispose(false);
		}
		#endregion
	}

	#region Service events args

	/// <summary>
	/// Service events arfs
	/// </summary>
	public class ServiceEventArgs : EventArgs
	{
		#region Properties

		/// <summary>
		/// Gets the status of the service when the event fires.
		/// </summary>
		public ServiceStatus Status { get; private set; }

		/// <summary>
		/// Gets the service that fired the event.
		/// </summary>
		public Service Service { get; private set; }
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of the event args
		/// </summary>
		/// <param name="service">The status of the service when the event fires.</param>
		/// <param name="status">The service that fired the event.</param>
		public ServiceEventArgs(Service service, ServiceStatus status)
		{
			this.Service = service;
			this.Status = status;
		}
		#endregion
	}
	#endregion
}
