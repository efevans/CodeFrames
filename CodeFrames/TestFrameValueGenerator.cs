using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeFrames;

namespace CodeFrames
{
    public class TestFrameValueGenerator : IFrameValueGetter
    {
        private readonly List<string> Values = new List<string>()
        {
            "Pirate",
            "Wolf",
            "Marisa",
            "Plant",
            "Controller",
            "Lamp",
            "Phone",
            "Poster",
            "Frame",
            "Water",
            "Bottle",
            "Brain",
            "Horse," +
            "Vaseline",
            "Trash",
            "Hamper",
            "Computer",
            "Door",
            "Bed",
            "Sheet",
            "Clock",
            "Speaker",
            "Headphone",
            "Drawer",
            "Shelf",
            "Vegetable"
        };

        public string GetNext()
        {
            if(Values.Any())
            {
                string value = Values.ElementAt(0);
                Values.RemoveAt(0);
                return value;
            }
            return "";
        }

        public void Reset()
        {
            
        }
    }
}
