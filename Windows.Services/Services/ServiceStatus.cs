using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Windows.Services.Interop;
using Utilities.Extansions.Object;

namespace Utilities.Windows.Services
{
	/// <summary>
	/// Contains status information for a service.
	/// </summary>
	public class ServiceStatus : IEquatable<ServiceStatus>
	{
		#region Consts

		private const int NO_PROC_ID = -1;

		/// <summary>
		/// When this value returned in the Win32ExitCode property, the ServiceSpecificExitCode would
		/// contain a specific error code.
		/// </summary>
		public const int ERROR_SERVICE_SPECIFIC_ERROR = ServiceStatusProcess.ERROR_SERVICE_SPECIFIC_ERROR;

		/// <summary>
		/// This the the value reported in the Win32ExitCode property when no error occured.
		/// </summary>
		public const int NO_ERROR = 0;
		#endregion

		#region Properties

		/// <summary>
		/// Gets the type of service.
		/// </summary>
		public ServiceType Type { get; private set; }

		/// <summary>
		/// Gets the current state of the service.
		/// </summary>
		public State State { get; private set; }
		
		/// <summary>
		/// Gets the control codes the service accepts and processes in its handler function. 
		/// A user interface process can control a service by specifying a control command in the SendControl function. 
		/// By default, all services accept the Interrogate value.
		/// </summary>
		public AcceptedControls AcceptedControls { get; private set; }

		/// <summary>
		/// Gets the process identifier of the service.
		/// </summary>
		public int ProcessId { get; private set; }

		/// <summary>
		/// Gets flags of the service's process.
		/// </summary>
		public ProcessFlags ServiceProcFlags { get; private set; }

		/// <summary>
		/// Gets the error code that the service uses to report an error that occurs when it is starting or stopping. 
		/// To return an error code specific to the service, the service must set this value to 
		/// ERROR_SERVICE_SPECIFIC_ERROR to indicate that the ServiceSpecificExitCode property contains the error code. 
		/// The service should set this value to NO_ERROR when it is running and when it terminates normally.
		/// </summary>
		public int Win32ExitCode { get; private set; }

		/// <summary>
		/// Gets the service-specific error code that the service returns when an error occurs while 
		/// the service is starting or stopping. 
		/// This value is null unless the Win32ExitCode property is set to ERROR_SERVICE_SPECIFIC_ERROR.
		/// </summary>
		public int? ServiceSpecificExitCode { get; private set; }

		/// <summary>
		/// Gets the check-point value that the service increments periodically to report its progress during a lengthy start,
		/// stop, pause, or continue operation. 
		/// For example, the service should increment this value as it completes each step of its initialization 
		/// when it is starting up. 
		/// The user interface program that invoked the operation on the service uses this value to track 
		/// the progress of the service during a lengthy operation. 
		/// This value is not valid and should be zero when the service does not have a start, 
		/// stop, pause, or continue operation pending.
		/// </summary>
		public int CheckPoint { get; private set; }
		
		/// <summary>
		/// Get the estimated time required for a pending start, stop, pause, or continue operation, in milliseconds. 
		/// Before the specified amount of time has elapsed, the service should make its next call to 
		/// the SetServiceStatus function with either an incremented CheckPoint value or a change in State.
		/// If the amount of time specified by dwWaitHint passes, 
		/// and dwCheckPoint has not been incremented or dwCurrentState has not changed, 
		/// the service control manager or service control program can assume that an error has occurred and 
		/// the service should be stopped. 
		/// However, if the service shares a process with other services, 
		/// the service control manager cannot terminate the service application because 
		/// it would have to terminate the other services sharing the process as well.
		/// </summary>
		public int WaitHint { get; private set; }
		#endregion

		#region Ctor

		internal ServiceStatus(Services.Interop.ServiceStatus ss)
		{
			this.Type = ss.type;
			this.State = ss.state;
			this.AcceptedControls = ss.acceptedControls;
			this.ProcessId = NO_PROC_ID;
			this.ServiceProcFlags = ProcessFlags.None;
			this.Win32ExitCode = (int)ss.win32ExitCode;
			this.ServiceSpecificExitCode = ss.win32ExitCode == ERROR_SERVICE_SPECIFIC_ERROR
				? (int?)ss.specificExitCode
				: null;
			this.CheckPoint = ss.checkPoint;
			this.WaitHint = ss.waitHint;
		}

		internal ServiceStatus(ServiceStatusProcess ssp)
		{
			this.Type = ssp.type;
			this.State = ssp.state;
			this.AcceptedControls = ssp.acceptedControls;
			this.ProcessId = (int)ssp.processId;
			this.ServiceProcFlags = ssp.flags;
			this.Win32ExitCode = (int)ssp.win32ExitCode;
			this.ServiceSpecificExitCode = ssp.win32ExitCode == ServiceStatusProcess.ERROR_SERVICE_SPECIFIC_ERROR
				? (int?)ssp.specificExitCode
				: null;
			this.CheckPoint = (int)ssp.checkPoint;
			this.WaitHint = (int)ssp.waitHint;
		}
		#endregion

		#region Methods

		/// <summary>
		/// Returns Process instance to the service's process.
		/// </summary>
		/// <returns>A Process instance to the service's process.</returns>
		public Process GetProcess()
		{
			return this.ProcessId != NO_PROC_ID
				? Process.GetProcessById(this.ProcessId)
				: null;
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(ServiceStatus other)
		{
			return (other != null) &&
				(this.Type == other.Type) &&
				(this.State == other.State) &&
				(this.AcceptedControls == other.AcceptedControls) &&
				(this.ProcessId == other.ProcessId) &&
				(this.ServiceProcFlags == other.ServiceProcFlags) &&
				(this.Win32ExitCode == other.Win32ExitCode) &&
				(this.ServiceSpecificExitCode == other.ServiceSpecificExitCode) &&
				(this.CheckPoint == other.CheckPoint) &&
				(this.WaitHint == other.WaitHint);
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			//       
			// See the full list of guidelines at
			//   http://go.microsoft.com/fwlink/?LinkID=85237  
			// and also the guidance for operator== at
			//   http://go.microsoft.com/fwlink/?LinkId=85238
			//

			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			// TODO: write your implementation of Equals() here
			return Equals((ServiceStatus)obj);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return ObjectExtansions.CreateHashCode(
				this.Type,
				this.State,
				this.AcceptedControls,
				this.ProcessId,
				this.ServiceProcFlags,
				this.Win32ExitCode,
				this.ServiceSpecificExitCode,
				this.CheckPoint,
				this.WaitHint);
		}
		#endregion

		#region Operators

		/// <summary></summary><param name="left"></param><param name="right"></param><returns></returns>
		public static bool operator ==(ServiceStatus left, ServiceStatus right)
		{
			if (object.ReferenceEquals(left, right))
			{
				return true;
			}
			else if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
			{
				return false;
			}
			else
			{
				return left.Equals(right);
			}
		}

		/// <summary></summary><param name="left"></param><param name="right"></param><returns></returns>
		public static bool operator !=(ServiceStatus left, ServiceStatus right)
		{
			return !(left == right);
		}
		#endregion
	}
}
