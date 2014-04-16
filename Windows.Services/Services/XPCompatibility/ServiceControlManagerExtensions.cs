using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Services.XPCompatibility
{
	/// <summary>
	/// Provides extension methods to ServiceControlManager.
	/// </summary>
	public static class ServiceControlManagerExtensions
	{
		/// <summary>
		/// Creates a lock to the SCM.
		/// </summary>
		/// <param name="scm">The SCM to be locked.</param>
		/// <returns>ServiceControlLock object that should be used to unlock the SCM.</returns>
		public static ServiceControlLock Lock(this ServiceControlManager scm)
		{
			return new ServiceControlLock(scm);
		}

		/// <summary>
		/// Gets the lock status of the SCM.
		/// </summary>
		/// <param name="scm">The SCM to be queried.</param>
		/// <returns>ServiceLockStatus object that contains information, if, and by whom the SCM is locked.</returns>
		public static ServiceLockStatus GetLockStatus(this ServiceControlManager scm)
		{
			return ServiceControlLock.QueryLockStatus(scm);
		}
	}
}
