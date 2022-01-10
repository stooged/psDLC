using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

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
        public event EventHandler<PDL> GotDlcInfo;
        public event EventHandler<PDL> DlcInfoError;
        public event EventHandler<PDL> GotImage;
        public event EventHandler<PDL> ImageError;
        public event EventHandler<PDL> GotSearch;
        public event EventHandler<PDL> SearchDataError;

        public string DlcListData { get; internal set; }
        public string DlcListErrorMessage { get; internal set; }
        public string PkgListData { get; internal set; }
        public string PkgListErrorMessage { get; internal set; }
        public string ManifestData { get; internal set; }
        public string ManifestErrorMessage { get; internal set; }
        public string DlcInfoData { get; internal set; }
        public string DlcInfoErrorMessage { get; internal set; }
        public string ImageErrorMessage { get; internal set; }
        public string SearchData { get; internal set; }
        public string SearchDataErrorMessage { get; internal set; }


        public void GetDlcList(string TitleID, string Region, bool tryV2 = false )
        {
            if (Region == String.Empty) { Region = "en-us"; }
            WebClient oWeb = new WebClient();
            oWeb.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetDlcList_DownloadStringCompleted);
            oWeb.Headers.Add("Accept-Language", "en-US");
            oWeb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64)");
            if (TitleID.ToLower().StartsWith("ppsa"))
            {
                oWeb.Headers.Add("Accept", "text/html");
                oWeb.Headers.Add("Referer", "https://serialstation.com");
                TitleID = TitleID.ToLower().Replace("_00", "");
                TitleID = TitleID.Replace("ppsa", "");
                oWeb.DownloadStringAsync(new Uri("https://serialstation.com/titles/PPSA/" + TitleID));
            }
            if (TitleID.ToLower().StartsWith("cusa") && tryV2)
            {
                oWeb.Headers.Add("Accept", "text/html");
                oWeb.Headers.Add("Referer", "https://serialstation.com");
                TitleID = TitleID.ToLower().Replace("_00", "");
                TitleID = TitleID.Replace("cusa", "");
                oWeb.DownloadStringAsync(new Uri("https://serialstation.com/titles/CUSA/" + TitleID));
            }
            else if (TitleID.ToLower().StartsWith("cusa"))
            {
                oWeb.Headers.Add("Accept", "application/json");
                oWeb.Headers.Add("Referer", "https://store.playstation.com/");
                oWeb.DownloadStringAsync(new Uri("https://store.playstation.com/store/api/chihiro/00_09_000/titlecontainer/" + Region.Substring(3, 2).ToUpper() + "/" + Region.Substring(0, 2) + "/999/" + TitleID));
            }
            else if (TitleID.ToLower().StartsWith("up") || TitleID.ToLower().StartsWith("ep"))
            {
                oWeb.Headers.Add("Accept", "application/json");
                oWeb.Headers.Add("Referer", "https://store.playstation.com/");
                oWeb.DownloadStringAsync(new Uri("https://store.playstation.com/store/api/chihiro/00_09_000/container/" + Region.Substring(3, 2).ToUpper() + "/" + Region.Substring(0, 2) + "/999/" + TitleID));
            }
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


        public void GetDlcInfo(string Region, string contentID)
        {
            if (Region == String.Empty) { Region = "en-us"; }
            WebClient oWeb = new WebClient();
            oWeb.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetDlcInfo_DownloadStringCompleted);
            oWeb.Headers.Add("Referer", "https://store.playstation.com");
            oWeb.Headers.Add("Accept", "application/json");
            oWeb.Headers.Add("Accept-Language", "en-US");
            oWeb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64)");
            oWeb.DownloadStringAsync(new Uri("https://store.playstation.com/store/api/chihiro/00_09_000/container/" + Region.Substring(3, 2).ToUpper() + "/" + Region.Substring(0, 2) + "/999/" + contentID));
        }


        public void GetImage(string strUrl, string fPath)
        {
            WebClient oWeb = new WebClient();
            strUrl = strUrl.Replace("&amp;", "&");
            oWeb.DownloadFileCompleted += DownloadFileCompleted;
            oWeb.Headers.Add("Referer", strUrl);
            oWeb.Headers.Add("Accept", "*/*");
            oWeb.Headers.Add("Accept-Language", "en-US");
            oWeb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64)");
            oWeb.DownloadFileAsync(new Uri(strUrl), fPath);
        }


        public void GetSearch(string strQry, string Region)
        {
            if (Region == String.Empty) { Region = "en-us"; }
            WebClient oWeb = new WebClient();
            oWeb.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetSearch_DownloadStringCompleted);
            oWeb.Headers.Add("Referer", "https://store.playstation.com");
            oWeb.Headers.Add("Accept", "application/json");
            oWeb.Headers.Add("Accept-Language", "en-US");
            oWeb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64)");
            oWeb.DownloadStringAsync(new Uri("https://store.playstation.com/store/api/chihiro/00_09_000/search/" + Region.Substring(3, 2).ToUpper() + "/" + Region.Substring(0, 2) + "/999/" + HttpUtility.UrlEncode(strQry)));
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
                if (ex.InnerException != null)
                {
                    DataEvent.DlcListErrorMessage = ex.InnerException.Message;
                }
                else
                {
                    DataEvent.DlcListErrorMessage = ex.Message;
                }
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

                if (ex.InnerException != null)
                {
                    DataEvent.PkgListErrorMessage = ex.InnerException.Message;
                }
                else
                {
                    DataEvent.PkgListErrorMessage = ex.Message;
                }

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
                if (ex.InnerException != null)
                {
                    DataEvent.ManifestErrorMessage = ex.InnerException.Message;
                }
                else
                {
                    DataEvent.ManifestErrorMessage = ex.Message;
                }
                ManifestError(this, DataEvent);
            }
        }


        void GetDlcInfo_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            PDL DataEvent = new PDL();
            try
            {
                DataEvent.DlcInfoData = e.Result;
                GotDlcInfo(this, DataEvent);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    DataEvent.DlcInfoErrorMessage = ex.InnerException.Message;
                }
                else
                {
                    DataEvent.DlcInfoErrorMessage = ex.Message;
                }
                DlcInfoError(this, DataEvent);
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
                if (e.Error.InnerException != null)
                {
                    DataEvent.ImageErrorMessage = e.Error.InnerException.Message;
                }
                else
                {
                    DataEvent.ImageErrorMessage = e.Error.Message;
                }
                ImageError(this, DataEvent);
            }
        }


        void GetSearch_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            PDL DataEvent = new PDL();
            try
            {
                DataEvent.SearchData = e.Result;
                GotSearch(this, DataEvent);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    DataEvent.SearchDataErrorMessage = ex.InnerException.Message;
                }
                else
                {
                    DataEvent.SearchDataErrorMessage = ex.Message;
                }
                SearchDataError(this, DataEvent);
            }
        }

    }
}
