using CodeFrames;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeFramesAPI
{
    public class DefaultImagesImageGetter : IFrameValueGetter
    {
        private List<string> _images = new List<string>()
        {
            "airconditioning",
            "banjo",
            "battery",
            "bicycle",
            "book",
            "brush",
            "computer",
            "curry",
            "duster",
            "fish",
            "guitar",
            "hairspray",
            "labcoat",
            "meat",
            "microphone",
            "pasta",
            "plane",
            "police",
            "razor",
            "restaurant",
            "rice",
            "soap",
            "taxi",
            "vacuumcleaner",
            "washingmachine",
        };

        private Queue<string> _tempImages;

        public DefaultImagesImageGetter()
        {
            Reset();
        }

        public string GetNext()
        {
            return _tempImages.Dequeue();
        }

        public void Reset()
        {
            _tempImages = new Queue<string>(_images.Select(i => $"Images/Frames/{i}.png"));
        }
    }
}
