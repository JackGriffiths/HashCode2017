﻿using System.Collections.Generic;
using System.Linq;

namespace HashCode2017 {
    public class Endpoint {
        public int Id { get; set; }
        public int DataCenterLatency { get; set; }
        public Dictionary<CacheServer, int> CacheServerLatencies { get; set; }
        public Dictionary<Video, int> VideoRequests { get; set; }

        public Endpoint() {
            CacheServerLatencies = new Dictionary<CacheServer, int>();
            VideoRequests = new Dictionary<Video, int>();
        }

        public int CalculateBestLatencySaving() {
            if (!CacheServerLatencies.Any()) {
                return 0;
            }

            return DataCenterLatency - CacheServerLatencies.Min(i => i.Value);
        }

        public void TryCacheVideoAtClosestServer(Video video) {
            var orderedServers = CacheServerLatencies
                .OrderBy(i => i.Value) // Order by latency
                .Select(i => i.Key);

            foreach (var server in orderedServers) {
                if (server.Videos.Contains(video)) {
                    // Already cached.
                    break;
                }

                if (server.CanAddVideo(video)) {
                    server.AddVideo(video);

                    break;
                }
            }
        }
    }
}
