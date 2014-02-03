using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TestService
{
	public partial class TestSVC : ServiceBase
	{
		public TestSVC()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			Trace.WriteLine("The service is now started");
		}

		protected override void OnStop()
		{
			Trace.WriteLine("The service is now stopped");
		}

		protected override void OnContinue()
		{
			Trace.WriteLine("The service is now resumed");
		}

		protected override void OnPause()
		{
			Trace.WriteLine("The service is now paused");
		}
	}
}
