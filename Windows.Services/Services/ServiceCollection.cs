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
	partial class ServiceControlManager
	{
		/// <summary>
		/// A collection of statuses of the services in a service control manager database.
		/// </summary>
		public class ServiceCollection : ServiceCollectionBase
		{
			#region Fields

			private ServiceControlManager scm;
			#endregion

			#region Properties

			/// <summary>
			/// Queries for services of specific types, states, or under specific groups
			/// </summary>
			/// <param name="type">The type of services to be enumerated.</param>
			/// <param name="state">The state of the services to be enumerated.</param>
			/// <param name="groupName">
			/// The load-order group name. 
			/// The only services enumerated are those that belong to the group that has the name specified by the string. 
			/// If this parameter is an empty string, only services that do not belong to any group are enumerated. 
			/// If this parameter is NULL, group membership is ignored and all services are enumerated.
			/// </param>
			/// <returns>An enumerable that enumerates the corresponding services' status</returns>
			public IEnumerable<ServiceInfo> this[
				ServiceType type,
				StateQuery state,
				string groupName]
			{
				get { return QueryServicesInumerator(type, state, groupName); }
			}

			/// <summary>
			/// Queries for services of specific types, states, or have specific groups membership
			/// </summary>
			/// <param name="type">The type of services to be enumerated.</param>
			/// <param name="state">The state of the services to be enumerated.</param>
			/// <param name="groupMembership">
			/// Specifies which services to be enumerated base on loading group membership
			/// </param>
			/// <returns>An enumerable that enumerates the corresponding services' status</returns>
			public IEnumerable<ServiceInfo> this[
				ServiceType type,
				StateQuery state,
				LoadGroupMembership groupMembership]
			{
				get
				{
					string group = groupMembership == LoadGroupMembership.NotInGroup
						? NOT_IN_GROUP
						: ALL_GROUPS;

					return this[type, state, group];
				}
			}

			/// <summary>
			/// Queries for services of specific types, or states
			/// </summary>
			/// <param name="type">The type of services to be enumerated.</param>
			/// <param name="state">The state of the services to be enumerated.</param>
			/// <returns>An enumerable that enumerates the corresponding services' status</returns>
			public IEnumerable<ServiceInfo> this[
				ServiceType type,
				StateQuery state]
			{
				get { return this[type, state, ALL_GROUPS]; }
			}

			/// <summary>
			/// Queries for services of specific types
			/// </summary>
			/// <param name="type">The type of services to be enumerated.</param>
			/// <returns>An enumerable that enumerates the corresponding services' status</returns>
			public IEnumerable<ServiceInfo> this[ServiceType type]
			{
				get { return this[type, StateQuery.All]; }
			}

			/// <summary>
			/// Queries for services of specific states, or under specific groups
			/// </summary>
			/// <param name="state">The state of the services to be enumerated.</param>
			/// <param name="groupName">
			/// The load-order group name. 
			/// The only services enumerated are those that belong to the group that has the name specified by the string. 
			/// If this parameter is an empty string, only services that do not belong to any group are enumerated. 
			/// If this parameter is NULL, group membership is ignored and all services are enumerated.
			/// </param>
			/// <returns>An enumerable that enumerates the corresponding services' status</returns>
			public IEnumerable<ServiceInfo> this[
				StateQuery state,
				string groupName]
			{
				get { return this[ServiceType.All, state, groupName]; }
			}

			/// <summary>
			/// Queries for services of specific states, or have specific groups membership
			/// </summary>
			/// <param name="state">The state of the services to be enumerated.</param>
			/// <param name="groupMembership">
			/// Specifies which services to be enumerated base on loading group membership
			/// </param>
			/// <returns>An enumerable that enumerates the corresponding services' status</returns>
			public IEnumerable<ServiceInfo> this[
				StateQuery state,
				LoadGroupMembership groupMembership]
			{
				get
				{
					string group = groupMembership == LoadGroupMembership.NotInGroup
						? NOT_IN_GROUP
						: ALL_GROUPS;

					return this[state, group];
				}
			}

			/// <summary>
			/// Queries for services of specific states
			/// </summary>
			/// <param name="state">The state of the services to be enumerated.</param>
			/// <returns>An enumerable that enumerates the corresponding services' status</returns>
			public IEnumerable<ServiceInfo> this[StateQuery state]
			{
				get { return this[state, ALL_GROUPS]; }
			}

			/// <summary>
			/// Queries for services under specific groups
			/// </summary>
			/// <param name="groupName">
			/// The load-order group name. 
			/// The only services enumerated are those that belong to the group that has the name specified by the string. 
			/// If this parameter is an empty string, only services that do not belong to any group are enumerated. 
			/// If this parameter is NULL, group membership is ignored and all services are enumerated.
			/// </param>
			/// <returns>An enumerable that enumerates the corresponding services' status</returns>
			public IEnumerable<ServiceInfo> this[string groupName]
			{
				get { return this[StateQuery.All, groupName]; }
			}

			/// <summary>
			/// Queries for services that have specific groups membership
			/// </summary>
			/// <param name="groupMembership">
			/// Specifies which services to be enumerated base on loading group membership
			/// </param>
			/// <returns>An enumerable that enumerates the corresponding services' status</returns>
			public IEnumerable<ServiceInfo> this[LoadGroupMembership groupMembership]
			{
				get
				{
					string group = groupMembership == LoadGroupMembership.NotInGroup
						? NOT_IN_GROUP
						: ALL_GROUPS;

					return this[group];
				}
			}
			#endregion

			#region Ctor

			internal ServiceCollection(ServiceControlManager scm)
			{
				this.scm = scm;
			}
			#endregion

			#region Methods

			/// <summary>
			/// Throws an exception if the collection is disposed.
			/// </summary>
			protected override void ThrowIfDisposed()
			{
				this.scm.ThrowIfDisposed();
			}

			/// <summary>
			/// Queries for services of specific types, states, or under specific groups
			/// </summary>
			/// <param name="type">The type of services to be enumerated.</param>
			/// <param name="state">The state of the services to be enumerated.</param>
			/// <param name="groupName">
			/// The load-order group name. 
			/// The only services enumerated are those that belong to the group that has the name specified by the string. 
			/// If this parameter is an empty string, only services that do not belong to any group are enumerated. 
			/// If this parameter is NULL, group membership is ignored and all services are enumerated.
			/// </param>
			/// <returns>An enumerable that enumerates the corresponding services' status</returns>
			protected override unsafe IEnumerable<ServiceInfo> QueryServicesInumerator(
				ServiceType type,
				StateQuery state,
				string groupName)
			{
				ThrowIfDisposed();
				return new Enumerator(this, type, state, groupName);
			}
			#endregion

			#region Enumerator

			private new unsafe class Enumerator : ServiceCollectionBase.Enumerator
			{
				#region Consts

				private static readonly Dictionary<int, string> MSGS_ENUM_ERRORS = new Dictionary<int, string>()
				{
					{ Win32API.ERROR_ACCESS_DENIED, "The handle does not have the SC_MANAGER_ENUMERATE_SERVICE access right." },
					{ Win32API.ERROR_INVALID_PARAMETER, "An illegal parameter value was used." },
					{ Win32API.ERROR_INVALID_HANDLE, "The handle is invalid." },
					{ Win32API.ERROR_INVALID_LEVEL, "The InfoLevel parameter contains an unsupported value." },
					{ Win32API.ERROR_SHUTDOWN_IN_PROGRESS, "The system is shutting down; this function cannot be called." },
				};
				#endregion

				#region Fields


				private uint resumeHandle = 0;
				private ServiceCollection collection;
				private ServiceType type;
				private string groupName;
				private EnumServiceStatusProcess* pESSP = null;
				#endregion

				#region Properties

				public override ServiceInfo Current
				{
					get
					{
						ThrowIfDisposed();
						return new ServiceInfo(this.collection.scm, this.pESSP[this.index]);
					}
				}
				#endregion

				#region Ctor

				public Enumerator(
					ServiceCollection collection,
					ServiceType type,
					StateQuery state,
					string groupName)
					: base()
				{
					this.collection = collection;
					this.type = type;
					this.state = state;
					this.groupName = groupName;

					// Allocating and loading services buffer
					LoadBuffer();
				}
				#endregion

				#region Methods

				private void LoadBuffer()
				{
					// Checking if memory should be allocated.
					if ((this.needed == 0) ||
						(this.needed > this.allocated))
					{
						// If memory wasn't allocated before, querying the needed buffer size.
						if (this.needed == 0)
						{
							this.lastError = EnumServiceStatus();

							ThrowIfError(this.lastError);
						}

						// If last run failed due to small buffer - reallocating it.
						if (this.lastError == Win32API.ERROR_MORE_DATA)
						{
							// Allocating/reallocating the buffer
							this.pESSP = this.pESSP == null
								? (EnumServiceStatusProcess*)Marshal.AllocHGlobal((int)this.needed)
								: (EnumServiceStatusProcess*)Marshal.ReAllocHGlobal(
								(IntPtr)this.pESSP,
								(IntPtr)this.needed);

							this.allocated = this.needed;
						}
					}

					// Loading data to buffer
					this.lastError = EnumServiceStatus();

					ThrowIfError(this.lastError);
				}

				private void ThrowIfError(int lastError)
				{
					// Throwing exception if invalid last error.
					if ((lastError != Win32API.ERROR_SUCCESS) &&
						(lastError != Win32API.ERROR_MORE_DATA))
					{
						throw ServiceException.Create(MSGS_ENUM_ERRORS, lastError);
					}
				}

				public override bool MoveNext()
				{
					this.index++;
					bool haveItem = true;

					// Checking if index have reached the end of the buffer
					if (this.index >= this.returned)
					{
						// If last call was success, there are no more items
						if (this.lastError == Win32API.ERROR_SUCCESS)
						{
							haveItem = false;
						}
						// Otherwise, we should load the next page, and reset the index
						else
						{
							LoadBuffer();
							this.index = 0;
							haveItem = this.returned > 0;
						}
					}

					return haveItem;
				}

				private unsafe int EnumServiceStatus()
				{
					if (Win32API.EnumServicesStatus(
							this.collection.scm.Handle,
							Win32API.SC_ENUM_PROCESS_INFO,
							this.type,
							this.state,
							this.pESSP,
							this.allocated,
							out this.needed,
							out this.returned,
							ref this.resumeHandle,
							this.groupName))
					{
						return NO_ERROR;
					}
					else
					{
						return Marshal.GetLastWin32Error();
					}
				}

				protected override void ThrowIfDisposed()
				{
					this.collection.scm.ThrowIfDisposed();
				}

				protected override void Dispose(bool disposing)
				{
					Marshal.FreeHGlobal((IntPtr)pESSP);
					this.pESSP = null;
				}

				public override void Reset()
				{
					base.Reset();
					this.resumeHandle = 0;
				}
				#endregion
			}
			#endregion
		}
	}
}
