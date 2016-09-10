using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeddAppProjector;

namespace WeddAppEmailReader
{
    class MailMessage : DisplayObject
    {
        private string _subject;
        private string _message;

        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public MailMessage(string inSubject, string inMessage) : base(DisplayType.Greeting)
        {
            this._subject = inSubject;
            this._message = inMessage;
        }
    }
}
