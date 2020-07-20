using System.Collections;
using System.Collections.Generic;

namespace BikeDataProject.Data.GPS
{
    internal class RunningAverage : IReadOnlyList<(double longitude, double latitude, double accuracy, long timeOffset)>
    {
        private readonly IReadOnlyList<(double longitude, double latitude, double accuracy, long timeOffset)> _track;
        private readonly int _window;

        public RunningAverage(
            IReadOnlyList<(double longitude, double latitude, double accuracy, long timeOffset)> track, int window)
        {
            _track = track;
            _window = window;

            this.Count = _track.Count - (_window - 1);
        }

        public int Count { get; }

        public (double longitude, double latitude, double accuracy, long timeOffset) this[int index]
        {
            get
            {
                double longitude = 0, latitude = 0, accuracy = 0;
                for (var i = index; i < index + _window; i++)
                {
                    var val = _track[i];
                    longitude += val.longitude;
                    latitude += val.latitude;
                    accuracy += val.accuracy;
                }

                longitude /= _window;
                latitude /= _window;
                accuracy /= _window;

                return (longitude, latitude, accuracy, _track[index + (_window / 2)].timeOffset);
            }
        }

        public IEnumerator<(double longitude, double latitude, double accuracy, long timeOffset)> GetEnumerator()
        {
            for (var i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}