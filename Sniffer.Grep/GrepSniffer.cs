using CodeSniffer.Core.Sniffer;
using Serilog;

namespace Sniffer.Grep
{
    public class GrepSniffer : ICsSniffer
    {
        private readonly ILogger logger;
        private readonly GrepOptions options;


        public GrepSniffer(ILogger logger, GrepOptions options)
        {
            this.logger = logger;
            this.options = options;
        }


        public ValueTask<ICsReport?> Execute(string path, ICsScanContext context, CancellationToken cancellationToken)
        {
            // TODO scan for matching files
            // TODO scan files for matching text

            return ValueTask.FromResult<ICsReport?>(null);
        }
    }
}
