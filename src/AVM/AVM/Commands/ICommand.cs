using System.Threading.Tasks;

namespace AVM.Commands
{
    public interface ICommand
    {
        Task<int> ExecuteAsync();
    }
}
