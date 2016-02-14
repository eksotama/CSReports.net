﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSKernelClient;
using CSReportDll;
using CSReportGlobals;
using System.Windows.Forms;
using System.Drawing;

namespace CSReportEditor
{
    public static class cGlobals
    {

        public const String C_MODULE = "CSReportEditor.cGlobals";

        public const String C_KEY_HEADER = "RH";
        public const String C_KEY_FOOTER = "RF";
        public const String C_KEY_DETAIL = "RD";
        public const String C_KEY_GROUPH = "GH";
        public const String C_KEY_GROUPF = "GF";

        public const String c_BTN_PRINT        = "PRINT";
        public const String c_BTN_PROPERTIES   = "PROPERTIES";
        public const String c_BTN_DB           = "DB";
        public const String c_BTN_SAVE         = "SAVE";
        public const String c_BTN_OPEN         = "OPEN";
        public const String c_BTN_TOOL         = "TOOL";
        public const String c_BTN_NEW          = "NEW";
        public const String c_BTN_PREV         = "PREV";

        public const String c_BTN_ALIGN_LEFT   = "ALIGN_LEFT";
        public const String c_BTN_ALIGN_CENTER = "ALIGN_CENTER";
        public const String c_BTN_ALIGN_RIGHT  = "ALIGN_RIGHT";

        public const string c_BTN_FONT_BOLD = "FONT_BOLD";
        public const string c_BTN_SEARCH = "SEARCH";

        public const String c_BTN_CTL_ALIGN_TOP        = "CTL_ALIGN_TOP";
        public const String c_BTN_CTL_ALIGN_BOTTOM     = "CTL_ALIGN_BOTTOM";
        public const String c_BTN_CTL_ALIGN_VERTICAL   = "CTL_ALIGN_VERTICAL";
        public const String c_BTN_CTL_ALIGN_HORIZONTAL = "CTL_ALIGN_HORIZONTAL";
        public const String c_BTN_CTL_ALIGN_LEFT       = "CTL_ALIGN_LEFT";
        public const String c_BTN_CTL_ALIGN_RIGHT      = "CTL_ALIGN_RIGHT";

        public const String c_BTN_CTL_WIDTH  = "CTL_WIDTH";
        public const String c_BTN_CTL_HEIGHT = "CTL_HEIGHT";

        public const String C_CONTROL_NAME = "Control";

        public const int C_TOTINRECENTLIST = 7;

        public const int C_HEIGHT_NEW_SECTION = 23;
        public const int C_HEIGHT_BAR_SECTION = 8;

        public const int C_NO_CHANGE = -32768;

        public const String C_MAIN_HEADER = "Main Header";
        public const String C_MAIN_DETAIL = "Detail";
        public const String C_MAIN_FOOTER = "Main Footer";

        public const String C_GROUP_LABEL = "Group";

		// TODO: refactor
		public const int ShiftMask = 1;

        private static cEditor m_editor = null;

        public static void setStatus()
        { 
        
        }

        public static bool isNumberField(int fieldType)
        {
            return false;
        }

		public static bool showDbFields(ref string field, ref int fieldType, ref int index, cEditor editor)
		{
            fColumns fc = null;

            try {
                fc = new fColumns();

                fc.clearColumns();

                cReport report = editor.getReport();
                
                cReportConnect connect = report.getConnect();
                fc.fillColumns(connect.getDataSource(), connect.getColumns());

                for (int _i = 0; _i < report.getConnectsAux().count(); _i++)
                {
                    connect = report.getConnectsAux().item(_i);
                    fc.fillColumns(connect.getDataSource(), connect.getColumns());
                }

                fc.setField(field);
                fc.ShowDialog();

                if (fc.getOk())
                {
                    field = fc.getField();
                    fieldType = fc.getFieldType();
                    index = fc.getIndex();

                    return true;
                }
                else
                {
                    return false;
                }

            } catch (Exception ex) {
                cError.mngError(ex, "showDbFields", C_MODULE, "");
                return false;
            }
            finally {
                if (fc != null)
                {
                    fc.Close();
                }
            }      
		}

		public static void setEditAlignTextState(object length)
		{
            implementThisMessage("setEditAlignTextState", "(CSReportEditor cGlobals)");
		}

		public static void setEditAlignCtlState(bool b)
		{
            implementThisMessage("setEditAlignCtlState", "(CSReportEditor cGlobals)");
		}

		public static void setEditFontBoldValue(int bBold)
		{
            implementThisMessage("setEditFontBoldValue", "(CSReportEditor cGlobals)");
		}

		public static void setEditAlignValue(int align)
		{
            implementThisMessage("setEditAlignValue", "(CSReportEditor cGlobals)");
		}

		public static void setParametersAux(CSConnect.cConnect connect, object connect2)
		{
			throw new NotImplementedException ();
		}

		public static fToolbox getToolBox(cEditor cEditor)
		{
			throw new NotImplementedException ();
		}

        public static void moveGroup(cReportGroup group, cEditor editor)
        {
            throw new NotImplementedException();
        }

        public static string getDataSourceStr(string dataSource)
        {
            return "{" + dataSource + "}.";
        }

        internal static void clearToolBox(cEditor editor)
        {
            throw new NotImplementedException();
        }

        internal static fControls getCtrlBox(cEditor editor)
        {
            throw new NotImplementedException();
        }

        internal static void setDocInacActive(cEditor editor)
        {
            throw new NotImplementedException();
        }

        internal static void createStandarSections(cReport report, Rectangle tr)
        {
            report.getHeaders().add(null, C_KEY_HEADER);
            report.getFooters().add(null, C_KEY_FOOTER);
            report.getDetails().add(null, C_KEY_DETAIL);

            // 
            // main header
            //
            cReportSection sec = report.getHeaders().item(C_KEY_HEADER);
            sec.setName("Main header");

            cReportAspect aspect = sec.getAspect();
            aspect.setTop(0);
            aspect.setHeight(tr.height * 0.25f);
            aspect.setWidth(tr.width);
            cReportSectionLine secLn = sec.getSectionLines().item(0);
            secLn.setSectionName("Main header");
            aspect = secLn.getAspect();
            aspect.setTop(0);
            aspect.setHeight(tr.height * 0.25f);
            aspect.setWidth(tr.width);

            // 
            // detail
            //
            sec = report.getDetails().item(C_KEY_DETAIL);
            sec.setName("Detail");

            aspect = sec.getAspect();
            aspect.setTop(tr.height * 0.25f);
            aspect.setHeight(tr.height * 0.25f);
            aspect.setWidth(tr.width);
            secLn = sec.getSectionLines().item(0);
            secLn.setSectionName("Detail");
            aspect = secLn.getAspect();
            aspect.setTop(tr.height * 0.25f);
            aspect.setHeight(tr.height * 0.25f);
            aspect.setWidth(tr.width);

            // 
            // main footer
            //
            sec = report.getFooters().item(C_KEY_FOOTER);
            sec.setName("Main footer");

            aspect = sec.getAspect();
            aspect.setTop(tr.height * 0.75f);
            aspect.setHeight(tr.height * 0.25f);
            aspect.setWidth(tr.width);
            secLn = sec.getSectionLines().item(0);
            secLn.setSectionName("Main footer");
            aspect = secLn.getAspect();
            aspect.setTop(tr.height * 0.75f);
            aspect.setHeight(tr.height * 0.25f);
            aspect.setWidth(tr.width);
        }

        internal static void clearCtrlBox(cEditor editor)
        {
            throw new NotImplementedException();
        }

        public static void implementThisMessage(string functionName, string moduleName) 
        {
            Console.WriteLine(String.Format("Implement this: public static void {0} {1}", functionName, moduleName));
        }

    }

    public class Rectangle
    {
        public long height;
        public long width;

        public Rectangle(RectangleF rect)
        {
            height = (long)rect.Height;
            width = (long)rect.Width;
        }
    }

    public interface cIDatabaseFieldSelector 
    {
        int getFieldType();
        void setFieldType(int rhs);
        int getIndex();
        void setIndex(int rhs);
        System.Windows.Forms.TextBox txDbField { get; }
    }

    public enum csRptEditorMoveType {
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

    public enum csRptEditCtrlType {
        CSRPTEDITNONE,
        CSRPTEDITLABEL,
        CSRPTEDITFIELD,
        CSRPTEDITFORMULA,
        CSRPTEDITIMAGE,
        CSRPTEDITCHART
    }
}
