using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Services
{
	/// <summary>
	/// Throws an exception for a Win32 service error code.
	/// </summary>
	[Serializable]
	public class ServiceException : Win32Exception
	{
		/// <summary>
		/// Initializes a new instance of the ServiceException class with the last Win32 error that occurred.
		/// </summary>
		public ServiceException() : base() { }

		/// <summary>
		/// Initializes a new instance of the ServiceException class with the specified error.
		/// </summary>
		/// <param name="error">The Win32 error code associated with this exception.</param>
		public ServiceException(int error) : base(error) { }

		/// <summary>
		/// Initializes a new instance of the ServiceException class with 
		/// the specified error and the specified detailed description.
		/// </summary>
		/// <param name="error">The Win32 error code associated with this exception.</param>
		/// <param name="message">A detailed description of the error.</param>
		public ServiceException(int error, string message) : base(error, message) { }

		/// <summary>
		/// Initializes a new instance of the ServiceException class with the specified detailed description.
		/// </summary>
		/// <param name="message">A detailed description of the error.</param>
		public ServiceException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the ServiceException class with
		/// the specified detailed description and the specified exception.
		/// </summary>
		/// <param name="message">A detailed description of the error.</param>
		/// <param name="inner">A reference to the inner exception that is the cause of this exception.</param>
		public ServiceException(string message, Exception inner) : base(message, inner) { }

		/// <summary>
		/// Initializes a new instance of the ServiceException class with
		/// the specified context and the serialization information.
		/// </summary>
		/// <param name="info">The SerializationInfo associated with this exception.</param>
		/// <param name="context">A StreamingContext that represents the context of this exception.</param>
		protected ServiceException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }

		internal static ServiceException Create(IDictionary<int, string> messages, int error)
		{
			ServiceException ex;

			if (messages.ContainsKey(error))
			{
				ex = new ServiceException(error, messages[error]);
			}
			else
			{
				ex = new ServiceException(error);
			}

			return ex;
		}
	}

	/// <summary>
	/// Exception for features that doesn't supported on certain OS
	/// </summary>
	[Serializable]
	public class FeatureNotSupportedException : Exception
	{
		#region Consts

		private const string ERROR_MSG = "The requested feature is unsupported under current operating system version";
		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the FeatureNotSupportedException class with the default message
		/// </summary>
		public FeatureNotSupportedException() : base(ERROR_MSG) { }

		/// <summary>
		/// Initializes a new instance of the FeatureNotSupportedException class with 
		/// inner ServiceException with the specified error.
		/// </summary>
		/// <param name="error">The Win32 error code associated with the inner exception.</param>
		public FeatureNotSupportedException(int error) : base(ERROR_MSG, new ServiceException(error)) { }

		/// <summary>
		/// Initializes a new instance of the ServiceException class with 
		/// inner ServiceException with the specified error and the specified detailed description.
		/// </summary>
		/// <param name="error">The Win32 error code associated with the inner exception.</param>
		/// <param name="message">A detailed description of the error for the inner exception.</param>
		public FeatureNotSupportedException(int error, string message)
			: base(ERROR_MSG, new ServiceException(error, message)) { }

		/// <summary>
		/// Initializes a new instance of the FeatureNotSupportedException class with the default message,
		/// and the specified inner exception
		/// </summary>
		/// <param name="inner">A reference to the inner exception that is the cause of this exception.</param>
		public FeatureNotSupportedException(Exception inner) : base(ERROR_MSG, inner) { }

		/// <summary>
		/// Initializes a new instance of the Exception class with serialized data.
		/// </summary>
		/// <param name="info">
		/// The SerializationInfo that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The StreamingContext that contains contextual information about the source or destination.
		/// </param>
		protected FeatureNotSupportedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { } 
		#endregion

		#region Methods

		/// <summary>
		/// Return the specified exception, if the errorCode is specified in generateUnsupportedFor;
		/// Otherwise, return new FeatureNotSupportedException, with the specified exception as inner exception.
		/// </summary>
		/// <param name="inner">The exception to return</param>
		/// <param name="errorCode">The error code to check</param>
		/// <param name="generateUnsupportedFor">List of error codes that means that the feature is unspported</param>
		/// <returns>The given exception, or FeatureNotSupportedException</returns>
		public static Exception GetUnsupportedForCodes(
			Exception inner,
			int errorCode, 
			params int[] generateUnsupportedFor)
		{
			if (!generateUnsupportedFor.Contains(errorCode))
			{
				return inner;
			}
			else
			{
				return new FeatureNotSupportedException(inner);
			}
		}
		#endregion
	}
}
