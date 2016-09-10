using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeddAppProjector
{
    public class DisplayObject
    {
        public enum DisplayType {Photo, Greeting};

        private DisplayType _type;

        public DisplayType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public DisplayObject(DisplayType inType)
        {
            this.Type = inType;
        }
    }
}
