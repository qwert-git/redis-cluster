using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

const int ExpireCacheTimeInSeconds = 40;
const int ThreadsNumber = 4;
const int Delta = 8;

InitCache(ExpireCacheTimeInSeconds);

await Parallel.ForEachAsync(Enumerable.Range(1, ThreadsNumber), new ParallelOptions { MaxDegreeOfParallelism = ThreadsNumber },
    async (threadNumber, _) =>
    {
        var cache = new StoreWithProbabilisticCache(Delta, ExpireCacheTimeInSeconds);

        while (true)
        {
            for (int id = 0; id < 5; id++)
            {
                var item = cache.Retrieve(id);
            }

            Thread.Sleep(5000 + (int)(new Random().NextDouble() * 1000));
        }
    });

void InitCache(int expireTime)
{
    var cache = new StoreWithProbabilisticCache(Delta, ExpireCacheTimeInSeconds);

    Console.WriteLine("Init cache..");
    for (int id = 0; id < 5; id++)
    {
        cache.Add(id, "valueFromTheInitCacheMethod");
    }
}