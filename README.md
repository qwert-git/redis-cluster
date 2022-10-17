# Probabilistic Cache with Redis
It is a learning project to set up the Redis cluster with Sentinel and try the probabilistic cache to solve the Cache stampede problem.
## Redis cluster
The Redis cluster was created with the docker-compose file. It consists of one master, one slave, and one sentinel server, as an example, but for production purposes, it could be scaled. The Redis team recommends using at least three sentinel servers for production purposes.
## Probabilistic cache
The probabilistic cache implemented with the test console application could be used for production implementation as an example.

### Log examples:
```
[11:05:57 AM] valueFromTheDatabaseWithId4 cache, probability diff = 31120.844482421875
[11:05:57 AM] valueFromTheDatabaseWithId4 cache, probability diff = 24677.632080078125
[11:06:02 AM] valueFromTheDatabaseWithId0 cache, probability diff = 6869.6201171875
[11:06:02 AM] valueFromTheDatabaseWithId1 cache, probability diff = 4378.9111328125
[11:06:02 AM] Cache with key 'ProbabilisticCacheTest:ExpireCache:1' updating...
```
