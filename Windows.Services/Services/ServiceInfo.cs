using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Interop;
using Utilities.Extansions;
using static Utilities.Extansions.Object.ObjectExtansions;

namespace Windows.Services
{
	/// <summary>
	/// Contains the name of a service in a service control manager database and information about the service.
	/// </summary>
	public sealed class ServiceInfo : ServiceStatus, IEquatable<ServiceInfo>
	{
		#region Properties

		/// <summary>
		/// Gets the name of a service in the service control manager database. 
		/// </summary>
		public string ServiceName { get; }

		/// <summary>
		/// Gets a display name that can be used by service control programs, such as Services in Control Panel, 
		/// to identify the service.
		/// </summary>
		public string DisplayName { get; }
		internal ServiceControlManager Scm { get; }
		#endregion

		#region Ctor

		internal unsafe ServiceInfo(ServiceControlManager scm, EnumServiceStatusProcess essp)
			: base(essp.processStatus)
		{
			this.Scm = scm;
			this.ServiceName = new string(essp.lpServiceName);
			this.DisplayName = new string(essp.lpDisplayName);
		}

		internal unsafe ServiceInfo(ServiceControlManager scm, EnumServiceStatus ess)
			: base(ess.status)
		{
			this.Scm = scm;
			this.ServiceName = new string(ess.lpServiceName);
			this.DisplayName = new string(ess.lpDisplayName);
		}
		#endregion

		#region Methods

		/// <summary>
		/// Returns the service associated with current information.
		/// </summary>
		/// <returns>The service associated with current information.</returns>
		public Service GetService() =>
			Scm.OpenService(this.ServiceName);

		/// <summary>
		/// Returns the service associated with current information.
		/// </summary>
		/// <param name="desiredAccess">The desired access to the service.</param>
		/// <returns>The service associated with current information.</returns>
		public Service GetService(ServiceAccessRights desiredAccess) =>
			Scm.OpenService(this.ServiceName, desiredAccess);

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(ServiceInfo other) =>
			base.Equals((ServiceStatus)other) &&
				(this.ServiceName == other.ServiceName) &&
				(this.DisplayName == other.DisplayName);

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
			return Equals((ServiceInfo)obj);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			// TODO: write your implementation of GetHashCode() here
			return CreateHashCode(
				base.GetHashCode(),
				this.ServiceName,
				this.DisplayName);
		}
		#endregion

		#region Operators

		/// <summary></summary><param name="left"></param><param name="right"></param><returns></returns>
		public static bool operator ==(ServiceInfo left, ServiceInfo right)
		{
			if (object.ReferenceEquals(left, right))
			{
				return true;
			}
			else if (object.ReferenceEquals(left, null))
			{
				return object.ReferenceEquals(right, null);
			}
			else
			{
				return left.Equals(right);
			}
		}

		/// <summary></summary><param name="left"></param><param name="right"></param><returns></returns>
		public static bool operator !=(ServiceInfo left, ServiceInfo right) =>
			!(left == right);
		#endregion
	}
}
