using System;

namespace Thumber
{
    class Thumb
    {
        public string name;
        public int width;

        public Thumb()
        { }

        public Thumb(string name, int width)
        {
            this.name = name;
            this.width = width;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1} px)", name, width);
        }
    }
}
