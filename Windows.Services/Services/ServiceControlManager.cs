#define WIN32WAIT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Interop;
using Windows.Services.Interop;
using Utilities;

namespace Windows.Services
{
	/// <summary>
	/// Represents a service control manager database.
	/// </summary>
	public sealed partial class ServiceControlManager : IDisposable, INotificationWaiter
	{
		#region Consts

		private const ScmAccessRights DEFAULT_ACCESS = ScmAccessRights.GenericRead;
		private const string DESFAULT_MACHINE_NAME = "localhost";
		private const string CREATE_PREFIX = "/";
		#endregion

		#region Fields

		private static readonly Dictionary<int, string> creationMessages = new Dictionary<int, string>()
		{
			[Win32API.ERROR_ACCESS_DENIED] = "The requested access was denied.",
			[Win32API.ERROR_DATABASE_DOES_NOT_EXIST] = "The specified database does not exist.",
		};

		private static readonly Dictionary<int, string> notifyBootConfigStatusMessages = new Dictionary<int, string>()
		{
			[Win32API.ERROR_ACCESS_DENIED] = 
				"The user does not have permission to perform this operation. " +
				"Only the system and members of the Administrator's group can do so.",
		};

		private static readonly Dictionary<int, string> openServiceMessages = new Dictionary<int, string>()
		{
			[Win32API.ERROR_ACCESS_DENIED] = "The handle does not have access to the service.",
			[Win32API.ERROR_INVALID_HANDLE] = "The specified handle is invalid.",
			[Win32API.ERROR_INVALID_NAME] = "The specified service name is invalid.",
			[Win32API.ERROR_SERVICE_DOES_NOT_EXIST] = "The specified service does not exist.",
		};

		private static readonly WeakReference<ServiceControlManager> current = new WeakReference<ServiceControlManager>(null);
		private ServiceEvents events;
		#endregion

		#region Properties

		/// <summary>
		/// Gets the handle to the service control manager database.
		/// </summary>
		public IntPtr Handle { get; }

		/// <summary>
		/// Gets the access rights used to open the SCM.
		/// </summary>
		public AccessRights AccessRights { get; }

		/// <summary>
		/// Gets value indicates if this object has disposed.
		/// </summary>
		public bool IsClosed { get; private set; }

		/// <summary>
		/// Gets the SCM on the current machine, with read-only access
		/// </summary>
		public static ServiceControlManager CurrentMachine
		{
			get
			{
				ServiceControlManager current;

				if (!ServiceControlManager.current.TryGetTarget(out current) ||
					current.IsClosed)
				{
					current = new ServiceControlManager();
					ServiceControlManager.current.SetTarget(current);
				}

				return current;
			}
		}

		/// <summary>
		/// Gets the name of the machine in which the current SCM resides.
		/// </summary>
		public string MachineName { get; }

		/// <summary>
		/// Gets collection of services' statuses that in the current SCM database.
		/// </summary>
		public ServiceCollection Services { get; }
		#endregion

		#region Delegates

		private unsafe delegate bool GetServiceUniqueNameDelegate(
			IntPtr hSCManager,
			string inputName,
			char* pOutputName,
			ref uint buffSize);
		#endregion

		#region Events
		
		#region Delegates

		private Dictionary<Notification, EventHandler<ServiceControlEventArgs>> handlers =
			new Dictionary<Notification, EventHandler<ServiceControlEventArgs>>()
			{
				[Notification.Created] = (s, e) => { },
				[Notification.Deleted] = (s, e) => { },
			};
		#endregion

		/// <summary>
		/// Occurs when service is created.
		/// </summary>
		public event EventHandler<ServiceControlEventArgs> ServiceCreated
		{
			add
			{
				this.handlers[Notification.Created] += value;
				HookEvents();
			}
			remove
			{
				this.handlers[Notification.Created] -= value;
			}
		}

		/// <summary>
		/// Occurs when service is deleted.
		/// </summary>
		public event EventHandler<ServiceControlEventArgs> ServiceDeleted
		{
			add
			{
				this.handlers[Notification.Deleted] += value;
				HookEvents();
			}
			remove
			{
				this.handlers[Notification.Deleted] -= value;
			}
		}
		#endregion

		#region Ctor

		/// <summary>
		/// Opens connection to the SCM on the provided machine, with the provided access rights.
		/// </summary>
		/// <param name="machineName">The machine to connect to its SCM database.</param>
		/// <param name="desiredAccess">The desired access right for the connection</param>
		public ServiceControlManager(string machineName, ScmAccessRights desiredAccess)
		{
			this.Handle = Win32API.OpenSCManager(machineName, Win32API.SERVICES_ACTIVE_DATABASE, desiredAccess);

			if (this.Handle == IntPtr.Zero)
			{
				throw ServiceException.Create(creationMessages, Marshal.GetLastWin32Error());
			}

			if (string.IsNullOrEmpty(machineName))
			{
				machineName = DESFAULT_MACHINE_NAME;
			}

			this.MachineName = machineName;
			this.Services = new ServiceCollection(this);
			this.AccessRights = (AccessRights)desiredAccess;
		}

		/// <summary>
		/// Opens connection to the SCM on the provided machine, with the default access rights.
		/// </summary>
		/// <param name="machineName">The machine to connect to its SCM database.</param>
		public ServiceControlManager(string machineName)
			: this(machineName, DEFAULT_ACCESS) { }

		/// <summary>
		/// Opens connection to the SCM on the local machine, with the provided access rights.
		/// </summary>
		/// <param name="desiredAccess">The desired access right for the connection</param>
		public ServiceControlManager(ScmAccessRights desiredAccess)
			: this(null, desiredAccess) { }

		/// <summary>
		/// Opens connection to the SCM on the local machine, with the default access rights.
		/// </summary>
		public ServiceControlManager() : this(DEFAULT_ACCESS) { }

		/// <summary>
		/// Closes the connection if it's opened.
		/// </summary>
		~ServiceControlManager()
		{
			Dispose(false);
		}
		#endregion

		#region Methods

		/// <summary>
		/// Blocks the thread until wanted notification is raised, or until the timeout elapses.
		/// </summary>
		/// <param name="waitFor">
		/// Flags of the notification to wait for. 
		/// If one of the notifications raised - the block ends.
		/// </param>
		/// <param name="millisecondsTimeout">
		/// The timeout, in milliseconds, until the block ends, even if no notification raised.
		/// </param>
		/// <param name="triggered">Return the notification that was actually raised.</param>
		/// <returns>
		/// true if one of notification was raised before the timeout elapsed;
		/// otherwise false.
		/// </returns>
		public bool WaitForNotification(Notification waitFor, int millisecondsTimeout, out Notification triggered)
		{
			HookEvents();

			return this.events.WaitForNotification(waitFor, millisecondsTimeout, out triggered);
		}

		/// <summary>
		/// Blocks the thread until wanted notification is raised, or until the timeout elapses.
		/// </summary>
		/// <param name="waitFor">
		/// Flags of the notification to wait for. 
		/// If one of the notifications raised - the block ends.
		/// </param>
		/// <param name="timeout">
		/// The timeout, until the block ends, even if no notification raised.
		/// </param>
		/// <param name="triggered">Return the notification that was actually raised.</param>
		/// <returns>
		/// true if one of notification was raised before the timeout elapsed;
		/// otherwise false.
		/// </returns>
		public bool WaitForNotification(Notification waitFor, TimeSpan timeout, out Notification triggered) =>
			WaitForNotification(waitFor, timeout.Milliseconds, out triggered);

		/// <summary>
		/// Blocks the thread until wanted notification is raised.
		/// </summary>
		/// <param name="waitFor">
		/// Flags of the notification to wait for. 
		/// If one of the notifications raised - the block ends.
		/// </param>
		/// <returns>The notification that was actually raised.</returns>
		public Notification WaitForNotification(Notification waitFor)
		{
			Notification triggered;
			WaitForNotification(waitFor, Timeout.Infinite, out triggered);

			return triggered;
		}

		private void HookEvents()
		{
			if (this.events == null)
			{
				this.events = new ServiceEvents(
					this.Handle,
					Notification.MSCNotification,
					false);

				this.events.ServiceNotification += Events_ServiceNotification;
			}
		}

		private void Events_ServiceNotification(object sender, ServiceNotificationEventArgs e)
		{
			if ((e.Event & Notification.Created) == Notification.Created)
			{
				IEnumerable<string> createdServices = from svcName in e.ServiceNames
													  where svcName.StartsWith(CREATE_PREFIX)
													  select svcName.Substring(CREATE_PREFIX.Length);

				foreach (string createdName in createdServices)
				{
					OnServiceCreated(createdName);
				}
			}

			if ((e.Event & Notification.Deleted) == Notification.Deleted)
			{
				IEnumerable<string> deletedServices = from svcName in e.ServiceNames
													  where !svcName.StartsWith(CREATE_PREFIX)
													  select svcName;

				foreach (string deletedName in deletedServices)
				{
					OnServiceDeleted(deletedName);
				}
			}
		}

		/// <summary>
		/// Fire the ServiceCreates event
		/// </summary>
		/// <param name="serviceName">The name of the service that has been created</param>
		private void OnServiceCreated(string serviceName)
		{
			this.handlers[Notification.Created](this, new ServiceControlEventArgs(serviceName));
		}

		/// <summary>
		/// Fire the ServiceDeleted event
		/// </summary>
		/// <param name="serviceName">The name of the service that has been deleted</param>
		private void OnServiceDeleted(string serviceName)
		{
			this.handlers[Notification.Deleted](this, new ServiceControlEventArgs(serviceName));
		}
		
		/// <summary>
		/// Reports the boot status to the service control manager. 
		/// It is used by boot verification programs. 
		/// This function can be called only by a process running in the LocalSystem or Administrator's account.
		/// </summary>
		/// <param name="isBootAcceptable">
		/// If the value is TRUE, the system saves the configuration as the last-known good configuration. 
		/// If the value is FALSE, the system immediately reboots, using the previously saved last-known good configuration.
		/// </param>
		public static void NotifyBootConfigStatus(bool isBootAcceptable)
		{
			if (!Win32API.NotifyBootConfigStatus(isBootAcceptable))
			{
				throw ServiceException.Create(notifyBootConfigStatusMessages, Marshal.GetLastWin32Error());
			}
		}

		/// <summary>
		/// Release unmanaged resources
		/// </summary>
		/// <param name="disposing">Value indicates if the object is disposed correctly</param>
		private void Dispose(bool disposing)
		{
			if (!this.IsClosed && (this.Handle != IntPtr.Zero))
			{
				this.IsClosed = true;

				if (!Win32API.CloseServiceHandle(this.Handle))
				{
					var exception = new ServiceException(Marshal.GetLastWin32Error());
					Trace.WriteLine(exception.ToString(), "Error");
				}

				if (disposing)
				{
					if (this.events != null)
					{
						this.events.Dispose(); 
					}
				}
			}
		}

		void IDisposable.Dispose() =>
			Close();

		/// <summary>
		/// Closes the connection to the SCM database, and releases unmanaged resources/
		/// </summary>
		public void Close()
		{
			if (!this.IsClosed)
			{
				Dispose(true);
				this.IsClosed = true;
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Creates a service object and adds it to the service control manager database.
		/// </summary>
		/// <param name="name">
		/// The name of the service to install.
		/// <remarks>
		/// The maximum string length is 256 characters. 
		/// The service control manager database preserves the case of the characters, 
		/// but service name comparisons are always case insensitive. 
		/// Forward-slash (/) and backslash (\) are not valid service name characters.
		/// </remarks>
		/// </param>
		/// <param name="displayName">
		/// The display name to be used by user interface programs to identify the service. 
		/// <remarks>
		/// This string has a maximum length of 256 characters. 
		/// The name is case-preserved in the service control manager. 
		/// Display name comparisons are always case-insensitive.
		/// </remarks>
		/// </param>
		/// <param name="desiredAccess">The access to the service.</param>
		/// <param name="type">The service type.</param>
		/// <param name="startType">The service start options.</param>
		/// <param name="errorControl">
		/// The severity of the error, and action taken, if this service fails to start.
		/// </param>
		/// <param name="binaryPath">
		/// The fully qualified path to the service binary file. 
		/// If the path contains a space, it must be quoted so that it is correctly interpreted. 
		/// For example, "d:\\my share\\myservice.exe" should be specified as "\"d:\\my share\\myservice.exe\"".
		/// <remarks>
		/// The path can also include arguments for an auto-start service. 
		/// For example, "d:\\myshare\\myservice.exe arg1 arg2". 
		/// These arguments are passed to the service entry point (typically the main function).
		/// 
		/// If you specify a path on another computer, 
		/// the share must be accessible by the computer account of the local computer because this is 
		/// the security context used in the remote call. 
		/// However, this requirement allows any potential vulnerabilities in the remote computer to affect 
		/// the local computer. Therefore, it is best to use a local file.
		/// </remarks>
		/// </param>
		/// <param name="loadOrderGroup">
		/// The names of the load ordering group of which this service is a member. 
		/// Specify NULL or an empty string if the service does not belong to a group.
		/// </param>
		/// <param name="tagId">
		/// The tag value that is unique in the group specified in the lpLoadOrderGroup parameter. 
		/// Specify NULL if you are not changing the existing tag.
		/// </param>
		/// <param name="dependencies">
		/// A collection of names of services or load ordering groups that the system must start before this service.
		/// <remarks>
		/// You must prefix group names with Service.SC_GROUP_IDENTIFIER so that they can be distinguished from a service name, 
		/// because services and service groups share the same name space.
		/// </remarks>
		/// </param>
		/// <param name="serviceAccount">
		/// The name of the account under which the service should run. 
		/// If the service type is OwnProcess, use an account name in the form DomainName\UserName. 
		/// The service process will be logged on as this user. 
		/// If the account belongs to the built-in domain, you can specify .\UserName.
		/// 
		/// If this parameter is NULL, CreateService uses the LocalSystem account. 
		/// If the service type specifies Interactive process, the service must run in the LocalSystem account.
		/// 
		/// If this parameter is NT AUTHORITY\LocalService, CreateService uses the LocalService account. 
		/// If the parameter is NT AUTHORITY\NetworkService, CreateService uses the NetworkService account.
		/// 
		/// A shared process can run as any user.
		/// 
		/// If the service type is SERVICE_KERNEL_DRIVER or SERVICE_FILE_SYSTEM_DRIVER, 
		/// the name is the driver object name that the system uses to load the device driver. 
		/// Specify NULL if the driver is to use a default object name created by the I/O system.
		/// 
		/// A service can be configured to use a managed account or a virtual account. 
		/// If the service is configured to use a managed service account, the name is the managed service account name. 
		/// If the service is configured to use a virtual account, specify the name as NT SERVICE\ServiceName. 
		/// </param>
		/// <param name="password">
		/// The password to the account name specified by the serviceAccount parameter. 
		/// Specify an empty string if the account has no password or if the service runs in
		/// the LocalService, NetworkService, or LocalSystem account.
		/// </param>
		/// <returns>The created service.</returns>
		public Service CreateService(
			string name,
			string displayName,
			ServiceAccessRights desiredAccess,
			ServiceType type,
			StartType startType,
			ErrorControl errorControl,
			string binaryPath,
			string loadOrderGroup = null,
			uint? tagId = null,
			IEnumerable<string> dependencies = null,
			string serviceAccount = null,
			string password = null) =>
			GetOrThrowIfDisposed(() => 
				Service.Create(
					this,
					name,
					displayName,
					desiredAccess,
					type,
					startType,
					errorControl,
					binaryPath,
					loadOrderGroup,
					tagId,
					dependencies,
					serviceAccount,
					password));

		private void ThrowIfDisposed()
		{
			if (this.IsClosed)
			{
				throw new ObjectDisposedException(this.MachineName);
			}
		}

		private TResult GetOrThrowIfDisposed<TResult>(Func<TResult> getter)
		{
			if (getter == null)
			{
				throw new NullReferenceException(nameof(getter));
			}

			ThrowIfDisposed();
			return getter();
		}

		/// <summary>
		/// Retrieves the display name of the specified service.
		/// </summary>
		/// <param name="serviceName">The service name. This name is the same as the service's registry key name.</param>
		/// <returns>The service's display name.</returns>
		public unsafe string GetServiceDisplayName(string serviceName) =>
			GetServiceUniqueName(serviceName, Win32API.GetServiceDisplayName);

		private unsafe string GetServiceUniqueName(string inputName, GetServiceUniqueNameDelegate getServiceUniqueName)
		{
			ThrowIfDisposed();

			char* pOutputName = null;
			int lastError;
			uint length = 0;

			try
			{
				do
				{
					if (length != 0)
					{
						// Including the null terminator
						length++;

						pOutputName = pOutputName == null
							? (char*)Marshal.AllocHGlobal((int)(length * sizeof(char)))
							: (char*)Marshal.ReAllocHGlobal(
								(IntPtr)pOutputName,
								(IntPtr)(length * sizeof(char)));
					}

					lastError = getServiceUniqueName(this.Handle, inputName, pOutputName, ref length)
						? Win32API.ERROR_SUCCESS
						: Marshal.GetLastWin32Error();

				} while (lastError == Win32API.ERROR_INSUFFICIENT_BUFFER);

				if (lastError != Win32API.ERROR_SUCCESS)
				{
					throw new ServiceException(lastError);
				}

				return new string(pOutputName);
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pOutputName);
			}
		}

		/// <summary>
		/// Retrieves the service name of the specified service.
		/// </summary>
		/// <param name="displayName">The service display name.</param>
		/// <returns>The service name.</returns>
		public unsafe string GetServiceName(string displayName) =>
			GetServiceUniqueName(displayName, Win32API.GetServiceKeyName);

		internal IntPtr GetServiceHandle(string serviceName) =>
			GetServiceHandle(serviceName, (ServiceAccessRights)GetDefaultAccessRights());

		internal IntPtr GetServiceHandle(string serviceName, ServiceAccessRights desiredAccess)
		{
			ThrowIfDisposed();

			IntPtr hService = Win32API.OpenService(this.Handle, serviceName, desiredAccess);

			if (hService == IntPtr.Zero)
			{
				throw ServiceException.Create(openServiceMessages, Marshal.GetLastWin32Error());
			}

			return hService;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceName"></param>
		/// <returns></returns>
		public Service OpenService(string serviceName) =>
			OpenService(serviceName, (ServiceAccessRights)GetDefaultAccessRights());

		/// <summary>
		/// Opens an existing service.
		/// </summary>
		/// <param name="serviceName">The name of the service to be opened. </param>
		/// <param name="desiredAccess">The access rights to the service.</param>
		/// <returns>Service instance for therequested service.</returns>
		public Service OpenService(string serviceName, ServiceAccessRights desiredAccess) =>
			new Service(this, GetServiceHandle(serviceName, desiredAccess), serviceName);

		private uint GetDefaultAccessRights()
		{
			var desiredAccess = AccessRights.SvcGenericRead;

			if ((this.AccessRights & AccessRights.ScmGeenricWrite) == AccessRights.ScmGeenricWrite)
			{
				desiredAccess |= AccessRights.SvcGenericWrite;
			}

			if ((this.AccessRights & AccessRights.ScmGenericExecute) == AccessRights.ScmGenericExecute)
			{
				desiredAccess |= AccessRights.SvcGenericExecute;
			}

			if ((this.AccessRights & AccessRights.ScmAllAccess) == AccessRights.ScmAllAccess)
			{
				desiredAccess |= AccessRights.SvcAllAccess;
			}

			return (uint)desiredAccess;
		}

		/// <summary>
		/// Asynchronically waits for notification to be triggered.
		/// </summary>
		/// <param name="waitFor">Flags of the notification to be waited.</param>
		/// <param name="millisecondsTimeout">The timeout in milliseconds.</param>
		/// <returns>Task for the wait operation.</returns>
		public Task<TimedResult<Notification>> WaitForNotificationAsync(
			Notification waitFor, 
			int millisecondsTimeout) =>
			Task.Factory.StartNew(
				() =>
				{
					Notification triggered;
					bool didTimedOut = !WaitForNotification(waitFor, millisecondsTimeout, out triggered);

					return didTimedOut
						? new TimedResult<Notification>()
						: new TimedResult<Notification>(triggered);
				}
			,
			TaskCreationOptions.LongRunning);

		/// <summary>
		/// Asynchronically waits for notification to be triggered.
		/// </summary>
		/// <param name="waitFor">Flags of the notification to be waited.</param>
		/// <param name="timeout">The timeout before wait is cancelled.</param>
		/// <returns>Task for the wait operation.</returns>
		public Task<TimedResult<Notification>> WaitForNotificationAsync(
			Notification waitFor,
			TimeSpan timeout) =>
			WaitForNotificationAsync(waitFor, timeout.Milliseconds);

		/// <summary>
		/// Asynchronically waits for notification to be triggered.
		/// </summary>
		/// <param name="waitFor">Flags of the notification to be waited.</param>
		/// <returns>Task for the wait operation.</returns>
		public async Task<Notification> WaitForNotificationAsync(Notification waitFor) =>
			(await WaitForNotificationAsync(waitFor, Timeout.Infinite)).Result;
		#endregion
	}

	/// <summary>
	/// Event args for service control manager events
	/// </summary>
	public sealed class ServiceControlEventArgs : EventArgs
	{
		#region Properties

		/// <summary>
		/// Gets the service name of the relevant service.
		/// </summary>
		public string ServiceName { get; }
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new event args instance
		/// </summary>
		/// <param name="serviceName">The service name of the relevant service.</param>
		public ServiceControlEventArgs(string serviceName)
		{
			this.ServiceName = serviceName;
		}
		#endregion
	}
}
