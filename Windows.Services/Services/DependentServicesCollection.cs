using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	partial class Service
	{
		/// <summary>
		/// A collection of dependent services' status
		/// </summary>
		public class DependentServicesCollection : ServiceCollectionBase
		{
			#region Fields

			private Service service;
			#endregion

			#region Ctor

			internal DependentServicesCollection(Service service)
			{
				this.service = service;
			}
			#endregion

			#region Methods

			/// <summary>
			/// Throw an exception if the collection is disposed.
			/// </summary>
			protected override void ThrowIfDisposed()
			{
				this.service.ThrowIfDisposed();
			}

			/// <summary>
			/// Gets enumerator that enumerates services by state
			/// </summary>
			/// <param name="type">This value is ignored.</param>
			/// <param name="state">The state of the services to enumerate.</param>
			/// <param name="groupName">This value is ignored.</param>
			/// <returns>An enumerator for the corresponding services.</returns>
			protected override IEnumerable<ServiceInfo> QueryServicesInumerator(
				ServiceType type,
				StateQuery state,
				string groupName)
			{
				return new Enumerator(this, state);
			}
			#endregion

			#region Enumerator

			private new unsafe class Enumerator : ServiceCollectionBase.Enumerator
			{
				#region Consts

				private static readonly Dictionary<int, string> MSGS_ENUMERATION = new Dictionary<int, string>
				{
					{ Win32API.ERROR_ACCESS_DENIED, 
						"The handle does not have the SERVICE_ENUMERATE_DEPENDENTS access right." },
					{ Win32API.ERROR_INVALID_HANDLE, "The specified handle is invalid." },
					{ Win32API.ERROR_INVALID_PARAMETER, "A parameter that was specified is invalid." },
				};
				#endregion

				#region Fields

				private DependentServicesCollection collection;
				private EnumServiceStatus* pESS = null;
				#endregion

				#region Ctor

				public Enumerator(
					DependentServicesCollection collection,
					StateQuery state)
					: base()
				{
					this.collection = collection;
					this.state = state;

					do
					{
						if (this.needed != 0)
						{
							if (this.pESS == null)
							{
								this.pESS = (EnumServiceStatus*)Marshal.AllocHGlobal((int)needed);
							}
							else
							{
								this.pESS = (EnumServiceStatus*)Marshal.ReAllocHGlobal(
									(IntPtr)this.pESS,
									(IntPtr)needed);
							}

							this.allocated = this.needed;
						}

						if (Win32API.EnumDependentServices(
							this.collection.service.Handle,
							this.state,
							this.pESS,
							this.allocated,
							out this.needed,
							out this.returned))
						{
							this.lastError = Win32API.ERROR_SUCCESS;
						}
						else
						{
							this.lastError = Marshal.GetLastWin32Error();
						}
					} while (lastError == Win32API.ERROR_MORE_DATA);

					if (lastError != Win32API.ERROR_SUCCESS)
					{
						throw ServiceException.Create(MSGS_ENUMERATION, lastError);
					}
				}
				#endregion

				#region Properties

				public override ServiceInfo Current
				{
					get
					{
						ThrowIfDisposed();
						return new ServiceInfo(this.collection.service.Scm, this.pESS[this.index]);
					}
				}
				#endregion

				#region Methods

				protected override void ThrowIfDisposed()
				{
					this.collection.service.ThrowIfDisposed();
				}

				protected override void Dispose(bool disposing)
				{
					Marshal.FreeHGlobal((IntPtr)this.pESS);
					this.pESS = null;
				}
				#endregion
			}


			#endregion
		}
	}
}
