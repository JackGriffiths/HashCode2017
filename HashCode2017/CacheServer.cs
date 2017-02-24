using System.Collections.Generic;

namespace HashCode2017 {
    public class CacheServer {
        public int Id { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentCapacity { get; private set; }
        public List<Video> Videos { get; set; }

        public CacheServer() {
            Videos = new List<Video>();
        }

        public bool CanAddVideo(Video video) {
            return
                (MaxCapacity - CurrentCapacity) >= video.Size &&
                !Videos.Contains(video);
        }

        public void AddVideo(Video video) {
            Videos.Add(video);
            CurrentCapacity += video.Size;
        }
    }
}
