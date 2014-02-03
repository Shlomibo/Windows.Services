using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Windows.Services.Interop
{
	internal static unsafe class Win32API
	{
		public const uint SERVICE_CONTROL_STATUS_REASON_INFO = 1;
		public const uint SC_ENUM_PROCESS_INFO = 0;
		public const uint SC_STATUS_PROCESS_INFO = 0;

		public const string NOT_GROUPED = "";
		public const string ALL_GROUPS = null;
		public const string SERVICES_ACTIVE_DATABASE = "ServicesActive";

		public const uint VALID_SERVICE_BITS = 0x3FF0C084;
		public const uint INVALID_SERVICE_BITS = ~VALID_SERVICE_BITS;

		public const int ERROR_NOT_ENOUGH_MEMORY = 0x8;
		public const int ERROR_SERVICE_NOT_IN_EXE = 1083;
		public const int ERROR_INVALID_DATA = 13;
		public const int ERROR_INVALID_HANDLE = 6;
		public const int ERROR_ACCESS_DENIED = 5;
		public const int ERROR_CIRCULAR_DEPENDENCY = 1059;
		public const int ERROR_DUPLICATE_SERVICE_NAME = 1078;
		public const int ERROR_INVALID_PARAMETER = 87;
		public const int ERROR_INVALID_SERVICE_ACCOUNT = 1057;
		public const int ERROR_SERVICE_MARKED_FOR_DELETE = 1072;
		public const int ERROR_INVALID_SERVICE_CONTROL = 1052;
		public const int ERROR_SERVICE_CANNOT_ACCEPT_CTRL = 1061;
		public const int ERROR_SERVICE_NOT_ACTIVE = 1062;
		public const int ERROR_DEPENDENT_SERVICES_RUNNING = 1051;
		public const int ERROR_SERVICE_NOTIFY_CLIENT_LAGGING = 0x50E;
		public const int ERROR_DATABASE_DOES_NOT_EXIST = 1065;
		public const int ERROR_SERVICE_DOES_NOT_EXIST = 1060;
		public const int ERROR_PATH_NOT_FOUND = 3;
		public const int ERROR_SERVICE_ALREADY_RUNNING = 1056;
		public const int ERROR_SERVICE_DATABASE_LOCKED = 1055;
		public const int ERROR_SERVICE_DEPENDENCY_DELETED = 1075;
		public const int ERROR_SERVICE_DEPENDENCY_FAIL = 1068;
		public const int ERROR_SERVICE_DISABLED = 1058;
		public const int ERROR_SERVICE_LOGON_FAILED = 1069;
		public const int ERROR_SERVICE_NO_THREAD = 1054;
		public const int ERROR_SERVICE_REQUEST_TIMEOUT = 1053;
		public const int ERROR_SHUTDOWN_IN_PROGRESS = 1115;
		public const int ERROR_INVALID_NAME = 123;
		public const int ERROR_SERVICE_EXISTS = 1073;
		public const int ERROR_INSUFFICIENT_BUFFER = 122;
		public const int ERROR_INVALID_LEVEL = 124;
		public const int ERROR_SUCCESS = 0;
		public const int ERROR_MORE_DATA = 234;
		public const int ERROR_INVALID_SERVICE_LOCK = 1071;

		/// <summary>
		/// Registers a function to handle service control requests.
		/// 
		/// This function has been superseded by the RegisterServiceCtrlHandlerEx function. 
		/// A service can use either function, but the new function supports user-defined context data, 
		/// and the new handler function supports additional extended control codes.
		/// </summary>
		/// <param name="serviceName">
		/// The name of the service run by the calling thread. This is the service name that 
		/// the service control program specified in the CreateService function when creating the service.
		/// 
		/// If the service type is SERVICE_WIN32_OWN_PROCESS, 
		/// the function does not verify that the specified name is valid, 
		/// because there is only one registered service in the process.
		/// </param>
		/// <param name="handlerFunction">A pointer to the handler function to be registered. </param>
		/// <returns>
		/// If the function succeeds, the return value is a service status handle.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager:
		/// 
		/// ERROR_NOT_ENOUGH_MEMORY:
		/// Not enough memory is available to convert an ANSI string parameter to Unicode. 
		/// This error does not occur for Unicode string parameters.
		/// 
		/// ERROR_SERVICE_NOT_IN_EXE:
		/// The service entry was specified incorrectly when the process called 
		/// the StartServiceCtrlDispatcher function.
		/// </returns>
		[DllImport(
			"Advapi32.dll",
			EntryPoint = "RegisterServiceCtrlHandler",
			SetLastError = true,
			CharSet = CharSet.Unicode)]
		public static extern IntPtr RegisterServiceControlHandler(string serviceName, IntPtr handlerFunction);

		/// <summary>
		/// Registers a function to handle extended service control requests.
		/// </summary>
		/// <param name="serviceName">
		/// The name of the service run by the calling thread. 
		/// This is the service name that the service control program specified in 
		/// the CreateService function when creating the service.
		/// </param>
		/// <param name="handlerFunction">A pointer to the handler function to be registered.</param>
		/// <param name="context">
		/// Any user-defined data. 
		/// This parameter, which is passed to the handler function, can help identify 
		/// the service when multiple services share a process.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is a service status handle.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager:
		/// 
		/// ERROR_NOT_ENOUGH_MEMORY:
		/// Not enough memory is available to convert an ANSI string parameter to Unicode. 
		/// This error does not occur for Unicode string parameters.
		/// 
		/// ERROR_SERVICE_NOT_IN_EXE:
		/// The service entry was specified incorrectly when the process called 
		/// the StartServiceCtrlDispatcher function.
		/// </returns>
		[DllImport(
			"Advapi32.dll",
			EntryPoint = "RegisterServiceCtrlHandlerEx",
			SetLastError = true,
			CharSet = CharSet.Unicode)]
		public static extern IntPtr RegisterServiceControlHandler(
			string serviceName,
			IntPtr handlerFunction,
			IntPtr context);

		/// <summary>
		/// Registers a service type with the service control manager and the Server service. 
		/// The Server service can then announce the registered service type as one it currently supports. 
		/// The NetServerGetInfo and NetServerEnum functions obtain a specified machine's supported service types.
		/// </summary>
		/// <param name="hServiceStatus">
		/// A handle to the status information structure for the service. 
		/// A service obtains the handle by calling the RegisterServiceCtrlHandlerEx function.
		/// </param>
		/// <param name="bits">
		/// The service type.
		/// </param>
		/// <param name="areBitsAreOn">
		/// If this value is TRUE, the bits in dwServiceBit are to be set. 
		/// If this value is FALSE, the bits are to be cleared.
		/// </param>
		/// <param name="isUpdateImmediately">
		/// If this value is TRUE, the Server service is to perform an immediate update. 
		/// If this value is FALSE, the update is not be performed immediately.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool SetServiceBits(
			IntPtr hServiceStatus,
			uint bits,
			bool areBitsAreOn,
			bool isUpdateImmediately);

		/// <summary>
		/// Updates the service control manager's status information for the calling service.
		/// </summary>
		/// <param name="hServiceStatus">
		/// A handle to the status information structure for the current service. 
		/// This handle is returned by the RegisterServiceCtrlHandlerEx function.
		/// </param>
		/// <param name="pStatus">
		/// A pointer to the ServiceStatus structure the contains the latest status information for the calling service.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager. 
		/// Other error codes can be set by the registry functions that are called by the service control manager:
		/// 
		/// ERROR_INVALID_DATA:
		/// The specified service status structure is invalid.
		/// 
		/// ERROR_INVALID_HANDLE:
		/// The specified handle is invalid.
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static unsafe extern bool SetServiceStatus(IntPtr hServiceStatus, ServiceStatus* pStatus);

		/// <summary>
		/// Changes the configuration parameters of a service.
		/// </summary>
		/// <param name="hService">
		/// A handle to the service. 
		/// This handle is returned by the OpenService or CreateService function and 
		/// must have the SERVICE_CHANGE_CONFIG access right.
		/// </param>
		/// <param name="type">
		/// The type of service. 
		/// Specify NoChange if you are not changing the existing service type.
		/// </param>
		/// <param name="startType">
		/// The service start options. 
		/// Specify NoChange if you are not changing the existing start type
		/// </param>
		/// <param name="errorControl">
		/// The severity of the error, and action taken, if this service fails to start. 
		/// Specify noChange if you are not changing the existing error control.
		/// </param>
		/// <param name="binaryPathName">
		/// The fully qualified path to the service binary file. 
		/// Specify NULL if you are not changing the existing path. 
		/// If the path contains a space, it must be quoted so that it is correctly interpreted. 
		/// For example, "d:\\my share\\myservice.exe" should be specified as "\"d:\\my share\\myservice.exe\"".
		/// 
		/// The path can also include arguments for an auto-start service. 
		/// For example, "d:\\myshare\\myservice.exe arg1 arg2". 
		/// These arguments are passed to the service entry point (typically the main function).
		/// 
		/// If you specify a path on another computer, the share must be accessible by 
		/// the computer account of the local computer because this is the security context used in the remote call. 
		/// However, this requirement allows any potential vulnerabilities in the remote computer to affect 
		/// the local computer. Therefore, it is best to use a local file.
		/// </param>
		/// <param name="loadOrdergroup">
		/// The name of the load ordering group of which this service is a member. 
		/// Specify NULL if you are not changing the existing group. 
		/// Specify an empty string if the service does not belong to a group.
		/// 
		/// The startup program uses load ordering groups to load groups of services in 
		/// a specified order with respect to the other groups. 
		/// The list of load ordering groups is contained in the ServiceGroupOrder value of the following registry key:
		/// 
		/// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control
		/// </param>
		/// <param name="tagId">
		/// A pointer to a variable that receives a tag value that is unique in 
		/// the group specified in the lpLoadOrderGroup parameter. 
		/// Specify NULL if you are not changing the existing tag.
		/// 
		/// You can use a tag for ordering service startup within a load ordering group by 
		/// specifying a tag order vector in the GroupOrderList value of the following registry key:
		/// 
		/// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control
		/// 
		/// Tags are only evaluated for driver services that have BootStart or SystemStart start types.
		/// </param>
		/// <param name="dependencies">
		/// A pointer to a double null-terminated array of null-separated names of services or 
		/// load ordering groups that the system must start before this service can be started. 
		/// (Dependency on a group means that this service can run if at least one member of 
		/// the group is running after an attempt to start all members of the group.) 
		/// Specify NULL if you are not changing the existing dependencies.
		/// Specify an empty string if the service has no dependencies.
		/// 
		/// You must prefix group names with SC_GROUP_IDENTIFIER so that they can be distinguished from a service name,
		/// because services and service groups share the same name space.
		/// </param>
		/// <param name="accountName">
		/// The name of the account under which the service should run. 
		/// Specify NULL if you are not changing the existing account name. 
		/// If the service type is SERVICE_WIN32_OWN_PROCESS, use an account name in the form DomainName\UserName. 
		/// The service process will be logged on as this user. 
		/// If the account belongs to the built-in domain, you can specify .\UserName 
		/// (note that the corresponding C/C++ string is ".\\UserName"). 
		/// 
		/// A shared process can run as any user.
		/// 
		/// If the service type is KernelDriver or FileSystemDriver, 
		/// the name is the driver object name that the system uses to load the device driver. 
		/// Specify NULL if the driver is to use a default object name created by the I/O system.
		/// 
		/// A service can be configured to use a managed account or a virtual account. 
		/// If the service is configured to use a managed service account, the name is the managed service account name. 
		/// If the service is configured to use a virtual account, specify the name as NT SERVICE\ServiceName.
		/// </param>
		/// <param name="password">
		/// The password to the account name specified by the lpServiceStartName parameter. 
		/// Specify NULL if you are not changing the existing password. 
		/// Specify an empty string if the account has no password or if the service runs in 
		/// the LocalService, NetworkService, or LocalSystem account.
		/// 
		/// If the account name specified by the lpServiceStartName parameter is the name of 
		/// a managed service account or virtual account name, the lpPassword parameter must be NULL.
		/// 
		/// Passwords are ignored for driver services.
		/// </param>
		/// <param name="displayName">
		/// The display name to be used by applications to identify the service for its users. 
		/// Specify NULL if you are not changing the existing display name; 
		/// otherwise, this string has a maximum length of 256 characters. 
		/// The name is case-preserved in the service control manager. 
		/// Display name comparisons are always case-insensitive.
		/// 
		/// This parameter can specify a localized string using the following format:
		/// @[path\]dllname,-strID
		/// 
		/// The string with identifier strID is loaded from dllname; the path is optional.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes may be set by the service control manager. 
		/// Other error codes may be set by the registry functions that are called by the service control manager.
		/// ERROR_ACCESS_DENIED, ERROR_CIRCULAR_DEPENDENCY, ERROR_DUPLICATE_SERVICE_NAME,ERROR_INVALID_HANDLE,
		/// ERROR_INVALID_PARAMETER, ERROR_INVALID_SERVICE_ACCOUNT, ERROR_SERVICE_MARKED_FOR_DELETE
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static unsafe extern bool ChangeServiceConfig(
			IntPtr hService,
			ServiceType type,
			StartType startType,
			ErrorControl errorControl,
			string binaryPathName,
			string loadOrdergroup,
			uint* tagId,
			char* dependencies,
			string accountName,
			string password,
			string displayName);

		/// <summary>
		/// Changes the optional configuration parameters of a service.
		/// </summary>
		/// <param name="hService">
		/// A handle to the service. 
		/// This handle is returned by the OpenService or CreateService function and must have the 
		/// SERVICE_CHANGE_CONFIG access right. For more information, see Service Security and Access Rights.
		/// 
		/// If the service controller handles the SC_ACTION_RESTART action
		/// , hService must have the SERVICE_START access right.
		/// </param>
		/// <param name="config">The configuration information to be changed.</param>
		/// <param name="info">
		/// A pointer to the new value to be set for the configuration information. 
		/// The format of this data depends on the value of the dwInfoLevel parameter. 
		/// If this value is NULL, the information remains unchanged.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(
			"Advapi32.dll",
			SetLastError = true,
			EntryPoint = "ChangeServiceConfig2",
			CharSet = CharSet.Unicode)]
		public static unsafe extern bool ChangeServiceOptionalConfig(
			IntPtr hService,
			Config config,
			void* info);

		/// <summary>
		/// Closes a handle to a service control manager or service object.
		/// </summary>
		/// <param name="hSCObject">
		/// A handle to the service control manager object or the service object to close. 
		/// Handles to service control manager objects are returned by the OpenSCManager function, 
		/// and handles to service objects are returned by either the OpenService or CreateService function.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error code can be set by the service control manager. 
		/// Other error codes can be set by registry functions that are called by the service control manager.
		/// ERROR_INVALID_HANDLE
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool CloseServiceHandle(IntPtr hSCObject);

		/// <summary>
		/// Sends a control code to a service.
		/// </summary>
		/// <param name="hService">
		/// A handle to the service. 
		/// This handle is returned by the OpenService or CreateService function. 
		/// The access rights required for this handle depend on the dwControl code requested.
		/// </param>
		/// <param name="control">One of the control codes.</param>
		/// <param name="status">
		/// A pointer to a SERVICE_STATUS structure that receives the latest service status information. 
		/// The information returned reflects the most recent status that the service reported to 
		/// the service control manager.
		/// 
		/// The service control manager fills in the structure only when ControlService returns one of 
		/// the following error codes: 
		/// NO_ERROR, ERROR_INVALID_SERVICE_CONTROL, ERROR_SERVICE_CANNOT_ACCEPT_CTRL, 
		/// or ERROR_SERVICE_NOT_ACTIVE. Otherwise, the structure is not filled in.
		/// </param>
		/// <returns></returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool ControlService(
			IntPtr hService,
			ControlCode control,
			out ServiceStatus status);

		/// <summary>
		/// Sends a control code to a service.
		/// </summary>
		/// <param name="hService">
		/// A handle to the service. 
		/// This handle is returned by the OpenService or CreateService function. 
		/// The access rights required for this handle depend on the dwControl code requested.
		/// </param>
		/// <param name="control">A serivce control code</param>
		/// <param name="infoLevel">
		/// The information level for the service control parameters. 
		/// This parameter must be set to SERVICE_CONTROL_STATUS_REASON_INFO (1).
		/// </param>
		/// <param name="params">A pointer to the service control parameters. </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager. 
		/// Other error codes can be set by the registry functions that are called by the service control manager:
		/// ERROR_ACCESS_DENIED, ERROR_DEPENDENT_SERVICES_RUNNING, ERROR_SERVICE_CANNOT_ACCEPT_CTRL, 
		/// ERROR_INVALID_SERVICE_CONTROL, ERROR_INVALID_PARAMETER, ERROR_INVALID_HANDLE, ERROR_SERVICE_NOT_ACTIVE,
		/// ERROR_SERVICE_REQUEST_TIMEOUT, ERROR_SHUTDOWN_IN_PROGRESS.
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, EntryPoint = "ControlServiceEx", CharSet = CharSet.Unicode)]
		public static unsafe extern bool ControlService(
			IntPtr hService,
			uint control,
			uint infoLevel,
			ServiceControlStatusReasonParams* @params);

		/// <summary>
		/// Creates a service object and adds it to the specified service control manager database.
		/// </summary>
		/// <param name="hSCManager">
		/// A handle to the service control manager database. 
		/// This handle is returned by the OpenSCManager function and must have the SC_MANAGER_CREATE_SERVICE access right. 
		/// </param>
		/// <param name="serviceName">
		/// The name of the service to install. 
		/// The maximum string length is 256 characters. 
		/// The service control manager database preserves the case of the characters, 
		/// but service name comparisons are always case insensitive. 
		/// Forward-slash (/) and backslash (\) are not valid service name characters.
		/// </param>
		/// <param name="displayName">
		/// The display name to be used by user interface programs to identify the service. 
		/// This string has a maximum length of 256 characters. 
		/// The name is case-preserved in the service control manager. 
		/// Display name comparisons are always case-insensitive.
		/// </param>
		/// <param name="desiredAccess">
		/// The access to the service. Before granting the requested access, 
		/// the system checks the access token of the calling process. 
		/// </param>
		/// <param name="type">The service type. </param>
		/// <param name="startType">The service start options.</param>
		/// <param name="errorControl">The severity of the error, and action taken, if this service fails to start.</param>
		/// <param name="binaryPathName">
		/// The fully qualified path to the service binary file. 
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
		/// However, this requirement allows any potential vulnerabilities in 
		/// the remote computer to affect the local computer. Therefore, it is best to use a local file.
		/// </param>
		/// <param name="loadOrderGroup">
		/// The names of the load ordering group of which this service is a member. 
		/// Specify NULL or an empty string if the service does not belong to a group.
		/// 
		/// The startup program uses load ordering groups to load groups of services in a specified order 
		/// with respect to the other groups. 
		/// The list of load ordering groups is contained in the following registry value:
		/// 
		/// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\ServiceGroupOrder
		/// </param>
		/// <param name="tagId">
		/// A pointer to a variable that receives a tag value that is unique in 
		/// the group specified in the lpLoadOrderGroup parameter. Specify NULL if you are not changing the existing tag.
		/// 
		/// You can use a tag for ordering service startup within a load ordering group by 
		/// specifying a tag order vector in the following registry value:
		/// 
		/// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\GroupOrderList
		/// 
		/// Tags are only evaluated for driver services that have SERVICE_BOOT_START or SERVICE_SYSTEM_START start types.
		/// </param>
		/// <param name="dependencies">
		/// A pointer to a double null-terminated array of null-separated names of services or 
		/// load ordering groups that the system must start before this service. 
		/// Specify NULL or an empty string if the service has no dependencies. 
		/// Dependency on a group means that this service can run if at least one member of 
		/// the group is running after an attempt to start all members of the group.
		/// 
		/// You must prefix group names with SC_GROUP_IDENTIFIER so that they can be distinguished from a service name, 
		/// because services and service groups share the same name space.
		/// </param>
		/// <param name="serviceAccount">
		/// The name of the account under which the service should run. 
		/// If the service type is SERVICE_WIN32_OWN_PROCESS, use an account name in the form DomainName\UserName. 
		/// The service process will be logged on as this user. 
		/// If the account belongs to the built-in domain, you can specify .\UserName.
		/// 
		/// If this parameter is NULL, CreateService uses the LocalSystem account. 
		/// If the service type specifies SERVICE_INTERACTIVE_PROCESS, the service must run in the LocalSystem account.
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
		/// The password to the account name specified by the lpServiceStartName parameter. 
		/// Specify an empty string if the account has no password or if the service runs in the LocalService, 
		/// NetworkService, or LocalSystem account. For more information, see Service Record List.
		/// 
		/// If the account name specified by the lpServiceStartName parameter is 
		/// the name of a managed service account or virtual account name, the lpPassword parameter must be NULL.
		/// 
		/// Passwords are ignored for driver services.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is a handle to the service.
		/// 
		/// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager. 
		/// Other error codes can be set by the registry functions that are called by the service control manager:
		/// ERROR_ACCESS_DENIED, ERROR_CIRCULAR_DEPENDENCY, ERROR_DUPLICATE_SERVICE_NAME, ERROR_INVALID_HANDLE,
		/// ERROR_INVALID_NAME, ERROR_INVALID_PARAMETER, ERROR_INVALID_SERVICE_ACCOUNT, ERROR_SERVICE_EXISTS,
		/// ERROR_SERVICE_MARKED_FOR_DELETE.
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static unsafe extern IntPtr CreateService(
			IntPtr hSCManager,
			string serviceName,
			string displayName,
			ServiceAccessRights desiredAccess,
			ServiceType type,
			StartType startType,
			ErrorControl errorControl,
			string binaryPathName,
			string loadOrderGroup,
			uint* tagId,
			char* dependencies,
			string serviceAccount,
			string password);

		/// <summary>
		/// Marks the specified service for deletion from the service control manager database.
		/// </summary>
		/// <param name="hService">
		/// A handle to the service. This handle is returned by the OpenService or CreateService function, 
		/// and it must have the DELETE access right. 
		/// </param>
		/// <returns>
		/// f the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes may be set by the service control manager. 
		/// Others may be set by the registry functions that are called by the service control manager:
		/// ERROR_ACCESS_DENIED, ERROR_INVALID_HANDLE, ERROR_SERVICE_MARKED_FOR_DELETE.
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool DeleteService(IntPtr hService);

		/// <summary>
		/// Retrieves the name and status of each service that depends on the specified service; 
		/// that is, the specified service must be running before the dependent services can run.
		/// </summary>
		/// <param name="hService">
		/// A handle to the service. 
		/// This handle is returned by the OpenService or CreateService function, 
		/// and it must have the SERVICE_ENUMERATE_DEPENDENTS access right.
		/// </param>
		/// <param name="state">
		/// The state of the services to be enumerated. 
		/// </param>
		/// <param name="serices">
		/// A pointer to an array of EnumServiceStatus structures that receives the name and 
		/// service status information for each dependent service in the database. 
		/// The buffer must be large enough to hold the structures, plus the strings to which their members point.
		/// 
		/// The order of the services in this array is the reverse of the start order of the services. 
		/// In other words, the first service in the array is the one that would be started last, 
		/// and the last service in the array is the one that would be started first.
		/// 
		/// The maximum size of this array is 64,000 bytes. 
		/// To determine the required size, specify NULL for this parameter and 0 for the cbBufSize parameter. 
		/// The function will fail and GetLastError will return ERROR_MORE_DATA. 
		/// The pcbBytesNeeded parameter will receive the required size.
		/// </param>
		/// <param name="bufferSize">The size of the buffer pointed to by the lpServices parameter, in bytes.</param>
		/// <param name="bytesNeeded">
		/// A pointer to a variable that receives the number of bytes needed to store the array of service entries. 
		/// The variable only receives this value if the buffer pointed to by lpServices is too small, 
		/// indicated by function failure and the ERROR_MORE_DATA error; 
		/// otherwise, the contents of pcbBytesNeeded are undefined.
		/// </param>
		/// <param name="servicesReturned">
		/// A pointer to a variable that receives the number of service entries returned.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes may be set by the service control manager. 
		/// Other error codes may be set by the registry functions that are called by the service control manager:
		/// ERROR_ACCESS_DENIED, ERROR_INVALID_HANDLE, ERROR_INVALID_PARAMETER, ERROR_MORE_DATA.
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static unsafe extern bool EnumDependentServices(
			IntPtr hService,
			StateQuery state,
			EnumServiceStatus* serices,
			uint bufferSize,
			out uint bytesNeeded,
			out uint servicesReturned);

		/// <summary>
		/// Enumerates services in the specified service control manager database. 
		/// The name and status of each service are provided.
		/// </summary>
		/// <param name="hSCManager">
		/// A handle to the service control manager database. 
		/// This handle is returned by the OpenSCManager function, 
		/// and must have the SC_MANAGER_ENUMERATE_SERVICE access right. 
		/// </param>
		/// <param name="type">The type of services to be enumerated.</param>
		/// <param name="state">The state of the services to be enumerated.</param>
		/// <param name="services">
		/// A pointer to a buffer that contains an array of EnumServiceStatus structures that receive 
		/// the name and service status information for each service in the database. 
		/// The buffer must be large enough to hold the structures, plus the strings to which their members point.
		/// 
		/// The maximum size of this array is 256K bytes. 
		/// To determine the required size, specify NULL for this parameter and 0 for the bufferSize parameter. 
		/// The function will fail and GetLastError will return ERROR_INSUFFICIENT_BUFFER.
		/// The pcbBytesNeeded parameter will receive the required size.
		/// </param>
		/// <param name="bufferSize">The size of the buffer pointed to by the services parameter, in bytes.</param>
		/// <param name="bytesNeeded">
		/// A pointer to a variable that receives the number of bytes needed to return 
		/// the remaining service entries, if the buffer is too small.
		/// </param>
		/// <param name="returnedCount">
		/// A pointer to a variable that receives the number of service entries returned.
		/// </param>
		/// <param name="serviceIndex">
		/// A pointer to a variable that, on input, specifies the starting point of enumeration. 
		/// You must set this value to zero the first time this function is called. 
		/// On output, this value is zero if the function succeeds. 
		/// However, if the function returns zero and the GetLastError function returns ERROR_MORE_DATA, 
		/// this value is used to indicate the next service entry to be read when 
		/// the function is called to retrieve the additional data.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. 
		/// To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager. 
		/// Other error codes can be set by the registry functions that are called by the service control manager:
		/// ERROR_ACCESS_DENIED, ERROR_INVALID_HANDLE, ERROR_INVALID_PARAMETER, ERROR_MORE_DATA
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static unsafe extern bool EnumServicesStatus(
			IntPtr hSCManager,
			ServiceType type,
			StateQuery state,
			EnumServiceStatus* services,
			uint bufferSize,
			out uint bytesNeeded,
			out uint returnedCount,
			ref uint serviceIndex);

		/// <summary>
		/// Enumerates services in the specified service control manager database. 
		/// The name and status of each service are provided, 
		/// along with additional data based on the specified information level.
		/// </summary>
		/// <param name="hSCManager">
		/// A handle to the service control manager database. 
		/// This handle is returned by the OpenSCManager function, 
		/// and must have the SC_MANAGER_ENUMERATE_SERVICE access right.
		/// </param>
		/// <param name="infoLevel">
		/// The service attributes that are to be returned. 
		/// Use SC_ENUM_PROCESS_INFO to retrieve the name and service status information for each service in the database.
		/// The services parameter is a pointer to a buffer that receives an array of EnumServiceStatusProcess structures.
		/// The buffer must be large enough to hold the structures as well as the strings to which their members point.
		/// 
		/// Currently, no other information levels are defined.
		/// </param>
		/// <param name="type">The type of services to be enumerated. </param>
		/// <param name="state">The state of the services to be enumerated. </param>
		/// <param name="services">
		/// A pointer to the buffer that receives the status information. 
		/// The format of this data depends on the value of the InfoLevel parameter.
		/// 
		/// The maximum size of this array is 256K bytes. 
		/// To determine the required size, specify NULL for this parameter and 0 for the bufferSize parameter. 
		/// The function will fail and GetLastError will return ERROR_MORE_DATA. 
		/// The bytesNeeded parameter will receive the required size.
		/// </param>
		/// <param name="bufferSize">The size of the buffer pointed to by the services parameter, in bytes.</param>
		/// <param name="bytesNeeded">
		/// A pointer to a variable that receives the number of bytes needed to return the remaining service entries, 
		/// if the buffer is too small.
		/// </param>
		/// <param name="servicesReturned">
		/// A pointer to a variable that receives the number of service entries returned.
		/// </param>
		/// <param name="resumeHandle">
		/// A pointer to a variable that, on input, specifies the starting point of enumeration. 
		/// You must set this value to zero the first time the EnumServicesStatusEx function is called. 
		/// On output, this value is zero if the function succeeds. 
		/// However, if the function returns zero and the GetLastError function returns ERROR_MORE_DATA, 
		/// this value indicates the next service entry to be read when the EnumServicesStatusEx function 
		/// is called to retrieve the additional data.
		/// </param>
		/// <param name="groupName">
		/// The load-order group name. 
		/// If this parameter is a string, 
		/// the only services enumerated are those that belong to the group that has the name specified by the string. 
		/// If this parameter is an empty string, only services that do not belong to any group are enumerated. 
		/// If this parameter is NULL, group membership is ignored and all services are enumerated.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. 
		/// To get extended error information, call GetLastError. The following errors may be returned:
		/// ERROR_ACCESS_DENIED, ERROR_MORE_DATA, ERROR_INVALID_PARAMETER, ERROR_INVALID_HANDLE,
		/// ERROR_INVALID_LEVEL, ERROR_SHUTDOWN_IN_PROGRESS
		/// </returns>
		[DllImport(
			"Advapi32.dll",
			SetLastError = true,
			EntryPoint = "EnumServicesStatusEx",
			CharSet = CharSet.Unicode)]
		public static unsafe extern bool EnumServicesStatus(
			IntPtr hSCManager,
			uint infoLevel,
			ServiceType type,
			StateQuery state,
			EnumServiceStatusProcess* services,
			uint bufferSize,
			out uint bytesNeeded,
			out uint servicesReturned,
			ref uint resumeHandle,
			string groupName);

		/// <summary>
		/// Retrieves the display name of the specified service.
		/// </summary>
		/// <param name="hSCManager">
		/// A handle to the service control manager database, as returned by the OpenSCManager function.
		/// </param>
		/// <param name="serviceName">
		/// The service name. This name is the same as the service's registry key name. 
		/// It is best to choose a name that is less than 256 characters.
		/// </param>
		/// <param name="displayName">
		/// A pointer to a buffer that receives the service's display name. 
		/// If the function fails, this buffer will contain an empty string.
		/// 
		/// The maximum size of this array is 4K bytes. 
		/// To determine the required size, specify NULL for this parameter and 0 for the bufferSize parameter. 
		/// The function will fail and GetLastError will return ERROR_INSUFFICIENT_BUFFER. 
		/// The bufferSize parameter will receive the required size.
		/// 
		/// This parameter can specify a localized string using the following format:
		/// @[path\]dllname,-strID
		/// 
		/// The string with identifier strID is loaded from dllname; the path is optional.
		/// </param>
		/// <param name="bufferSize">
		/// A pointer to a variable that specifies the size of the buffer pointed to by displayName, in TCHARs. 
		/// On output, this variable receives the size of the service's display name, 
		/// in characters, excluding the null-terminating character.
		/// 
		/// If the buffer pointed to by displayName is too small to contain the display name, 
		/// the function does not store it. 
		/// When the function returns, bufferSize contains the size of the service's display name, 
		/// excluding the null-terminating character.
		/// </param>
		/// <returns>
		/// If the functions succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static unsafe extern bool GetServiceDisplayName(
			IntPtr hSCManager,
			string serviceName,
			char* displayName,
			ref uint bufferSize);

		/// <summary>
		/// Retrieves the service name of the specified service.
		/// </summary>
		/// <param name="hSCManager">
		/// A handle to the computer's service control manager database, as returned by OpenSCManager.
		/// </param>
		/// <param name="displayName">
		/// The service display name. This string has a maximum length of 256 characters.
		/// </param>
		/// <param name="serviceName">
		/// A pointer to a buffer that receives the service name. 
		/// If the function fails, this buffer will contain an empty string.
		/// 
		/// The maximum size of this array is 4K bytes. 
		/// To determine the required size, specify NULL for this parameter and 0 for the lpcchBuffer parameter. 
		/// The function will fail and GetLastError will return ERROR_INSUFFICIENT_BUFFER. 
		/// The lpcchBuffer parameter will receive the required size.
		/// </param>
		/// <param name="bufferSize">
		/// A pointer to variable that specifies the size of the buffer pointed to by the lpServiceName parameter, 
		/// in TCHARs. 
		/// When the function returns, this parameter contains the size of the service name, in TCHARs, 
		/// excluding the null-terminating character.
		/// 
		/// If the buffer pointed to by lpServiceName is too small to contain the service name, 
		/// the function stores no data in it. 
		/// When the function returns, lpcchBuffer contains the size of the service name, excluding the NULL terminator.
		/// </param>
		/// <returns>
		/// If the functions succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static unsafe extern bool GetServiceKeyName(
			IntPtr hSCManager,
			string displayName,
			char* serviceName,
			ref uint bufferSize);

		/// <summary>
		/// Requests ownership of the service control manager (SCM) database lock. 
		/// Only one process can own the lock at any specified time.
		/// </summary>
		/// <param name="hSCMananger">
		/// A handle to the SCM database. 
		/// This handle is returned by the OpenSCManager function, and must have the SC_MANAGER_LOCK access right.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is a lock to the specified SCM database.
		/// 
		/// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the SCM. 
		/// Other error codes can be set by registry functions that are called by the SCM:
		/// ERROR_ACCESS_DENIED, ERROR_INVALID_HANDLE, ERROR_SERVICE_DATABASE_LOCKED
		/// </returns>
		[Obsolete("As of Windows Vista, this function is provided for application " +
			"compatibility and has no effect on the database.")]
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr LockServiceDatabase(IntPtr hSCMananger);

		/// <summary>
		/// Reports the boot status to the service control manager. 
		/// It is used by boot verification programs. 
		/// This function can be called only by a process running in the LocalSystem or Administrator's account.
		/// </summary>
		/// <param name="isBootAcceptable">
		/// If the value is TRUE, the system saves the configuration as the last-known good configuration. 
		/// If the value is FALSE, the system immediately reboots, 
		/// using the previously saved last-known good configuration.
		/// </param>
		/// <returns>
		/// If the BootAcceptable parameter is FALSE, the function does not return.
		/// 
		/// If the last-known good configuration was successfully saved, the return value is nonzero.
		/// 
		/// If an error occurs, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes may be set by the service control manager. 
		/// Other error codes may be set by the registry functions that are called by 
		/// the service control manager to set parameters in the configuration registry:
		/// ERROR_ACCESS_DENIED
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool NotifyBootConfigStatus(bool isBootAcceptable);

		/// <summary>
		/// Enables an application to receive notification when the specified service is created or 
		/// deleted or when its status changes.
		/// </summary>
		/// <param name="hService">
		/// A handle to the service or the service control manager. 
		/// Handles to services are returned by the OpenService or CreateService function and must have 
		/// the SERVICE_QUERY_STATUS access right. 
		/// Handles to the service control manager are returned by the 
		/// OpenSCManager function and must have the SC_MANAGER_ENUMERATE_SERVICE access right. 
		/// 
		/// There can only be one outstanding notification request per service.
		/// </param>
		/// <param name="notifyMask">The type of status changes that should be reported. </param>
		/// <param name="notifyBuffer">
		/// A pointer to a ServiceNotify structure that contains notification information, 
		/// such as a pointer to the callback function. 
		/// This structure must remain valid until the callback function is invoked or the calling thread cancels
		/// the notification request.
		/// 
		/// Do not make multiple calls to NotifyServiceStatusChange with the same buffer parameter until 
		/// the callback function from the first call has finished with the buffer or 
		/// the first notification request has been canceled. 
		/// Otherwise, there is no guarantee which version of the buffer the callback function will receive.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is ERROR_SUCCESS. 
		/// If the service has been marked for deletion, the return value is ERROR_SERVICE_MARKED_FOR_DELETE and 
		/// the handle to the service must be closed. 
		/// If service notification is lagging too far behind the system state, 
		/// the function returns ERROR_SERVICE_NOTIFY_CLIENT_LAGGING. 
		/// In this case, the client should close the handle to the SCM, open a new handle, and call this function again.
		/// 
		/// If the function fails, the return value is one of the system error codes.
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static unsafe extern uint NotifyServiceStatusChange(
			IntPtr hService,
			Notification notifyMask,
			ServiceNotify* notifyBuffer);

		/// <summary>
		/// Establishes a connection to the service control manager on the specified computer and 
		/// opens the specified service control manager database.
		/// </summary>
		/// <param name="machineName">
		/// The name of the target computer. 
		/// If the pointer is NULL or points to an empty string, 
		/// the function connects to the service control manager on the local computer.
		/// </param>
		/// <param name="databaseName">
		/// The name of the service control manager database. 
		/// This parameter should be set to SERVICES_ACTIVE_DATABASE. 
		/// If it is NULL, the SERVICES_ACTIVE_DATABASE database is opened by default.
		/// </param>
		/// <param name="desiredAccess">
		/// The access to the service control manager. 
		/// For a list of access rights, see Service Security and Access Rights.
		/// 
		/// Before granting the requested access rights, 
		/// the system checks the access token of the calling process against 
		/// the discretionary access-control list of the security descriptor associated with the service control manager.
		/// 
		/// The SC_MANAGER_CONNECT access right is implicitly specified by calling this function.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is a handle to the specified service control manager database.
		/// 
		/// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the SCM. 
		/// Other error codes can be set by the registry functions that are called by the SCM:
		/// ERROR_ACCESS_DENIED, ERROR_DATABASE_DOES_NOT_EXIST
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr OpenSCManager(
			string machineName,
			string databaseName,
			ScmAccessRights desiredAccess);

		/// <summary>
		/// Opens an existing service.
		/// </summary>
		/// <param name="hSCManager">
		/// A handle to the service control manager database. The OpenSCManager function returns this handle.
		/// </param>
		/// <param name="serviceName">
		/// The name of the service to be opened. 
		/// This is the name specified by the lpServiceName parameter of the CreateService function when 
		/// the service object was created, not the service display name that is shown by 
		/// user interface applications to identify the service.
		/// 
		/// The maximum string length is 256 characters. 
		/// The service control manager database preserves the case of the characters, 
		/// but service name comparisons are always case insensitive. 
		/// Forward-slash (/) and backslash (\) are invalid service name characters.
		/// </param>
		/// <param name="desiredAccess">
		/// The access to the service. For a list of access rights, see Service Security and Access Rights.
		/// 
		/// Before granting the requested access, 
		/// the system checks the access token of the calling process against 
		/// the discretionary access-control list of the security descriptor associated with the service object.
		/// </param>
		/// <returns>
		/// the function succeeds, the return value is a handle to the service.
		/// 
		/// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager. 
		/// Others can be set by the registry functions that are called by the service control manager:
		/// ERROR_ACCESS_DENIED, ERROR_INVALID_HANDLE, ERROR_INVALID_NAME, ERROR_SERVICE_DOES_NOT_EXIST.
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr OpenService(
			IntPtr hSCManager,
			string serviceName,
			ServiceAccessRights desiredAccess);

		/// <summary>
		/// Retrieves the configuration parameters of the specified service.
		/// </summary>
		/// <param name="hService">
		/// A handle to the service. 
		/// This handle is returned by the OpenService or CreateService function, 
		/// and it must have the SERVICE_QUERY_CONFIG access right.
		/// </param>
		/// <param name="serviceConfig">
		/// A pointer to a buffer that receives the service configuration information. 
		/// The format of the data is a QUERY_SERVICE_CONFIG structure.
		/// 
		/// The maximum size of this array is 8K bytes. 
		/// To determine the required size, specify NULL for this parameter and 0 for the cbBufSize parameter. 
		/// The function will fail and GetLastError will return ERROR_INSUFFICIENT_BUFFER. 
		/// The pcbBytesNeeded parameter will receive the required size.
		/// </param>
		/// <param name="bufferSize">
		/// The size of the buffer pointed to by the lpServiceConfig parameter, in bytes.
		/// </param>
		/// <param name="bytesNeeded">
		/// A pointer to a variable that receives the number of bytes needed to store all the configuration information, 
		/// if the function fails with ERROR_INSUFFICIENT_BUFFER.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager. 
		/// Others can be set by the registry functions that are called by the service control manager:
		/// ERROR_ACCESS_DENIED, ERROR_INSUFFICIENT_BUFFER, ERROR_INVALID_HANDLE
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static unsafe extern bool QueryServiceConfig(
			IntPtr hService,
			QueryServiceConfig* serviceConfig,
			uint bufferSize,
			out uint bytesNeeded);

		/// <summary>
		/// Retrieves the optional configuration parameters of the specified service.
		/// </summary>
		/// <param name="hService">
		/// A handle to the service. 
		/// This handle is returned by the OpenService or CreateService function and must have 
		/// the SERVICE_QUERY_CONFIG access right. 
		/// </param>
		/// <param name="configuration">The configuration information to be queried.</param>
		/// <param name="buffer">
		/// A pointer to the buffer that receives the service configuration information. 
		/// The format of this data depends on the value of the dwInfoLevel parameter.
		/// 
		/// The maximum size of this array is 8K bytes. 
		/// To determine the required size, specify NULL for this parameter and 0 for the cbBufSize parameter. 
		/// The function fails and GetLastError returns ERROR_INSUFFICIENT_BUFFER. 
		/// The pcbBytesNeeded parameter receives the needed size.
		/// </param>
		/// <param name="bufferSize">The size of the structure pointed to by the lpBuffer parameter, in bytes.</param>
		/// <param name="bytesNeeded">
		/// A pointer to a variable that receives the number of bytes required to store the configuration information, 
		/// if the function fails with ERROR_INSUFFICIENT_BUFFER.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager. 
		/// Others can be set by the registry functions that are called by the service control manager:
		/// ERROR_ACCESS_DENIED, ERROR_INSUFFICIENT_BUFFER, ERROR_INVALID_HANDLE
		/// </returns>
		[DllImport(
			"Advapi32.dll",
			SetLastError = true,
			EntryPoint = "QueryServiceConfig2",
			CharSet = CharSet.Unicode)]
		public static unsafe extern bool QueryServiceOptionalConfig(
			IntPtr hService,
			Config configuration,
			void* buffer,
			uint bufferSize,
			out uint bytesNeeded);

		/// <summary>
		/// Retrieves dynamic information related to the current service start.
		/// </summary>
		/// <param name="hServiceStatus">
		/// A service status handle provided by RegisterServiceCtrlHandlerEx
		/// </param>
		/// <param name="infoLevel">
		/// Indicates the information level.
		/// </param>
		/// <param name="dynamicInfo">
		/// A dynamic information buffer. 
		/// If this parameter is valid, the callback function must free the buffer after use with the LocalFree function.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is TRUE.
		/// If the function fails, the return value is FALSE. 
		/// When this happens the GetLastError function should be called to retrieve the error code.
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool QueryServiceDynamicInformation(
			IntPtr hServiceStatus,
			uint infoLevel,
			IntPtr dynamicInfo);

		/// <summary>
		/// Retrieves the lock status of the specified service control manager database.
		/// </summary>
		/// <param name="hSCManager">
		/// A handle to the service control manager database. 
		/// The OpenSCManager function returns this handle, which must have the SC_MANAGER_QUERY_LOCK_STATUS access right.
		/// </param>
		/// <param name="lockStatus">
		/// A pointer to a QueryServiceLockStatus structure that receives the lock status of 
		/// the specified database is returned, plus the strings to which its members point.
		/// </param>
		/// <param name="bufferSize">The size of the buffer pointed to by the lpLockStatus parameter, in bytes.</param>
		/// <param name="bytesNeeded">
		/// A pointer to a variable that receives the number of bytes needed to return all the lock status information, 
		/// if the function fails.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager. 
		/// Other error codes can be set by the registry functions that are called by the service control manager:
		/// ERROR_ACCESS_DENIED, ERROR_INSUFFICIENT_BUFFER, ERROR_INVALID_HANDLE
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static unsafe extern bool QueryServiceLockStatus(
			IntPtr hSCManager,
			QueryServiceLockStatus* lockStatus,
			uint bufferSize,
			out uint bytesNeeded);

		/// <summary>
		/// Retrieves the current status of the specified service.
		/// </summary>
		/// <param name="hService">
		/// A handle to the service. 
		/// This handle is returned by the OpenService or the CreateService function, 
		/// and it must have the SERVICE_QUERY_STATUS access right.
		/// </param>
		/// <param name="status">A pointer to a ServiceStatus structure that receives the status information.</param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager. 
		/// Other error codes can be set by the registry functions that are called by the service control manager:
		/// ERROR_ACCESS_DENIED, ERROR_INVALID_HANDLE
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool QueryServiceStatus(
			IntPtr hService,
			out ServiceStatus status);

		/// <summary>
		/// Retrieves the current status of the specified service based on the specified information level.
		/// </summary>
		/// <param name="hService">
		/// A handle to the service. 
		/// This handle is returned by the CreateService or OpenService function, 
		/// and it must have the SERVICE_QUERY_STATUS access right.
		/// </param>
		/// <param name="infoLevel">
		/// The service attributes to be returned. 
		/// Use SC_STATUS_PROCESS_INFO to retrieve the service status information. 
		/// The lpBuffer parameter is a pointer to a ServiceStatusProcess structure.
		/// 
		/// Currently, no other information levels are defined.
		/// </param>
		/// <param name="serviceStatus">
		/// A pointer to the buffer that receives the status information. 
		/// The format of this data depends on the value of the InfoLevel parameter.
		/// 
		/// The maximum size of this array is 8K bytes. 
		/// To determine the required size, specify NULL for this parameter and 0 for the cbBufSize parameter. 
		/// The function will fail and GetLastError will return ERROR_INSUFFICIENT_BUFFER. 
		/// The pcbBytesNeeded parameter will receive the required size.
		/// </param>
		/// <param name="bufferSize">The size of the buffer pointed to by the lpBuffer parameter, in bytes.</param>
		/// <param name="bytesNeeded">
		/// A pointer to a variable that receives the number of bytes needed to store all status information, 
		/// if the function fails with ERROR_INSUFFICIENT_BUFFER.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. 
		/// To get extended error information, call GetLastError. 
		/// The following errors can be returned:
		/// ERROR_INVALID_HANDLE, ERROR_ACCESS_DENIED, ERROR_INSUFFICIENT_BUFFER, ERROR_INVALID_PARAMETER,
		/// ERROR_INVALID_LEVEL, ERROR_SHUTDOWN_IN_PROGRESS
		/// </returns>
		[DllImport(
			"Advapi32.dll",
			SetLastError = true,
			EntryPoint = "QueryServiceStatusEx",
			CharSet = CharSet.Unicode)]
		public static unsafe extern bool QueryServiceStatus(
			IntPtr hService,
			uint infoLevel,
			ServiceStatusProcess* serviceStatus,
			uint bufferSize,
			out uint bytesNeeded);

		/// <summary>
		/// Starts a service.
		/// </summary>
		/// <param name="hService">
		/// A handle to the service. 
		/// This handle is returned by the OpenService or CreateService function, 
		/// and it must have the SERVICE_START access right.
		/// </param>
		/// <param name="serviceArgsCount">
		/// The number of strings in the lpServiceArgVectors array. 
		/// If lpServiceArgVectors is NULL, this parameter can be zero.
		/// </param>
		/// <param name="serviceArgs">
		/// The null-terminated strings to be passed to the ServiceMain function for the service as arguments. 
		/// If there are no arguments, this parameter can be NULL. 
		/// Otherwise, the first argument (lpServiceArgVectors[0]) is the name of the service, 
		/// followed by any additional arguments (lpServiceArgVectors[1] through lpServiceArgVectors[dwNumServiceArgs-1]).
		/// 
		/// Driver services do not receive these arguments.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager. 
		/// Others can be set by the registry functions that are called by the service control manager:
		/// ERROR_ACCESS_DENIED, ERROR_INVALID_HANDLE, ERROR_PATH_NOT_FOUND, ERROR_SERVICE_ALREADY_RUNNING,
		/// ERROR_SERVICE_DATABASE_LOCKED, ERROR_SERVICE_DEPENDENCY_DELETED, ERROR_SERVICE_DEPENDENCY_FAIL,
		/// ERROR_SERVICE_DISABLED, ERROR_SERVICE_LOGON_FAILED, ERROR_SERVICE_MARKED_FOR_DELETE,
		/// ERROR_SERVICE_NO_THREAD, ERROR_SERVICE_REQUEST_TIMEOUT
		/// </returns>
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static unsafe extern bool StartService(
			IntPtr hService,
			uint serviceArgsCount,
			char** serviceArgs);

		/// <summary>
		/// Unlocks a service control manager database by releasing the specified lock.
		/// </summary>
		/// <param name="scLock">
		/// The lock, which is obtained from a previous call to the LockServiceDatabase function.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// 
		/// The following error codes can be set by the service control manager. 
		/// Other error codes can be set by the registry functions that are called by the service control manager:
		/// ERROR_INVALID_SERVICE_LOCK
		/// </returns>
		[Obsolete("This function has no effect as of Windows Vista.")]
		[DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool UnlockServiceDatabase(IntPtr scLock);
	}

	#region Delegates

	internal unsafe delegate void CallbackDelegate(ServiceNotify* sn);
	#endregion
}
