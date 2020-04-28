namespace MinerFusionConsole.Infrastructure
{
    public static class API
    {
        public static class Miner
        {
            public static string AddMinerData(string baseUri) => $"{baseUri}/AddMinerData";
        }
    }
}
