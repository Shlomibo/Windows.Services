using System.Collections.Generic;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	partial class Service
	{
		#region Consts

		private static readonly Dictionary<int, string> MSGS_DELETE_SERVICE = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_ACCESS_DENIED, "The handle does not have the DELETE access right." },
			{ Win32API.ERROR_INVALID_HANDLE, "The specified handle is invalid." },
			{ Win32API.ERROR_SERVICE_MARKED_FOR_DELETE, "The specified service has already been marked for deletion." },
		};

		private static readonly Dictionary<int, string> MSGS_CREATE_SERVICE = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_ACCESS_DENIED, 
				"The handle to the SCM database does not have the SC_MANAGER_CREATE_SERVICE access right." },
			{ Win32API.ERROR_CIRCULAR_DEPENDENCY, "A circular service dependency was specified." },
			{ Win32API.ERROR_DUPLICATE_SERVICE_NAME, 
				"The display name already exists in the service control manager database either as a service name or as another display name." },
			{ Win32API.ERROR_INVALID_HANDLE, "The handle to the specified service control manager database is invalid." },
			{ Win32API.ERROR_INVALID_NAME, "The specified service name is invalid." },
			{ Win32API.ERROR_INVALID_PARAMETER, "A parameter that was specified is invalid." },
			{ Win32API.ERROR_INVALID_SERVICE_ACCOUNT, 
				"The user account name specified in the serviceAccount parameter does not exist." },
			{ Win32API.ERROR_SERVICE_EXISTS, "The specified service already exists in this database." },
			{ Win32API.ERROR_SERVICE_MARKED_FOR_DELETE, 
				"The specified service already exists in this database and has been marked for deletion." },
		};

		private static readonly Dictionary<int, string> MSGS_LOAD_CNFG = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_ACCESS_DENIED, "The handle does not have the SERVICE_QUERY_CONFIG access right." },
			{ Win32API.ERROR_INVALID_HANDLE, "The specified handle is invalid." },
		};

		private static readonly Dictionary<int, string> MSGS_SET_CNFG = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_ACCESS_DENIED, "The handle does not have the SERVICE_CHANGE_CONFIG access right." },
			{ Win32API.ERROR_CIRCULAR_DEPENDENCY, "A circular service dependency was specified." },
			{ Win32API.ERROR_DUPLICATE_SERVICE_NAME, "The display name already exists in the service controller manager database, " +
				"either as a service name or as another display name." },
			{ Win32API.ERROR_INVALID_HANDLE, "The specified handle is invalid." },
			{ Win32API.ERROR_INVALID_PARAMETER, "A parameter that was specified is invalid." },
			{ Win32API.ERROR_INVALID_SERVICE_ACCOUNT, "The account name does not exist, " +
				"or a service is specified to share the same binary file as an already installed service but with " +
				"an account name that is not the same as the installed service." },
			{ Win32API.ERROR_SERVICE_MARKED_FOR_DELETE, "The service has been marked for deletion." },
		};

		private static readonly Dictionary<int, string> MSGS_SEND_CTRL = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_ACCESS_DENIED, "The handle does not have the required access right." },
			{ Win32API.ERROR_DEPENDENT_SERVICES_RUNNING, 
				"The service cannot be stopped because other running services are dependent on it." },
			{ Win32API.ERROR_INVALID_HANDLE, 
				"The specified handle was not obtained using CreateService or OpenService, " +
				"or the handle is no longer valid." },
			{ Win32API.ERROR_INVALID_PARAMETER, 
				"The requested control code in the dwControl parameter is undefined, " +
				"or control is Stop but the stopReason or stopComment parameters are not valid." },
			{ Win32API.ERROR_INVALID_SERVICE_CONTROL, 
				"The requested control code is not valid, or it is unacceptable to the service." },
			{ Win32API.ERROR_SERVICE_CANNOT_ACCEPT_CTRL, 
				"The requested control code cannot be sent to the service because the state of the service is Stopped, " +
				"StartPending, or StopPending." },
			{ Win32API.ERROR_SERVICE_NOT_ACTIVE, "The service has not been started." },
			{ Win32API.ERROR_SERVICE_REQUEST_TIMEOUT, 
				"The process for the service was started, but it did not call StartServiceCtrlDispatcher, " +
				"or the thread that called StartServiceCtrlDispatcher may be blocked in a control handler function." },
			{ Win32API.ERROR_SHUTDOWN_IN_PROGRESS, "The system is shutting down." },
		};

		private static readonly Dictionary<int, string> MSGS_SERVICE_STATUS = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_INVALID_HANDLE, "The handle is invalid." },
			{ Win32API.ERROR_ACCESS_DENIED, "The handle does not have the SERVICE_QUERY_STATUS access right." },
			{ Win32API.ERROR_INSUFFICIENT_BUFFER, 
				"The buffer is too small for the SERVICE_STATUS_PROCESS structure. Nothing was written to the structure." },
			{ Win32API.ERROR_INVALID_PARAMETER, "The cbSize member of SERVICE_STATUS_PROCESS is not valid." },
			{ Win32API.ERROR_INVALID_LEVEL, "The InfoLevel parameter contains an unsupported value." },
			{ Win32API.ERROR_SHUTDOWN_IN_PROGRESS, "The system is shutting down; this function cannot be called." },
		};

		private static readonly Dictionary<int, string> MSGS_START_SVC = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_ACCESS_DENIED, "The handle does not have the SERVICE_START access right." },
			{ Win32API.ERROR_INVALID_HANDLE, "The handle is invalid." },
			{ Win32API.ERROR_PATH_NOT_FOUND, "The service binary file could not be found." },
			{ Win32API.ERROR_SERVICE_ALREADY_RUNNING, "An instance of the service is already running." },
			{ Win32API.ERROR_SERVICE_DATABASE_LOCKED, "The database is locked." },
			{ Win32API.ERROR_SERVICE_DEPENDENCY_DELETED, 
				"The service depends on a service that does not exist or has been marked for deletion." },
			{ Win32API.ERROR_SERVICE_DEPENDENCY_FAIL, "The service depends on another service that has failed to start." },
			{ Win32API.ERROR_SERVICE_DISABLED, "The service has been disabled." },
			{ Win32API.ERROR_SERVICE_LOGON_FAILED, 
				"The service did not start due to a logon failure. " +
				"This error occurs if the service is configured to run under an account that does not " +
				@"have the ""Log on as a service"" right." },
			{ Win32API.ERROR_SERVICE_MARKED_FOR_DELETE, "The service has been marked for deletion." },
			{ Win32API.ERROR_SERVICE_NO_THREAD, "A thread could not be created for the service." },
			{ Win32API.ERROR_SERVICE_REQUEST_TIMEOUT, "The process for the service was started, " +
				"but it did not call StartServiceCtrlDispatcher, or the thread that called " +
				"StartServiceCtrlDispatcher may be blocked in a control handler function." },
		};
		#endregion
	}
}