using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Windows.Interop;
using Windows.Services.Interop;

namespace Windows.Services
{
	partial class Service
	{
		#region Properties

		/// <summary>
		/// Gets value indicates if the current instance has been disposed.
		/// </summary>
		public bool IsClosed { get; private set; }

		/// <summary>
		/// Gets the name of the service
		/// </summary>
		public string ServiceName
		{
			get
			{
				ThrowIfDisposed();
				return this.serviceName.Value;
			}
			private set { this.serviceName = new Lazy<string>(() => value); }
		}

		/// <summary>
		/// Gets or sets the display name to be used by applications to identify the service for its users. 
		/// </summary>
		public string DisplayName
		{
			get
			{
				ThrowIfDisposed();
				return this.displayName.Value;
			}
			set
			{
				ThrowIfDisposed();

				value = value ?? "";

				if (this.isInitialized)
				{
					SetConfig(displayName: value);
				}

				this.displayName = new Lazy<string>(() => value);
			}
		}

		/// <summary>
		/// Gets or sets the type of service.
		/// </summary>
		public ServiceType Type
		{
			get
			{
				ThrowIfDisposed();
				return this.type.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (value == ServiceType.NoChange)
				{
					throw new ArgumentException("NoChange is not a valid value for type");
				}

				if (this.isInitialized)
				{
					SetConfig(type: value);
				}

				this.type = new Lazy<ServiceType>(() => value);
			}
		}

		/// <summary>
		/// Gets or sets the service start options.
		/// </summary>
		public StartType StartType
		{
			get
			{
				ThrowIfDisposed();
				return this.startType.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (value == StartType.NoChange)
				{
					throw new ArgumentException("NoChange is not a valid value for start type");
				}

				if (this.isInitialized)
				{
					SetConfig(startType: value);
				}

				this.startType = new Lazy<StartType>(() => value);
			}
		}

		/// <summary>
		/// Gets or sets the severity of the error, and action taken, if this service fails to start.
		/// </summary>
		public ErrorControl ErrorControl
		{
			get
			{
				ThrowIfDisposed();
				return this.errorControl.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (value == ErrorControl.NoChange)
				{
					throw new ArgumentException("NoChange is not a valid value for start type");
				}

				if (this.isInitialized)
				{
					SetConfig(errorControl: value);
				}

				this.errorControl = new Lazy<ErrorControl>(() => value);
			}
		}

		/// <summary>
		/// Gets or sets the fully qualified path to the service binary file. 
		/// </summary>
		/// <remarks>
		/// If the path contains a space, it must be quoted so that it is correctly interpreted. 
		/// For example, "d:\\my share\\myservice.exe" should be specified as "\"d:\\my share\\myservice.exe\"".
		/// 
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
		public string BinaryPath
		{
			get
			{
				ThrowIfDisposed();
				return this.binaryPath.Value;
			}
			set
			{
				ThrowIfDisposed();

				value = value ?? "";

				if (this.isInitialized)
				{
					SetConfig(binaryPath: value);
				}

				this.binaryPath = new Lazy<string>(() => value);
			}
		}

		/// <summary>
		/// Gets or sets the name of the load ordering group of which this service is a member. 
		/// </summary>
		/// <remarks>
		/// The startup program uses load ordering groups to load groups of services in a specified
		/// order with respect to the other groups. 
		/// The list of load ordering groups is contained in the ServiceGroupOrder value of the following registry key:
		/// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control
		/// </remarks>
		public string LoadOrderGroup
		{
			get
			{
				ThrowIfDisposed();
				return this.loadOrderGroup.Value;
			}
			set
			{
				ThrowIfDisposed();

				value = value ?? "";

				if (this.isInitialized)
				{
					SetConfig(loadOrderGroup: value);
				}

				this.loadOrderGroup = new Lazy<string>(() => value);
			}
		}

		/// <summary>
		/// Gets or sets that is unique in the group specified in the lpLoadOrderGroup parameter.
		/// </summary>
		public uint Tag
		{
			get
			{
				ThrowIfDisposed();
				return this.tag.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					SetConfig(tag: value);
				}

				this.tag = new Lazy<uint>(() => value);
			}
		}

		/// <summary>
		/// Gets a collection of names of services or load ordering groups that the system must start before 
		/// this service can be started. 
		/// (Dependency on a group means that this service can run if at least one member of the group 
		/// is running after an attempt to start all members of the group.)
		/// </summary>
		/// <remarks>
		/// You must prefix group names with SC_GROUP_IDENTIFIER so that they can be distinguished from a service name, 
		/// because services and service groups share the same name space.
		/// </remarks>
		public ICollection<string> Dependencies
		{
			get
			{
				ThrowIfDisposed();

				if (this.dependencies == null)
				{
					LoadAndGet<object>(() => null);
				}

				return this.dependencies;
			}
			private set
			{
				if (this.dependencies != null)
				{
					this.dependencies.Changing -= Dependencies_Changing;
				}

				this.dependencies = value != null
					? new MultiString(value)
					: new MultiString();

				this.dependencies.Changing += Dependencies_Changing;
			}
		}

		/// <summary>
		/// Gets or sets the name of the account under which the service should run. 
		/// </summary>
		/// <remarks>
		/// If the service type is OwnProcess, use an account name in the form DomainName\UserName. 
		/// The service process will be logged on as this user. 
		/// If the account belongs to the built-in domain, you can specify .\UserName 
		/// (note that the corresponding C/C++ string is ".\\UserName"). 
		/// 
		/// A shared process can run as any user.
		/// 
		/// If the service type is KernelDriver or FileSystemDriver,  the name is the driver object name that 
		/// the system uses to load the device driver. 
		/// Specify NULL if the driver is to use a default object name created by the I/O system.
		/// 
		/// A service can be configured to use a managed account or a virtual account. 
		/// If the service is configured to use a managed service account, the name is the managed service account name. 
		/// If the service is configured to use a virtual account, specify the name as NT SERVICE\ServiceName.
		/// </remarks>
		public string AccountName
		{
			get
			{
				ThrowIfDisposed();
				return this.accountName.Value;
			}
			set
			{
				ThrowIfDisposed();

				if ((this.Type != ServiceType.FileSystemDriver) &&
					(this.Type != ServiceType.KernelDriver))
				{
					value = value ?? "";
				}

				if (this.isInitialized)
				{
					SetConfig(accountName: value);
				}

				this.accountName = new Lazy<string>(() => value);
			}
		}

		/// <summary>
		/// Sets the password to the account name specified by the AccountName parameter. 
		/// Specify an empty string if the account has no password or if the service runs in 
		/// the LocalService, NetworkService, or LocalSystem account.
		/// </summary>
		public string Password
		{
			private get { return null; }
			set
			{
				ThrowIfDisposed();

				value = value ?? "";

				if (this.isInitialized)
				{
					SetConfig(password: value);
				}
			}
		}

		/// <summary>
		/// Gets or sets value that if this member is TRUE, 
		/// the service is started after other auto-start services are started plus a short delay. 
		/// Otherwise, the service is started during system boot.
		/// </summary>
		/// <remarks>This setting is ignored unless the service is an auto-start service.</remarks>
		public bool IsAutoStartDelayed
		{
			get
			{
				ThrowIfDisposed();
				return this.isAutoStartDelayed.Value;
			}
			set
			{
				unsafe
				{
					ThrowIfDisposed();

					if (this.isInitialized)
					{
						ServiceDelayedAutoStartInfo sdasi = new ServiceDelayedAutoStartInfo
						{
							isAutoStartDelayed = value ? 1 : 0,
						};

						if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.DelayedAutoStartInfo, &sdasi))
						{
							int lastError = Marshal.GetLastWin32Error();
							throw FeatureNotSupportedException.GetUnsupportedForCodes(
								new ServiceException(lastError),
								lastError,
								Win32API.ERROR_INVALID_LEVEL);
						}
					}

					this.isAutoStartDelayed = new Lazy<bool>(() => value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the description of the service. 
		/// </summary>
		public string Description
		{
			get
			{
				ThrowIfDisposed();
				return this.description.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					unsafe
					{
						fixed (char* lpDescription = value)
						{
							ServiceDescription sd = new ServiceDescription
							{
								lpDescription = lpDescription,
							};

							if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.Description, &sd))
							{
								throw new ServiceException(Marshal.GetLastWin32Error());
							}
						}
					}
				}

				this.description = new Lazy<string>(() => value);
			}
		}

		/// <summary>
		/// Gets the failure actions for the service.
		/// </summary>
		public FailureActions FailureActions { get; private set; }

		/// <summary>
		/// Gets or sets the node number of the preferred node.
		/// Specify null to delete the prefered node.
		/// </summary>
		public ushort? PreferedNodeId
		{
			get
			{
				ThrowIfDisposed();
				return this.preferedNodeId.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					ServicePreferedNodeInfo spni = new ServicePreferedNodeInfo
					{
						isDeleted = !value.HasValue,
						preferedNode = value.HasValue
							? value.Value
							: default(ushort),
					};

					unsafe
					{
						if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.PreferredNode, &spni))
						{
							int lastError = Marshal.GetLastWin32Error();
							throw FeatureNotSupportedException.GetUnsupportedForCodes(
								new ServiceException(lastError),
								lastError,
								Win32API.ERROR_INVALID_LEVEL);
						}
					}
				}

				this.preferedNodeId = new Lazy<ushort?>(() => value);
			}
		}

		/// <summary>
		/// Gets or sets the time-out value, in milliseconds.
		/// </summary>
		/// <remarks>
		/// The default preshutdown time-out value is 180,000 milliseconds (three minutes).
		/// 
		/// After the service control manager sends the Shutdown notification to the HandlerEx function, 
		/// it waits for one of the following to occur before proceeding with other shutdown actions: 
		/// the specified time elapses or the service enters the Stopped state. 
		/// 
		/// The service can continue to update its status for as long as it is in the StopPending state.
		/// </remarks>
		public int PreShutdownTimeout
		{
			get
			{
				ThrowIfDisposed();
				return this.preShutdownTimeout.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (value < 0)
				{
					throw new ArgumentException("PreShutdownTimeout cannot be less than zero", "PreShutdownTimeout");
				}

				if (this.isInitialized)
				{
					var spsi = new ServicePreShutdownInfo
					{
						timeout = (uint)value,
					};

					unsafe
					{
						if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.PreshutdownInfo, &spsi))
						{
							int lastError = Marshal.GetLastWin32Error();

							throw FeatureNotSupportedException.GetUnsupportedForCodes(
								new ServiceException(lastError),
								lastError,
								Win32API.ERROR_INVALID_LEVEL);
						}
					}
				}

				this.preShutdownTimeout = new Lazy<int>(() => value);
			}
		}

		/// <summary>
		/// Gets a collection of the required privileges for the service.
		/// </summary>
		public ICollection<string> RequiredPrivileges
		{
			get
			{
				ThrowIfDisposed();

				if (this.requiredPrivileges == null)
				{
					LoadRquiredPrivileges();
				}

				return this.requiredPrivileges;
			}
			private set
			{
				if (this.requiredPrivileges != null)
				{
					this.requiredPrivileges.Changing -= RequiredPrivileges_Changing;
				}

				this.requiredPrivileges = value != null
					? new MultiString(value)
					: new MultiString();

				if (this.requiredPrivileges != null)
				{
					this.requiredPrivileges.Changing += RequiredPrivileges_Changing;
				}
			}
		}

		/// <summary>
		/// Gets or sets the service SID type.
		/// </summary>
		public SidType SidType
		{
			get
			{
				ThrowIfDisposed();
				return this.sidType.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					ServiceSidInfo ssi = new ServiceSidInfo
					{
						serviceSidType = value,
					};

					unsafe
					{
						if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.ServiceSidInfo, &ssi))
						{
							throw new ServiceException(Marshal.GetLastWin32Error());
						}
					}
				}

				this.sidType = new Lazy<SidType>(() => value);
			}
		}

		/// <summary>
		/// Gets or sets a collection of service triggers
		/// </summary>
		public ReadOnlyCollection<Trigger> Triggers
		{
			get
			{
				ThrowIfDisposed();
				return this.triggers.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					unsafe
					{
						ServiceTriggerInfo sti = new ServiceTriggerInfo();

						try
						{
							sti.triggers = (ServiceTrigger*)Marshal.AllocHGlobal(
								sizeof(ServiceTrigger) * value.Count);
							sti.triggersCount = (uint)value.Count;

							for (int i = 0; i < value.Count; i++)
							{
								value[i].ToUnmanaged(ref sti.triggers[i]);
							}

							if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.TriggerInfo, &sti))
							{
								int lastError = Marshal.GetLastWin32Error();

								throw FeatureNotSupportedException.GetUnsupportedForCodes(
									new ServiceException(lastError),
									lastError,
									Win32API.ERROR_INVALID_LEVEL);
							}
						}
						finally
						{
							if (sti.triggers != null)
							{
								for (int i = 0; i < value.Count; i++)
								{
									Trigger.FreeUnmanaged(ref sti.triggers[i]);
								}

								Marshal.FreeHGlobal((IntPtr)sti.triggers);
							}
						}
					}
				}

				this.triggers = new Lazy<ReadOnlyCollection<Trigger>>(() => value);
			}
		}

		/// <summary>
		/// Gets or sets value that indicates a service protection type.
		/// </summary>
		public LaunchProtected LaunchProtection
		{
			get
			{
				ThrowIfDisposed();
				return this.launchProtection.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					ServiceLaunchProtectedInfo slpi = new ServiceLaunchProtectedInfo
					{
						launchProtected = value,
					};

					unsafe
					{
						if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.LaunchProtected, &slpi))
						{
							int lastError = Marshal.GetLastWin32Error();

							throw FeatureNotSupportedException.GetUnsupportedForCodes(
								new ServiceException(lastError),
								lastError,
								Win32API.ERROR_INVALID_LEVEL);
						}
					}
				}

				this.launchProtection = new Lazy<LaunchProtected>(() => value);
			}
		}

		/// <summary>
		/// Gets the handle of the service.
		/// </summary>
		public IntPtr Handle { get; private set; }

		/// <summary>
		/// Gets the service control database that the service belongs to.
		/// </summary>
		public ServiceControlManager Scm { get; private set; }

		/// <summary>
		/// Gets a collection of the dependent services.
		/// </summary>
		public DependentServicesCollection DependentServices { get; private set; }

		/// <summary>
		/// Gets the service status.
		/// </summary>
		public ServiceStatus Status
		{
			get
			{
				unsafe
				{
					ThrowIfDisposed();
					ServiceStatusProcess ssp = new ServiceStatusProcess();
					uint stub;

					if (!Win32API.QueryServiceStatus(
								this.Handle,
								Win32API.SC_STATUS_PROCESS_INFO,
								&ssp,
								(uint)sizeof(ServiceStatusProcess),
								out stub))
					{
						throw ServiceException.Create(MSGS_SERVICE_STATUS, Marshal.GetLastWin32Error());
					}

					return new ServiceStatus(ssp); 
				}
			}
		}

		/// <summary>
		/// Gets true if the service can be stopped; otherwise false.
		/// </summary>
		public bool CanStop
		{
			get
			{
				return (this.Status.AcceptedControls & AcceptedControls.Stop) == AcceptedControls.Stop;
			}
		}
		 
		/// <summary>
		/// Gets true if the service can be paused or continued; otherwise false.
		/// </summary>
		public bool CanPauseContinue 
		{
			get
			{
				return (this.Status.AcceptedControls & AcceptedControls.PauseContinue) == AcceptedControls.PauseContinue;
			}
		}
		#endregion
	}
}