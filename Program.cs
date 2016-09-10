using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Xml;
using Google.GData.Photos;
using System.Drawing;
using Google.GData.Client;
using WeddAppProjector;

namespace WeddAppEmailReader
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PhotoForm pf = new PhotoForm();
                //pf.SetPhoto(lstDisplayObjects[(rand.Next(0, lstDisplayObjects.Count() - 1))]);
                while (true)
                {
                    pf.ShowDialog();
                    Thread.Sleep(20000);
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                System.Diagnostics.EventLog.WriteEntry("WeddApp", ex.Message, System.Diagnostics.EventLogEntryType.Error, 626);
            }
        }
    }
}
