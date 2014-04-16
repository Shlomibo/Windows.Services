using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Services.Interop
{
	/// <summary>
	/// Contains status information for a service.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceStatus
	{
		public const uint ERROR_SERVICE_SPECIFIC_ERROR = 1066;
		public const uint NO_ERROR = 0;

		/// <summary>The type of service. </summary>
		public ServiceType type;
		/// <summary>The current state of the service.</summary>
		public State state;
		/// <summary>
		/// The control codes the service accepts and processes in its handler function (see Handler and HandlerEx). 
		/// A user interface process can control a service by specifying a control command in 
		/// the ControlService or ControlServiceEx function. 
		/// By default, all services accept the Interrogate value. To accept the DeviceEvent value, 
		/// the service must register to receive device events by using the RegisterDeviceNotification function.
		/// </summary>
		public AcceptedControls acceptedControls;
		/// <summary>
		/// The error code the service uses to report an error that occurs when it is starting or stopping. 
		/// To return an error code specific to the service, 
		/// the service must set this value to ERROR_SERVICE_SPECIFIC_ERROR to indicate that 
		/// the specificExitCode member contains the error code. 
		/// The service should set this value to NO_ERROR when it is running and on normal termination.
		/// </summary>
		public uint win32ExitCode;
		/// <summary>
		/// A service-specific error code that the service returns when an error occurs while
		/// the service is starting or stopping. 
		/// This value is ignored unless the win32ExitCode member is set to ERROR_SERVICE_SPECIFIC_ERROR.
		/// </summary>
		public int specificExitCode;
		/// <summary>
		/// The check-point value the service increments periodically to report its progress during a lengthy start, 
		/// stop, pause, or continue operation. 
		/// For example, the service should increment this value as it completes each step of 
		/// its initialization when it is starting up. 
		/// The user interface program that invoked the operation on the service uses this value to track 
		/// the progress of the service during a lengthy operation. 
		/// This value is not valid and should be zero when the service does not have a start,
		/// stop, pause, or continue operation pending.
		/// </summary>
		public int checkPoint;
		/// <summary>
		/// The estimated time required for a pending start, stop, pause, or continue operation, in milliseconds. 
		/// Before the specified amount of time has elapsed, 
		/// the service should make its next call to the SetServiceStatus function with either 
		/// an incremented dwCheckPoint value or a change in dwCurrentState. 
		/// If the amount of time specified by dwWaitHint passes, 
		/// and dwCheckPoint has not been incremented or dwCurrentState has not changed, 
		/// the service control manager or service control program can assume that 
		/// an error has occurred and the service should be stopped. 
		/// However, if the service shares a process with other services, 
		/// the service control manager cannot terminate the service application because it would have 
		/// to terminate the other services sharing the process as well.
		/// </summary>
		public int waitHint;
	}

	/// <summary>
	/// Contains process status information for a service. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceStatusProcess
	{
		public const int ERROR_SERVICE_SPECIFIC_ERROR = 1066;

		/// <summary>The type of service.</summary>
		public ServiceType type;
		/// <summary>The current state of the service. </summary>
		public State state;
		/// <summary>
		/// The control codes the service accepts and processes in its handler function (see Handler and HandlerEx). 
		/// A user interface process can control a service by specifying a control command in 
		/// the ControlService or ControlServiceEx function. 
		/// By default, all services accept the Interrogate value. To accept the DeviceEvent value, 
		/// the service must register to receive device events by using the RegisterDeviceNotification function.
		/// </summary>
		public AcceptedControls acceptedControls;
		/// <summary>
		/// The error code the service uses to report an error that occurs when it is starting or stopping. 
		/// To return an error code specific to the service, 
		/// the service must set this value to ERROR_SERVICE_SPECIFIC_ERROR to indicate that 
		/// the specificExitCode member contains the error code. 
		/// The service should set this value to NO_ERROR when it is running and on normal termination.
		/// </summary>
		public uint win32ExitCode;
		/// <summary>
		/// A service-specific error code that the service returns when an error occurs while
		/// the service is starting or stopping. 
		/// This value is ignored unless the win32ExitCode member is set to ERROR_SERVICE_SPECIFIC_ERROR.
		/// </summary>
		public int specificExitCode;
		/// <summary>
		/// The check-point value the service increments periodically to report its progress during a lengthy start, 
		/// stop, pause, or continue operation. 
		/// For example, the service should increment this value as it completes each step of 
		/// its initialization when it is starting up. 
		/// The user interface program that invoked the operation on the service uses this value to track 
		/// the progress of the service during a lengthy operation. 
		/// This value is not valid and should be zero when the service does not have a start,
		/// stop, pause, or continue operation pending.
		/// </summary>
		public int checkPoint;
		/// <summary>
		/// The estimated time required for a pending start, stop, pause, or continue operation, in milliseconds. 
		/// Before the specified amount of time has elapsed, 
		/// the service should make its next call to the SetServiceStatus function with either 
		/// an incremented dwCheckPoint value or a change in dwCurrentState. 
		/// If the amount of time specified by dwWaitHint passes, 
		/// and dwCheckPoint has not been incremented or dwCurrentState has not changed, 
		/// the service control manager or service control program can assume that 
		/// an error has occurred and the service should be stopped. 
		/// However, if the service shares a process with other services, 
		/// the service control manager cannot terminate the service application because it would have 
		/// to terminate the other services sharing the process as well.
		/// </summary>
		public int waitHint;
		/// <summary>The process identifier of the service.</summary>
		public uint processId;
		/// <summary>Specifies service process flags.</summary>
		public ProcessFlags flags;
	}

	/// <summary>
	/// Contains the name of a service in a service control manager database and information about that service. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct EnumServiceStatus
	{
		/// <summary>
		/// The name of a service in the service control manager database. 
		/// The maximum string length is 256 characters. 
		/// The service control manager database preserves the case of the characters, 
		/// but service name comparisons are always case insensitive. 
		/// A slash (/), backslash (\), comma, and space are invalid service name characters.
		/// </summary>
		public char* lpServiceName;
		/// <summary>
		/// A display name that can be used by service control programs, 
		/// such as Services in Control Panel, to identify the service. 
		/// This string has a maximum length of 256 characters. 
		/// The name is case-preserved in the service control manager. 
		/// Display name comparisons are always case-insensitive.
		/// </summary>
		public char* lpDisplayName;
		/// <summary>
		/// A ServiceStatus structure that contains status information for the lpServiceName service.
		/// </summary>
		public ServiceStatus status;
	}
	
	/// <summary>
	/// Contains the name of a service in a service control manager database and information about the service.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct EnumServiceStatusProcess
	{
		/// <summary>
		/// The name of a service in the service control manager database. 
		/// The maximum string length is 256 characters. 
		/// The service control manager database preserves the case of the characters, 
		/// but service name comparisons are always case insensitive. 
		/// A slash (/), backslash (\), comma, and space are invalid service name characters.
		/// </summary>
		public char* lpServiceName;
		/// <summary>
		/// A display name that can be used by service control programs, 
		/// such as Services in Control Panel, to identify the service. 
		/// This string has a maximum length of 256 characters. 
		/// The case is preserved in the service control manager. 
		/// Display name comparisons are always case-insensitive.
		/// </summary>
		public char* lpDisplayName;
		/// <summary>
		/// A ServiceStatusProcess structure that contains status information for the lpServiceName service.
		/// </summary>
		public ServiceStatusProcess processStatus;
	}

	/// <summary>
	/// Contains configuration information for an installed service.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct QueryServiceConfig
	{
		public const char SC_GROUP_IDENTIFIER = '+';

		/// <summary>The type of service.</summary>
		public ServiceType type;
		/// <summary>When to start the service. This member can be one of the following values.</summary>
		public StartType startType;
		/// <summary>The severity of the error, and action taken, if this service fails to start.</summary>
		public ErrorControl errorControl;
		/// <summary>
		/// The fully qualified path to the service binary file.
		/// 
		/// The path can also include arguments for an auto-start service. 
		/// These arguments are passed to the service entry point (typically the main function).
		/// </summary>
		public char* lpBinaryPathName;
		/// <summary>
		/// The name of the load ordering group to which this service belongs. 
		/// If the member is NULL or an empty string, the service does not belong to a load ordering group.
		/// 
		/// The startup program uses load ordering groups to load groups of services in a specified 
		/// order with respect to the other groups. 
		/// The list of load ordering groups is contained in the following registry value:
		/// 
		/// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\ServiceGroupOrder
		/// </summary>
		public char* lpLoadOrderGroup;
		/// <summary>
		/// A unique tag value for this service in the group specified by the lpLoadOrderGroup parameter. 
		/// A value of zero indicates that the service has not been assigned a tag. 
		/// You can use a tag for ordering service startup within a load order group by 
		/// specifying a tag order vector in the registry located at:
		/// 
		/// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\GroupOrderList
		/// 
		/// Tags are only evaluated for KernelDriver and FileSystemDriver type services that have RootStart or 
		/// SystemStart start types.
		/// </summary>
		public uint tagId;
		/// <summary>
		/// A pointer to an array of null-separated names of services or load ordering groups that 
		/// must start before this service. 
		/// The array is doubly null-terminated. 
		/// If the pointer is NULL or if it points to an empty string, the service has no dependencies. 
		/// If a group name is specified, it must be prefixed by the SC_GROUP_IDENTIFIER (defined in WinSvc.h) 
		/// character to differentiate it from a service name, 
		/// because services and service groups share the same name space. 
		/// Dependency on a service means that this service can only run if the service it depends on is running. 
		/// Dependency on a group means that this service can run if at least one member of the group is running 
		/// after an attempt to start all members of the group.
		/// </summary>
		public char* lpDependencies;
		/// <summary>
		/// If the service type is Win32OwnProcess or Win32SharedProcess,
		/// this member is the name of the account that the service process will be logged on as when it runs. 
		/// This name can be of the form Domain\UserName. 
		/// If the account belongs to the built-in domain, the name can be of the form .\UserName. 
		/// The name can also be "LocalSystem" if the process is running under the LocalSystem account.
		/// 
		/// If the service type is KernelDriver or FileSystemDriver, 
		/// this member is the driver object name (that is, \FileSystem\Rdr or \Driver\Xns) 
		/// which the input and output (I/O) system uses to load the device driver. 
		/// If this member is NULL, the driver is to be run with a default object name created by the I/O system, 
		/// based on the service name.
		/// </summary>
		public char* lpServiceStartName;
		/// <summary>
		/// The display name to be used by service control programs to identify the service. 
		/// This string has a maximum length of 256 characters. The name is case-preserved in the service control manager.
		/// Display name comparisons are always case-insensitive.
		/// 
		/// This parameter can specify a localized string using the following format:
		/// 
		/// @[Path\]DLLName,-StrID
		/// 
		/// The string with identifier StrID is loaded from DLLName; the Path is optional.
		/// </summary>
		public char* lpDisplayName;
	}
	/// <summary>
	/// Contains information about the lock status of a service control manager database. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct QueryServiceLockStatus
	{
		/// <summary>
		/// The lock status of the database. If this member is nonzero, the database is locked. 
		/// If it is zero, the database is unlocked.
		/// </summary>
		public bool isLocked;
		/// <summary>The name of the user who acquired the lock.</summary>
		public char* lpLockOwner;
		/// <summary>The time since the lock was first acquired, in seconds.</summary>
		public uint lockDuration;
	}

	/// <summary>
	/// Represents an action that the service control manager can perform.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct SCAction
	{
		/// <summary>The action to be performed.</summary>
		public ServiceControlActionType actionType;
		/// <summary>The time to wait before performing the specified action, in milliseconds.</summary>
		public uint delay;
	}

	/// <summary>
	/// Contains the delayed auto-start setting of an auto-start service.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceDelayedAutoStartInfo
	{
		/// <summary>
		/// If this member is TRUE, the service is started after other auto-start services are started plus a short delay. 
		/// Otherwise, the service is started during system boot.
		/// 
		/// This setting is ignored unless the service is an auto-start service.
		/// </summary>
		public int isAutoStartDelayed;
	}

	/// <summary>
	/// Contains a service description.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceDescription
	{
		/// <summary>
		/// The description of the service. 
		/// If this member is NULL, the description remains unchanged. 
		/// If this value is an empty string (""), the current description is deleted.
		/// 
		/// The service description must not exceed the size of 255.
		/// 
		/// This member can specify a localized string using the following format:
		/// 
		/// @[path\]dllname,-strID
		/// 
		/// The string with identifier strID is loaded from dllname; the path is optional.
		/// </summary>
		public char* lpDescription;
	}

	/// <summary>
	/// Represents the action the service controller should take on each failure of a service. 
	/// A service is considered failed when it terminates without reporting a status of Stopped to the service controller.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceFailureActions
	{
		public const uint INFINITE = 0xFFFFFFFF;
		/// <summary>
		/// The time after which to reset the failure count to zero if there are no failures, in seconds. 
		/// Specify INFINITE to indicate that this value should never be reset.
		/// </summary>
		public uint resetPeriod;
		/// <summary>
		/// The message to be broadcast to server users before rebooting in 
		/// response to the Reboot service controller action.
		/// 
		/// If this value is NULL, the reboot message is unchanged. 
		/// If the value is an empty string (""), the reboot message is deleted and no message is broadcast.
		/// 
		/// This member can specify a localized string using the following format:
		/// 
		/// @[path\]dllname,-strID
		/// 
		/// The string with identifier strID is loaded from dllname; the path is optional.
		/// </summary>
		public char* lpRebootMessage;
		/// <summary>
		/// The command line of the process for the CreateProcess function to execute in response to the 
		/// RunCommand service controller action. 
		/// This process runs under the same account as the service.
		/// 
		/// If this value is NULL, the command is unchanged. 
		/// If the value is an empty string (""), the command is deleted and no program is run when the service fails.
		/// </summary>
		public char* lpCommand;
		/// <summary>
		/// The number of elements in the lpsaActions array.
		/// 
		/// If this value is 0, but lpsaActions is not NULL, the reset period and array of failure actions are deleted.
		/// </summary>
		public uint actionsCount;
		/// <summary>
		/// A pointer to an array of SCAction structures.
		/// 
		/// If this value is NULL, the cActions and dwResetPeriod members are ignored.
		/// </summary>
		public SCAction* lpActions;
	}

	/// <summary>
	/// Contains the failure actions flag setting of a service. 
	/// This setting determines when failure actions are to be executed.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceFailureActionsFlag
	{
		/// <summary>
		/// If this member is TRUE and the service has configured failure actions, 
		/// the failure actions are queued if the service process terminates without reporting 
		/// a status of Stopped or if it enters the Stopped state but the Win32ExitCode member of
		/// the ServiceStatus structure is not ERROR_SUCCESS (0).
		/// 
		/// If this member is FALSE and the service has configured failure actions,
		/// the failure actions are queued only if the service terminates without reporting a status of Stopped.
		/// 
		/// This setting is ignored unless the service has configured failure actions.
		/// </summary>
		[MarshalAs(UnmanagedType.Bool)]
		public bool failureActionsOnNonCrashFailures;
	}

	/// <summary>
	/// Represents service status notification information.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceNotify
	{
		public const uint SERVICE_NOTIFY_STATUS_CHANGE = 2;
		public const uint ERROR_SUCCESS = 0;
		public const uint ERROR_SERVICE_MARKED_FOR_DELETE = 1072;

		/// <summary>
		/// The structure version. This member must be SERVICE_NOTIFY_STATUS_CHANGE
		/// </summary>
		public uint version;
		/// <summary>
		/// A pointer to the callback function of the signature:
		/// 
		/// void Func(void* parameter)
		/// </summary>
		public IntPtr notifyCallback;
		/// <summary>Any user-defined data to be passed to the callback function.</summary>
		public IntPtr context;
		/// <summary>
		/// A value that indicates the notification status. 
		/// If this member is ERROR_SUCCESS, 
		/// the notification has succeeded and the ServiceStatus member contains valid information. 
		/// If this member is ERROR_SERVICE_MARKED_FOR_DELETE, 
		/// the service has been marked for deletion and the service 
		/// handle used by NotifyServiceStatusChange must be closed.
		/// </summary>
		public uint notificationStatus;
		/// <summary>
		/// A ServiceStatusProcess structure that contains the service status information. 
		/// This member is only valid if notificationStatus is ERROR_SUCCESS.
		/// </summary>
		public ServiceStatusProcess serviceStatus;
		/// <summary>
		/// If notificationStatus is ERROR_SUCCESS, 
		/// this member contains a bitmask of the notifications that triggered this call to the callback function.
		/// </summary>
		public Notification notificationTriggered;
		/// <summary>
		/// If notificationStatus is ERROR_SUCCESS and the notification is Created or Deleted, 
		/// this member is valid and it is a MULTI_SZ string that contains one or more service names.
		/// The names of the created services will have a '/' prefix so you can distinguish them from 
		/// the names of the deleted services.
		/// 
		/// If this member is valid, the notification callback function must free the string using the LocalFree function.
		/// </summary>
		public char* serviceName;
	}
	/// <summary>
	/// Contains preshutdown settings.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServicePreShutdownInfo
	{
		/// <summary>The time-out value, in milliseconds.</summary>
		public uint timeout;
	}

	/// <summary>
	/// Represents the required privileges for a service.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceRequiredPrivilegesInfo
	{
		/// <summary>
		/// A multi-string that specifies the privileges.
		/// </summary>
		public char* requiredPrivileges;
	}

	/// <summary>
	/// Represents a service security identifier (SID).
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceSidInfo
	{
		/// <summary>The service SID type.</summary>
		public SidType serviceSidType;
	}

	/// <summary>
	/// Specifies the ServiceMain function for a service that can run in the calling process. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceTableEntry
	{
		/// <summary>
		/// Enrty to signals the end of the table
		/// </summary>
		public static ServiceTableEntry EndOfTable
		{
			get { return new ServiceTableEntry(); }
		}

		/// <summary>
		/// The name of a service to be run in this service process.
		/// 
		/// If the service is installed with the SERVICE_WIN32_OWN_PROCESS service type, this member is ignored, 
		/// but cannot be NULL. This member can be an empty string ("").
		/// 
		/// If the service is installed with the SERVICE_WIN32_SHARE_PROCESS service type, 
		/// this member specifies the name of the service that uses the ServiceMain function pointed to by 
		/// the lpServiceProc member.
		/// </summary>
		public char* serviceName;
		/// <summary>
		/// A pointer to a ServiceMain function.
		/// </summary>
		public IntPtr serviceProc;
	}

	/// <summary>
	/// Represents a service trigger event. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceTrigger
	{
		/// <summary>
		/// The event is triggered when a request is made to open the named pipe specified by pDataItems. 
		/// The triggerType member must be NetworkEndPoint. 
		/// The action member must be Start.
		/// </summary>
		public static readonly Guid NAMED_PIPE_EVENT_GUID = new Guid("1F81D131-3FAC-4537-9E0C-7E7B0C2F4B55");
		/// <summary>
		/// The event is triggered when an endpoint resolution request arrives for
		/// the RPC interface GUID specified by dataItems. 
		/// The triggerType member must be NetworkEndPoint. 
		/// The dwAction member must be Start.
		/// </summary>
		public static readonly Guid RPC_INTERFACE_EVENT_GUID = new Guid("BC90D167-9470-4139-A9BA-BE0BBBF5B74D");
		/// <summary>
		/// The event is triggered when the computer joins a domain. 
		/// The triggerType member must be DomainJoin.
		/// </summary>
		public static readonly Guid DOMAIN_JOIN_GUID = new Guid("1ce20aba-9851-4421-9430-1ddeb766e809");
		/// <summary>
		/// The event is triggered when the computer leaves a domain. 
		/// The triggerType member must be Domain Join
		/// </summary>
		public static readonly Guid DOMAIN_LEAVE_GUID = new Guid("ddaf516e-58c2-4866-9574-c3b615d42ea1");
		/// <summary>
		/// The event is triggered when the specified firewall port is opened. 
		/// The triggerType member must be FirewallPortEvent.
		/// </summary>
		public static readonly Guid FIREWALL_PORT_OPEN_GUID = new Guid("b7569e07-8421-4ee0-ad10-86915afdad09");
		/// <summary>
		/// The event is triggered approximately 60 seconds after the specified firewall port is closed. 
		/// The triggerType member must be FirewallPortEvent.
		/// </summary>
		public static readonly Guid FIREWALL_PORT_CLOSE_GUID = new Guid("a144ed38-8e12-4de4-9d96-e64740b1a524");
		/// <summary>
		/// The event is triggered when the machine policy has changed. 
		/// The triggerType member must be GroupPolicy.
		/// </summary>
		public static readonly Guid MACHINE_POLICY_PRESENT_GUID = new Guid("659FCAE6-5BDB-4DA9-B1FF-CA2A178D46E0");
		/// <summary>
		/// The event is triggered when the first IP address on the TCP/IP networking stack becomes available. 
		/// The triggerType member must be IPAddressAvailability.
		/// </summary>
		public static readonly Guid NETWORK_MANAGER_FIRST_IP_ADDRESS_ARRIVAL_GUID =
			new Guid("4f27f2de-14e2-430b-a549-7cd48cbc8245");
		/// <summary>
		/// The event is triggered when the last IP address on the TCP/IP networking stack becomes unavailable. 
		/// The triggerType member must be IPAddressAvailability.
		/// </summary>
		public static readonly Guid NETWORK_MANAGER_LAST_IP_ADDRESS_REMOVAL_GUID =
			new Guid("cc4ba62a-162e-4648-847a-b6bdf993e335");
		/// <summary>
		/// The event is triggered when the user policy has changed. 
		/// The triggerType member must be GroupPolicy.
		/// </summary>
		public static readonly Guid USER_POLICY_PRESENT_GUID = new Guid("54FB46C8-F089-464C-B1FD-59D1B62C3B50");

		/// <summary>The trigger event type. </summary>
		public TriggerType triggerType;
		/// <summary>The action to take when the specified trigger event occurs.</summary>
		public TriggerAction action;
		/// <summary>
		/// Points to a GUID that identifies the trigger event subtype. 
		/// The value of this member depends on the value of the dwTriggerType member.
		/// 
		/// If triggerType is SERVICE_TRIGGER_TYPE_CUSTOM, 
		/// triggerSubType is the GUID that identifies the custom event provider.
		/// 
		/// If triggerType is SERVICE_TRIGGER_TYPE_DEVICE_INTERFACE_ARRIVAL, 
		/// triggerSubType is the GUID that identifies the device interface class.
		/// 
		/// If triggerType is SERVICE_TRIGGER_TYPE_NETWORK_ENDPOINT, triggerSubType is one of the following values:
		/// NAMED_PIPE_EVENT_GUID, RPC_INTERFACE_EVENT_GUID.
		/// 
		/// For other trigger event types, triggerSubType can be one of the following values:
		/// DOMAIN_JOIN_GUID, DOMAIN_LEAVE_GUID, FIREWALL_PORT_OPEN_GUID, FIREWALL_PORT_CLOSE_GUID, 
		/// MACHINE_POLICY_PRESENT_GUID, NETWORK_MANAGER_FIRST_IP_ADDRESS_ARRIVAL_GUID, 
		/// NETWORK_MANAGER_LAST_IP_ADDRESS_REMOVAL_GUID, USER_POLICY_PRESENT_GUID.
		/// </summary>
		public Guid* triggerSubType;
		/// <summary>
		/// The number of SERVICE_TRIGGER_SPECIFIC_DATA_ITEM structures in the array pointed to by pDataItems.
		/// 
		/// This member is valid only if the dataType member is 
		/// Custom, DeviceArrival, FirewallPortEvent, or NetworkEndPoint.
		/// </summary>
		public uint dataItemsCount;
		/// <summary>
		/// A pointer to an array of TriggerSpecificDataItem structures that contain trigger-specific data.
		/// </summary>
		public TriggerSpecificDataItem* dataItems;
	}

	/// <summary>
	/// Contains trigger-specific data for a service trigger event. 
	/// This structure is used by the SERVICE_TRIGGER structure for 
	/// Custom, DeviceArrival, FirewallPortEvent, or NetworkEndPoint trigger events.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct TriggerSpecificDataItem
	{
		/// <summary>The data type of the trigger-specific data pointed to by data.</summary>
		public DataItemType type;
		/// <summary>
		/// The size of the trigger-specific data pointed to data, in bytes. The maximum value is 1024.
		/// </summary>
		public uint bytesCount;
		/// <summary>
		/// A pointer to the trigger-specific data for the service trigger event. 
		/// The trigger-specific data depends on the trigger event type; see Remarks.
		/// 
		/// If the type member is Binary, the trigger-specific data is an array of bytes.
		/// 
		/// If the type member is String, the trigger-specific data is a null-terminated string or a multistring.
		/// </summary>
		public byte* data;
	}

	/// <summary>
	/// Contains trigger event information for a service. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceTriggerInfo
	{
		/// <summary>
		/// The number of triggers in the array of ServiceTrigger structures pointed to by the triggers member.
		/// </summary>
		public uint triggersCount;
		/// <summary>
		/// A pointer to an array of ServiceTrigger structures that specify the trigger events for the service. 
		/// If the triggersCount member is 0, this member is not used.
		/// </summary>
		public ServiceTrigger* triggers;
		/// <summary>
		/// This member is reserved and must be NULL.
		/// </summary>
		public IntPtr reserved;
	}

	/// <summary>
	/// 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct PowerBroadcastSetting
	{
		public Guid powerSetting;
		public uint dataLen;
		public fixed byte data[1];
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct SessionNotification
	{
		public uint size;
		public uint sessionId;
	}

	/// <summary>
	/// Contains system time change settings.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceTimeChangeInfo
	{
		/// <summary>The new system time.</summary>
		public long newTime;
		/// <summary>The previous system time.</summary>
		public long oldTime;
	}

	/// <summary>
	/// Represents the preferred node on which to run a service.
	/// </summary>
	internal struct ServicePreferedNodeInfo
	{
		/// <summary>The node number of the preferred node.</summary>
		public ushort preferedNode;
		/// <summary>If this member is TRUE, the preferred node setting is deleted.</summary>
		[MarshalAs(UnmanagedType.I1)]
		public bool isDeleted;
	}

	/// <summary>
	/// Indicates a service protection type.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceLaunchProtectedInfo
	{
		/// <summary>The protection type of the service. </summary>
		public LaunchProtected launchProtected;
	}

	/// <summary>
	///  Contains service control parameters.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceControlStatusReasonParams
	{
		public const uint NO_ERROR = 0;
		public const uint ERROR_INVALID_SERVICE_CONTROL = 1052;
		public const uint ERROR_SERVICE_CANNOT_ACCEPT_CTRL = 1061;
		public const uint ERROR_SERVICE_NOT_ACTIVE = 1062;

		/// <summary>
		/// The reason for changing the service status to Stop. 
		/// If the current control code is not Stop, this member is ignored.
		/// 
		/// This member must be set to a combination of one general code, 
		/// one major reason code, and one minor reason code.
		/// </summary>
		public StopReasonFlag reason;
		/// <summary>
		/// An optional string that provides additional information about the service stop. 
		/// This string is stored in the event log along with the stop reason code. 
		/// This member must be NULL or a valid string that is less than 128 characters, 
		/// including the terminating null character.
		/// </summary>
		public char* comment;
		/// <summary>
		/// A pointer to a ServiceStatusProcess structure that receives the latest service status information.
		/// The information returned reflects the most recent status that the service reported to
		/// the service control manager.
		/// 
		/// The service control manager fills in the structure only when ControlService returns one of
		/// the following error codes: 
		/// NO_ERROR, ERROR_INVALID_SERVICE_CONTROL, ERROR_SERVICE_CANNOT_ACCEPT_CTRL, 
		/// or ERROR_SERVICE_NOT_ACTIVE. Otherwise, the structure is not filled in.
		/// </summary>
		public ServiceStatusProcess serviceStatus;
	}
}
