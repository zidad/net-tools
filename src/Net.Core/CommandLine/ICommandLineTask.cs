using System.Threading;
using System.Threading.Tasks;

namespace Net.CommandLine
{
    public interface ICommandLineTask<in TParameters>
    {
        Task Run(TParameters parameters, CancellationToken token);
    }

    public interface ICommandLineTask
    {
        Task Run(CancellationToken cancellationToken);
    }
}