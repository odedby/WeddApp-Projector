using Google.GData.Client;
using Google.GData.Photos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WeddAppProjector;

namespace WeddAppEmailReader
{
    public partial class PhotoForm : Form
    {
        private Thread InvokeThread;
        List<DisplayObject> _listDisplayObjects;

        public List<DisplayObject> ListDisplayObjects
        {
            get { return _listDisplayObjects; }
            set { _listDisplayObjects = value; }
        }

        public PhotoForm()
        {
            InitializeComponent();
            timer2_Tick(null, null);
            this.timer1.Enabled = true;
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            this.timer2.Enabled = true;
            this.timer2.Interval = 60000;
            this.timer2.Tick += new System.EventHandler(this.timer1_Tick);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        public void Start()
        {
            InvokeThread = new Thread(new ThreadStart(InvokeMethod));
            InvokeThread.SetApartmentState(ApartmentState.STA);
            this.Invoke();
        }

        public void Invoke()
        {
            InvokeThread.Start();
        }

        private void InvokeMethod()
        {
            try
            {
                this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
                this.ShowDialog();
            }
            catch { }
        }

        public void DeInvoke()
        {
            InvokeThread.Abort();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                Random rand = new Random();
                DisplayObject disObj = this.ListDisplayObjects.ElementAt(rand.Next(0, this.ListDisplayObjects.Count()));
                switch (disObj.Type)
                {
                    case DisplayObject.DisplayType.Photo:
                        Bitmap b = new Bitmap(((PhotoMessage)disObj).Path);
                        this.picBox.BackgroundImage = b;
                        this.picBox.BackgroundImageLayout = ImageLayout.Zoom;
                        this.picBox.Dock = DockStyle.Fill;
                        this.txtSubject.Visible = false;
                        this.txtMessage.Visible = false;
                        this.picBox.Visible = true;
                        break;
                    case DisplayObject.DisplayType.Greeting:
                        this.txtSubject.Text = ((MailMessage)disObj).Subject;
                        this.txtSubject.Visible = true;
                        this.txtMessage.Text = ((MailMessage)disObj).Message;
                        this.txtMessage.Visible = true;
                        this.picBox.Visible = false;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                System.Diagnostics.EventLog.WriteEntry("WeddApp", ex.Message, System.Diagnostics.EventLogEntryType.Error, 626);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            List<DisplayObject> lstDisplayObjects = new List<DisplayObject>();
            List<PhotoMessage> lstPhotos = GetPhotos();
            List<MailMessage> lstMails = GetMails();
            lstDisplayObjects.AddRange(lstPhotos);
            lstDisplayObjects.AddRange(lstMails);

            this.ListDisplayObjects = lstDisplayObjects;
        }

        private static List<MailMessage> GetMails()
        {
            List<MailMessage> ans = new List<MailMessage>();
            try
            {
                // Build and convert the authorization information 
                // The variables account and password are strings with the GMail credentials 
                StringBuilder sb = new StringBuilder("hagit.oded");
                sb.Append(":");
                sb.Append("0542686874");
                Byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());

                // Create the request 
                WebRequest feedRequest = WebRequest.Create("https://mail.google.com/mail/feed/atom");
                // Google uses Basic Auth so we add this header to the request 
                feedRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytes));
                // Get the response in the form of a stream 
                Stream feedStream = feedRequest.GetResponse().GetResponseStream();

                // We are going to use a DataSet and XmlReader to get at the contents of the Xml Response 
                DataSet dsFeed = new DataSet();
                XmlReader feedReader = XmlReader.Create(feedStream);
                dsFeed.ReadXml(feedReader);

                string source = dsFeed.Tables["feed"].Rows[0].ItemArray[0].ToString();

                for (Int32 loopMsg = 0; dsFeed.Tables["entry"] != null && loopMsg < dsFeed.Tables["entry"].Rows.Count; loopMsg++)
                {
                    string strAuthor = dsFeed.Tables["author"].Rows[loopMsg].ItemArray[0].ToString();
                    DateTime dtDate = DateTime.Parse(dsFeed.Tables["entry"].Rows[loopMsg].ItemArray[4].ToString());
                    string strDate = dtDate.ToString("g");
                    string strSubject = dsFeed.Tables["entry"].Rows[loopMsg].ItemArray[0].ToString();
                    string strMessage = dsFeed.Tables["entry"].Rows[loopMsg].ItemArray[1].ToString();
                    if (strSubject.StartsWith("Greeting: "))
                        ans.Add(new MailMessage(strSubject.Substring("Greeting: ".Length), strMessage));
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                System.Diagnostics.EventLog.WriteEntry("WeddApp", ex.Message, System.Diagnostics.EventLogEntryType.Error, 626);
            }

            return ans;
        }

        private static List<PhotoMessage> GetPhotos()
        {
            List<PhotoMessage> ans = new List<PhotoMessage>();
            try
            {
                string fileName;
                Uri uriPath;
                WebClient HttpClient = new WebClient();
                PhotoQuery query = new PhotoQuery();
                query.Uri = new Uri(PhotoQuery.CreatePicasaUri("hagit.oded", "5780002529047522017"));
                PicasaService service = new PicasaService("PicasaAlbumDownloader");
                PicasaFeed feed = (PicasaFeed)service.Query(query);
                Directory.SetCurrentDirectory("C:\\Photos\\");
                foreach (AtomEntry aentry in feed.Entries)
                {
                    uriPath = new Uri(aentry.Content.Src.ToString());
                    fileName = uriPath.LocalPath.Substring(uriPath.LocalPath.LastIndexOf('/') + 1);
                    try
                    {
                        Console.WriteLine("Downloading: " + fileName);
                        HttpClient.DownloadFile(aentry.Content.Src.ToString(), fileName);
                        ans.Add(new PhotoMessage("C:\\Photos\\" + fileName));
                    }
                    catch (WebException we)
                    {
                        try
                        {
                            HttpClient.DownloadFile(aentry.Content.Src.ToString(), fileName);
                            ans.Add(new PhotoMessage("C:\\Photos\\" + fileName));
                        }
                        catch (WebException we2)
                        {
                            Console.WriteLine(we2.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                System.Diagnostics.EventLog.WriteEntry("WeddApp", ex.Message, System.Diagnostics.EventLogEntryType.Error, 626);
            }

            return ans;
        }

    }
}
