using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace psDLC
{
    class PDL
    {

        public event EventHandler<PDL> GotDlcList;
        public event EventHandler<PDL> DlcListError;
        public event EventHandler<PDL> GotPkgList;
        public event EventHandler<PDL> PkgListError;
        public event EventHandler<PDL> GotManifest;
        public event EventHandler<PDL> ManifestError;
        public event EventHandler<PDL> GotImage;
        public event EventHandler<PDL> ImageError;

        public string DlcListData { get; internal set; }
        public string DlcListErrorMessage { get; internal set; }
        public string PkgListData { get; internal set; }
        public string PkgListErrorMessage { get; internal set; }
        public string ManifestData { get; internal set; }
        public string ManifestErrorMessage { get; internal set; }
        public string ImageErrorMessage { get; internal set; }

        public void GetDlcList(string TitleID, string Region, int Pagenumber)
        {
            WebClient oWeb = new WebClient();
            oWeb.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetDlcList_DownloadStringCompleted);
            oWeb.Headers.Add("Referer", "https://store.playstation.com/");
            oWeb.Headers.Add("Accept", "text/html");
            oWeb.Headers.Add("Accept-Language", "en-US");
            oWeb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64)");
            oWeb.DownloadStringAsync(new Uri("https://store.playstation.com/" + Region + "/grid/" + TitleID + "/" +  Pagenumber + "?relationship=add-ons"));
        }


        public void GetPkgList(string GameID)
        {
            WebClient oWeb = new WebClient();
            byte[] hInp = Encoding.ASCII.GetBytes("np_" + GameID);
            string hKey = "AD62E37F905E06BC19593142281C112CEC0E7EC3E97EFDCAEFCDBAAFA6378D84";
            byte[] Key = new byte[hKey.Length / 2];
            for (int i = 0; i < Key.Length; i++)
            {
                Key[i] = Convert.ToByte(hKey.Substring(i * 2, 2), 16);
            }
            HMACSHA256 hmac = new HMACSHA256(Key);
            oWeb.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetPkgList_DownloadStringCompleted);
            oWeb.Headers.Add("Referer", "http://gs-sec.ww.np.dl.playstation.net");
            oWeb.Headers.Add("Accept", "application/xml");
            oWeb.Headers.Add("Accept-Language", "en-US");
            oWeb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64)");
            oWeb.DownloadStringAsync(new Uri("http://gs-sec.ww.np.dl.playstation.net/plo/np/" + GameID + "/" + BitConverter.ToString(hmac.ComputeHash(hInp)).Replace("-", "").ToLower() + "/" + GameID + "-ver.xml"));
        }


        public void GetManifest(string strUrl)
        {
            WebClient oWeb = new WebClient();
            oWeb.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetManifest_DownloadStringCompleted);
            oWeb.Headers.Add("Referer", strUrl);
            oWeb.Headers.Add("Accept", "application/json");
            oWeb.Headers.Add("Accept-Language", "en-US");
            oWeb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64)");
            oWeb.DownloadStringAsync(new Uri(strUrl));
        }


        public void GetImage(string strUrl, string fPath)
        {
            WebClient oWeb = new WebClient();
            string imgSize = "496";
            strUrl = strUrl.Replace("&amp;", "&");
            strUrl = strUrl.Replace("w=124", "w=" + imgSize);
            strUrl = strUrl.Replace("h=124", "h=" + imgSize);
            strUrl = strUrl.Replace("w=186", "w=" + imgSize);
            strUrl = strUrl.Replace("h=186", "h=" + imgSize);
            strUrl = strUrl.Replace("w=248", "w=" + imgSize);
            strUrl = strUrl.Replace("h=248", "h=" + imgSize);
            strUrl = strUrl.Replace("w=372", "w=" + imgSize);
            strUrl = strUrl.Replace("h=372", "h=" + imgSize);
            oWeb.DownloadFileCompleted += DownloadFileCompleted;
            oWeb.Headers.Add("Referer", strUrl);
            oWeb.Headers.Add("Accept", "*/*");
            oWeb.Headers.Add("Accept-Language", "en-US");
            oWeb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64)");
            oWeb.DownloadFileAsync(new Uri(strUrl), fPath);
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


        void GetPkgList_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            PDL DataEvent = new PDL();
            try
            {
                  DataEvent.PkgListData = e.Result;
                  GotPkgList(this, DataEvent);
            }
            catch (Exception ex)
            {
                 DataEvent.PkgListErrorMessage = ex.Message;
                 PkgListError(this, DataEvent);
            }
        }


        void GetManifest_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            PDL DataEvent = new PDL();
            try
            {
                DataEvent.ManifestData = e.Result;
                GotManifest(this, DataEvent);
            }
            catch (Exception ex)
            {
                DataEvent.ManifestErrorMessage = ex.Message;
                ManifestError(this, DataEvent);
            }
        }


        void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            PDL DataEvent = new PDL();
            if (e.Error == null)
            {
                GotImage(this, DataEvent);
            }
            else
            {
                DataEvent.ImageErrorMessage = e.Error.Message;
                ImageError(this, DataEvent);
            }
        }
    }
}
