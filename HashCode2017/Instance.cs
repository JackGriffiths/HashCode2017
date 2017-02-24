using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HashCode2017 {
    public class Instance {

        private string name;
        private int score;

        private List<CacheServer> cacheServers;
        private List<Video> videos;
        private List<Endpoint> endpoints;

        public Instance(string name) {
            this.name = name;

            cacheServers = new List<CacheServer>();
            videos = new List<Video>();
            endpoints = new List<Endpoint>();
        }

        public int RunA() {
            ReadInputFile();
            CacheVideosA();
            WriteOutputFile();
            CalculateScore();

            return score;
        }

        public int RunB() {
            ReadInputFile();
            CacheVideosB();
            WriteOutputFile();
            CalculateScore();

            return score;
        }

        public int RunC() {
            ReadInputFile();
            CacheVideosC();
            WriteOutputFile();
            CalculateScore();

            return score;
        }

        private void ReadInputFile() {
            using (var fs = File.OpenRead(Path.Combine("Input Files", name + ".in"))) {
                using (var reader = new StreamReader(fs)) {
                    int[] props = reader.ReadLine().SplitByCharAndConvertToInts(' ');

                    int numberVideos = props[0];
                    int numberEndpoints = props[1];
                    int numberRequestDescs = props[2];
                    int numberCacheServers = props[3];
                    int cacheServerSize = props[4];

                    for (int i = 0; i < numberCacheServers; i++) {
                        cacheServers.Add(new CacheServer() {
                            Id = i,
                            MaxCapacity = cacheServerSize,
                        });
                    }

                    int[] videoSizes = reader.ReadLine().SplitByCharAndConvertToInts(' ');

                    for (int i = 0; i < numberVideos; i++) {
                        videos.Add(new Video() {
                            Id = i,
                            Size = videoSizes[i],
                        });
                    }

                    for (int i = 0; i < numberEndpoints; i++) {
                        int[] endpointProps = reader.ReadLine().SplitByCharAndConvertToInts(' ');

                        var endpoint = new Endpoint() {
                            Id = i,
                            DataCenterLatency = endpointProps[0],
                        };

                        for (int j = 0; j < endpointProps[1]; j++) {
                            int[] cacheServerProps = reader.ReadLine().SplitByCharAndConvertToInts(' ');

                            endpoint.CacheServerLatencies.Add(
                                cacheServers.Single(c => c.Id == cacheServerProps[0]),
                                cacheServerProps[1]);
                        }

                        endpoints.Add(endpoint);
                    }

                    for (int i = 0; i < numberRequestDescs; i++) {
                        int[] descProps = reader.ReadLine().SplitByCharAndConvertToInts(' ');

                        var video = videos.Single(v => v.Id == descProps[0]);
                        var endpoint = endpoints.Single(e => e.Id == descProps[1]);
                        int numRequests = descProps[2];

                        if (endpoint.VideoRequests.ContainsKey(video)) {
                            endpoint.VideoRequests[video] += numRequests;
                        } else {
                            endpoint.VideoRequests.Add(video, numRequests);
                        }
                    }
                }
            }
        }

        public void CacheVideosA() {
            foreach (var endpoint in endpoints) {
                var videoRequests = endpoint.VideoRequests
                    .Select(i => new {
                        Video = i.Key,
                        Requests = i.Value,
                    })
                    .OrderByDescending(i => i.Requests);

                foreach (var videoRequest in videoRequests) {
                    var video = videoRequest.Video;

                    var orderedServers = endpoint.CacheServerLatencies
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

        public void CacheVideosB() {
            var requests = endpoints
                .SelectMany(e => e.VideoRequests, (e, vr) => new {
                    Endpoint = e,
                    Video = vr.Key,
                    NumberOfRequests = vr.Value,
                })
                .OrderByDescending(r => r.NumberOfRequests * r.Endpoint.CalculateBestLatencySaving());

            foreach (var request in requests) {
                var video = request.Video;

                var orderedServers = request.Endpoint.CacheServerLatencies
                    .OrderBy(i => i.Value) // Order by latency
                    .Select(i => i.Key);

                foreach (var server in orderedServers) {
                    if (server.CanAddVideo(video)) {
                        server.AddVideo(video);

                        break;
                    }
                }
            }
        }

        public void CacheVideosC() {
            int minVideoSize = videos.Min(i => i.Size);
            int maxVideoSize = videos.Max(i => i.Size);

            var requests = endpoints
                .SelectMany(e => e.VideoRequests, (e, vr) => new {
                    Endpoint = e,
                    Video = vr.Key,
                    NumberOfRequests = vr.Value,
                })
                .OrderByDescending(r =>
                    r.NumberOfRequests *
                    r.Endpoint.CalculateBestLatencySaving() *
                    (1 - Scale(minVideoSize, maxVideoSize, r.Video.Size)));

            foreach (var request in requests) {
                var video = request.Video;

                var orderedServers = request.Endpoint.CacheServerLatencies
                    .OrderBy(i => i.Value) // Order by latency
                    .Select(i => i.Key);

                foreach (var server in orderedServers) {
                    if (server.CanAddVideo(video)) {
                        server.AddVideo(video);

                        break;
                    }
                }
            }
        }

        private decimal Scale(int min, int max, int value) {
            return (decimal)(value - min) / (max - min);
        }

        private void WriteOutputFile() {
            using (var fs = File.Create(name + ".out")) {
                using (var writer = new StreamWriter(fs)) {
                    var serversWithVideos = cacheServers.Where(s => s.Videos.Any());

                    writer.WriteLine(serversWithVideos.Count());

                    foreach (var server in serversWithVideos) {
                        writer.Write(server.Id);
                        foreach (var video in server.Videos) {
                            writer.Write(" " + video.Id);
                        }

                        writer.WriteLine();
                    }
                }
            }
        }

        private void CalculateScore() {
            long totalSaved = 0;
            long totalRequests = 0;

            foreach (var endpoint in endpoints) {
                foreach (var request in endpoint.VideoRequests) {
                    var serversWithVideo = endpoint.CacheServerLatencies
                        .Where(l => l.Key.Videos.Contains(request.Key))
                        .OrderBy(l => l.Value);

                    totalSaved += (endpoint.DataCenterLatency - (serversWithVideo.Any() ? serversWithVideo.First().Value : endpoint.DataCenterLatency)) * request.Value;
                    totalRequests += request.Value;
                }
            }

            score = Convert.ToInt32(Math.Round(1000 * (totalSaved / (decimal)totalRequests), 0));

            Console.WriteLine($"Score for '{name}' is {score}");
        }
    }
}
