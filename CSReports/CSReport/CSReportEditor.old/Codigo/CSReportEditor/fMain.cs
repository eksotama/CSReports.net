﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSKernelClient;

namespace CSReportEditor
{
    public partial class fMain : Form
    {
        static fMain instance;

        private const String C_MODULE = "fMain";

        public fMain()
        {
            InitializeComponent();

            // it is the first thing we need to do
            //
            CSKernelClient.cUtil.setSepDecimal();

            cEditor editor = new cEditor(this, pnEditor, pnRule, pnReport, tbpEditor);
        }

        public cEditor getReportCopySource()
        {
            return null;
        }

        private cEditor createEditor() 
        {
            TabPage tab = new TabPage();
            Panel pnEditor = new Panel();
            PictureBox pnRule = new PictureBox();
            PictureBox pnReport = new PictureBox();

            pnEditor.Controls.Add(pnRule);
            pnEditor.Controls.Add(pnReport);
            tab.Controls.Add(pnEditor);
            pnEditor.Dock = DockStyle.Fill;
            tabReports.TabPages.Add(tab);
            tab.Text = "New Report";

            return new cEditor(this, pnEditor, pnRule, pnReport, tab);
        }

        private void mnuNewReport_Click(object sender, EventArgs e)
        {
            createEditor();
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            mnuNewReport_Click(sender, e);
        }

        public void setEditAlignTextState(bool status)
        {
            var buttons = this.tbMain.Items;

            buttons[cGlobals.c_BTN_ALIGN_CENTER].Enabled = status;
            buttons[cGlobals.c_BTN_ALIGN_LEFT].Enabled = status;
            buttons[cGlobals.c_BTN_ALIGN_RIGHT].Enabled = status;
            buttons[cGlobals.c_BTN_FONT_BOLD].Enabled = status;
        }

        public void setEditAlignCtlState(bool status) 
        {
            var buttons = this.tbMain.Items;

            buttons[cGlobals.c_BTN_CTL_ALIGN_BOTTOM].Enabled = status;
            buttons[cGlobals.c_BTN_CTL_ALIGN_TOP].Enabled = status;

            buttons[cGlobals.c_BTN_CTL_ALIGN_VERTICAL].Enabled = status;
            buttons[cGlobals.c_BTN_CTL_ALIGN_HORIZONTAL].Enabled = status;
            buttons[cGlobals.c_BTN_CTL_ALIGN_LEFT].Enabled = status;
            buttons[cGlobals.c_BTN_CTL_ALIGN_RIGHT].Enabled = status;

            buttons[cGlobals.c_BTN_CTL_HEIGHT].Enabled = status;
            buttons[cGlobals.c_BTN_CTL_WIDTH].Enabled = status;
        }

        public void setMenuAux(bool enabled)
        {
            this.mnuEditAddControl.Enabled = enabled;
            this.mnuEditAddHeader.Enabled = enabled;
            this.mnuEditAddLabel.Enabled = enabled;
            this.mnuEditAddGroup.Enabled = enabled;
            this.mnuEditAddFooter.Enabled = enabled;
            this.mnuEditAddLine.Enabled = enabled;
            this.mnuEditAddSec.Enabled = enabled;
            this.mnuEditMove.Enabled = enabled;
            this.mnuDataBaseEditEx.Enabled = enabled;
            this.mnuPreviewReport.Enabled = enabled;
            this.mnuPrintReport.Enabled = enabled;
            this.mnuSaveReport.Enabled = enabled;
            this.mnuReportSaveAs.Enabled = enabled;
            this.mnuDataBaseSetDisconnected.Enabled = enabled;
            this.mnuEditSearch.Enabled = enabled;
            this.mnuDataBaseEditStrConnect.Enabled = enabled;
            this.mnuDataBaseSetToMainConnect.Enabled = enabled;
            this.mnuDataBaseEditEx.Enabled = enabled;
            this.mnuDataBaseConnectsAuxCfg.Enabled = enabled;
            this.mnuViewGridMain.Enabled = enabled;
            this.mnuViewToolbar.Enabled = enabled;
            this.mnuViewControls.Enabled = enabled;
            this.mnuViewTreeViewCtrls.Enabled = enabled;

            var buttons = this.tbMain.Items;
            buttons[cGlobals.c_BTN_PRINT].Enabled = enabled;
            buttons[cGlobals.c_BTN_PROPERTIES].Enabled = enabled;
            buttons[cGlobals.c_BTN_DB].Enabled = enabled;
            buttons[cGlobals.c_BTN_SAVE].Enabled = enabled;
            buttons[cGlobals.c_BTN_TOOL].Enabled = enabled;
            buttons[cGlobals.c_BTN_PREV].Enabled = enabled;
            buttons[cGlobals.c_BTN_SEARCH].Enabled = enabled;
        }

        public void addToRecentList(String fileName)
        {
            int i = 0;
            int j = 0;
            bool found = false;
            var menuItems = this.mnuFileRecentList.DropDownItems;

            for (i = 0; i < menuItems.Count; i++)
            {
                if (fileName == menuItems[i].Text)
                {
                    j = i;
                    found = true;
                    break;
                }
            }

            if (menuItems.Count < cGlobals.C_TOTINRECENTLIST && found == false)
            {
                var menu = this.mnuFileRecentList.DropDownItems.Add("");
                menu.Visible = true;
            }

            if (!found) { j = menuItems.Count - 1; }

            for (i = j; i > 0; i--)
            {
                menuItems[i].Text = menuItems[i - 1].Text;
            }

            menuItems[0].Text = fileName;
        }

        public void loadRecentList(List<String> recentList)
        {
            int i = 0;
            String recent = "";

            for (i = 0; i < Math.Min(cGlobals.C_TOTINRECENTLIST, recentList.Count); i++)
            {
                recent = recentList[i];
                var menu = this.mnuFileRecentList.DropDownItems.Add(recent);
                menu.Visible = true;
            }

            if (this.mnuFileRecentList.DropDownItems.Count > 1)
            {
                this.mnuFileRecentList.Visible = true;
            }
        }

        public void saveRecentList()
        {
            int i = 0;

            for (i = 0; i < this.mnuFileRecentList.DropDownItems.Count; i++)
            {
                // TODO: implement
            }
        }

        public void setStatus(String status)
        {
            // TODO: implement
        }

        public void setBarText(String text)
        {
            // TODO: implement
        }

        public void setDisconnectedReport(bool isDisconnectedReport)
        {
            // TODO: implement
        }

		public void setsbPnlCtrl (string msg)
		{
			throw new NotImplementedException ();
		}

        internal void setReportCopySource(cEditor cEditor)
        {
            throw new NotImplementedException();
        }

        internal CSReportGlobals.csReportPaperType getPaperSize()
        {
            throw new NotImplementedException();
        }

        internal int getOrientation()
        {
            throw new NotImplementedException();
        }

        private void mnuOpenReport_Click(object sender, EventArgs e)
        {
            try {
                
                cEditor editor = createEditor();

                editor.init();
                /*
                if(editor.openDocument()) {
                    addToRecentList(editor.getFileName());
                    saveRecentList();
                }*/

            } catch (Exception ex) {
                cError.mngError(ex, "mnuOpenReport_Click", C_MODULE, "");
            }
        }

        //------------------------------------------------------------------------------------------------------------------

        // expose controls

        //------------------------------------------------------------------------------------------------------------------

        public OpenFileDialog openFileDialog 
        {
            get
            {
                return openFileDlg;
            }
        }

        public SaveFileDialog saveFileDialog
        {
            get
            {
                return saveFielDlg;
            }            
        }
    }
}
