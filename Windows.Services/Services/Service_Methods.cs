using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	partial class Service
	{
		#region Consts

		private const string LOCAL_SYSTEM = "LocalSystem";
		private const int RECHECK_TIMEOUT = 500;

		/// <summary>The LocalService account name.</summary>
		public const string ACCOUNT_LOCAL_SERVICE = @"NT AUTHORITY\LocalService";
		/// <summary>The NetworkService account name.</summary>
		public const string ACCOUNT_NETWORK_SERVICE = @"NT AUTHORITY\NetworkService";
		/// <summary>The LocalSystem account name.</summary>
		public const string ACCOUNT_LOCAL_SYSTEM = @".\" + LOCAL_SYSTEM;
		#endregion

		#region Fields

		private static readonly HashSet<string> internalAccounts = new HashSet<string>(new[]
			{
				ACCOUNT_LOCAL_SERVICE,
				ACCOUNT_NETWORK_SERVICE,
				ACCOUNT_LOCAL_SYSTEM,
				LOCAL_SYSTEM,
				Environment.MachineName + @"\" + LOCAL_SYSTEM,

			},
			StringComparer.InvariantCultureIgnoreCase);
		#endregion

		#region Methods

		private void HookEvents()
		{
			if (this.events == null)
			{
				this.events = new ServiceEvents(
					this.Handle,
					Notification.ServiceNotification,
					true);
				this.events.ServiceNotification += Events_ServiceNotification;
			}
		}

		/// <summary>
		/// Sets the service's user account
		/// </summary>
		/// <param name="accountName">
		/// The name of the account under which the service should run. 
		/// If the service type is OwnProcess, use an account name in the form DomainName\UserName. 
		/// The service process will be logged on as this user. 
		/// If the account belongs to the built-in domain, you can specify .\UserName 
		/// (note that the corresponding C/C++ string is ".\\UserName").
		/// 
		/// You can user one of the account constants.
		/// </param>
		/// <param name="password">
		/// The password to the account name specified by the accountName parameter.
		/// Specify an empty string if the account has no password or 
		/// if the service runs in the LocalService, NetworkService, or LocalSystem account.
		/// </param>
		public void SetUserAccount(string accountName, string password)
		{
			accountName = accountName ?? "";

			password = internalAccounts.Contains(accountName)
				? ""
				: password ?? "";

			SetConfig(accountName: accountName, password: password);
		}

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
		/// <remarks>
		/// This method is suppoeted only on Windows Vista and above.
		/// For older operating systems, use WaitForState.
		/// </remarks>
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
		/// <remarks>
		/// This method is suppoeted only on Windows Vista and above.
		/// For older operating systems, use WaitForState.
		/// </remarks>
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
		/// <remarks>
		/// This method is suppoeted only on Windows Vista and above.
		/// For older operating systems, use WaitForState.
		/// </remarks>
		public Notification WaitForNotification(Notification waitFor)
		{
			Notification triggered;
			WaitForNotification(waitFor, Timeout.Infinite, out triggered);

			return triggered;
		}

		/// <summary>
		/// Asynchronically waits for notification to be triggered.
		/// </summary>
		/// <param name="waitFor">Flags of the notification to be waited.</param>
		/// <param name="millisecondsTimeout">The timeout in milliseconds.</param>
		/// <returns>Task for the wait operation.</returns>
		/// <remarks>
		/// This method is suppoeted only on Windows Vista and above.
		/// For older operating systems, use WaitForStateAsync.
		/// </remarks>
		public Task<TimedResult<Notification>> WaitForNotificationAsync(
			Notification waitFor,
			int millisecondsTimeout)
		{
			return Task.Factory.StartNew(
				() =>
				{
					Notification triggered;
					bool didTimedOut = !WaitForNotification(waitFor, millisecondsTimeout, out triggered);

					return didTimedOut
						? new TimedResult<Notification>()
						: new TimedResult<Notification>(triggered);
				},
				TaskCreationOptions.LongRunning);
		}

		/// <summary>
		/// Asynchronically waits for notification to be triggered.
		/// </summary>
		/// <param name="waitFor">Flags of the notification to be waited.</param>
		/// <param name="timeout">The timeout before wait is cancelled.</param>
		/// <returns>Task for the wait operation.</returns>
		/// <remarks>
		/// This method is suppoeted only on Windows Vista and above.
		/// For older operating systems, use WaitForStateAsync.
		/// </remarks>
		public Task<TimedResult<Notification>> WaitForNotificationAsync(
			Notification waitFor,
			TimeSpan timeout) =>
			WaitForNotificationAsync(waitFor, timeout.Milliseconds);

		/// <summary>
		/// Asynchronically waits for notification to be triggered.
		/// </summary>
		/// <param name="waitFor">Flags of the notification to be waited.</param>
		/// <returns>Task for the wait operation.</returns>
		/// <remarks>
		/// This method is suppoeted only on Windows Vista and above.
		/// For older operating systems, use WaitForStateAsync.
		/// </remarks>
		public async Task<Notification> WaitForNotificationAsync(Notification waitFor) =>
			(await WaitForNotificationAsync(waitFor, Timeout.Infinite)).Result;

		private void Events_ServiceNotification(object sender, ServiceNotificationEventArgs e)
		{
			OnEvent(e.Event, e.Status);
		}

		private unsafe LaunchProtected GetLaunchProtection()
		{
			ServiceLaunchProtectedInfo slpi = new ServiceLaunchProtectedInfo();
			uint stub;

			if (!Win32API.QueryServiceOptionalConfig(
				this.Handle,
				Config.LaunchProtected,
				&slpi,
				(uint)sizeof(ServiceLaunchProtectedInfo),
				out stub))
			{
				int lastError = Marshal.GetLastWin32Error();

				throw FeatureNotSupportedException.GetUnsupportedForCodes(
					new ServiceException(lastError),
					lastError,
					Win32API.ERROR_INVALID_LEVEL);
			}

			return slpi.launchProtected;
		}

		private unsafe ReadOnlyCollection<Trigger> GetTriggers()
		{
			ServiceTriggerInfo* pST = null;
			uint allocated = 0;
			uint needed = 0;

			try
			{
				int lastError;

				do
				{
					if (needed != 0)
					{
						if (pST == null)
						{
							pST = (ServiceTriggerInfo*)Marshal.AllocHGlobal((int)needed);
						}
						else
						{
							pST = (ServiceTriggerInfo*)Marshal.ReAllocHGlobal((IntPtr)pST, (IntPtr)needed);
						}

						allocated = needed;
					}

					if (Win32API.QueryServiceOptionalConfig(
						this.Handle,
						Config.TriggerInfo,
						pST, allocated,
						out needed))
					{
						lastError = Win32API.ERROR_SUCCESS;
					}
					else
					{
						lastError = Marshal.GetLastWin32Error();
					}
				}
				while (lastError == Win32API.ERROR_INSUFFICIENT_BUFFER);

				if (lastError != Win32API.ERROR_SUCCESS)
				{
					throw FeatureNotSupportedException.GetUnsupportedForCodes(
						new ServiceException(lastError),
						lastError,
						Win32API.ERROR_INVALID_LEVEL);
				}

				var triggers = new Trigger[pST->triggersCount];

				for (int i = 0; i < triggers.Length; i++)
				{
					triggers[i] = new Trigger(ref pST->triggers[i]);
				}

				return Array.AsReadOnly(triggers);
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pST);
			}
		}

		private unsafe SidType GetSidType()
		{
			ServiceSidInfo ssi = new ServiceSidInfo();
			uint stub;

			if (!Win32API.QueryServiceOptionalConfig(
				this.Handle,
				Config.ServiceSidInfo,
				&ssi,
				(uint)sizeof(ServiceSidInfo),
				out stub))
			{
				throw new ServiceException(Marshal.GetLastWin32Error());
			}

			return ssi.serviceSidType;
		}

		private unsafe void RequiredPrivileges_Changing(object sender, CancelEventArgs e)
		{
			string requiredPrivileges = this.RequiredPrivileges.ToString();

			fixed (char* pmszRequiredPrivileges = requiredPrivileges)
			{
				ServiceRequiredPrivilegesInfo srpi = new ServiceRequiredPrivilegesInfo
				{
					requiredPrivileges = pmszRequiredPrivileges,
				};

				if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.RequiredPrivilegesInfo, &srpi))
				{
					e.Canceled = true;
					int lastError = Marshal.GetLastWin32Error();

					throw FeatureNotSupportedException.GetUnsupportedForCodes(
						new ServiceException(lastError),
						lastError,
						Win32API.ERROR_INVALID_LEVEL);
				}
			}
		}

		private unsafe void LoadRquiredPrivileges()
		{
			ServiceRequiredPrivilegesInfo* pSRP = null;
			uint allocated = 0;
			uint needed = 0;

			try
			{
				int lastError;

				do
				{
					if (needed != 0)
					{
						if (pSRP == null)
						{
							pSRP = (ServiceRequiredPrivilegesInfo*)Marshal.AllocHGlobal((int)needed);
						}
						else
						{
							pSRP = (ServiceRequiredPrivilegesInfo*)Marshal.ReAllocHGlobal(
								(IntPtr)pSRP,
								(IntPtr)needed);
						}

						allocated = needed;
					}

					if (Win32API.QueryServiceOptionalConfig(
						this.Handle,
						Config.RequiredPrivilegesInfo,
						pSRP,
						allocated,
						out needed))
					{
						lastError = Win32API.ERROR_SUCCESS;
					}
					else
					{
						lastError = Marshal.GetLastWin32Error();
					}
				} while (lastError == Win32API.ERROR_INSUFFICIENT_BUFFER);

				if (lastError != Win32API.ERROR_SUCCESS)
				{
					throw FeatureNotSupportedException.GetUnsupportedForCodes(
						new ServiceException(lastError),
						lastError,
						Win32API.ERROR_INVALID_LEVEL);
				}

				this.requiredPrivileges = new MultiString(pSRP->requiredPrivileges);
				this.requiredPrivileges.Changing += RequiredPrivileges_Changing;
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pSRP);
			}
		}

		private unsafe int GetPreShutdownTimeout()
		{
			var spsi = new ServicePreShutdownInfo();
			uint stub;

			if (!Win32API.QueryServiceOptionalConfig(
				this.Handle,
				Config.PreshutdownInfo,
				&spsi,
				(uint)sizeof(ServicePreShutdownInfo),
				out stub))
			{
				int lastError = Marshal.GetLastWin32Error();

				throw FeatureNotSupportedException.GetUnsupportedForCodes(
					new ServiceException(lastError),
					lastError,
					Win32API.ERROR_INVALID_LEVEL);
			}

			return (int)spsi.timeout;
		}

		private unsafe ushort? GetPreferedNodeId()
		{
			ServicePreferedNodeInfo spni = new ServicePreferedNodeInfo();
			uint stub;
			ushort? preferedNode;

			if (Win32API.QueryServiceOptionalConfig(
				this.Handle,
				Config.PreferredNode,
				&spni,
				(uint)sizeof(ServicePreferedNodeInfo),
				out stub))
			{
				preferedNode = !spni.isDeleted
				? (ushort?)spni.preferedNode
				: null;
			}
			else
			{
				int lastError = Marshal.GetLastWin32Error();

				throw FeatureNotSupportedException.GetUnsupportedForCodes(
					new ServiceException(lastError),
					lastError,
					Win32API.ERROR_INVALID_LEVEL,
					Win32API.ERROR_INVALID_PARAMETER);
			}

			return preferedNode;
		}

		//public Task

		private unsafe string GetDescription()
		{
			ServiceDescription* pSD = null;
			uint allocated = 0;
			uint needed = 0;

			try
			{
				int lastError;

				do
				{
					if (needed != 0)
					{
						if (pSD == null)
						{
							pSD = (ServiceDescription*)Marshal.AllocHGlobal((int)needed);
						}
						else
						{
							pSD = (ServiceDescription*)Marshal.ReAllocHGlobal((IntPtr)pSD, (IntPtr)needed);
						}

						allocated = needed;
					}

					if (Win32API.QueryServiceOptionalConfig(this.Handle, Config.Description, pSD, allocated, out needed))
					{
						lastError = Win32API.ERROR_SUCCESS;
					}
					else
					{
						lastError = Marshal.GetLastWin32Error();
					}
				}
				while (lastError == Win32API.ERROR_INSUFFICIENT_BUFFER);

				if (lastError != Win32API.ERROR_SUCCESS)
				{
					throw new ServiceException(lastError);
				}

				return new string(pSD->lpDescription);
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pSD);
			}
		}

		private unsafe bool GetIsAutoStartDelayed()
		{
			ServiceDelayedAutoStartInfo sdasi = new ServiceDelayedAutoStartInfo();
			uint stab;

			if (!Win32API.QueryServiceOptionalConfig(
				this.Handle,
				Config.DelayedAutoStartInfo,
				&sdasi,
				(uint)sizeof(ServiceDelayedAutoStartInfo),
				out stab))
			{
				int lastError = Marshal.GetLastWin32Error();
				throw FeatureNotSupportedException.GetUnsupportedForCodes(
					new ServiceException(lastError),
					lastError,
					Win32API.ERROR_INVALID_LEVEL);
			}

			return sdasi.isAutoStartDelayed != 0;
		}

		private unsafe void SetConfig(
			ServiceType type = ServiceType.NoChange,
			StartType startType = StartType.NoChange,
			ErrorControl errorControl = ErrorControl.NoChange,
			string binaryPath = null,
			string loadOrderGroup = null,
			uint? tag = null,
			string dependenciesMultiString = null,
			string accountName = null,
			string password = null,
			string displayName = null)
		{
			uint newTagId = tag ?? 0;

			fixed (char* lpDependencies = dependenciesMultiString)
			{
				if (!Win32API.ChangeServiceConfig(
					this.Handle,
					type,
					startType,
					errorControl,
					binaryPath,
					loadOrderGroup,
					tag != null
						? &newTagId
						: null,
					lpDependencies,
					accountName,
					password,
					displayName))
				{
					throw ServiceException.Create(setConfigMessages, Marshal.GetLastWin32Error());
				}
			}
		}

		internal void ThrowIfDisposed()
		{
			if (this.IsClosed)
			{
				throw new ObjectDisposedException(this.serviceName.Value);
			}
		}

		internal TResult GetOrThrowIfDisposed<TResult>(Func<TResult> getter)
		{
			if (getter == null)
			{
				throw new NullReferenceException(nameof(getter));
			}

			ThrowIfDisposed();
			return getter();
		}

		private unsafe T LoadAndGet<T>(Func<T> getFunc)
		{
			QueryServiceConfig* pQSC = null;

			try
			{
				uint allocated = 0;
				uint needed = 0;
				int lastError;

				do
				{
					if (needed != 0)
					{
						if (pQSC == null)
						{
							pQSC = (QueryServiceConfig*)Marshal.AllocHGlobal((int)needed);
						}
						else
						{
							pQSC = (QueryServiceConfig*)Marshal.ReAllocHGlobal((IntPtr)pQSC, (IntPtr)needed);
						}

						allocated = needed;
					}

					if (Win32API.QueryServiceConfig(this.Handle, pQSC, allocated, out needed))
					{
						lastError = Win32API.ERROR_SUCCESS;
					}
					else
					{
						lastError = Marshal.GetLastWin32Error();
					}
				}
				while (lastError == Win32API.ERROR_INSUFFICIENT_BUFFER);

				if (lastError != Win32API.ERROR_SUCCESS)
				{
					throw ServiceException.Create(loadConfigMessages, lastError);
				}

				ServiceType type = pQSC->type;
				this.type = new Lazy<ServiceType>(() => type);

				StartType startType = pQSC->startType;
				this.startType = new Lazy<StartType>(() => startType);

				ErrorControl errorControl = pQSC->errorControl;
				this.errorControl = new Lazy<ErrorControl>(() => errorControl);

				string binaryPath = pQSC->lpBinaryPathName != null
					? new string(pQSC->lpBinaryPathName)
					: null;
				this.binaryPath = new Lazy<string>(() => binaryPath);

				string laodOrderGroup = pQSC->lpLoadOrderGroup != null
					? new string(pQSC->lpLoadOrderGroup)
					: null;
				this.loadOrderGroup = new Lazy<string>(() => laodOrderGroup);

				uint tag = pQSC->tagId;
				this.tag = new Lazy<uint>(() => tag);

				var multiString = new MultiString(pQSC->lpDependencies);
				multiString.Changing += Dependencies_Changing;
				this.dependencies = multiString;

				string accountName = new string(pQSC->lpServiceStartName);
				this.accountName = new Lazy<string>(() => accountName);

				string displayName = new string(pQSC->lpDisplayName);
				this.displayName = new Lazy<string>(() => displayName);

				this.ServiceName = this.Scm.GetServiceName(this.DisplayName);

				return getFunc();
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pQSC);
			}
		}

		internal static unsafe Service Create(
			ServiceControlManager scm,
			string name,
			string displayName,
			ServiceAccessRights desiredAccess,
			ServiceType type,
			StartType startType,
			ErrorControl errorControl,
			string binaryPath,
			string loadOrderGroup,
			uint? tagId,
			IEnumerable<string> dependencies,
			string serviceAccount,
			string password)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			dependencies = dependencies ?? new string[0];
			var dependenciesMultiStr = new MultiString(dependencies);
			uint newTag = tagId ?? 0;

			fixed (char* pDependencies = dependenciesMultiStr.ToString())
			{
				IntPtr hService = Win32API.CreateService(
					scm.Handle,
					name,
					displayName,
					desiredAccess,
					type,
					startType,
					errorControl,
					binaryPath,
					loadOrderGroup,
					tagId.HasValue
						? &newTag
						: null,
					pDependencies,
					serviceAccount,
					password);

				if (hService == IntPtr.Zero)
				{
					throw ServiceException.Create(createServiceMessages, Marshal.GetLastWin32Error());
				}

				return new Service(
					scm,
					hService,
					name,
					displayName,
					startType,
					errorControl,
					binaryPath,
					loadOrderGroup,
					newTag,
					dependencies.ToArray(),
					serviceAccount,
					password);
			}
		}

		private void Dependencies_Changing(object sender, CancelEventArgs e)
		{
			string dependencies = this.dependencies.ToString();

			try
			{
				SetConfig(dependenciesMultiString: dependencies);
			}
			catch
			{
				e.Canceled = true;
				throw;
			}
		}

		void IDisposable.Dispose()
		{
			Close();
		}

		/// <summary>
		/// Closes the connection to the service, and releases unmanaged resources.
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
		/// Releases unmanaged resources.
		/// </summary>
		/// <param name="disposed">Value indicates if the object is disposed correctly</param>
		private void Dispose(bool disposed)
		{
			Win32API.CloseServiceHandle(this.Handle);

			if (this.events != null)
			{
				this.events.Dispose();
			}
		}

		/// <summary>
		/// Sends a control code to a service.
		/// </summary>
		/// <param name="control">The control code for the service.</param>
		public ServiceStatus SendControl(ControlCode control)
		{
			ThrowIfDisposed();
			Interop.ServiceStatus status;

			if (!Win32API.ControlService(this.Handle, control, out status))
			{
				ServiceException.Create(sendControlMessages, Marshal.GetLastWin32Error());
			}

			return new ServiceStatus(status);
		}

		private unsafe ServiceStatus SendControl(
			ControlCode control,
			StopReasonFlag stopReason,
			string stopComment)
		{
			try
			{
				ThrowIfDisposed();

				fixed (char* lpStopComment = stopComment)
				{
					ServiceControlStatusReasonParams scsrp = new ServiceControlStatusReasonParams
					{
						comment = lpStopComment,
						reason = stopReason,
						serviceStatus = new ServiceStatusProcess(),
					};

					if (!Win32API.ControlService(
						this.Handle,
						control.Code,
						Win32API.SERVICE_CONTROL_STATUS_REASON_INFO,
						&scsrp))
					{
						throw ServiceException.Create(sendControlMessages, Marshal.GetLastWin32Error());
					}

					return new ServiceStatus(scsrp.serviceStatus);
				}
			}
			catch (EntryPointNotFoundException ex)
			{
				throw new FeatureNotSupportedException(ex);
			}
		}

		/// <summary>
		/// Stops the service.
		/// </summary>
		/// <param name="stopReason">
		/// The reason for changing the service status to Stop. 
		/// If the current control code is not Stop, this member is ignored.
		/// </param>
		/// <param name="stopComment">
		/// An optional string that provides additional information about the service stop. 
		/// </param>
		/// <returns>The reported status of the service after the control was processed.</returns>
		public ServiceStatus Stop(
			StopReasonFlag stopReason,
			string stopComment)
		{
			return SendControl(ControlCode.Stop, stopReason, stopComment);
		}

		/// <summary>
		/// Marks the specified service for deletion from the service control manager database.
		/// </summary>
		public void Delete()
		{
			ThrowIfDisposed();

			if (!Win32API.DeleteService(this.Handle))
			{
				throw ServiceException.Create(deleteServiceMessages, Marshal.GetLastWin32Error());
			}
		}

		/// <summary>
		/// Starts a service.
		/// </summary>
		public void Start()
		{
			InternalStart(null);
		}

		/// <summary>
		/// Starts a service.
		/// </summary>
		/// <param name="args">
		/// The null-terminated strings to be passed to the ServiceMain function for the service as arguments.
		/// 
		/// If there are no arguments, this parameter can be NULL. 
		/// Otherwise, the first argument (args[0]) is the name of the service, 
		/// followed by any additional arguments 
		/// (args[1] through args[Length-1]).
		/// </param>
		public void Start(params string[] args)
		{
			InternalStart(args);
		}

		/// <summary>
		/// Starts a service.
		/// </summary>
		/// <param name="args">
		/// The null-terminated strings to be passed to the ServiceMain function for the service as arguments.
		/// 
		/// If there are no arguments, this parameter can be NULL. 
		/// Otherwise, the first argument (args[0]) is the name of the service, 
		/// followed by any additional arguments 
		/// (args[1] through args[Length-1]).
		/// </param>
		private unsafe void InternalStart(string[] args = null)
		{
			char** lpszArgs = null;
			int count = 0;
			string argsString = null;
			MultiString multiStringArgs = null;

			try
			{
				if ((args != null) && (args.Length > 0))
				{
					multiStringArgs = new MultiString(new string[] { this.ServiceName }.Concat(args));
					argsString = multiStringArgs.ToString();
					count = args.Length + 1;

					lpszArgs = (char**)Marshal.AllocHGlobal(sizeof(char*) * (args.Length + 1));
				}

				fixed (char* lpArgsString = argsString)
				{
					for (int i = 0, charIndex = 0;
						i < count;
						charIndex += multiStringArgs[i++].Length + 1)
					{
						lpszArgs[i] = lpArgsString + charIndex;
					}

					if (!Win32API.StartService(this.Handle, (uint)count, lpszArgs))
					{
						throw ServiceException.Create(startServiceMessages, Marshal.GetLastWin32Error());
					}
				}
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)lpszArgs);
			}
		}
		#endregion
	}
}