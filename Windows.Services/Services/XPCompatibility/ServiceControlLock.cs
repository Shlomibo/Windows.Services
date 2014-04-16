using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services.XPCompatibility
{
	/// <summary>
	/// A lock on SCM database.
	/// </summary>
	public class ServiceControlLock : IDisposable
	{
#pragma warning disable 612, 618

		#region consts

		private static readonly Dictionary<int, string> MSGS_LOCK_ERRORS = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_ACCESS_DENIED, "The handle does not have the SC_MANAGER_LOCK access right." },
			{ Win32API.ERROR_INVALID_HANDLE, "The specified handle is not valid." },
			{ Win32API.ERROR_SERVICE_DATABASE_LOCKED, "The database is locked." },
		};

		private static readonly Dictionary<int, string> MSGS_UNLOCK_ERRORS = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_INVALID_SERVICE_LOCK, "The specified lock is invalid." },
		};
		#endregion

		#region Fields

		private IntPtr scLock;
		#endregion

		#region Properties

		/// <summary>
		/// Gets value that indicates if the lock have been released.
		/// </summary>
		public bool IsUnlocked { get; private set; }
		#endregion

		#region Ctor

		/// <summary>
		/// Aquires a lock on the given SCM database.
		/// </summary>
		/// <param name="scm">The SCM database to lock.</param>
		public ServiceControlLock(ServiceControlManager scm)
		{
			this.scLock = Win32API.LockServiceDatabase(scm.Handle);

			if (this.scLock == IntPtr.Zero)
			{
				throw ServiceException.Create(MSGS_LOCK_ERRORS, Marshal.GetLastWin32Error());
			}
		}

		/// <summary>
		/// Releases the lock.
		/// </summary>
		~ServiceControlLock()
		{
			Dispose(false);
		}
		#endregion

		#region Methods

		/// <summary>
		/// Checks the lock status on the given SCM database.
		/// </summary>
		/// <param name="scm">The SCM database to check.</param>
		/// <returns>A ServiceLockStatus that contains data about the lock status of the given database.</returns>
		public static unsafe ServiceLockStatus QueryLockStatus(ServiceControlManager scm)
		{
			QueryServiceLockStatus* pQSLS = null;

			try
			{
				int allocated = sizeof(QueryServiceLockStatus);
				pQSLS = (QueryServiceLockStatus*)Marshal.AllocHGlobal(allocated);
				uint needed = 0;
				int lastError;

				do
				{
					if (Win32API.QueryServiceLockStatus(scm.Handle, pQSLS, (uint)allocated, out needed))
					{
						lastError = Win32API.ERROR_SUCCESS;
					}
					else
					{
						lastError = Marshal.GetLastWin32Error();
					}

					if (lastError == Win32API.ERROR_INSUFFICIENT_BUFFER)
					{
						allocated = (int)needed;
						pQSLS = (QueryServiceLockStatus*)Marshal.ReAllocHGlobal((IntPtr)pQSLS, (IntPtr)allocated);
					}
				} while (lastError == Win32API.ERROR_INSUFFICIENT_BUFFER);

				return new ServiceLockStatus(ref *pQSLS);
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pQSLS);
			}
		}

		void IDisposable.Dispose()
		{
			Unlock();
		}

		/// <summary>
		/// Release the lock, and free resources aquired by the lock.
		/// </summary>
		public void Unlock()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Release resources
		/// </summary>
		/// <param name="disposing">Value indicates if the object was disposed correctly.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.IsUnlocked)
			{
				this.IsUnlocked = true;

				if (!Win32API.UnlockServiceDatabase(this.scLock) && disposing)
				{
					throw ServiceException.Create(MSGS_UNLOCK_ERRORS, Marshal.GetLastWin32Error());
				} 
			}
		} 
		#endregion

#pragma warning restore 612, 618
	}
}
