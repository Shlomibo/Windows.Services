using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Windows.Services.XPCompatibility
{
	/// <summary>
	/// Provides extension methods for Service
	/// </summary>
	[Obsolete("WinXp is no longer supported", true)]
	public static class ServiceExtensions
	{
		#region Consts

		private const int RECHECK_TIMEOUT = 500;
		#endregion

		#region Methods

		/// <summary>
		/// Asynchronically waits for service to change state.
		/// </summary>
		/// <param name="service">The service to wait for its status</param>
		/// <param name="token">Cancellation token for the operation</param>
		/// <param name="millisecondsTimeout">A timeout in milliseconds before the wait is cancelled.</param>
		/// <param name="states">The states to wait for.</param>
		/// <returns>A task for the wait operation.</returns>
		/// <remarks>
		/// This method is deprecated on Windows Vista and above.
		/// It is recommended to to use WaitForNotificationAsync instead.
		/// </remarks>
		public static async Task<bool> WaitForStatesAsync(
			this Service service,
			CancellationToken token,
			int millisecondsTimeout,
			params State[] states)
		{
			var timeoutWatch = Stopwatch.StartNew();
			ServiceStatus status = service.Status;

			while (states.All(state => state != status.State) &&
				!token.IsCancellationRequested &&
				((millisecondsTimeout == Timeout.Infinite) || (timeoutWatch.ElapsedMilliseconds < millisecondsTimeout)))
			{
				await Task.Delay(RECHECK_TIMEOUT, token);
				status = service.Status;
			}

			timeoutWatch.Stop();

			return !token.IsCancellationRequested &&
				states.Any(state => state == status.State); ;
		}

		/// <summary>
		/// Asynchronically waits for service to change state.
		/// </summary>
		/// <param name="service">The service to wait for its status</param>
		/// <param name="token">Cancellation token for the operation</param>
		/// <param name="timeout">A timeout before the wait is cancelled.</param>
		/// <param name="states">The states to wait for.</param>
		/// <returns>A task for the wait operation.</returns>
		/// <remarks>
		/// This method is deprecated on Windows Vista and above.
		/// It is recommended to to use WaitForNotificationAsync instead.
		/// </remarks>
		public static Task<bool> WaitForStatesAsync(
			this Service service,
			CancellationToken token,
			TimeSpan timeout,
			params State[] states) =>
			WaitForStatesAsync(service, token, timeout.Milliseconds, states);

		/// <summary>
		/// Asynchronically waits for service to change state.
		/// </summary>
		/// <param name="service">The service to wait for its status</param>
		/// <param name="token">Cancellation token for the operation</param>
		/// <param name="states">The states to wait for.</param>
		/// <returns>A task for the wait operation.</returns>
		/// <remarks>
		/// This method is deprecated on Windows Vista and above.
		/// It is recommended to to use WaitForNotificationAsync instead.
		/// </remarks>
		public static Task<bool> WaitForStatesAsync(
			this Service service,
			CancellationToken token,
			params State[] states) =>
			WaitForStatesAsync(service, token, Timeout.Infinite, states);

		/// <summary>
		/// Asynchronically waits for service to change state.
		/// </summary>
		/// <param name="service">The service to wait for its status</param>
		/// <param name="timeout">A timeout before the wait is cancelled.</param>
		/// <param name="states">The states to wait for.</param>
		/// <returns>A task for the wait operation.</returns>
		/// <remarks>
		/// This method is deprecated on Windows Vista and above.
		/// It is recommended to to use WaitForNotificationAsync instead.
		/// </remarks>
		public static Task<bool> WaitForStatesAsync(
			this Service service,
			TimeSpan timeout,
			params State[] states) =>
			WaitForStatesAsync(service, CancellationToken.None, timeout, states);

		/// <summary>
		/// Asynchronically waits for service to change state.
		/// </summary>
		/// <param name="service">The service to wait for its status</param>
		/// <param name="millisecondsTimeout">A timeout in milliseconds before the wait is cancelled.</param>
		/// <param name="states">The states to wait for.</param>
		/// <returns>A task for the wait operation.</returns>
		/// <remarks>
		/// This method is deprecated on Windows Vista and above.
		/// It is recommended to to use WaitForNotificationAsync instead.
		/// </remarks>
		public static Task<bool> WaitForStatesAsync(
			this Service service,
			int millisecondsTimeout,
			params State[] states) =>
			WaitForStatesAsync(service, CancellationToken.None, millisecondsTimeout, states);

		/// <summary>
		/// Asynchronically waits for service to change state.
		/// </summary>
		/// <param name="service">The service to wait for its status</param>
		/// <param name="states">The states to wait for.</param>
		/// <returns>A task for the wait operation.</returns>
		/// <remarks>
		/// This method is deprecated on Windows Vista and above.
		/// It is recommended to to use WaitForNotificationAsync instead.
		/// </remarks>
		public static async Task WaitForStatesAsync(
			this Service service,
			params State[] states) =>
			await WaitForStatesAsync(service, Timeout.Infinite, states);

		/// <summary>
		/// Blocks the calling thread until the service state is changed to the given state.
		/// </summary>
		/// <param name="service">The service to wait for its status</param>
		/// <param name="millisecondsTimeout">A timeout in milliseconds before the wait is cancelled.</param>
		/// <param name="states">The states to wait for.</param>
		/// <returns>
		/// true if the state of the service has changed, before the timeout was elapsed; otherwise false.
		/// </returns>
		/// <remarks>
		/// This method is deprecated on Windows Vista and above.
		/// It is recommended to to use WaitForNotification instead.
		/// </remarks>
		public static bool WaitForStates(this Service service, int millisecondsTimeout, params State[] states)
		{
			Task<bool> waitTask = WaitForStatesAsync(service, millisecondsTimeout, states);
			waitTask.Wait();

			if (waitTask.IsFaulted)
			{
				throw new Exception("Error while wating for state to change", waitTask.Exception);
			}

			return waitTask.Result;
		}

		/// <summary>
		/// Blocks the calling thread until the service state is changed to the given state.
		/// </summary>
		/// <param name="service">The service to wait for its status</param>
		/// <param name="states">The states to wait for.</param>
		/// <remarks>
		/// This method is deprecated on Windows Vista and above.
		/// It is recommended to to use WaitForNotification instead.
		/// </remarks>
		public static void WaitForStates(this Service service, params State[] states) =>
			WaitForStates(service, Timeout.Infinite, states);

		/// <summary>
		/// Blocks the calling thread until the service state is changed to the given state.
		/// </summary>
		/// <param name="service">The service to wait for its status</param>
		/// <param name="timeout">A timeout before the wait is cancelled.</param>
		/// <param name="states">The states to wait for.</param>
		/// <returns>
		/// true if the state of the service has changed, before the timeout was elapsed; otherwise false.
		/// </returns>
		/// <remarks>
		/// This method is deprecated on Windows Vista and above.
		/// It is recommended to to use WaitForNotification instead.
		/// </remarks>
		public static bool WaitForStates(this Service service, TimeSpan timeout, params State[] states) =>
			WaitForStates(service, timeout.Milliseconds, states);

		/// <summary>
		/// Stops the service.
		/// </summary>
		/// <returns>The reported status of the service after the control was processed.</returns>
		public static ServiceStatus Stop(this Service service) =>
			service.SendControl(ControlCode.Stop);
		#endregion
	}
}
