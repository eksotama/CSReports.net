using System;
using System.Globalization;
using CSReportGlobals;
using CSKernelClient;
using System.Collections.Generic;
using CSReportDll;
using CSMaskEdit;

namespace CSReportEditor
{
	public class cMainEditor {

	    private const string C_MODULE = "mPublic";
		
	    private const int NOERROR = 0;

		public DateTime CSNOFECHA = DateTime.ParseExact("01/01/1900", "dd/mm/yyyy", CultureInfo.InvariantCulture);
		public const string C_KEY_HEADER = "RH";
		public const string C_KEY_FOOTER = "RF";
		public const string C_KEY_DETAIL = "RD";
		public const string C_KEY_GROUPH = "GH";
		public const string C_KEY_GROUPF = "GF";

		public const string c_BTN_PRINT = "PRINT";
		public const string c_BTN_PROPERTIES = "PROPERTIES";
		public const string c_BTN_DB = "DB";
		public const string c_BTN_SAVE = "SAVE";
		public const string c_BTN_OPEN = "OPEN";
		public const string c_BTN_TOOL = "TOOL";
		public const string c_BTN_NEW = "NEW";
		public const string c_BTN_PREV = "PREV";

		public const string c_BTN_CTL_ALIGN_TOP = "CTL_ALIGN_TOP";
		public const string c_BTN_CTL_ALIGN_BOTTOM = "CTL_ALIGN_BOTTOM";
		public const string c_BTN_CTL_ALIGN_VERTICAL = "CTL_ALIGN_VERTICAL";
		public const string c_BTN_CTL_ALIGN_HORIZONTAL = "CTL_ALIGN_HORIZONTAL";
		public const string c_BTN_CTL_ALIGN_LEFT = "CTL_ALIGN_LEFT";
		public const string c_BTN_CTL_ALIGN_RIGHT = "CTL_ALIGN_RIGHT";

		public const string c_BTN_CTL_WIDTH = "CTL_WIDTH";
		public const string c_BTN_CTL_HEIGHT = "CTL_HEIGHT";

		public const string C_Control_Name = "Control";

	    public const int C_HEIGHT_BAR_SECTION = 120;
	    public const int C_HEIGHT_NEW_SECTION = 350;

	    private const string C_KEYRECENTLIST = "Recent";

	    private const string C_CONFIG = "Interfaz";
	    private const string C_LEFTBARCOLOR = "LeftBarColor";
	    private const string C_HIDELEFTBAR = "HideLeftBar";
	    private const string C_BACKCOLOR = "BackColor";
	    private const string C_WORKFOLDER = "WorkFolder";

	    public static int gNextReport = 0;
	    private static cEditor m_editor;
	    private static cEditor m_toolBoxOwner;
	    private static cEditor m_ctrlBoxOwner;
	    private static cEditor m_ctrlTreeBoxOwner;

		public static int gBackColor = 0;
	    public static int gLeftBarColor = 0;
	    public static bool gHideLeftBar;
	    public static String gWorkFolder = "";
	    public static bool gbFirstOpen;

        private static fMain fmain;

        public static fMain initEditor() {
            if (fmain == null) {
                fmain = new fMain();
            }
            return fmain;
        }

	    public static cEditor getDocActive() {
	        return m_editor;
	    }

	    public static void setDocActive(cEditor f) {
	        m_editor = f;
	        setMenu();
	    }

	    public static void setDocInacActive(cEditor f) {
	        if (m_editor != f) { return; }
	        m_editor = null;
	        setMenu();
	        setEditAlignTextState(false);
	    }

	    public static void setStatus() {
	        try {
	            if (m_editor == null) {
	                setStatus("");
	            } 
	            else {
	                setStatus(pGetStatus());
	            }

	        } catch (Exception ex) {
	            cError.mngError(ex, "setStatus", C_MODULE, "");
	        }
	    }

        public static void setStatus(String status) { 
        
        }

        public static void setBarText(String text) { 
        
        }

        public static void setDisconnectedReport(bool isDisconnectedReport) { 
        
        }

        public static void setEditAlignTextState(bool status) {
            fmain.setEditAlignTextState(status);
        }

        public static void setEditAlignCtlState(bool status) {
            fmain.setEditAlignCtlState(status);
        }

        public static void setMenuAux(bool enabled) {
            fmain.setMenuAux(enabled);
        }

        public static void addToRecentList(String fileName){
            fmain.addToRecentList(fileName);
        }

        public static void loadRecentList() {
            // TODO: implement
            fmain.loadRecentList(new List<String>());
        }

        public static void saveRecentList() {
            fmain.saveRecentList();
        }
	    public static void setEditFontBoldValue(int bBold) {
			// TODO: implement
	    }

		public static void setEditAlignValue(HorizontalAlignment align) {
			// TODO: implement
	    }

	    private static void setMenu() {
	        try {

	            if (m_editor == null) {
	                fmain.setMenuAux(false);
	                fmain.setBarText("");
	                fmain.setStatus("");
	            } 
	            else {
	                fmain.setMenuAux(true);
	                fmain.setDisconnectedReport(m_editor.getReport().getReportDisconnected());
	                fmain.setBarText(m_editor.getReport().getName());
	                fmain.setStatus(pGetStatus());
	            }
	        } catch (Exception ex) {
	            cError.mngError(ex, "SetMenu", C_MODULE, "");
	        }
	    }



	}

	public class Rectangle {
		public long height;
		public long width;
	}


	public enum SpecialFolderIDs {
	    SFIDDESKTOP = 0x0,
	    SFIDPROGRAMS = 0x2,
	    SFIDPERSONAL = 0x5,
	    SFIDFAVORITES = 0x6,
	    SFIDSTARTUP = 0x7,
	    SFIDRECENT = 0x8,
	    SFIDSENDTO = 0x9,
	    SFIDSTARTMENU = 0xB,
	    SFIDDESKTOPDIRECTORY = 0x10,
	    SFIDNETHOOD = 0x13,
	    SFIDFONTS = 0x14,
	    SFIDTEMPLATES = 0x15,
	    SFIDCOMMON_STARTMENU = 0x16,
	    SFIDCOMMON_PROGRAMS = 0x17,
	    SFIDCOMMON_STARTUP = 0x18,
	    SFIDCOMMON_DESKTOPDIRECTORY = 0x19,
	    SFIDAPPDATA = 0x1A,
	    SFIDPRINTHOOD = 0x1B,
	    SFIDPROGRAMS_FILES = 0x26,
	    SFIDPROGRAMFILES = 0x10000,
	    SFIDCOMMONFILES = 0x10001
	}


	public enum csEAlignConst {
	    CSEALIGNTEXTLEFT = 1,
	    CSEALIGNTEXTRIGHT,
	    CSEALIGNTEXTCENTER,
	    CSEALIGNCTLLEFT,
	    CSEALIGNCTLHORIZONTAL,
	    CSEALIGNCTLRIGHT,
	    CSEALIGNCTLVERTICAL,
	    CSEALIGNCTLTOP,
	    CSEALIGNCTLBOTTOM,
	    CSEALIGNCTLWIDTH,
	    CSEALIGNCTLHEIGHT
	}

	public enum CSRptEditroMoveType {
	    CSRPTEDMOVTHORIZONTAL,
	    CSRPTEDMOVTVERTICAL,
	    CSRPTEDMOVTALL,
	    CSRPTEDMOVLEFT,
	    CSRPTEDMOVRIGHT,
	    CSRPTEDMOVUP,
	    CSRPTEDMOVDOWN,
	    CSRPTEDMOVLEFTDOWN,
	    CSRPTEDMOVLEFTUP,
	    CSRPTEDMOVRIGHTDOWN,
	    CSRPTEDMOVRIGHTUP,
	    CSRPTEDMOVTNONE
	}

}

