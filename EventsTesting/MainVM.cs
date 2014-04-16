using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Windows.Services;

namespace EventsTesting
{
	class MainVM
	{
		ServiceControlManager scm;
		Service svc;

		public ObservableCollection<EventItem> Events { get; private set; }

		public MainVM()
		{
			Events = new ObservableCollection<EventItem>();

			scm = new ServiceControlManager();
			scm.ServiceCreated += (s, e) => Events.Add(new ScmEventItem("Service created", e));
			scm.ServiceDeleted += (s, e) => Events.Add(new ScmEventItem("Service deleted", e));

			svc = scm.OpenService("netlogon", ServiceAccessRights.AllAccess);
			svc.DeletePeinding += (s, e) =>
				{
					Events.Add(new ServiceEventItem(e.Service.ServiceName + " delete pending", e));
					svc.Close();
				};
			svc.Paused += (s, e) => Events.Add(new ServiceEventItem(e.Service.ServiceName + " paused", e));
			svc.Running += (s, e) => Events.Add(new ServiceEventItem(e.Service.ServiceName + " running", e));
			svc.Stopped += (s, e) => Events.Add(new ServiceEventItem(e.Service.ServiceName + " stopped", e));
		}
	}
}
