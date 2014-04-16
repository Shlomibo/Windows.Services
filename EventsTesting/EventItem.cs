using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.Windows.Services;

namespace EventsTesting
{
	abstract class EventItem
	{
		public string Title { get; protected set; }
		public string Status { get; protected set; }
	}

	class ScmEventItem : EventItem
	{
		public ScmEventItem(string title, ServiceControlEventArgs e)
		{
			this.Title = title;
			this.Status = "Service name: " + e.ServiceName;
		}
	}

	class ServiceEventItem : EventItem
	{
		public ServiceEventItem(string title, ServiceEventArgs e)
		{
			this.Title = title;
			
			this.Status = string.Format(
				@"Service: {0}
Status:
	Type: {1}
	State: {2}
	Win32 exit code: {3}
	Specific exit code: {4}
	Check point: {5}
	Wait hint: {6}",
				   e.Service.DisplayName,
				   e.Status.Type,
				   e.Status.State,
				   e.Status.Win32ExitCode,
				   e.Status.ServiceSpecificExitCode,
				   e.Status.CheckPoint,
				   e.Status.WaitHint);
		}
	}
}
