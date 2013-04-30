using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using CloudyBank.MVVM;
using System.Xml.Linq;
using System.Linq;

namespace CloudyBank.Web.Ria.ViewModels
{
    public class NewsViewModel : ViewModelBase
    {
        public NewsViewModel()
        {
           
        }
        

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                XDocument doc = XDocument.Parse(e.Result);
                var news = from item in doc.Descendants("item")
                           select new NewsItem
                           {
                               Title = item.Element("title").Value,
                               Link = item.Element("link").Value
                           };

                News = new ObservableCollection<NewsItem>(news);
            }
            catch (Exception)
            {

            }
            finally
            {
                InProgress = false;
            }
        }

        public class NewsItem: ViewModelBase
        {
            public String Title { get; set; }
            public String Link { get; set; }
        }
        private ObservableCollection<NewsItem> _news;

        public ObservableCollection<NewsItem> News
        {
            get {
                if (_news == null)
                {
                    InProgress = true;
                    WebClient client = new WebClient();
                    client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                    client.DownloadStringAsync(new Uri("http://pipes.yahooapis.com/pipes/pipe.run?_id=DqsF_ZG72xGLbes9l7okhQ&_render=rss", UriKind.Absolute));
                }
                return _news; 
            }

            set { 
                _news = value;
                OnPropertyChanged(() => News);
            }
        }


        private bool _inProgress;

        public bool InProgress
        {
            get { return _inProgress; }
            set { 
                _inProgress = value;
                OnPropertyChanged(() => InProgress);
            }
        }
    }
}
