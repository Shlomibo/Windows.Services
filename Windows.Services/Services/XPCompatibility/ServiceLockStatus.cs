﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Interop;

namespace Windows.Services.XPCompatibility
{
	/// <summary>
	/// The lock status for SCM
	/// </summary>
	[Obsolete("WinXp is no longer supported", true)]
	public sealed class ServiceLockStatus
	{
		#region Properties

		/// <summary>
		/// Gets the name of the user who acquired the lock.
		/// </summary>
		public string Owner { get; }

		/// <summary>
		/// Gets the time since the lock was first acquired, in seconds.
		/// </summary>
		public int LockDuration { get; }

		/// <summary>
		/// Gets value that indicates if the SCM database is locked.
		/// </summary>
		public bool IsLocked { get; }
		#endregion

		#region Ctor

		internal unsafe ServiceLockStatus(ref QueryServiceLockStatus lockStatus)
		{
			this.Owner = lockStatus.lpLockOwner != null
				? new string(lockStatus.lpLockOwner)
				: null;

			this.LockDuration = (int)lockStatus.lockDuration;

			this.IsLocked = lockStatus.isLocked;
		}
		#endregion
	}
}
