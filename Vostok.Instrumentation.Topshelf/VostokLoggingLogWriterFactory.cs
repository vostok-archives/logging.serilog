using System.Collections.Generic;
using Topshelf.Logging;
using Vostok.Logging;

namespace Vostok.Instrumentation.Topshelf
{
	public class VostokLoggingLogWriterFactory : LogWriterFactory
	{
		private readonly ILog log;

		public VostokLoggingLogWriterFactory(ILog log)
		{
			this.log = log;
		}

		public LogWriter Get(string name)
		{
			return new VostokLoggingLogWriterAdapter(string.IsNullOrEmpty(name) ? log : log.WithProperties(new Dictionary<string, object> { { "topshelf.name", name } }));
		}

		public void Shutdown()
		{
		}
	}
}