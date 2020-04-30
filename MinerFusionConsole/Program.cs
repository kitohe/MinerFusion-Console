using System.Threading.Tasks;
using MinerFusionConsole.Controllers;

namespace MinerFusionConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var fileCtrl = new FileController();
            var minerCtrl = new MinerController(await fileCtrl.Setup());

            await minerCtrl.Work();
        }
    }
}
