using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace psDLC
{
    public partial class Form1 : Form
    {

        private PDL PDL1 = new PDL();
        private Settings settings = new Settings();

        String htmBuffer, titleID, titleRgn, selName, selCid, selManifest, selImg;
        Boolean text3Hint;
        Boolean text4Hint;
        Boolean isSearch = false;

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
            PDL1.GotSearch += GotSearch;
            PDL1.SearchDataError += SearchDataError;

            text3Hint = true;
            textBox3.ForeColor = Color.Gray;
            textBox3.Text = "Display Name For DLC";
            text4Hint = true;
            textBox4.ForeColor = Color.Gray;
            textBox4.Text = "XX0000-CUSA00000_00-0000000000000000";

            checkBox13.Checked = settings.GetSetting("check13", false);
            checkBox14.Checked = settings.GetSetting("check14", true);

            comboBox1.Text = settings.GetSetting("region", "en-us");

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
            textBox1.Width = Width - comboBox1.Width - Button1.Width - Button3.Width - button6.Width - 100;
            comboBox1.Left = textBox1.Right + 3;
            Button1.Left = comboBox1.Right + 3;
            button6.Left = Button1.Right + 3;
            LV1.Width = Width - 33;
            LV1.Height = Height - textBox2.Height - 135;
            panel1.Width = LV1.Width;
            panel1.Height = Height - label1.Height - 60;
            panel2.Width = LV1.Width;
            panel2.Height = LV1.Height + 30;
            panel3.Width = LV1.Width;
            panel3.Height = LV1.Height + Button2.Height + 30;
            label8.Left = panel2.Width - label8.Width;
            label13.Left = panel3.Width - label13.Width;
            label14.Left = panel1.Width - label14.Width;
            label4.Width = panel2.Width - pictureBox1.Width - 16;
            label4.Height = panel2.Height - label8.Height;
            Button2.Top = LV1.Bottom + 5;
            linkLabel1.Top = LV1.Bottom + 10;
            Button2.Left = LV1.Right - Button2.Width;
            Button3.Left = LV1.Right - Button3.Width;
            textBox2.Top = Button2.Bottom + 8;
            textBox2.Width = Width - 33;
            button7.Top = Button2.Top;
            button7.Left = Button2.Left;
        }


        void DlcListError(object sender, PDL e)
        {
            AppLog("ERROR: " + e.DlcListErrorMessage);
        }


        void GotDlcList(object sender, PDL e)
        {
            string PlData = e.DlcListData;
            string[] Spl1, Spl2, Spl3, Spl4;
            string TmpTitle, TmpURL, TmpImgUrl, TmpType, TmpPlatForm;

            if (PlData.Contains("\"default_sku\":"))
            {
                LV1.BeginUpdate();
                LV1.Items.Clear();
                Spl1 = Regex.Split(PlData, "\"default_sku\":");

                for (int i = 1; i < Spl1.Length; i++)
                {

                    if (Spl1[i].Contains("\"top_category\":\"add_on\"") || Spl1[i].Contains("\"top_category\":\"avatar\"") || Spl1[i].Contains("\"top_category\":\"theme\"") || Spl1[i].Contains("\"top_category\":\"game_content\""))
                    {

                        Spl3 = Regex.Split(Spl1[i], "\"name\":\"");
                        Spl4 = Regex.Split(Spl3[1], "\"");
                        TmpTitle = Spl4[0].Trim();
                        TmpTitle = WebUtility.HtmlDecode(TmpTitle);
                        TmpTitle = Regex.Replace(TmpTitle, "[^a-zA-Z0-9 -<>&,]", "");
                        Spl3 = Regex.Split(Spl1[i], "/999/");
                        Spl4 = Regex.Split(Spl3[1], "/");
                        switch (Spl4[0].Trim().Substring(0, 1))
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
                        TmpURL = "https://store.playstation.com/" + titleRgn + "/product/" + Spl4[0].Trim();
                        Spl3 = Regex.Split(Spl1[i], "\"url\":\"");
                        Spl4 = Regex.Split(Spl3[1], "\"");
                        TmpImgUrl = Spl4[0].Trim();

                        Spl3 = Regex.Split(Spl1[i], "\"top_category\":\"");
                        Spl4 = Regex.Split(Spl3[1], "\"");
                        TmpType = Spl4[0].Trim().ToUpper();
                        TmpPlatForm = "";
                        string[] TmpItem = { TmpTitle, TmpType, TmpPlatForm, TmpURL, TmpImgUrl };
                        var LvItem = new ListViewItem(TmpItem);
                        LV1.Items.Add(LvItem);
                    }
                }
                LV1.EndUpdate();
            }
            else if (PlData.Contains("\">Content</"))
            {
                LV1.BeginUpdate();
                LV1.Items.Clear();

                if (PlData.Contains("<h1>"))
                {
                    Spl1 = Regex.Split(PlData, "<h1>");
                    Spl2 = Regex.Split(Spl1[1], "</h1>");
                    Text = WebUtility.HtmlDecode(Spl2[0].Trim());
                }

                Spl1 = Regex.Split(PlData, "\">Content</");
                Spl2 = Regex.Split(Spl1[1], "</table>");

                Spl1 = Regex.Split(Spl2[0], "<a href=\"");

                for (int i = 1; i < Spl1.Length; i++)
                {

                    if (Spl1[i].Contains("<td>DLC</td>") || Spl1[i].Contains("<td>Avatar</td>") || Spl1[i].Contains("<td>Theme</td>") || Spl1[i].Contains("<td>Unknown</td>"))
                    {

                        Spl3 = Regex.Split(Spl1[i], ">");
                        Spl4 = Regex.Split(Spl3[1], "<");
                        TmpTitle = Spl4[0].Trim();
                        TmpTitle = WebUtility.HtmlDecode(TmpTitle);
                        TmpTitle = Regex.Replace(TmpTitle, "[^a-zA-Z0-9 -<>&,]", "");
                        Spl3 = Regex.Split(Spl1[i], "cell\">");
                        Spl4 = Regex.Split(Spl3[1], "<");
                        switch (Spl4[0].Trim().Substring(0, 1))
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
                        TmpURL = "https://store.playstation.com/" + titleRgn + "/product/" + Spl4[0].Trim();
                        TmpImgUrl = "";
                        TmpType = "Unknown";
                        if (Spl1[i].Contains("<td>DLC</td>")) { TmpType = "DLC"; }
                        if (Spl1[i].Contains("<td>Avatar</td>") ) { TmpType = "Avatar"; }
                        if (Spl1[i].Contains("<td>Theme</td>") ) { TmpType = "Theme"; }
                        TmpPlatForm = "";
                        string[] TmpItem = { TmpTitle, TmpType, TmpPlatForm, TmpURL, TmpImgUrl };
                        var LvItem = new ListViewItem(TmpItem);
                        LV1.Items.Add(LvItem);
                    }
                }
                LV1.EndUpdate();
            }
            else
            {
                AppLog("ERROR: No HTML content found.");
            }
        }


        void GotPkgList(object sender, PDL e)
        {
            string PlData = e.PkgListData;
            if (PlData.Contains("titleid="))
            {
                string ContentID, TmpTitle; ;
                string[] Spl1, Spl2;

                Spl1 = Regex.Split(PlData, "content_id=\"");
                Spl2 = Regex.Split(Spl1[1], "\"");
                ContentID = Spl2[0].Trim();

                Spl1 = Regex.Split(PlData, "manifest_url=\"");
                Spl2 = Regex.Split(Spl1[1], "\"");
                selManifest = Spl2[0].Trim();

                Spl1 = Regex.Split(PlData, "<title>");
                Spl2 = Regex.Split(Spl1[1], "</title>");
                TmpTitle = Spl2[0].Trim();
                TmpTitle = WebUtility.HtmlDecode(TmpTitle);
                Regex rgrep = new Regex("[^ -~]+");
                TmpTitle = rgrep.Replace(TmpTitle, "");
                Text = TmpTitle;

                if (ContentID.Length >= 19)
                {
                    Button3.Visible = true;
                    htmBuffer = string.Empty;
                    titleID = ContentID.Substring(7, 12);

                    switch (ContentID.Substring(0, 1))
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

                    PDL1.GetDlcList(titleID, titleRgn);
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
            for (int i = 1; i < Spl1.Length; i++)
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
            string PlData = e.DlcInfoData, fsData, dlcDesc, dlcType, dlcImgUrl, dlcPlatfrm;
            string[] Spl1, Spl2;

            Spl1 = Regex.Split(PlData.Replace("\\\"", "'"), "\"long_desc\":\"");
            Spl2 = Regex.Split(Spl1[1], "\"");
            dlcDesc = Spl2[0].Trim();
            dlcDesc = WebUtility.HtmlDecode(dlcDesc);
            dlcDesc = Regex.Replace(dlcDesc, "[^a-zA-Z0-9 -<>&,]", "");
            dlcDesc = Regex.Replace(dlcDesc, "<br>", "\n");
            dlcDesc = Regex.Replace(dlcDesc, "<.*?>", "");


            Spl1 = Regex.Split(PlData, "\"size\":");
            Spl2 = Regex.Split(Spl1[1], ",");
            fsData = Spl2[0].Trim().Replace("}]",""); ;


            Spl1 = Regex.Split(PlData, "\"game_contentType\":\"");
            Spl2 = Regex.Split(Spl1[1], "\"");
            dlcType = Spl2[0].Trim();
            dlcType = WebUtility.HtmlDecode(dlcType);


            Spl1 = Regex.Split(PlData, "\"images\":");
            Spl1 = Regex.Split(Spl1[1], "\"url\":\"");
            Spl2 = Regex.Split(Spl1[1], "\"");
            dlcImgUrl = Spl2[0].Trim();


            Spl1 = Regex.Split(PlData, "\"platforms\":\\[");
            Spl2 = Regex.Split(Spl1[1], "\\]");
            dlcPlatfrm = Spl2[0].Trim();
            dlcPlatfrm = WebUtility.HtmlDecode(dlcPlatfrm);

            pictureBox1.ImageLocation = dlcImgUrl;
            pictureBox1.LoadAsync();

            label4.Text = dlcDesc.Replace("<br>","\n");
            label5.Text = "Size: " + fsData;

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


        void GotSearch(object sender, PDL e)
        {
            string PlData = e.SearchData;
            string[] Spl1, Spl3, Spl4;
            string TmpTitle, TmpURL, TmpImgUrl, TmpID, TmpPlatForm;

            if (PlData.Contains("\"default_sku\":"))
            {
                LV1.BeginUpdate();
                LV1.Items.Clear();
                Spl1 = Regex.Split(PlData, "\"default_sku\":");

                for (int i = 1; i < Spl1.Length; i++)
                {

                    if (Spl1[i].Contains("\"top_category\":\"downloadable_game\""))
                    {
                        Spl3 = Regex.Split(Spl1[i], "\"name\":\"");
                        Spl4 = Regex.Split(Spl3[1], "\"");
                        TmpTitle = Spl4[0].Trim();
                        TmpTitle = WebUtility.HtmlDecode(TmpTitle);
                        TmpTitle = Regex.Replace(TmpTitle, "[^a-zA-Z0-9 -<>&,]", "");
                        Spl3 = Regex.Split(Spl1[i], "/999/");
                        Spl4 = Regex.Split(Spl3[1], "/");
                        switch (Spl4[0].Trim().Substring(0, 1))
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
                        TmpURL = "https://store.playstation.com/" + titleRgn + "/product/" + Spl4[0].Trim();
                        TmpID = "GAME";
                        Spl3 = Regex.Split(Spl1[i], "\"url\":\"");
                        Spl4 = Regex.Split(Spl3[1], "\"");
                        TmpImgUrl = Spl4[0].Trim();
                        TmpPlatForm = "";
                        string[] TmpItem = { TmpTitle, TmpID, TmpPlatForm, TmpURL, TmpImgUrl };
                        var LvItem = new ListViewItem(TmpItem);
                        LV1.Items.Add(LvItem);
                    }
                }
                LV1.EndUpdate();
            }
            else
            {
                AppLog("Nothing found in search.");
            }
        }


        void SearchDataError(object sender, PDL e)
        {
            AppLog("ERROR: " + e.SearchDataErrorMessage);
        }


        void GotImage(object sender, PDL e)
        {
            string imagePath = AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_temp/sce_sys/icon0";
            var bitmap = Bitmap.FromFile(imagePath + ".jpeg");
            bitmap.Save(imagePath + ".png", ImageFormat.Png);
            bitmap.Dispose();
            File.Delete(imagePath + ".jpeg");
            CreatePKG(selCid, selName, titleID, true) ;

        }


        void ImageError(object sender, PDL e)
        {
            AppLog("IMAGE ERROR: " + e.ImageErrorMessage + Environment.NewLine + "Creating without icon0.png");
            CreatePKG(selCid, selName, titleID);
        }
        
    
        private void Button1_Click(object sender, EventArgs e)
        {
            button7.Visible = false;
            isSearch = false;
            if (textBox1.Text != String.Empty)
            {
                string[] Spl1, Spl2;
                LV1.Items.Clear();
                textBox2.Clear();
                Button2.Visible = false;
                Button3.Visible = false;
                linkLabel1.Text = "";
                Text = "psDLC";  
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.SelectionLength = 0;

                if (textBox1.Text.Length == 9 && textBox1.Text.ToLower().Contains("cusa"))
                {
                    textBox1.Text = textBox1.Text.ToUpper();
                    PDL1.GetPkgList(textBox1.Text);
                }
                else if (textBox1.Text.Length == 9 && textBox1.Text.ToLower().Contains("ppsa"))
                {
                    textBox1.Text = textBox1.Text.ToUpper();
                    PDL1.GetDlcList(textBox1.Text,"");
                }
                else if (textBox1.Text.StartsWith("http") && textBox1.Text.Contains(".com/") && textBox1.Text.Contains("/product/") && textBox1.Text.ToLower().Contains("cusa") && textBox1.Text.Length > 36)
                {
                    htmBuffer = string.Empty;
                    Spl1 = Regex.Split(textBox1.Text, ".com/");
                    Spl2 = Regex.Split(Spl1[1], "/");
                    titleRgn = Spl2[0];
                    Spl1 = Regex.Split(textBox1.Text, "/product/");
                    titleID = Spl1[1].Substring(7, 12);

                    textBox1.Text = Spl1[1].Substring(7, 9);

                    PDL1.GetDlcList(titleID, titleRgn);
                }
                else if (textBox1.Text.Length >= 19 && textBox1.Text.ToLower().Contains("cusa"))
                {
                    textBox1.Text = textBox1.Text.ToUpper();
                    htmBuffer = string.Empty;
                    titleID = textBox1.Text.Substring(7, 12);
                    switch (textBox1.Text.Substring(0, 1))
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
                    textBox1.Text = textBox1.Text.Substring(7, 9);
                    PDL1.GetDlcList(titleID, titleRgn);
                }
                else if (textBox1.Text.Length >= 19 && textBox1.Text.ToLower().Contains("-ppsa") && textBox1.Text.ToLower().Contains("_00"))
                {
                    textBox1.Text = textBox1.Text.ToUpper();
                    Spl1 = Regex.Split(textBox1.Text, "-PPSA");
                    Spl2 = Regex.Split(Spl1[1], "_");
                    textBox1.Text = "PPSA" + Spl2[0];
                    PDL1.GetDlcList(textBox1.Text, "");
                }
                else
                {
                    isSearch = true;
                    PDL1.GetSearch(textBox1.Text, comboBox1.Text);
                }
            }
        }


        private void Button2_Click(object sender, EventArgs e)
        {
            button7.Visible = false;
            panel2.Visible = false;
            label4.Text = "";
            label5.Text = "";
            label6.Text = "";
            label7.Text = "";
            pictureBox1.Image = null;
            textBox2.Clear();
            Button2.Visible = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "orbis-pub-cmd.exe"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_temp/sce_sys/");
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_pkg/");

                if (selImg == null || selImg == String.Empty)
                {
                    CreatePKG(selCid, selName, "", false);
                }
                else
                { 
                PDL1.GetImage(selImg, AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_temp/sce_sys/icon0.jpeg");
                }
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


        private void button5_Click(object sender, EventArgs e)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "orbis-pub-cmd.exe"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_temp/sce_sys/");
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_pkg/");
                if (text3Hint == false && text4Hint == false && textBox4.Text.ToUpper().Contains("CUSA") || textBox4.Text.ToUpper().Contains("PPSA") && textBox4.Text.Length == 36)
                {
                    CreatePKG(textBox4.Text.ToUpper(), textBox3.Text, textBox4.Text.ToUpper().Substring(7, 12), false);
                    text3Hint = true;
                    textBox3.ForeColor = Color.Gray;
                    textBox3.Text = "Display Name For DLC";
                    text4Hint = true;
                    textBox4.ForeColor = Color.Gray;
                    textBox4.Text = "XX0000-CUSA00000_00-0000000000000000";
                }
                else
                {
                    AppLog("ERROR: You must enter a display name for the dlc and the content id." + Environment.NewLine + Environment.NewLine + "The content id must be in the following format: XX0000-CUSA00000_00-0000000000000000 or XX0000-PPSA00000_00-0000000000000000");
                }
            }
            else
            {
                AppLog("ERROR: orbis-pub-cmd.exe not found" + Environment.NewLine + "You need to place orbis-pub-cmd.exe in the same directory as this application");
            }
        }


        private void label1_Click(object sender, EventArgs e)
        {
            if (panel1.Visible == true)
            {
                panel1.Visible = false;
            }
            else
            {
                panel1.BringToFront();
                panel1.Visible = true; 
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

        
        private void label12_Click(object sender, EventArgs e)
        {
            if (panel3.Visible == true)
            {
                panel3.Visible = false;
            }
            else
            {
                panel3.BringToFront();
                panel3.Visible = true;
            }
        }


        private void label13_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
        }


        private void label14_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel1.Text.Length > 6 && linkLabel1.Text.StartsWith("http"))
            {
                Process.Start(linkLabel1.Text);
            }
        }

        
        private void CreatePKG(string CID, string Name, string TID, bool hasImage = false)
        {
            string[] Spl1;
            Spl1 = Regex.Split(CID, "/");
            Name = Regex.Replace(Name, "[^A-Za-z0-9 ]", "");
            string cntId = Spl1[Spl1.Length - 1];
            TID = cntId.ToUpper().Substring(7, 12);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "fake_dlc_temp/param_template.sfx", SFX(cntId, Name, TID.Substring(0, 9)));
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


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                Button1.PerformClick();
                e.Handled = true;
            }
        }


        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (text3Hint)
            {
                text3Hint = false;
                textBox3.Text = "";
                textBox3.ForeColor = Color.Black;
            }
        }


        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (!text3Hint && string.IsNullOrEmpty(textBox3.Text))
            {
                text3Hint = true;
                textBox3.Text = "Display Name For DLC";
                textBox3.ForeColor = Color.Gray;
            }
        }


        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (text4Hint)
            {
                text4Hint = false;
                textBox4.Text = "";
                textBox4.ForeColor = Color.Black;
            }
        }


        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (!text4Hint && string.IsNullOrEmpty(textBox4.Text))
            {
                text4Hint = true;
                textBox4.Text = "XX0000-CUSA00000_00-0000000000000000";
                textBox4.ForeColor = Color.Gray;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button7.Visible = false;
            isSearch = false;
            if (textBox1.Text != String.Empty)
            {
                string[] Spl1, Spl2;
                LV1.Items.Clear();
                textBox2.Clear();
                Button2.Visible = false;
                Button3.Visible = false;
                linkLabel1.Text = "";
                Text = "psDLC";
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.SelectionLength = 0;
                
                if (textBox1.Text.Length == 9 && textBox1.Text.ToLower().StartsWith("cusa"))
                {
                    textBox1.Text = textBox1.Text.ToUpper();
                    PDL1.GetDlcList(textBox1.Text, "", true);
                }
                else if (textBox1.Text.StartsWith("http") && textBox1.Text.Contains(".com/") && textBox1.Text.Contains("/product/") && textBox1.Text.ToLower().Contains("cusa") && textBox1.Text.Length > 36)
                {
                    htmBuffer = string.Empty;
                    Spl1 = Regex.Split(textBox1.Text, ".com/");
                    Spl2 = Regex.Split(Spl1[1], "/");
                    titleRgn = Spl2[0];
                    Spl1 = Regex.Split(textBox1.Text, "/product/");
                    titleID = Spl1[1].Substring(7, 12);

                    textBox1.Text = Spl1[1].Substring(7, 9);

                    PDL1.GetDlcList(titleID, titleRgn, true);
                }
                else if (textBox1.Text.Length >= 19 && textBox1.Text.ToLower().Contains("cusa"))
                {
                    textBox1.Text = textBox1.Text.ToUpper();
                    htmBuffer = string.Empty;
                    titleID = textBox1.Text.Substring(7, 12);
                    switch (textBox1.Text.Substring(0, 1))
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
                    textBox1.Text = textBox1.Text.Substring(7, 9);

                    PDL1.GetDlcList(titleID, titleRgn,true);
                }
                else
                {
                    Button1.PerformClick();
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            textBox1.Text = selCid;
            Button1.PerformClick();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("region", comboBox1.Text);
        }

        private void LV1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem LvItem = LV1.FocusedItem as ListViewItem;
            ListView.SelectedIndexCollection Lview = LV1.SelectedIndices;
            if (LvItem != null)
            {
                textBox2.Clear();

                if (isSearch)
                {
                    Button2.Visible = false;
                    button7.Visible = true;
                }
                else
                {
                    button7.Visible = false;
                    Button2.Visible = true;
                }


                if (!LvItem.SubItems[3].Text.Contains("-PPSA"))
                {
                    linkLabel1.Text = LvItem.SubItems[3].Text;
                }
                else
                {
                    linkLabel1.Text = LvItem.Text;
                }
                selName = LvItem.Text;
                selCid = LvItem.SubItems[3].Text;
                selImg = LvItem.SubItems[4].Text;


                if (settings.GetSetting("check14", false) == true && !selCid.Contains("-PPSA"))
                {
                    PDL1.GetDlcInfo(titleRgn, Regex.Split(selCid, "/")[Regex.Split(selCid, "/").Length - 1]);
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


        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check13", checkBox13.Checked);
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            settings.SaveSetting("check14", checkBox14.Checked);
        }


 
    }
}
