using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Interop;

namespace Windows.Services
{
	/// <summary>
	/// A services' status collection
	/// </summary>
	public abstract class ServiceCollectionBase : ICollection<ServiceInfo>
	{
		#region Consts

		/// <summary>
		/// No error code
		/// </summary>
		protected const int NO_ERROR = 0;

		/// <summary>
		/// String for all groups
		/// </summary>
		public const string ALL_GROUPS = Win32API.ALL_GROUPS;

		/// <summary>
		/// String for ungrouped services
		/// </summary>
		public const string NOT_IN_GROUP = Win32API.NOT_GROUPED;
		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of elements contained in the collection.
		/// </summary>
		public int Count => this.Count();

		bool ICollection<ServiceInfo>.IsReadOnly => true;
		#endregion

		#region Ctor

		internal ServiceCollectionBase() { }
		#endregion

		#region Methods

		/// <summary>
		/// Throws an exception if the collection is disposed.
		/// </summary>
		protected abstract void ThrowIfDisposed();

		/// <summary>
		/// Gets the result of 'getter' if the object isn't disposed; otherwise, throws.
		/// </summary>
		/// <typeparam name="TResult">The type of the returned value from getter.</typeparam>
		/// <param name="getter">A delegate to retrieve a value.</param>
		/// <returns>The value returned from getter.</returns>
		protected TResult GetOrThrowIfDisposed<TResult>(Func<TResult> getter)
		{
			if (getter == null)
			{
				throw new NullReferenceException(nameof(getter));
			}

			ThrowIfDisposed();
			return getter();
		}

		void ICollection<ServiceInfo>.Add(ServiceInfo item)
		{
			throw new NotSupportedException();
		}

		void ICollection<ServiceInfo>.Clear()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Copies the elements of the collection to an Array, starting at a particular Array index.
		/// </summary>
		/// <param name="array">
		/// The one-dimensional Array that is the destination of the elements copied from collection.
		/// The Array must have zero-based indexing.
		/// </param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(ServiceInfo[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new NullReferenceException(nameof(array));
			}

			if ((arrayIndex < 0) || (arrayIndex >= array.Length))
			{
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));
			}

			if (this.Count > array.Length - arrayIndex)
			{
				throw new ArgumentException();
			}

			foreach (ServiceInfo service in this)
			{
				array[arrayIndex++] = service;
			}
		}

		/// <summary>
		/// Determines whether the collection contains a specific value.
		/// </summary>
		/// <param name="service">The service status to locate in the collection.</param>
		/// <returns>true if service status is found in the collection; otherwise, false.</returns>
		public bool Contains(ServiceInfo service) =>
			GetOrThrowIfDisposed(() =>
				this.AsEnumerable().Contains(service));

		bool ICollection<ServiceInfo>.Remove(ServiceInfo item)
		{
			throw new NotSupportedException("Open and delete the service instead");
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>A IEnumerator&lt;ServiceInfo&gt; that can be used to iterate through the collection.</returns>
		public IEnumerator<ServiceInfo> GetEnumerator() =>
			QueryServicesInumerator(ServiceType.All, StateQuery.All, ALL_GROUPS).GetEnumerator();

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
		protected abstract IEnumerable<ServiceInfo> QueryServicesInumerator(
				ServiceType type,
				StateQuery state,
				string groupName);

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();
		#endregion

		/// <summary>
		/// Enumerator class
		/// </summary>
		protected unsafe abstract class Enumerator : IEnumerable<ServiceInfo>, IEnumerator<ServiceInfo>
		{
			#region Consts

			/// <summary>
			/// Reseted index
			/// </summary>
			protected const int RESETED = -1;
			#endregion

			#region Fields

			/// <summary>Query state</summary>
			protected StateQuery state;
			/// <summary>Last error</summary>
			protected int lastError = NO_ERROR;
			/// <summary>Allocated bytes count</summary>
			protected uint allocated = 0;
			/// <summary>Needed bytes count</summary>
			protected uint needed = 0;
			/// <summary>Items returned</summary>
			protected uint returned = 0;
			/// <summary>Current index</summary>
			protected int index;
			#endregion

			#region Ctor

			/// <summary>
			/// Creates new instance
			/// </summary>
			internal Enumerator()
			{
				Reset();
			}
			#endregion

			#region Properties

			/// <summary>
			/// Gets the element in the collection at the current position of the enumerator.
			/// </summary>
			public abstract ServiceInfo Current { get; }

			object IEnumerator.Current => this.Current;
			#endregion

			#region Ctor

			/// <summary>
			/// Destruct the enumerator
			/// </summary>
			~Enumerator()
			{
				Dispose(false);
			}
			#endregion

			#region Methods

			/// <summary>
			/// Throw an exception if the enumerator is disposed
			/// </summary>
			protected abstract void ThrowIfDisposed();

			/// <summary>
			/// Gets the result of 'getter' if the object isn't disposed; otherwise, throws.
			/// </summary>
			/// <typeparam name="TResult">The type of the returned value from getter.</typeparam>
			/// <param name="getter">A delegate to retrieve a value.</param>
			/// <returns>The value returned from getter.</returns>
			protected TResult GetOrThrowIfDisposed<TResult>(Func<TResult> getter)
			{
				if (getter == null)
				{
					throw new NullReferenceException(nameof(getter));
				}

				ThrowIfDisposed();
				return getter();
			}

			/// <summary>
			/// Returns an enumerator that iterates through the collection.
			/// </summary>
			/// <returns>A IEnumerator&lt;ServiceInfo&gt; that can be used to iterate through the collection.</returns>
			public IEnumerator<ServiceInfo> GetEnumerator() =>
				this;

			IEnumerator IEnumerable.GetEnumerator() =>
				GetEnumerator();

			/// <summary>
			/// Disposes the enumerator
			/// </summary>
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// Release resources
			/// </summary>
			/// <param name="disposing">Value indicates if the enumerator is disposing correctly</param>
			protected abstract void Dispose(bool disposing);

			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>
			/// true if the enumerator was successfully advanced to the next element;
			/// false if the enumerator has passed the end of the collection.
			/// </returns>
			public virtual bool MoveNext()
			{
				ThrowIfDisposed();
				this.index++;

				return this.index < this.returned;
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element in the collection. 
			/// </summary>
			public virtual void Reset()
			{
				this.index = RESETED;
			}
			#endregion
		}
	}
}
