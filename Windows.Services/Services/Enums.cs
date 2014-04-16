using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Services
{
	/// <summary>
	/// The type of service
	/// </summary>
	[Flags]
	public enum ServiceType : uint
	{
		/// <summary>No type</summary>
		None,
		/// <summary>Driver services</summary>
		KernelDriver = 0x00000001,
		/// <summary>File system driver services</summary>
		FileSystemDriver = 0x00000002,
		/// <summary>Services of type KernelDriver and FileSystemDriver</summary>
		Driver = 0x0000000B,
		/// <summary>Services that run in their own processes</summary>
		OwnProcess = 0x00000010,
		/// <summary>Services that share a process with one or more other services</summary>
		ShareProcess = 0x00000020,
		/// <summary>Services of type OwnProcess and ShareProcess</summary>
		Win32 = 0x00000030,
		/// <summary>The service can interact with the desktop</summary>
		InteractiveProcess = 0x00000100,
		/// <summary>Specifies not ot change the service</summary>
		NoChange = 0xffffffff,
		/// <summary>Query for all services</summary>
		All = KernelDriver | FileSystemDriver | Driver | Interactice,
		/// <summary>Query for all drivers services</summary>
		DriverService = KernelDriver | FileSystemDriver | Driver,
		/// <summary>Query for win32 interactive services</summary>
		Interactice = Win32 | InteractiveProcess,
		/// <summary>Interactive own process service</summary>
		InteractiveOwnProcess = InteractiveProcess | OwnProcess,
		/// <summary>Interactive shared process service</summary>
		InteractiveSharedProcess = InteractiveProcess | ShareProcess,
	}

	/// <summary>
	/// The type of status changes
	/// </summary>
	[Flags]
	public enum Notification : uint
	{
		/// <summary>No type</summary>
		None,
		/// <summary>The service has stopped</summary>
		Stopped = 0x00000001,
		/// <summary>The service is starting</summary>
		StartPending = 0x00000002,
		/// <summary>The service is stopping</summary>
		StopPending = 0x00000004,
		/// <summary>The service is running</summary>
		Running = 0x00000008,
		/// <summary>The service is about to continue</summary>
		ContinuePending = 0x00000010,
		/// <summary>The service is pausing</summary>
		PausePending = 0x00000020,
		/// <summary>The service has paused</summary>
		Paused = 0x00000040,
		/// <summary>
		/// The service has been created.
		/// The hService parameter must be a handle to the SCM.
		/// </summary>
		Created = 0x00000080,
		/// <summary>
		/// The service has been deleted. 
		/// An application cannot receive this notification if it has an open handle to the service.
		/// The hService parameter must be a handle to the SCM.
		/// </summary>
		Deleted = 0x00000100,
		/// <summary>
		/// An application has specified the service in a call to the DeleteService function. 
		/// Your application should close any handles to the service so it can be deleted.
		/// </summary>
		DeletePending = 0x00000200,
		/// <summary>All MSC notification</summary>
		MSCNotification = Created | Deleted,
		/// <summary>
		/// All service notification
		/// </summary>
		ServiceNotification = Stopped | 
			StartPending | 
			StopPending | 
			Running | 
			ContinuePending | 
			PausePending |
			Paused |
			DeletePending,
	}

	/// <summary>
	/// The current state of the service
	/// </summary>
	public enum State : uint
	{
		/// <summary>Np State</summary>
		None,
		/// <summary>The service is not running</summary>
		Stopped = 1,
		/// <summary>The service is starting</summary>
		StartPending = 2,
		/// <summary>The service is stopping</summary>
		StopPending = 3,
		/// <summary>The service is running</summary>
		Running = 4,
		/// <summary>The service continue is pending</summary>
		ContinuePending = 5,
		/// <summary>The service pause is pending</summary>
		PausePending = 6,
		/// <summary>The service is paused</summary>
		Paused = 7,
	}

	/// <summary>
	/// The state of the services to be enumerated
	/// </summary>
	[Flags]
	public enum StateQuery : uint
	{
		/// <summary>No services would be enumerated</summary>
		None,
		/// <summary>
		/// Enumerates services that are in the following states: 
		/// StartPending, 
		/// StopPending, 
		/// Running,
		/// ContinuePending, 
		/// PausePending, and 
		/// Paused.
		/// </summary>
		Active,
		/// <summary>Enumerates services that are in the Stopped state</summary>
		Inactive,
		/// <summary>Combines the following states: Active and Inactive</summary>
		All
	}

	/// <summary>
	/// The control codes the service accepts and processes in its handler function 
	/// </summary>
	[Flags]
	public enum AcceptedControls : uint
	{
		/// <summary>No control</summary>
		None,
		/// <summary>
		/// The service can be stopped.
		/// </summary>
		Stop = 0x00000001,
		/// <summary>
		/// The service can be paused and continued.
		/// This control code allows the service to receive Pause and Continue notifications.
		/// </summary>
		PauseContinue = 0x00000002,
		/// <summary>
		/// The service can perform preshutdown tasks.
		/// This control code enables the service to receive Shutdown notifications. 
		/// </summary>
		Shutdown = 0x00000004,
		/// <summary>
		/// The service can reread its startup parameters without being stopped and restarted.
		/// This control code allows the service to receive ParamChange notifications.
		/// </summary>
		ParamChange = 0x00000008,
		/// <summary>
		/// The service is a network component that can accept changes in its binding without being stopped and restarted.
		/// This control code allows the service to receive 
		/// NetBindAdd, 
		/// NetBindRemove, 
		/// NetBindEnable, and 
		/// NetBindDisable notifications.
		/// </summary>
		NetBindChange = 0x00000010,
		/// <summary>
		/// The service is notified when the computer's hardware profile has changed. 
		/// This enables the system to send SERVICE_CONTROL_HARDWAREPROFILECHANGE notifications to the service.
		/// </summary>
		HardwareProfileChange = 0x00000020,
		/// <summary>
		/// The service is notified when the computer's power status has changed. 
		/// This enables the system to send SERVICE_CONTROL_POWEREVENT notifications to the service.
		/// </summary>
		PowerEvent = 0x00000040,
		/// <summary>
		/// The service is notified when the computer's session status has changed. 
		/// This enables the system to send SERVICE_CONTROL_SESSIONCHANGE notifications to the service.
		/// </summary>
		SessionChange = 0x00000080,
		/// <summary>
		/// The service can perform preshutdown tasks.
		/// This control code enables the service to receive SERVICE_CONTROL_PRESHUTDOWN notifications. 
		/// </summary>
		PreShutdown = 0x00000100,
		/// <summary>
		/// The service is notified when the system time has changed. 
		/// This enables the system to send SERVICE_CONTROL_TIMECHANGE notifications to the service.
		/// </summary>
		TimeChange = 0x00000200,
		/// <summary>
		/// The service is notified when an event for which the service has registered occurs. 
		/// This enables the system to send SERVICE_CONTROL_TRIGGEREVENT notifications to the service.
		/// </summary>
		TriggerEvent = 0x00000400,
		/// <summary>The services is notified when the user initiates a reboot</summary>
		UserModeReboot = 0x00000800,
	}

	/// <summary>
	/// Controls that can be sent from applications (if the service accepts them
	/// </summary>
	public enum ApplicationControl : uint
	{
		/// <summary>No control</summary>
		None,
		/// <summary>
		/// Notifies a service that it should stop. The hService handle must have the SERVICE_STOP access right.
		/// After sending the stop request to a service, you should not send other controls to the service.
		/// </summary>
		Stop = 0x00000001,
		/// <summary>
		/// Notifies a service that it should pause. 
		/// The hService handle must have the SERVICE_PAUSE_CONTINUE access right.
		/// </summary>
		Pause = 0x00000002,
		/// <summary>
		/// Notifies a paused service that it should resume. 
		/// The hService handle must have the SERVICE_PAUSE_CONTINUE access right.
		/// </summary>
		Continue = 0x00000003,
		/// <summary>
		/// Notifies a service that it should report its current status information to the service control manager.
		/// The hService handle must have the SERVICE_INTERROGATE access right.
		/// Note that this control is not generally useful as the SCM is aware of the current state of the service.
		/// </summary>
		Interrogate = 0x00000004,
		/// <summary>
		/// Notifies a service that its startup parameters have changed.
		/// The hService handle must have the SERVICE_PAUSE_CONTINUE access right.
		/// </summary>
		[Obsolete]
		ParamChange = 0x00000006,
		/// <summary>
		/// Notifies a network service that there is a new component for binding.
		/// The hService handle must have the SERVICE_PAUSE_CONTINUE access right.
		/// However, this control code has been deprecated; use Plug and Play functionality instead.
		/// </summary>
		[Obsolete]
		NetBindAdd = 0x00000007,
		/// <summary>
		/// Notifies a network service that a component for binding has been removed.
		/// The hService handle must have the SERVICE_PAUSE_CONTINUE access right.
		/// However, this control code has been deprecated; use Plug and Play functionality instead.
		/// </summary>
		NetBindRemove = 0x00000008,
		/// <summary>
		/// Notifies a network service that a disabled binding has been enabled.
		/// The hService handle must have the SERVICE_PAUSE_CONTINUE access right.
		/// However, this control code has been deprecated; use Plug and Play functionality instead.
		/// </summary>
		[Obsolete]
		NetBindEnable = 0x00000009,
		/// <summary>
		/// Notifies a network service that one of its bindings has been disabled.
		/// The hService handle must have the SERVICE_PAUSE_CONTINUE access right.
		/// However, this control code has been deprecated; use Plug and Play functionality instead.
		/// </summary>
		[Obsolete]
		NetBindDisable = 0x0000000A,
	}

	/// <summary>
	/// Service process flags
	/// </summary>
	[Flags]
	public enum ProcessFlags : uint
	{
		/// <summary>
		/// The service is running in a process that is not a system process, or it is not running.
		/// If the service is running in a process that is not a system process, dwProcessId is nonzero.
		/// If the service is not running, dwProcessId is zero.
		/// </summary>
		None,
		/// <summary>The service runs in a system process that must always be running.</summary>
		RunInSystemProcess = 0x00000001,
	}

	/// <summary>
	/// The start type of the service
	/// </summary>
	public enum StartType : uint
	{
		/// <summary>
		/// A device driver started by the system loader.
		/// This value is valid only for driver services.
		/// </summary>
		BootStart = 0,
		/// <summary>
		/// A device driver started by the IoInitSystem function.
		/// This value is valid only for driver services.
		/// </summary>
		SystemStart = 1,
		/// <summary>
		/// A service started automatically by the service control manager during system startup.
		/// </summary>
		AutoStart = 2,
		/// <summary>
		/// A service started by the service control manager when a process calls the StartService function.
		/// </summary>
		OnDemandStart = 3,
		/// <summary>
		/// A service that cannot be started.
		/// Attempts to start the service result in the error code ERROR_SERVICE_DISABLED.
		/// </summary>
		Disabled = 4,
		/// <summary>
		/// Specify if you are not changing the existing start type.
		/// </summary>
		NoChange = 0xffffffff,
	}

	/// <summary>
	/// Specifies the severity of the error, and action taken, if this service fails to start.
	/// </summary>
	public enum ErrorControl : uint
	{
		/// <summary>The startup program ignores the error and continues the startup operation.</summary>
		Ingnore = 0,
		/// <summary>The startup program logs the error in the event log but continues the startup operation.</summary>
		Normal = 1,
		/// <summary>
		/// The startup program logs the error in the event log.
		/// If the last-known-good configuration is being started, the startup operation continues.
		/// Otherwise, the system is restarted with the last-known-good configuration.
		/// </summary>
		Severe = 2,
		/// <summary>
		/// The startup program logs the error in the event log, if possible.
		/// If the last-known-good configuration is being started, the startup operation fails.
		/// Otherwise, the system is restarted with the last-known good configuration.
		/// </summary>
		Critical = 3,
		/// <summary>
		/// Specify if you are not changing the existing start type.
		/// </summary>
		NoChange = 0xffffffff,
	}

	/// <summary>
	/// The action to be performed by the SCM.
	/// </summary>
	public enum ServiceControlActionType
	{
		/// <summary>No action</summary>
		None,
		/// <summary>Restart the service</summary>
		Restart,
		/// <summary>Reboot the computer</summary>
		Reboot,
		/// <summary>Run a command</summary>
		RunCommand
	}

	/// <summary>
	/// The service SID type.
	/// </summary>
	public enum SidType : uint
	{
		/// <summary>Use this type to reduce application compatibility issues.</summary>
		None,
		/// <summary>
		/// When the service process is created, the service SID is added to the service process
		/// token with the following attributes: SE_GROUP_ENABLED_BY_DEFAULT | SE_GROUP_OWNER.
		/// </summary>
		Unrestricted = 1,
		/// <summary>
		/// This type includes SERVICE_SID_TYPE_UNRESTRICTED. The service SID is also added to
		/// the restricted SID list of the process token.
		/// Three additional SIDs are also added to the restricted SID list:
		/// World SID S-1-1-0
		/// Service logon SID
		/// Write-restricted SID S-1-5-33
		/// 
		/// One ACE that allows GENERIC_ALL access for the service logon SID is also added to
		/// the service process token object.
		/// If there are multiple services hosted in the same process and one service
		/// has SERVICE_SID_TYPE_RESTRICTED, all services must have SERVICE_SID_TYPE_RESTRICTED.
		/// </summary>
		Restricted = 3,
	}

	/// <summary>
	/// The trigger event type.
	/// </summary>
	public enum TriggerType : uint
	{
		/// <summary>
		/// Invalid type.
		/// </summary>
		Invalid,
		/// <summary>
		/// The event is triggered when a device of the specified device interface class arrives or 
		/// is present when the system starts. This trigger event is commonly used to start a service.
		/// 
		/// The triggerSubtype member specifies the device interface class GUID. 
		/// These GUIDs are defined in device-specific header files provided with the Windows Driver Kit (WDK).
		/// 
		/// The pDataItems member specifies one or more hardware ID and compatible ID strings for
		/// the device interface class. Strings must be Unicode.
		/// If more than one string is specified, the event is triggered if any one of the strings match.
		/// For example, the Wpdbusenum service is started when a device of device interface class
		/// GUID_DEVINTERFACE_DISK {53f56307-b6bf-11d0-94f2-00a0c91efb8b} and a hardware
		/// ID string of "USBSTOR\GenDisk" arrives.
		/// </summary>
		DeviceInterfaceArrival = 1,
		/// <summary>
		/// The event is triggered when the first IP address on the TCP/IP networking stack becomes available or
		/// the last IP address on the stack becomes unavailable.
		/// This trigger event can be used to start or stop a service.
		/// 
		/// The triggerSubtype member specifies NETWORK_MANAGER_FIRST_IP_ADDRESS_ARRIVAL_GUID or
		/// NETWORK_MANAGER_LAST_IP_ADDRESS_REMOVAL_GUID.
		/// 
		/// The dataItems member is not used.
		/// </summary>
		IPAddressAvailability = 2,
		/// <summary>
		/// The event is triggered when the computer joins or leaves a domain.
		/// This trigger event can be used to start or stop a service.
		/// 
		/// The triggerSubtype member specifies DOMAIN_JOIN_GUID or DOMAIN_LEAVE_GUID.
		/// 
		/// The dataItems member is not used.
		/// </summary>
		DomainJoin = 3,
		/// <summary>
		/// The event is triggered when a firewall port is opened or approximately 60 seconds after 
		/// the firewall port is closed. This trigger event can be used to start or stop a service.
		/// 
		/// The triggerSubtype member specifies FIREWALL_PORT_OPEN_GUID or FIREWALL_PORT_CLOSE_GUID.
		/// 
		/// The dataItems member specifies the port, the protocol, and optionally the executable path and
		/// user information (SID string or name) of the service listening on the event.
		/// The "RPC" token can be used in place of the port to specify any listening socket used by RPC.
		/// The "system" token can be used in place of the executable path to specify ports created by and
		/// listened on by the Windows kernel.
		/// 
		/// The event is triggered only if all strings match. 
		/// For example, if MyService hosted inside MyServiceProcess.exe is to be trigger-started when
		/// port UDP 5001 opens, the trigger-specific data would be the Unicode representation of 
		/// "5001\0UDP\0%programfiles%\MyApplication\MyServiceProcess.exe\0MyService\0\0".
		/// 
		/// Note  Before this event can be registered, the Base Filtering Engine (BFE) service and 
		/// all services that depend on it must be stopped.
		/// After the event is registered, the BFE service and services that depend on it can be restarted.
		/// For more information, see Remarks.
		/// </summary>
		FirewallPortEvent = 4,
		/// <summary>
		/// The event is triggered when a machine policy or user policy change occurs. 
		/// This trigger event is commonly used to start a service.
		/// 
		/// The triggerSubtype member specifies MACHINE_POLICY_PRESENT_GUID or USER_POLICY_PRESENT_GUID.
		/// 
		/// The dataItems member is not used.
		/// </summary>
		GroupPolicy = 5,
		/// <summary>
		/// The event is triggered when a packet or request arrives on a particular network protocol.
		/// This request is commonly used to start a service that has stopped itself after 
		/// an idle time-out when there is no work to do.
		/// 
		/// Windows 7 and Windows Server 2008 R2:  This trigger type is not supported until Windows 8
		/// and Windows Server 2012.
		/// 
		/// The triggerSubtype member specifies one of the following values: 
		/// RPC_INTERFACE_EVENT_GUID,
		/// NAMED_PIPE_EVENT_GUID, 
		/// TCP_PORT_EVENT_GUID, or 
		/// UDP_EVENT_PORT_GUID.
		/// 
		/// The dataItems member specifies an endpoint or interface GUID.
		/// The string must be Unicode.
		/// The event triggers if the string is an exact match.
		/// 
		/// The dwAction member must be SERVICE_TRIGGER_ACTION_SERVICE_START.
		/// </summary>
		NetworkEndPoint = 6,
		/// <summary>
		/// The event is a custom event generated by an Event Tracing for Windows (ETW) provider.
		/// This trigger event can be used to start or stop a service.
		/// </summary>
		Custom = 20,
	}

	/// <summary>
	/// The action to take when the specified trigger event occurs.
	/// </summary>
	public enum TriggerAction : uint
	{
		/// <summary>Invalid</summary>
		Invalid,
		/// <summary>Start the service when the specified trigger event occurs.</summary>
		StartService = 1,
		/// <summary>Stop the service when the specified trigger event occurs.</summary>
		StopService = 2,
	}

	/// <summary>
	/// The data type of the trigger-specific data.
	/// </summary>
	public enum DataItemType : uint
	{
		/// <summary>Invalid</summary>
		Invalid,
		/// <summary>The trigger-specific data is in binary format.</summary>
		Binary = 1,
		/// <summary>The trigger-specific data is in string format.</summary>
		String = 2,
		/// <summary>TBD</summary>
		Level = 3,
		/// <summary>TBD</summary>
		KeyWordAny = 4,
		/// <summary>TBD</summary>
		KeyWordAll = 5,
	}

	/// <summary>
	/// The configuration information to be changed.
	/// </summary>
	public enum Config : uint
	{
		/// <summary>Invalid</summary>
		Invalid,
		/// <summary>The buffer parameter is a pointer to a ServiceDescription structure</summary>
		Description = 1,
		/// <summary>The buffer parameter is a pointer to a ServiceFailureActions structure.</summary>
		FailureActions = 2,
		/// <summary>The info parameter is a pointer to a DelayedAutoStartInfo structure.</summary>
		DelayedAutoStartInfo = 3,
		/// <summary>The info parameter is a pointer to a ServiceFailureActionsFlag structure.</summary>
		FailureActionsFlag = 4,
		/// <summary>The info parameter is a pointer to a ServiceSidInfo structure.</summary>
		ServiceSidInfo = 5,
		/// <summary>The info parameter is a pointer to a RequiredPrivilegesInfo structure.</summary>
		RequiredPrivilegesInfo = 6,
		/// <summary>The info parameter is a pointer to a ServicePreshutdownInfo structure.</summary>
		PreshutdownInfo = 7,
		/// <summary>The info parameter is a pointer to a ServiceTriggerInfo structure.</summary>
		TriggerInfo = 8,
		/// <summary>The info parameter is a pointer to a PreferedNodeInfo structure.</summary>
		PreferredNode = 9,
		/// <summary>The info parameter is a pointer a LaunchProtectedInfo structure.</summary>
		LaunchProtected = 12,
	}

	/// <summary>
	/// The protection type of the service
	/// </summary>
	public enum LaunchProtected : uint
	{
		/// <summary>None</summary>
		None,
		/// <summary>Reserved for internal Windows use only</summary>
		Windows,
		/// <summary>Reserved for internal Windows use only</summary>
		WindowsLight,
		/// <summary>
		/// Protection type that can be used by the anti-malware
		/// vendors to launch their anti-malware service as protected.
		/// </summary>
		AntiMalwareLight
	}

	/// <summary>
	/// The reason for changing the service status to Stop
	/// </summary>
	[Flags]
	public enum StopReasonFlag : uint
	{
		/// <summary>The serive isn't stopped</summary>
		NoReason = 0,
		/// <summary>The service stop was not planned.</summary>
		Unplanned = 0x10000000,
		/// <summary>
		/// The reason code is defined by the user. 
		/// If this flag is not present, the reason code is defined by the system. 
		/// If this flag is specified with a system reason code, the function call fails.
		/// 
		/// Users can create custom major reason codes in the range 
		/// SERVICE_STOP_REASON_MAJOR_MIN_CUSTOM (0x00400000) through SERVICE_STOP_REASON_MAJOR_MAX_CUSTOM (0x00ff0000) 
		/// and minor reason codes in the range SERVICE_STOP_REASON_MINOR_MIN_CUSTOM (0x00000100)
		/// through SERVICE_STOP_REASON_MINOR_MAX_CUSTOM (0x0000FFFF).
		/// </summary>
		Custom = 0x20000000,
		/// <summary>The service stop was planned.</summary>
		Planned = 0x40000000,

		/// <summary>Other issue.</summary>
		MajorOther = 0x00010000,
		/// <summary>Hardware issue.</summary>
		MajorHardware = 0x00020000,
		/// <summary>Operating system issue.</summary>
		MajorOperatingSystem = 0x00030000,
		/// <summary>Software issue.</summary>
		MajorSoftware = 0x00040000,
		/// <summary>Application issue.</summary>
		MajorApplication = 0x00050000,
		/// <summary>No major reason.</summary>
		MajorNone = 0x00060000,

		/// <summary>Other issue.</summary>
		MinorOther = 0x00000001,
		/// <summary>Maintenance.</summary>
		MinorMaintenance = 0x00000002,
		/// <summary>Installation.</summary>
		MinorInstallation = 0x00000003,
		/// <summary>Upgrade.</summary>
		MinorUpgrade = 0x00000004,
		/// <summary>Reconfigure.</summary>
		MinorReconfig = 0x00000005,
		/// <summary>Unresponsive.</summary>
		MinorHung = 0x00000006,
		/// <summary>Unstable.</summary>
		MinorUnstable = 0x00000007,
		/// <summary>Disk.</summary>
		MinorDisk = 0x00000008,
		/// <summary>Network card.</summary>
		MinorNetworkCard = 0x00000009,
		/// <summary>Environment.</summary>
		MinorEnvironment = 0x0000000a,
		/// <summary>Driver.</summary>
		MinorHardwareDriver = 0x0000000b,
		/// <summary>Other driver event.</summary>
		MinorOtherDriver = 0x0000000c,
		/// <summary>Service pack.</summary>
		MinorServicePack = 0x0000000d,
		/// <summary>Software update.</summary>
		MinorSoftwareUpdate = 0x0000000e,
		/// <summary>Software update uninstall.</summary>
		MinorSoftwareUpdateUninstall = MinorSoftwareUpdate,
		/// <summary>Security update.</summary>
		MinorSecurityFix = 0x0000000f,
		/// <summary>Security issue.</summary>
		MinorSecurity = 0x00000010,
		/// <summary>Network connectivity.</summary>
		MinorNetworkConnectivity = 0x00000011,
		/// <summary>WMI issue.</summary>
		MinorWMI = 0x00000012,
		/// <summary>Service pack uninstall.</summary>
		MinorServicePackUninstall = 0x00000013,
		/// <summary>Security update uninstall.</summary>
		MinorSecurityFixUninstall = 0x00000015,
		/// <summary>MMC issue.</summary>
		MinorMMC = 0x00000016,
		/// <summary>No minor reason.</summary>
		MinorNone = 0x00060000,
	}

	/// <summary>
	/// Access Rights for the Service Control Manager
	/// </summary>
	[Flags]
	public enum ScmAccessRights : uint
	{
		/// <summary>No access</summary>
		None,

		/// <summary>Required to connect to the service control manager.</summary>
		Connect = (uint)AccessRights.ScmConnect,
		/// <summary>
		/// Required to call the CreateService function to create a service object and add it to the database.
		/// </summary>
		CreateService = (uint)AccessRights.ScmCreateService,
		/// <summary>
		/// Required to call the EnumServicesStatus or EnumServicesStatusEx function to 
		/// list the services that are in the database.
		/// 
		/// Required to call the NotifyServiceStatusChange function to receive notification when 
		/// any service is created or deleted.
		/// </summary>
		EnumerateService = (uint)AccessRights.ScmEnumerateService,
		/// <summary>Required to call the LockServiceDatabase function to acquire a lock on the database.</summary>
		Lock = (uint)AccessRights.ScmLock,
		/// <summary>
		/// Required to call the QueryServiceLockStatus function to retrieve 
		/// the lock status information for the database.
		/// </summary>
		QueryLockStatus = (uint)AccessRights.ScmQueryLockStatus,
		/// <summary>Required to call the NotifyBootConfigStatus function.</summary>
		ModifyBootConfig = (uint)AccessRights.ScmModifyBootConfig,
		/// <summary>Includes STANDARD_RIGHTS_REQUIRED, in addition to all access rights in this table.</summary>
		AllAccess = (uint)AccessRights.ScmAllAccess,

		/// <summary>Generic read</summary>
		GenericRead = (uint)AccessRights.ScmGenericRead,
		/// <summary>Generic write</summary>
		GeenricWrite = (uint)AccessRights.ScmGeenricWrite,
		/// <summary>Generic execute</summary>
		GenericExecute = (uint)AccessRights.ScmGenericExecute,
		/// <summary>Generic all access</summary>
		GenericAll = (uint)AccessRights.ScmGenericAll,
	}

	/// <summary>
	/// Access Rights for services
	/// </summary>
	[Flags]
	public enum ServiceAccessRights : uint
	{
		/// <summary>No access</summary>
		None,

		/// <summary>
		/// Required to call the QueryServiceConfig and QueryServiceConfig2 functions to query the service configuration.
		/// </summary>
		QueryConfig = (uint)AccessRights.SvcQueryConfig,
		/// <summary>
		/// Required to call the ChangeServiceConfig or ChangeServiceConfig2 function to change the service configuration. 
		/// Because this grants the caller the right to change the executable file that the system runs, 
		/// it should be granted only to administrators.
		/// </summary>
		ChangeConfig = (uint)AccessRights.SvcChangeConfig,
		/// <summary>
		/// Required to call the QueryServiceStatus or QueryServiceStatusEx function to
		/// ask the service control manager about the status of the service.
		/// 
		/// Required to call the NotifyServiceStatusChange function to 
		/// receive notification when a service changes status.
		/// </summary>
		QueryStatus = (uint)AccessRights.SvcQueryStatus,
		/// <summary>
		/// Required to call the EnumDependentServices function to enumerate all the services dependent on the service.
		/// </summary>
		EnumerateDependents = (uint)AccessRights.SvcEnumerateDependents,
		/// <summary>
		/// Required to call the StartService function to start the service.
		/// </summary>
		Start = (uint)AccessRights.SvcStart,
		/// <summary>
		/// Required to call the ControlService function to stop the service.
		/// </summary>
		Stop = (uint)AccessRights.SvcStop,
		/// <summary>
		/// Required to call the ControlService function to pause or continue the service.
		/// </summary>
		PauseContinue = (uint)AccessRights.SvcPauseContinue,
		/// <summary>
		/// Required to call the ControlService function to ask the service to report its status immediately.
		/// </summary>
		Interrogate = (uint)AccessRights.SvcInterrogate,
		/// <summary>
		/// Required to call the ControlService function to specify a user-defined control code.
		/// </summary>
		UserDefinedControl =(uint)AccessRights.SvcUserDefinedControl,
		/// <summary>
		/// Includes STANDARD_RIGHTS_REQUIRED in addition to all access rights in this table.
		/// </summary>
		AllAccess = (uint)AccessRights.SvcAllAccess,
		 
		/// <summary>
		/// Required to call the QueryServiceObjectSecurity or SetServiceObjectSecurity function to access the SACL. 
		/// The proper way to obtain this access is to enable the SE_SECURITY_NAMEprivilege in 
		/// the caller's current access token, open the handle for ACCESS_SYSTEM_SECURITY access, 
		/// and then disable the privilege.
		/// </summary>
		AccessSystemSecurity = (uint)AccessRights.AccessSystemSecurity,
		/// <summary>
		/// Required to call the DeleteService function to delete the service.
		/// </summary>
		Delete = (uint)AccessRights.Delete,
		/// <summary>
		/// Required to call the QueryServiceObjectSecurity function to 
		/// query the security descriptor of the service object.
		/// </summary>
		ReadControl = (uint)AccessRights.ReadControl,
		/// <summary>
		/// Required to call the SetServiceObjectSecurity function to modify the Dacl 
		/// member of the service object's security descriptor.
		/// </summary>
		WriteDAC = (uint)AccessRights.WriteDAC,
		/// <summary>
		/// Required to call the SetServiceObjectSecurity function to modify 
		/// the Owner and Group members of the service object's security descriptor.
		/// </summary>
		WriteOwner = (uint)AccessRights.WriteOwner,

		/// <summary>Generic read</summary>
		GenericRead = (uint)AccessRights.SvcGenericRead,
		/// <summary>Generic write</summary>
		GenericWrite = (uint)AccessRights.SvcGenericWrite,
		/// <summary>Generic execute</summary>
		GenericExecute = (uint)AccessRights.SvcGenericExecute,
	}

	/// <summary>
	/// Access Rights for the Service Control Manager and services
	/// </summary>
	[Flags]
	public enum AccessRights : uint
	{
		/// <summary>No access</summary>
		None,

		/// <summary>Required to connect to the service control manager.</summary>
		ScmConnect = 0x0001,
		/// <summary>
		/// Required to call the CreateService function to create a service object and add it to the database.
		/// </summary>
		ScmCreateService = 0x0002,
		/// <summary>
		/// Required to call the EnumServicesStatus or EnumServicesStatusEx function to 
		/// list the services that are in the database.
		/// 
		/// Required to call the NotifyServiceStatusChange function to receive notification when 
		/// any service is created or deleted.
		/// </summary>
		ScmEnumerateService = 0x0004,
		/// <summary>Required to call the LockServiceDatabase function to acquire a lock on the database.</summary>
		ScmLock = 0x0008,
		/// <summary>
		/// Required to call the QueryServiceLockStatus function to retrieve 
		/// the lock status information for the database.
		/// </summary>
		ScmQueryLockStatus = 0x0010,
		/// <summary>Required to call the NotifyBootConfigStatus function.</summary>
		ScmModifyBootConfig = 0x0020,
		/// <summary>Includes STANDARD_RIGHTS_REQUIRED, in addition to all access rights in this table.</summary>
		ScmAllAccess = 0xF003F,

		/// <summary>Generic read</summary>
		ScmGenericRead = ScmEnumerateService | ScmQueryLockStatus | 0x00020000,
		/// <summary>Generic write</summary>
		ScmGeenricWrite = ScmCreateService | ScmModifyBootConfig | 0x00020000,
		/// <summary>Generic execute</summary>
		ScmGenericExecute = ScmConnect | ScmLock | 0x00020000,
		/// <summary>Generic all access</summary>
		ScmGenericAll = ScmAllAccess,

		/// <summary>
		/// Required to call the QueryServiceConfig and QueryServiceConfig2 functions to query the service configuration.
		/// </summary>
		SvcQueryConfig = 0x0001,
		/// <summary>
		/// Required to call the ChangeServiceConfig or ChangeServiceConfig2 function to change the service configuration. 
		/// Because this grants the caller the right to change the executable file that the system runs, 
		/// it should be granted only to administrators.
		/// </summary>
		SvcChangeConfig = 0x0002,
		/// <summary>
		/// Required to call the QueryServiceStatus or QueryServiceStatusEx function to
		/// ask the service control manager about the status of the service.
		/// 
		/// Required to call the NotifyServiceStatusChange function to 
		/// receive notification when a service changes status.
		/// </summary>
		SvcQueryStatus = 0x0004,
		/// <summary>
		/// Required to call the EnumDependentServices function to enumerate all the services dependent on the service.
		/// </summary>
		SvcEnumerateDependents = 0x0008,
		/// <summary>
		/// Required to call the StartService function to start the service.
		/// </summary>
		SvcStart = 0x0010,
		/// <summary>
		/// Required to call the ControlService function to stop the service.
		/// </summary>
		SvcStop = 0x0020,
		/// <summary>
		/// Required to call the ControlService function to pause or continue the service.
		/// </summary>
		SvcPauseContinue = 0x0040,
		/// <summary>
		/// Required to call the ControlService function to ask the service to report its status immediately.
		/// </summary>
		SvcInterrogate = 0x0080,
		/// <summary>
		/// Required to call the ControlService function to specify a user-defined control code.
		/// </summary>
		SvcUserDefinedControl = 0x0100,
		/// <summary>
		/// Includes STANDARD_RIGHTS_REQUIRED in addition to all access rights in this table.
		/// </summary>
		SvcAllAccess = 0xF01FF,

		/// <summary>
		/// Required to call the QueryServiceObjectSecurity or SetServiceObjectSecurity function to access the SACL. 
		/// The proper way to obtain this access is to enable the SE_SECURITY_NAMEprivilege in 
		/// the caller's current access token, open the handle for ACCESS_SYSTEM_SECURITY access, 
		/// and then disable the privilege.
		/// </summary>
		AccessSystemSecurity = 0x01000000,
		/// <summary>
		/// Required to call the DeleteService function to delete the service.
		/// </summary>
		Delete = 0x00010000,
		/// <summary>
		/// Required to call the QueryServiceObjectSecurity function to 
		/// query the security descriptor of the service object.
		/// </summary>
		ReadControl = 0x00020000,
		/// <summary>
		/// Required to call the SetServiceObjectSecurity function to modify the Dacl 
		/// member of the service object's security descriptor.
		/// </summary>
		WriteDAC = 0x00040000,
		/// <summary>
		/// Required to call the SetServiceObjectSecurity function to modify 
		/// the Owner and Group members of the service object's security descriptor.
		/// </summary>
		WriteOwner = 0x00080000,

		/// <summary>Generic read</summary>
		SvcGenericRead =
			SvcQueryConfig |
			SvcQueryStatus |
			SvcInterrogate |
			SvcEnumerateDependents |
			0x00020000,
		/// <summary>Generic write</summary>
		SvcGenericWrite = SvcChangeConfig | 0x00020000,
		/// <summary>Generic execute</summary>
		SvcGenericExecute =
			SvcStart |
			SvcStop |
			SvcPauseContinue |
			SvcUserDefinedControl |
			0x00020000,
	}

	/// <summary>
	/// Represents load group membership querying options
	/// </summary>
	public enum LoadGroupMembership
	{
		/// <summary>All services will be returned.</summary>
		AnyGroup,
		/// <summary>Only services which are not member of and load group, would be returned.</summary>
		NotInGroup,
	}
}
