using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeddAppProjector
{
    class PhotoMessage : DisplayObject
    {
        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public PhotoMessage(string inPath) : base(DisplayType.Photo)
        {
            this.Path = inPath;
        }
    }
}
