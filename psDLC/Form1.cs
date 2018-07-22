using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace psDLC
{
    public partial class Form1 : Form
    {

        PDL PDL1 = new PDL();
        int pageNum;
        String htmBuffer, titleID, titleRgn, selName, selCid, selManifest, selImg;
        Boolean textHint;

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            PDL1.GotDlcList += GotDlcList;
            PDL1.DlcListError += DlcListError;
            PDL1.GotPkgList += GotPkgList;
            PDL1.PkgListError += PkgListError;
            PDL1.GotManifest += GotManifest;
            PDL1.ManifestError += ManifestError;
            PDL1.GotImage += GotImage;
            PDL1.ImageError += ImageError;
            textHint = true;
            textBox1.ForeColor = Color.Gray;
            textBox1.Text = "CUSA00000";
        }


        void DlcListError(object sender, PDL e)
        {
            OrbisLog("ERROR: " + e.DlcListErrorMessage);
        }


        void GotDlcList(object sender, PDL e)
        {
            string PlData = e.DlcListData;

            if (Strings.InStr(PlData, "cell__title") > 0)
            {
                string[] Spl1, Spl2, Spl3, Spl4;
                string TmpTitle, TmpURL, TmpImgUrl, TmpType, TmpPlatForm;

                if (Strings.InStr(PlData, "paginator-control__end paginator-control__arrow-navigation internal-app-link ember-view") > 0)
                {
                    pageNum = pageNum + 1;
                    htmBuffer = htmBuffer + PlData;
                    PDL1.GetDlcList(titleID, titleRgn, pageNum);
                }
                else
                {
                    htmBuffer = htmBuffer + PlData;
                    LV1.BeginUpdate();
                    LV1.Items.Clear();

                    Spl1 = Regex.Split(htmBuffer, "desktop-presentation__grid-cell__base");

                    for (int i = 1; i < Information.UBound(Spl1) + 1; i++)
                    {

                        Spl2 = Regex.Split(Spl1[i], "grid-cell__footer");

                        Spl3 = Regex.Split(Spl2[0], "class=\"grid-cell__title\">");
                        Spl4 = Regex.Split(Spl3[1], "<");
                        TmpTitle = Strings.Trim(Spl4[0]);
                        TmpTitle = TmpTitle.Replace("&#x2122;", "");
                        TmpTitle = TmpTitle.Replace("&#x2019;", "’");
                        TmpTitle = TmpTitle.Replace("&apos;", "'");
                        TmpTitle = TmpTitle.Replace("&#xAE;", "");
                        TmpTitle = TmpTitle.Replace("&amp;", "&");

                        Spl3 = Regex.Split(Spl2[0], "a href=\"");
                        Spl4 = Regex.Split(Spl3[1], "\"");
                        TmpURL = "https://store.playstation.com" + Strings.Trim(Spl4[0]);

                        Spl3 = Regex.Split(Spl2[0], "img src=\"http");
                        Spl4 = Regex.Split(Spl3[1], "\"");
                        TmpImgUrl = "http" + Strings.Trim(Spl4[0]);

                        Spl3 = Regex.Split(Spl2[0], "left-detail--detail-2\">");
                        Spl4 = Regex.Split(Spl3[1], "<");
                        TmpType = Strings.Trim(Spl4[0]);

                        Spl3 = Regex.Split(Spl2[0], "left-detail--detail-1\">");
                        Spl4 = Regex.Split(Spl3[1], "<");
                        TmpPlatForm = Strings.Trim(Spl4[0]);

                        string[] TmpItem = { TmpTitle, TmpType, TmpPlatForm, TmpURL, TmpImgUrl };
                        var LvItem = new ListViewItem(TmpItem);
                        LV1.Items.Add(LvItem);

                    }
                    LV1.EndUpdate();
                }
            }
            else
            {
                OrbisLog("ERROR: No HTML content found.");
            }
        }


        void GotPkgList(object sender, PDL e)
        {
            string PlData = e.PkgListData;
            if (Strings.InStr(PlData, "titleid=") > 0)
            {
                string ContentID, TmpTitle; ;
                string[] Spl1, Spl2;

                Spl1 = Regex.Split(PlData, "content_id=\"");
                Spl2 = Regex.Split(Spl1[1], "\"");
                ContentID = Strings.Trim(Spl2[0]);

                Spl1 = Regex.Split(PlData, "manifest_url=\"");
                Spl2 = Regex.Split(Spl1[1], "\"");
                selManifest = Strings.Trim(Spl2[0]);

                Spl1 = Regex.Split(PlData, "<title>");
                Spl2 = Regex.Split(Spl1[1], "</title>");
                TmpTitle = Strings.Trim(Spl2[0]);
                Regex rgrep = new Regex("[^ -~]+");
                TmpTitle = rgrep.Replace(TmpTitle, "");
                TmpTitle = TmpTitle.Replace("&#x2122;", "");
                TmpTitle = TmpTitle.Replace("&#x2019;", "’");
                TmpTitle = TmpTitle.Replace("&apos;", "'");
                TmpTitle = TmpTitle.Replace("&#xAE;", "");
                TmpTitle = TmpTitle.Replace("&amp;", "&");
                Text = TmpTitle;

                if (ContentID.Length >= 19)
                {
                    Button3.Visible = true;
                    pageNum = 1;
                    htmBuffer = string.Empty;
                    titleID = Strings.Mid(ContentID, 8, 12);

                    switch (Strings.Mid(ContentID, 1, 1))
                    {
                        case "U":
                            titleRgn = "en-us";
                            break;
                        case "E":
                            titleRgn = "en-gb";
                            break;
                        default:
                            titleRgn = "ja-jp";
                            break;
                    }

                    PDL1.GetDlcList(titleID, titleRgn, pageNum);
                }
                else
                {
                    OrbisLog("ERROR: Invalid Content ID" + Environment.NewLine + "Failed to load content id for " + textBox1.Text);
                }
            }
            else
            {
                OrbisLog("ERROR: No XML content found.");
            }
        }


        void PkgListError(object sender, PDL e)
        {
            OrbisLog("ERROR: " + e.PkgListErrorMessage);
        }


        void GotManifest(object sender, PDL e)
        {
            string PlData = e.ManifestData;
            string[] Spl1, Spl2;
            Spl1 = Regex.Split(PlData, "\"url\":\"");
            for (int i = 1; i < Information.UBound(Spl1) + 1; i++)
            {

                Spl2 = Regex.Split(Spl1[i], "\"");
                textBox2.Text = textBox2.Text + Spl2[0] + Environment.NewLine;
            }
        }


        void ManifestError(object sender, PDL e)
        {
            OrbisLog("ERROR: " + e.ManifestErrorMessage);
        }


        void GotImage(object sender, PDL e)
        {
            string imagePath = AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_temp/sce_sys/icon0";
            var bitmap = Bitmap.FromFile(imagePath + ".jpeg");
            bitmap.Save(imagePath + ".png", ImageFormat.Png);
            bitmap.Dispose();
            File.Delete(imagePath + ".jpeg");
            CreatePKG(selCid, selName, titleID, true);
        }


        void ImageError(object sender, PDL e)
        {
            OrbisLog("IMAGE ERROR: " + e.ImageErrorMessage + Environment.NewLine + "Creating without icon0.png");
            CreatePKG(selCid, selName, titleID);
        }
        
    
        private void Button1_Click(object sender, EventArgs e)
        {
            if (textHint == false)
            {
                LV1.Items.Clear();
                textBox2.Clear();
                Button2.Visible = false;
                Button3.Visible = false;
                linkLabel1.Text = "";
                Text = "psDLC";
                textBox1.Text = textBox1.Text.ToUpper();
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.SelectionLength = 0;

                if (textBox1.Text.Length == 9)
                {
                    PDL1.GetPkgList(textBox1.Text);
                }
                else if (textBox1.Text.Length >= 19)
                {
                    pageNum = 1;
                    htmBuffer = string.Empty;
                    titleID = Strings.Mid(textBox1.Text, 8, 12);
                    switch (Strings.Mid(textBox1.Text, 1, 1))
                    {
                        case "U":
                            titleRgn = "en-us";
                            break;
                        case "E":
                            titleRgn = "en-gb";
                            break;
                        default:
                            titleRgn = "ja-jp";
                            break;
                    }
                    PDL1.GetDlcList(titleID, titleRgn, pageNum);
                }
                else
                {
                    OrbisLog("ERROR: Invalid Content ID" + Environment.NewLine + "Use the content id in the following format CUSA00000 or XX0000-CUSA00000_00-0000000000000000");
                }
            }
        }


        private void Button2_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            Button2.Visible = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "orbis-pub-cmd.exe"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_temp/sce_sys/");
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_pkg/");
                PDL1.GetImage(selImg, AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_temp/sce_sys/icon0.jpeg");
            }
            else
            {
                OrbisLog("ERROR: orbis-pub-cmd.exe not found" + Environment.NewLine + "You need to place orbis-pub-cmd.exe in the same directory as this application");
            }
        }


        private void Button3_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            PDL1.GetManifest(selManifest);
        }


        private void CreatePKG(string CID, string Name, string TID, bool hasImage = false)
        {
            string[] Spl1;
            Spl1 = Regex.Split(CID, "/");
            Name = Regex.Replace(Name, "[^A-Za-z0-9 ]", "");
            string cntId = Spl1[Information.UBound(Spl1)];
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_temp/param_template.sfx", SFX(cntId, Name, Strings.Mid(TID, 1, 9)));
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_temp/fake_dlc_project.gp4", GP4(cntId, hasImage));
            RunOrbis("sfo_create fake_dlc_temp\\param_template.sfx fake_dlc_temp\\sce_sys\\param.sfo");
            RunOrbis("img_create fake_dlc_temp\\fake_dlc_project.gp4 \"" + AppDomain.CurrentDomain.BaseDirectory + "\\fake_dlc_pkg\\" + cntId + "-A0000-V0100.pkg\"");
            Directory.Delete(AppDomain.CurrentDomain.BaseDirectory +"fake_dlc_temp", true);
        }


        string SFX(string CID, string Name, string TID)
        {
            string tmpStr = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>";
            tmpStr += "<paramsfo>";
            tmpStr += "<param key=\"ATTRIBUTE\">0</param>";
            tmpStr += "<param key=\"CATEGORY\">ac</param>";
            tmpStr += "<param key=\"CONTENT_ID\">" + CID + "</param>";
            tmpStr += "<param key=\"FORMAT\">obs</param>";
            tmpStr += "<param key=\"TITLE\">" + Name + "</param>";
            tmpStr += "<param key=\"TITLE_ID\">" + TID + "</param>";
            tmpStr += "<param key=\"VERSION\">01.00</param>";
            tmpStr += "</paramsfo>";
            return tmpStr;
        }


        string GP4(string CID, bool hasImage)
        {
            string cDir = AppDomain.CurrentDomain.BaseDirectory;
            string gTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            string tmpStr = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>";
            tmpStr += "<psproject fmt=\"gp4\" version=\"1000\">";
            tmpStr += "<volume>";
            tmpStr += "<volume_type>pkg_ps4_ac_data</volume_type>";
            tmpStr += "<volume_id>PS4VOLUME</volume_id>";
            tmpStr += "<volume_ts>" + gTime + "</volume_ts>";
            tmpStr += "<package content_id=\"" + CID + "\" passcode=\"00000000000000000000000000000000\"/>";
            tmpStr += "</volume>";
            tmpStr += "<files>";
            if (hasImage == true)
            {
                tmpStr += "<file targ_path=\"sce_sys/icon0.png\" orig_path=\"" + cDir + "fake_dlc_temp\\sce_sys\\icon0.png\"/>";
            }
            tmpStr += "<file targ_path=\"sce_sys/param.sfo\" orig_path=\"" + cDir + "fake_dlc_temp\\sce_sys\\param.sfo\"/>";
            tmpStr += "</files>";
            tmpStr += "<rootdir>";
            tmpStr += "<dir targ_name=\"sce_sys\"/>";
            tmpStr += "</rootdir>";
            tmpStr += "</psproject>";
            return tmpStr;
        }


        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textHint)
            {
                textHint = false;
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                Button1.PerformClick();
                e.Handled = true;
            }
        }


        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (!textHint && string.IsNullOrEmpty(textBox1.Text))
            {
                textHint = true;
                textBox1.Text = "CUSA00000";
                textBox1.ForeColor = Color.Gray;
            }
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel1.Text.Length > 6)
            {
                Process.Start(linkLabel1.Text);
            }
        }
        

        private void LV1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem LvItem = LV1.FocusedItem as ListViewItem;
            ListView.SelectedIndexCollection Lview = LV1.SelectedIndices;
            if (LvItem != null)
            {
                textBox2.Clear();
                linkLabel1.Text = LvItem.SubItems[3].Text;
                selName = LvItem.Text;
                selCid = LvItem.SubItems[3].Text;
                selImg = LvItem.SubItems[4].Text;
                Button2.Visible = true;
            }
        }


        private void RunOrbis(string oargs)
        {
            try
            {
                Process tProcess = new Process();
                ProcessStartInfo startinfo = new ProcessStartInfo();
                startinfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "orbis-pub-cmd.exe";
                startinfo.Arguments = oargs;
                startinfo.UseShellExecute = false;
                startinfo.RedirectStandardError = true;
                startinfo.RedirectStandardOutput = true;
                startinfo.CreateNoWindow = true;
                tProcess.StartInfo = startinfo;
                tProcess.ErrorDataReceived += Proc_DataReceived;
                tProcess.OutputDataReceived += Proc_DataReceived;
                tProcess.EnableRaisingEvents = true;
                tProcess.Start();
                tProcess.BeginOutputReadLine();
                tProcess.BeginErrorReadLine();
                tProcess.WaitForExit(2000);
            }
            catch(Exception ex)
            {
                OrbisLog("ERROR: " + ex.Message);
            }
        }


        void Proc_DataReceived(object sender, DataReceivedEventArgs e)
        {
            string ProcData = e.Data;
            try
            {
                OrbisLog(ProcData);
            }
            catch (Exception ex)
            {
                OrbisLog("ERROR: " + ex.Message);
            }
        }


        void OrbisLog(string strText)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(OrbisLog), new object[] { strText });
                return;
            }
            if (strText != null && strText.Length > 0)
            {
                textBox2.AppendText(strText + Environment.NewLine);
            }
        }
    }
}
