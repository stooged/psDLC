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

        private PDL PDL1 = new PDL();
        private Settings settings = new Settings();

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
            PDL1.GotDlcInfo += GotDlcInfo;
            PDL1.DlcInfoError += DlcInfoError;
            PDL1.GotImage += GotImage;
            PDL1.ImageError += ImageError;
            textHint = true;
            textBox1.ForeColor = Color.Gray;
            textBox1.Text = "CUSA00000";
            checkBox1.Checked = settings.GetSetting("check1", true);
            checkBox2.Checked = settings.GetSetting("check2", true);
            checkBox3.Checked = settings.GetSetting("check3", true);
            checkBox4.Checked = settings.GetSetting("check4", true);
            checkBox5.Checked = settings.GetSetting("check5", true);
            checkBox6.Checked = settings.GetSetting("check6", true);
            checkBox7.Checked = settings.GetSetting("check7", true);
            checkBox8.Checked = settings.GetSetting("check8", true);
            checkBox9.Checked = settings.GetSetting("check9", true);
            checkBox10.Checked = settings.GetSetting("check10", true);
            checkBox11.Checked = settings.GetSetting("check11", true);
            checkBox12.Checked = settings.GetSetting("check12", true);
            checkBox13.Checked = settings.GetSetting("check13", false);
            checkBox14.Checked = settings.GetSetting("check14", false);
            ScaleForm();
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                ScaleForm();
            }
        }


        void ScaleForm()
        {
            textBox1.Width = Width - Button1.Width - Button3.Width - 100;
            Button1.Left = textBox1.Right + 3;
            LV1.Width = Width - 33;
            LV1.Height = Height - textBox2.Height - 135;
            panel1.Width = LV1.Width;
            panel1.Height = LV1.Height;
            panel2.Width = LV1.Width;
            panel2.Height = LV1.Height + 30;
            label8.Left = panel2.Width - label8.Width;
            label4.Width = panel2.Width - pictureBox1.Width - 16;
            label4.Height = panel2.Height - label8.Height;
            Button2.Top = LV1.Bottom + 5;
            linkLabel1.Top = LV1.Bottom + 10;
            Button2.Left = LV1.Right - Button2.Width;
            Button3.Left = LV1.Right - Button3.Width;
            textBox2.Top = Button2.Bottom + 8;
            textBox2.Width = Width - 33;
        }


        void DlcListError(object sender, PDL e)
        {
            AppLog("ERROR: " + e.DlcListErrorMessage);
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

                        if (isAllowed(TmpType))
                        {
                            string[] TmpItem = { TmpTitle, TmpType, TmpPlatForm, TmpURL, TmpImgUrl };
                            var LvItem = new ListViewItem(TmpItem);
                            LV1.Items.Add(LvItem);
                        }

                    }
                    LV1.EndUpdate();
                }
            }
            else
            {
                AppLog("ERROR: No HTML content found.");
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
                        case "I":
                            titleRgn = "en-us";
                            break;
                        default:
                            titleRgn = "ja-jp";
                            break;
                    }

                    PDL1.GetDlcList(titleID, titleRgn, pageNum);
                }
                else
                {
                    AppLog("ERROR: Invalid Content ID" + Environment.NewLine + "Failed to load content id for " + textBox1.Text);
                }
            }
            else
            {
                AppLog("ERROR: No XML content found.");
            }
        }


        void PkgListError(object sender, PDL e)
        {
            AppLog("ERROR: " + e.PkgListErrorMessage);
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
            AppLog("ERROR: " + e.ManifestErrorMessage);
        }

        
        void GotDlcInfo(object sender, PDL e)
        {
            string PlData = e.DlcInfoData, fsData, fsUnit, fsValue, dlcDesc, dlcType, dlcImgUrl, dlcPlatfrm;
            string[] Spl1, Spl2;

            Spl1 = Regex.Split(PlData, "\"long-description\":\"");
            Spl2 = Regex.Split(Spl1[1], "\"");
            dlcDesc = Strings.Trim(Spl2[0]);
            dlcDesc = Regex.Replace(dlcDesc, "[^a-zA-Z0-9 -<>&,]", "");

            Spl1 = Regex.Split(PlData, "\"file-size\":{");
            Spl2 = Regex.Split(Spl1[1], "}");
            fsData = Strings.Trim(Spl2[0]);
            Spl1 = Regex.Split(fsData, "\"unit\":\"");
            Spl2 = Regex.Split(Spl1[1], "\"");
            fsUnit = Strings.Trim(Spl2[0]);
            Spl1 = Regex.Split(fsData, "\"value\":");
            fsValue = Strings.Trim(Spl1[1]);

            Spl1 = Regex.Split(PlData, "\"game-content-type\":\"");
            Spl2 = Regex.Split(Spl1[1], "\"");
            dlcType = Strings.Trim(Spl2[0]);

            Spl1 = Regex.Split(PlData, "\"thumbnail-url-base\":\"");
            Spl2 = Regex.Split(Spl1[1], "\"");
            dlcImgUrl = Strings.Trim(Spl2[0]) + "?w=350&h=350";

            Spl1 = Regex.Split(PlData, "\"platforms\":\\[");
            Spl2 = Regex.Split(Spl1[1], "\\]");
            dlcPlatfrm = Strings.Trim(Spl2[0]);

            pictureBox1.ImageLocation = dlcImgUrl;
            pictureBox1.LoadAsync();

            label4.Text = dlcDesc.Replace("<br>","\n");
            if (fsValue == "null" || String.IsNullOrEmpty(fsUnit))
            {
                fsValue = "0";
                fsUnit = "Bytes";
            }
            label5.Text = "Size: " + fsValue + " " + fsUnit;
            if (dlcType == "null" || String.IsNullOrEmpty(dlcType))
            {
                dlcType = "Unknown";
            }
            label6.Text = "Type: " + dlcType;
            label7.Text = "Platform: " + dlcPlatfrm.Replace("\"","");

            panel2.Visible = true;
        }


        void DlcInfoError(object sender, PDL e)
        {
            panel2.Visible = false;
            AppLog("ERROR: " + e.DlcInfoErrorMessage);
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
            AppLog("IMAGE ERROR: " + e.ImageErrorMessage + Environment.NewLine + "Creating without icon0.png");
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
                        case "I":
                            titleRgn = "en-us";
                            break;
                        default:
                            titleRgn = "ja-jp";
                            break;
                    }
                    PDL1.GetDlcList(titleID, titleRgn, pageNum);
                }
                else
                {
                    AppLog("ERROR: Invalid Content ID" + Environment.NewLine + "Use the content id in the following format CUSA00000 or XX0000-CUSA00000_00-0000000000000000");
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
                AppLog("ERROR: orbis-pub-cmd.exe not found" + Environment.NewLine + "You need to place orbis-pub-cmd.exe in the same directory as this application");
            }
        }


        private void Button3_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            PDL1.GetManifest(selManifest);
        }


        private void button4_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }


        private void label1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
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
            if (hasImage == true && settings.GetSetting("check13", false) == false)
            {
                tmpStr += "<volume_type>pkg_ps4_ac_data</volume_type>";
                tmpStr += "<volume_id>PS4VOLUME</volume_id>";
                tmpStr += "<volume_ts>" + gTime + "</volume_ts>";
                tmpStr += "<package content_id=\"" + CID + "\" passcode=\"00000000000000000000000000000000\"/>";
                tmpStr += "</volume>";
                tmpStr += "<files>";
                tmpStr += "<file targ_path=\"sce_sys/icon0.png\" orig_path=\"" + cDir + "fake_dlc_temp\\sce_sys\\icon0.png\"/>";
            }
            else
            {
                tmpStr += "<volume_type>pkg_ps4_ac_nodata</volume_type>";
                tmpStr += "<volume_id>PS4VOLUME</volume_id>";
                tmpStr += "<volume_ts>" + gTime + "</volume_ts>";
                tmpStr += "<package content_id=\"" + CID + "\" passcode=\"00000000000000000000000000000000\"/>";
                tmpStr += "</volume>";
                tmpStr += "<files img_no=\"0\">";
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


        private void label8_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            label4.Text = "";
            label5.Text = "";
            label6.Text = "";
            label7.Text = "";
            pictureBox1.Image = null;
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
                if (settings.GetSetting("check14", false) == true)
                {
                    PDL1.GetDlcInfo(titleRgn, Regex.Split(selCid, "/")[Information.UBound(Regex.Split(selCid, "/"))]);
                }
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
                AppLog("ERROR: " + ex.Message);
            }
        }


        void Proc_DataReceived(object sender, DataReceivedEventArgs e)
        {
            string ProcData = e.Data;
            try
            {
                AppLog(ProcData);
            }
            catch (Exception ex)
            {
                AppLog("ERROR: " + ex.Message);
            }
        }


        void AppLog(string strText)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppLog), new object[] { strText });
                return;
            }
            if (strText != null && strText.Length > 0)
            {
                textBox2.AppendText(strText + Environment.NewLine);
            }
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check1", checkBox1.Checked);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check2", checkBox2.Checked);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check3", checkBox3.Checked);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check4", checkBox4.Checked);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check5", checkBox5.Checked);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check6", checkBox6.Checked);
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check7", checkBox7.Checked);
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check8", checkBox8.Checked);
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check9", checkBox9.Checked);
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check10", checkBox10.Checked);
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check11", checkBox11.Checked);
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check12", checkBox12.Checked);
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check13", checkBox13.Checked);
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check14", checkBox14.Checked);
        }


        bool isAllowed(string cType)
        {
            switch (cType.ToLower())
            {
                case "add-on":
                    if (settings.GetSetting("check1", true) != true) return false;
                    break;
                case "game video":
                    if (settings.GetSetting("check2", true) != true) return false;
                    break;
                case "vehicle":
                    if (settings.GetSetting("check3", true) != true) return false;
                    break;
                case "map":
                    if (settings.GetSetting("check4", true) != true) return false;
                    break;
                case "bundle":
                    if (settings.GetSetting("check5", true) != true) return false;
                    break;
                case "full game":
                    if (settings.GetSetting("check6", true) != true) return false;
                    break;
                case "avatar":
                    if (settings.GetSetting("check7", true) != true) return false;
                    break;
                case "avatars":
                    if (settings.GetSetting("check7", true) != true) return false;
                    break;
                case "theme":
                    if (settings.GetSetting("check8", true) != true) return false;
                    break;
                case "static theme":
                    if (settings.GetSetting("check8", true) != true) return false;
                    break;
                case "dynamic theme":
                    if (settings.GetSetting("check8", true) != true) return false;
                    break;
                case "season pass":
                    if (settings.GetSetting("check9", true) != true) return false;
                    break;
                case "level":
                    if (settings.GetSetting("check10", true) != true) return false;
                    break;
                case "character":
                    if (settings.GetSetting("check11", true) != true) return false;
                    break;
                default:
                    if (settings.GetSetting("check12", true) !=true) return false;
                    break;
            }
            return true;
        }
    }
}
