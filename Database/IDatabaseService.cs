using System.Threading.Tasks;

namespace DiscordSandbot.Database
{
    public interface IDatabaseService
    {
        Task SetupAsync();
        Task DestroyAsync();
    }
}