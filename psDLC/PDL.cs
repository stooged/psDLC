using System;
using System.Net;

namespace psDLC
{
    class PDL
    {

        public event EventHandler<PDL> GotDlcList;
        public event EventHandler<PDL> DlcListError;
        public string DlcListData { get; internal set; }
        public string DlcListErrorMessage { get; internal set; }

        public void GetDlcList(string TitleID, string Region, int Pagenumber)
        {
            WebClient oWeb = new WebClient();
            oWeb.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetDlcList_DownloadStringCompleted);
            oWeb.Headers.Add("Referer", "https://store.playstation.com/");
            oWeb.Headers.Add("Accept", "text/html, */*");
            oWeb.Headers.Add("Accept-Language", "en-US");
            oWeb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64)");
            oWeb.DownloadStringAsync(new Uri("https://store.playstation.com/" + Region + "/grid/" + TitleID + "/" +  Pagenumber + "?relationship=add-ons"));
        }

        void GetDlcList_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            PDL DataEvent = new PDL();
            try
            {
                DataEvent.DlcListData = e.Result;
                GotDlcList(this, DataEvent);
            }
            catch(Exception ex)
            {
                DataEvent.DlcListErrorMessage = ex.Message;
                DlcListError(this, DataEvent);
            }
        }

    }
}
