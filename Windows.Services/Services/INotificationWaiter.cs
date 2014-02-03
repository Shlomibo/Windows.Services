using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace Windows.Services
{
	/// <summary>
	/// Represent object that can wait for notifications to accure
	/// </summary>
	public interface INotificationWaiter
	{
		/// <summary>
		/// Blocks the thread until wanted notification is raised, or until the timeout elapses.
		/// </summary>
		/// <param name="waitFor">
		/// Flags of the notification to wait for. 
		/// If one of the notifications raised - the block ends.
		/// </param>
		/// <param name="millisecondsTimeout">
		/// The timeout, in milliseconds, until the block ends, even if no notification raised.
		/// </param>
		/// <param name="triggered">Return the notification that was actually raised.</param>
		/// <returns>
		/// true if one of notification was raised before the timeout elapsed;
		/// otherwise false.
		/// </returns>
		bool WaitForNotification(Notification waitFor, int millisecondsTimeout, out Notification triggered);

		/// <summary>
		/// Blocks the thread until wanted notification is raised, or until the timeout elapses.
		/// </summary>
		/// <param name="waitFor">
		/// Flags of the notification to wait for. 
		/// If one of the notifications raised - the block ends.
		/// </param>
		/// <param name="timeout">
		/// The timeout, until the block ends, even if no notification raised.
		/// </param>
		/// <param name="triggered">Return the notification that was actually raised.</param>
		/// <returns>
		/// true if one of notification was raised before the timeout elapsed;
		/// otherwise false.
		/// </returns>
		bool WaitForNotification(Notification waitFor, TimeSpan timeout, out Notification triggered);

		/// <summary>
		/// Blocks the thread until wanted notification is raised.
		/// </summary>
		/// <param name="waitFor">
		/// Flags of the notification to wait for. 
		/// If one of the notifications raised - the block ends.
		/// </param>
		/// <returns>The notification that was actually raised.</returns>
		Notification WaitForNotification(Notification waitFor);

		/// <summary>
		/// Asynchronically waits for notification to be triggered.
		/// </summary>
		/// <param name="waitFor">Flags of the notification to be waited.</param>
		/// <param name="millisecondsTimeout">The timeout in milliseconds.</param>
		/// <returns>Task for the wait operation.</returns>
		Task<TimedResult<Notification>> WaitForNotificationAsync(
			Notification waitFor,
			int millisecondsTimeout);

		/// <summary>
		/// Asynchronically waits for notification to be triggered.
		/// </summary>
		/// <param name="waitFor">Flags of the notification to be waited.</param>
		/// <param name="timeout">The timeout before wait is cancelled.</param>
		/// <returns>Task for the wait operation.</returns>
		Task<TimedResult<Notification>> WaitForNotificationAsync(
			Notification waitFor,
			TimeSpan timeout);

		/// <summary>
		/// Asynchronically waits for notification to be triggered.
		/// </summary>
		/// <param name="waitFor">Flags of the notification to be waited.</param>
		/// <returns>Task for the wait operation.</returns>
		Task<Notification> WaitForNotificationAsync(
			Notification waitFor);
	}
}
