using System.Threading.Tasks;
using MinerFusionConsole.Controllers;

namespace MinerFusionConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var fileController = new FileController();
            var minerController = new MinerController(await fileController.Setup());

            await minerController.Work();
        }
    }
}
