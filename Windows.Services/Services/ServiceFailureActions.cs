using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Services.Interop;

namespace Windows.Services
{
	/// <summary>
	/// Represents the action the service controller should take on each failure of a service. 
	/// </summary>
	/// <remarks>
	/// A service is considered failed when it terminates without reporting a status of Stopped to the service controller.
	/// </remarks>
	public sealed class FailureActions
	{
		#region Fields

		private Service service;
		private Lazy<int> resetPeriod;
		private Lazy<string> rebootMessage;
		private Lazy<string> command;
		private Lazy<ReadOnlyCollection<ServiceControlAction>> actions;
		private Lazy<bool> isFailOnNonCrash;
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the time after which to reset the failure count to zero if there are no failures, in seconds. 
		/// Specify INFINITE to indicate that this value should never be reset.
		/// </summary>
		public int ResetPeriod
		{
			get
			{
				this.service.ThrowIfDisposed();
				return this.resetPeriod.Value;
			}
			set
			{
				this.service.ThrowIfDisposed();

				SetFailureActions(resetPeriod: (uint)value);
				this.resetPeriod = new Lazy<int>(() => value);
			}
		}

		/// <summary>
		/// Gets or sets the message to be broadcast to server users before rebooting in response to 
		/// the Reboot service controller action.
		/// </summary>
		public string RebootMessage
		{
			get
			{
				this.service.ThrowIfDisposed();
				return this.rebootMessage.Value;
			}
			set
			{
				this.service.ThrowIfDisposed();

				value = value ?? "";

				SetFailureActions(rebootMessage: value);
				this.rebootMessage = new Lazy<string>(() => value);
			}
		}

		/// <summary>
		/// Gets or sets tThe command line of the process for the CreateProcess function 
		/// to execute in response to the RunCommand service controller action. 
		/// This process runs under the same account as the service.
		/// </summary>
		public string Comamnd
		{
			get
			{
				this.service.ThrowIfDisposed();
				return this.command.Value;
			}
			set
			{
				this.service.ThrowIfDisposed();

				value = value ?? "";

				SetFailureActions(command: value);
				this.command = new Lazy<string>(() => value);
			}
		}

		/// <summary>
		/// Gets or sets a collection of ServiceControlAction structures.
		/// </summary>
		public ReadOnlyCollection<ServiceControlAction> Actions
		{
			get
			{
				this.service.ThrowIfDisposed();
				return this.actions.Value;
			}
			set
			{
				this.service.ThrowIfDisposed();

				value = value ?? Array.AsReadOnly(new ServiceControlAction[0]);

				SetFailureActions(actions: value);
				this.actions = new Lazy<ReadOnlyCollection<ServiceControlAction>>(() => value);
			}
		}

		/// <summary>
		/// Gets or set value indicates if failure actions are queued if the service process terminates without 
		/// reporting a status of Stopped or if it enters the Stopped state but the win32ExitCode member of 
		/// the ServiceStatus class is not ERROR_SUCCESS (0).
		/// 
		/// If this member is false and the service has configured failure actions, 
		/// the failure actions are queued only if the service terminates without reporting a status of Stopped.
		/// </summary>
		public bool IsFailOnNonCrash
		{
			get
			{
				this.service.ThrowIfDisposed();
				return this.isFailOnNonCrash.Value;
			}
			set
			{
				unsafe
				{
					ServiceFailureActionsFlag sfaf = new ServiceFailureActionsFlag
					{
						failureActionsOnNonCrashFailures = value,
					};

					if (!Win32API.ChangeServiceOptionalConfig(
						this.service.Handle,
						Config.FailureActionsFlag,
						&sfaf))
					{
						int lastError = Marshal.GetLastWin32Error();
						throw FeatureNotSupportedException.GetUnsupportedForCodes(
							new ServiceException(lastError),
							lastError,
							Win32API.ERROR_INVALID_LEVEL);
					}

					this.isFailOnNonCrash = new Lazy<bool>(() => value);
				}
			}
		}
		#endregion

		#region Ctor

		internal FailureActions(Service service)
		{
			this.service = service;
			this.resetPeriod = new Lazy<int>(() => LoadAndGet(() => this.ResetPeriod));
			this.rebootMessage = new Lazy<string>(() => LoadAndGet(() => this.RebootMessage));
			this.command = new Lazy<string>(() => LoadAndGet(() => this.Comamnd));
			this.actions = new Lazy<ReadOnlyCollection<ServiceControlAction>>(
				() => LoadAndGet(() => this.Actions));
			this.isFailOnNonCrash = new Lazy<bool>(GetIsFailOnNonCrash);
		}
		#endregion

		#region Methods

		private unsafe bool GetIsFailOnNonCrash()
		{
			ServiceFailureActionsFlag sfaf = new ServiceFailureActionsFlag();
			uint stab;

			if (!Win32API.QueryServiceOptionalConfig(
				this.service.Handle,
				Config.FailureActionsFlag,
				&sfaf,
				(uint)sizeof(ServiceFailureActionsFlag),
				out stab))
			{
				int lastError = Marshal.GetLastWin32Error();
				throw FeatureNotSupportedException.GetUnsupportedForCodes(
					new ServiceException(lastError),
					lastError,
					Win32API.ERROR_INVALID_LEVEL);
			}

			return sfaf.failureActionsOnNonCrashFailures;
		}

		private unsafe T LoadAndGet<T>(Func<T> getFunc)
		{
			ServiceFailureActions* pSFA = null;
			uint allocated = 0;
			uint needed = 0;

			try
			{
				int lastError;

				do
				{
					if (needed != 0)
					{
						if (pSFA == null)
						{
							pSFA = (ServiceFailureActions*)Marshal.AllocHGlobal((int)needed);
						}
						else
						{
							pSFA = (ServiceFailureActions*)Marshal.ReAllocHGlobal((IntPtr)pSFA, (IntPtr)needed);
						}

						allocated = needed;
					}

					if (Win32API.QueryServiceOptionalConfig(
						this.service.Handle,
						Config.FailureActions,
						pSFA, allocated,
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
					throw new ServiceException(lastError);
				}

				this.resetPeriod = new Lazy<int>(() => (int)pSFA->resetPeriod);

				this.RebootMessage = pSFA->lpRebootMessage != null
					? new string(pSFA->lpRebootMessage)
					: null;

				this.Comamnd = pSFA->lpCommand != null
					? new string(pSFA->lpCommand)
					: null;

				var actions = new ServiceControlAction[pSFA->actionsCount];

				for (int i = 0; i < actions.Length; i++)
				{
					actions[i] = new ServiceControlAction(
						pSFA->lpActions[i].actionType,
						unchecked((int)pSFA->lpActions[i].delay));
				}

				this.Actions = Array.AsReadOnly(actions);

				return getFunc();
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pSFA);
			}
		}

		private unsafe void SetFailureActions(
			uint resetPeriod = ServiceFailureActions.INFINITE,
			string rebootMessage = null,
			string command = null,
			ReadOnlyCollection<ServiceControlAction> actions = null)
		{
			uint actionsCount = actions != null
				? (uint)actions.Count
				: 0;
			SCAction[] actionsArray = actions != null
				? actions.Select(action =>
							  new SCAction
							  {
								  actionType = action.Action,
								  delay = (uint)action.Delay,
							  })
						.ToArray()
				: null;

			fixed (char* lpRebootMessage = rebootMessage)
			fixed (char* lpCommand = command)
			fixed (SCAction* lpActions = actionsArray)
			{
				var sfa = new ServiceFailureActions
				{
					resetPeriod = resetPeriod,
					lpRebootMessage = lpRebootMessage,
					lpCommand = lpCommand,
					actionsCount = actionsCount,
					lpActions = lpActions,
				};

				if (!Win32API.ChangeServiceOptionalConfig(this.service.Handle, Config.FailureActions, &sfa))
				{
					throw new ServiceException(Marshal.GetLastWin32Error());
				}
			}
		}
		#endregion
	}

	/// <summary>
	/// Represents an action that the service control manager can perform.
	/// </summary>
	public sealed class ServiceControlAction
	{
		#region Propeties

		/// <summary>
		/// Gets the action to be performed.
		/// </summary>
		public ServiceControlActionType Action { get; }

		/// <summary>
		/// Gets the time to wait before performing the specified action, in milliseconds.
		/// </summary>
		public int Delay { get; }
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new ServiceControlAction instance
		/// </summary>
		/// <param name="action">The action to be performed.</param>
		/// <param name="delay">The time to wait before performing the specified action, in milliseconds.</param>
		public ServiceControlAction(ServiceControlActionType action, int delay)
		{
			this.Action = action;
			this.Delay = delay;
		}
		#endregion
	}
}
