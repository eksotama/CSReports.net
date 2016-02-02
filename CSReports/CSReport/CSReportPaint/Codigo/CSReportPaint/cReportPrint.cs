﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CSKernelClient;
using CSIReportPrint;
using System.Windows.Forms;
using CSReportDll;
using CSReportGlobals;
using CSReportExport;

namespace CSReportPaint
{
	public class cReportPrint : cIReportPrint, IDisposable
    {

        private const String C_MODULE = "cReportPrint";

        private const int C_OFFSETHEADER = 0;
        private const int C_OFFSETDETAIL = 100000;
        private const int C_OFFSETFOOTER = 1000000;

        private CSReportDll.cReport m_report;
        private cReportPaint m_paint;
        private CSReportPreview.cReportPreview m_rpwPrint;
        private fPreview m_fPreview;

        private int m_lastIndexField = 0;

        private int m_currPage = 0;

        private Font[] m_fnt;
        private int m_iFontCount = 0;

        private int m_hDC = 0;

        private int m_x = 0;
        private int m_y = 0;

        private bool m_rePaintObject;

        private int m_realWidth = 0;
        private int m_realHeight = 0;

        private float m_scaleFont = 1;

        private float m_scaleY = 1;
        private float m_scaleX = 1;

        private bool m_bModal;

        private bool m_bHidePreviewWindow;

        private String m_fileToSavePDF = "";
        private csPDFQuality m_pDFQuality;

        private String m_exportFileName = "";

        //---------------------------------------------------------------------------
        //
        // Export to PDF

        // for cExportPDF
        //
        public String getFileToSavePDF()
        {
            return m_fileToSavePDF;
        }

        public void setFileToSavePDF(String rhs)
        { // TODO: Use of ByRef founded Public Property Let FileToSavePDF(ByRef rhs As String)
            m_fileToSavePDF = rhs;
        }

        public csPDFQuality getPDFQuality()
        {
            return m_pDFQuality;
        }

        public void setPDFQuality(csPDFQuality rhs)
        { // TODO: Use of ByRef founded Public Property Let PDFQuality(ByRef rhs As csPDFQuality)
            m_pDFQuality = rhs;
        }

        // for cPrintManager
        //
        public String getExportFileName()
        {
            return m_exportFileName;
        }

        public void setExportFileName(String rhs)
        { // TODO: Use of ByRef founded Public Property Let ExportFileName(ByRef rhs As String)
            m_exportFileName = rhs;
        }

        //
        //---------------------------------------------------------------------------

        public CSReportDll.cReport getReport()
        {
            return m_report;
        }

        public void setPreviewControl(CSReportPreview.cReportPreview rhs)
        { // TODO: Use of ByRef founded Public Property Set PreviewControl(ByRef rhs As Object)
            m_rpwPrint = rhs;
        }

        public int getCurrPage()
        {
            return m_currPage;
        }

        public void setCurrPage(int rhs)
        {
            m_currPage = rhs;
        }

        public void setModal(bool rhs)
        {
            m_bModal = rhs;
        }

        public void setHidePreviewWindow(bool rhs)
        {
            m_bHidePreviewWindow = rhs;
        }

        private void setReport(CSReportDll.cReport rhs)
        { // TODO: Use of ByRef founded Private Property Set Report(ByRef rhs As cReport)
            m_report = rhs;
        }

        public bool closePreviewWindow()
        {
            try
            {
                if (m_fPreview != null)
                {
                    m_fPreview.Dispose();
                    m_fPreview = null;
                }
                return true;

            }
            catch (Exception ex)
            {
                cError.mngError(ex, "ClosePreviewWindow", C_MODULE, "");
                return false;
            }
        }

        public CSReportDll.cReportPageFields getLine(int indexField)
        {
            CSReportDll.cReportPageField fld = null;
            fld = getField(indexField);

            if (fld == null)
            {
                return null;
            }
            else
            {
                CSReportDll.cReportPage w_item = m_report.getPages().item(m_currPage);
                if (indexField < C_OFFSETDETAIL)
                {
                    return pGetLineAux(fld.getIndexLine(), w_item.getHeader());
                }
                else if (indexField < C_OFFSETFOOTER)
                {
                    return pGetLineAux(fld.getIndexLine(), w_item.getDetail());
                }
                else
                {
                    return pGetLineAux(fld.getIndexLine(), w_item.getFooter());
                }
            }
        }

        public CSReportDll.cReportPageField getCtrlFooter(String ctrlName)
        {
            return getFieldByCtrlName(ctrlName, m_report.getPages().item(m_currPage).getFooter());
        }

        public int getIndexFieldByName(String ctrlName)
        {
            return m_paint.getPaintObjects().item(ctrlName).getIndexField();
        }

        public void refreshCtrl(int indexField)
        {
            cReportPaintObject paintObj = null;
            CSReportDll.cReportPageField fld = null;
            CSReportDll.cReportPage page = null;

            page = m_report.getPages().item(m_currPage);

            if (indexField < C_OFFSETDETAIL)
            {
                if (!pGetFieldFromIndexAux(page.getHeader(), indexField, ref fld))
                {
                    return;
                }
            }
            else if (indexField < C_OFFSETFOOTER)
            {
                if (!pGetFieldFromIndexAux(page.getDetail(), indexField - C_OFFSETDETAIL, ref fld))
                {
                    return;
                }
            }
            else
            {
                if (!pGetFieldFromIndexAux(page.getFooter(), indexField - C_OFFSETFOOTER, ref fld))
                {
                    return;
                }
            }

            paintObj = pGetPaintObjByIndex(indexField);

            CSReportDll.cReportFont ctrlFont = null;
            ctrlFont = fld.getInfo().getAspect().getFont();

            CSReportDll.cReportAspect w_aspect = paintObj.getAspect();
            CSReportDll.cReportFont w_font = w_aspect.getFont();
            w_font.setForeColor(ctrlFont.getForeColor());
            w_font.setBold(ctrlFont.getBold());
            w_font.setItalic(ctrlFont.getItalic());
            w_font.setName(ctrlFont.getName());
            w_font.setSize(ctrlFont.getSize());
            w_font.setStrike(ctrlFont.getStrike());
			w_font.setUnderline(ctrlFont.getUnderline());

            m_paint.refreshObject(paintObj.getKey(), m_rpwPrint.getGraph());
        }

        public void refreshCtrlFooter(String ctrlName)
        {
            cReportPaintObject paintObj = null;
            paintObj = pGetPaintObjByCtrlName(ctrlName, m_report.getPages().item(m_currPage).getFooter(), C_OFFSETFOOTER);
            paintObj.setText(getCtrlFooter(ctrlName).getValue());
            m_paint.refreshObject(paintObj.getKey(), m_rpwPrint.getGraph());
        }

        public CSReportDll.cReportPageField getFieldByCtrlName(
            String ctrlName,
            CSReportDll.cReportPageFields fields)
        {
            return getFieldByCtrlName(ctrlName, fields, 0);
        }

        public CSReportDll.cReportPageField getFieldByCtrlName(
            String ctrlName,
            CSReportDll.cReportPageFields fields,
            int indexField)
        {
            CSReportDll.cReportPageField fld = null;

            for (int _i = 0; _i < fields.count(); _i++)
            {
                fld = fields.item(_i);
                if (fld.getInfo().getName() == ctrlName)
                {
                    if (isInThisLine(ctrlName, indexField, fld))
                    {
                        return fld;
                    }
                }
            }
            return null;
        }

        public cReportPaintObject getPaintObjByCtrlNameEx(String ctrlName, int indexField)
        {
            CSReportDll.cReportPageField fld = null;
            CSReportDll.cReportPageFields fields = null;
            int offset = 0;

            CSReportDll.cReportPage w_item = m_report.getPages().item(m_currPage);

            fields = w_item.getHeader();
            offset = C_OFFSETHEADER;
            fld = getFieldByCtrlName(ctrlName, fields, indexField);

            if (fld == null)
            {
                fields = w_item.getDetail();
                offset = C_OFFSETDETAIL;
                fld = getFieldByCtrlName(ctrlName, fields, indexField);

                if (fld == null)
                {
                    fields = w_item.getFooter();
                    offset = C_OFFSETFOOTER;
                    fld = getFieldByCtrlName(ctrlName, fields, indexField);
                    if (fld == null)
                    {
                        return null;
                    }
                }
            }
            for (int _i = 0; _i < m_paint.getPaintObjects().count(); _i++)
            {
                var paintObj = m_paint.getPaintObjects().item(_i);
                if (fields.item(paintObj.getIndexField() - offset) == fld)
                {
                    if (isInThisLine(ctrlName, indexField, fld))
                    {
                        return paintObj;
                    }
                }
            }
            return null;
        }

        public bool isInThisLine(
            String ctrlName,
            int indexField,
            CSReportDll.cReportPageField testFld)
        { // TODO: Use of ByRef founded Public Function IsInThisLine(ByVal CtrlName As String, ByVal IndexField As Long, ByRef testFld As cReportPageField) As Boolean
            CSReportDll.cReportPageFields fields = null;
            CSReportDll.cReportPageField fld = null;

            if (indexField == 0)
            {
                return true;
            }
            fields = getLine(indexField);

            for (int _i = 0; _i < fields.count(); _i++)
            {
                fld = fields.item(_i);
                if (fld.getInfo().getName() == ctrlName)
                {
                    if (testFld == fld)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public CSReportDll.cReportPageField getField(int indexField)
        {
            CSReportDll.cReportPageField rtn = null;
            CSReportDll.cReportPage page = null;

            page = m_report.getPages().item(m_currPage);

            if (indexField < C_OFFSETDETAIL)
            {
                if (!pGetFieldFromIndexAux(page.getHeader(), indexField, ref rtn))
                {
                    return null;
                }
            }
            else if (indexField < C_OFFSETFOOTER)
            {
                if (!pGetFieldFromIndexAux(page.getDetail(), indexField - C_OFFSETDETAIL, ref rtn))
                {
                    return null;
                }
            }
            else
            {
                if (!pGetFieldFromIndexAux(page.getFooter(), indexField - C_OFFSETFOOTER, ref rtn))
                {
                    return null;
                }
            }
            return rtn;
        }

        public bool fieldIsInDetail(int indexField)
        {
            return indexField >= C_OFFSETDETAIL && indexField < C_OFFSETFOOTER;
        }

        public void printPage(int moveTo)
        {
            printPage(moveTo, false);
        }

        public void printPage(int nPage, bool inPrinter)
        {
            CSReportDll.cReportPage page = null;

            cMouseWait mouse = new cMouseWait();

            m_rePaintObject = true;

            if (nPage > 1)
            {
                m_currPage = nPage;
            }
            else
            {
                switch (nPage)
                {
                    case (int)csEMoveTo.C_FIRSTPAGE:
                        m_currPage = 1;
                        break;
                    case (int)csEMoveTo.C_LASTPAGE:
                        m_currPage = m_report.getPages().count();
                        break;
                    case (int)csEMoveTo.C_NEXTPAGE:
                        if (m_currPage + 1 <= m_report.getPages().count())
                        {
                            m_currPage = m_currPage + 1;
                        }
                        else
                        {
                            m_currPage = m_report.getPages().count();
                        }
                        break;
                    case (int)csEMoveTo.C_PREVIOUSPAGE:
                        if (m_currPage - 1 >= 1)
                        {
                            m_currPage = m_currPage - 1;
                        }
                        else
                        {
                            m_currPage = 1;
                        }
                        break;
                }
            }
            if (m_currPage == 0) { return; }

            page = m_report.getPages().item(m_currPage);

            // we need to clear the print object
            //
            m_paint.getPaintObjects().clear();

            createPaintObjects(page.getHeader(), C_OFFSETHEADER);
            createPaintObjects(page.getDetail(), C_OFFSETDETAIL);
            createPaintObjects(page.getFooter(), C_OFFSETFOOTER);

            if (!inPrinter)
            {
                // set the current page in the preview window
                //
                m_rpwPrint.setCurrPage(m_currPage);

                m_rpwPrint.refresh();
            }
        }

        public bool doPrint(cIPrintClient objClient)
        {
            return pDoPrint(objClient);
        }

        //----------------------------------------------------
        // cIReportPrint implementation
        //
        public bool makeReport()
        {
            return make();
        }

        public bool makeXml()
        {
            // TODO: not implemented yet
            //
            return false;
        }

        public bool previewReport()
        {
            setPreviewForm();

            pCreatePaint();

            m_rpwPrint.setPages(m_report.getPages().count());
            printPage((int)csEMoveTo.C_FIRSTPAGE, false);

            Form f = (Form)m_rpwPrint.Parent;

            if (m_bModal)
            {
                f.ShowDialog();
            }
            else
            {
                if (!m_bHidePreviewWindow)
                {
                    f.Show();
                    if (f.WindowState == FormWindowState.Minimized)
                    {
                        f.WindowState = FormWindowState.Normal;
                    }
                }
            }

            return true;
        }

        public bool cIReportPrint_PrintReport()
        {
            return pDoPrint(null);
        }

        private bool pDoPrint(cIPrintClient objClient)
        {
            try
            {
                int copies = 0;
                int q = 0;

                pCreatePaint();

                m_rePaintObject = true;

                cPrinter printer = null;

                // if the printer is not defined
                //
                if (m_report.getLaunchInfo().getPrinter() == null)
                {
                    printer = cPrintAPI.getcPrinterFromDefaultPrinter();
                }
                // we use the printer asigned by the caller
                //
                else
                {
                    printer = m_report.getLaunchInfo().getPrinter();
                }

                cReportLaunchInfo w_launchInfo = m_report.getLaunchInfo();
                copies = w_launchInfo.getCopies();
                if (w_launchInfo.getShowPrintersDialog())
                {
                    printer.setCopies(copies);
                    if (!printer.showDialog(m_report.getPages().count()))
                    {
                        return false;
                    }
                    copies = printer.getCopies();
                }
                else
                {
                    printer.getPaperInfo().setPagesToPrint("1-" + m_report.getPages().count().ToString());
                }

                for (q = 1; q <= copies; q++)
                {
                    if (!printPagesToPrinter(printer, objClient))
                    {
                        return false;
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                cError.mngError(ex, "pDoPrint", C_MODULE, "");
                return false;
            }
            finally
            {
                if (m_rpwPrint != null)
                {
                    printPage(m_currPage, false);
                    m_rpwPrint.refresh();
                }
            }
        }

        private cReportPaintObject pGetPaintObjByIndex(int indexField)
        {
            for (int _i = 0; _i < m_paint.getPaintObjects().count(); _i++)
            {
                cReportPaintObject po = m_paint.getPaintObjects().item(_i);
                if (po.getIndexField() == indexField)
                {
                    return po;
                }
            }
            return null;
        }

        private cReportPaintObject pGetPaintObjByCtrlName(
            String ctrlName,
            CSReportDll.cReportPageFields fields,
            int offset)
        {
            CSReportDll.cReportPageField fld = getFieldByCtrlName(ctrlName, fields);

            for (int _i = 0; _i < m_paint.getPaintObjects().count(); _i++)
            {
                cReportPaintObject rtn = m_paint.getPaintObjects().item(_i);
                if (fields.item(rtn.getIndexField() - offset) == fld)
                {
                    return rtn;
                }
            }
            return null;
        }

        private void pCreatePaint()
        {
            if (m_paint == null)
            {
                m_paint = new cReportPaint();
            }
            m_paint.setNotBorder(true);
        }

        private bool printPagesToPrinter(cPrinter printer, cIPrintClient objClient)
        {
            int oldZoom = 0;
            float oldScaleY = 0;
            float oldScaleX = 0;
            float oldScaleFont = 0;
            try
            {
                int i = 0;
                int[] vPages = null;
                float dpiX = 0;
                float dpiY = 0;

                CSReportDll.cReportPaperInfo w_paperInfo = m_report.getPaperInfo();
                if (!printer.starDoc(m_report.getName(),
                                        w_paperInfo.getPaperSize(),
                                        w_paperInfo.getOrientation()))
                {
                    return false;
                }

                vPages = pGetPagesToPrint(printer.getPaperInfo().getPagesToPrint());

                oldScaleX = m_paint.getScaleX();
                oldScaleY = m_paint.getScaleY();
                oldScaleFont = m_scaleFont;
                oldZoom = m_paint.getZoom();

                dpiX = printer.getGraph().DpiX;
                dpiY = printer.getGraph().DpiY;

                m_scaleX = dpiX / 100;
                m_scaleY = dpiY / 100;

                float twipsPerPixelX = 1440f / dpiX;
                int dPI = 0;
                dPI = (int)(1440f / twipsPerPixelX);

                if (dPI != 96 && dPI > 0)
                {
                    m_scaleX = m_scaleX * (96 / dPI);
                    m_scaleY = m_scaleY * (96 / dPI);
                }

                m_paint.setScaleX(m_scaleX);
                m_paint.setScaleY(m_scaleY);

                m_paint.setZoom(100);
                m_scaleFont = 1;


                for (i = 1; i <= m_report.getPages().count(); i++)
                {
                    if (pHaveToPrintThisPage(i, vPages))
                    {
                        if (!printer.starPage())
                        {
                            throw new ReportPaintException(csRptPaintErrors.CSRPTPATINTERRPRINTING,
                                                      C_MODULE,
                                                      "Ocurrio un error al imprimir el reporte."
                                                      );
                        }
                        printPage(i, true);

                        if (!drawPage(printer))
                        {
                            return false;
                        }
                        if (!printer.endPage())
                        {
                            throw new ReportPaintException(csRptPaintErrors.CSRPTPATINTERRPRINTING,
                                                      C_MODULE,
                                                      "Ocurrio un error al imprimir el reporte."
                                                      );
                        }
                        if (!pRefreshObjClient(i, objClient))
                        {
                            return false;
                        }
                    }
                }

                if (!printer.endDoc())
                {
                    throw new ReportPaintException(csRptPaintErrors.CSRPTPATINTERRPRINTING,
                                              C_MODULE,
                                              "Ocurrio un error al imprimir el reporte."
                                              );
                }

                return true;

            }
            catch (Exception ex)
            {
                cError.mngError(ex, "printPagePrinter", C_MODULE, "");
                return false;
            }
            finally
            {
                printer.endDoc();
                m_paint.setZoom(oldZoom);
                m_scaleX = oldScaleX;
                m_scaleY = oldScaleY;
                m_paint.setScaleX(oldScaleX);
                m_paint.setScaleY(oldScaleY);
                m_scaleFont = oldScaleFont;
            }
        }

        private bool pRefreshObjClient(int iPage, cIPrintClient objClient)
        { // TODO: Use of ByRef founded Private Function pRefreshObjClient(ByVal iPage As Long, ByRef ObjClient As Object) As Boolean
            if (objClient == null)
            {
                return true;
            }
            else
            {
                return objClient.printingPage(iPage);
            }
        }

        private bool pHaveToPrintThisPage(int page, int[] v)
        { // TODO: Use of ByRef founded Private Function pHaveToPrintThisPage(ByVal Page As Long, ByRef v() As Long) As Boolean
            int n = 0;

            for (n = 1; n <= v.Length; n++)
            {
                if (page == v[n])
                {
                    return true;
                }
            }
            return false;
        }

        private int[] pGetPagesToPrint(String pagesToPrint)
        {
            String[] v = null;
            int i = 0;
            int[] n = null;
            int k = 0;
            String[] v2 = null;
            int t = 0;
            int r = 0;
            bool addInterval = false;

            v = pagesToPrint.Split(',');

            G.redim(ref n, 0);

            for (i = 0; i <= v.Length; i++)
            {
                k = v[i].IndexOf("-", 1);
                if (k > 0)
                {
                    v2 = v[i].Split('-');
                    addInterval = false;
                    for (t = 0; t <= v2.Length; t++)
                    {
                        if (G.isNumeric(v2[t]))
                        {
                            if (addInterval)
                            {
                                for (r = n[n.Length - 1] + 1; r <= int.Parse(v2[t]) - 1; r++)
                                {
                                    G.redimPreserve(ref n, n.Length + 1);
                                    n[n.Length - 1] = r;
                                }
                            }
                            else
                            {
                                addInterval = true;
                            }
                            G.redimPreserve(ref n, n.Length + 1);
                            n[n.Length - 1] = int.Parse(v2[t]);
                        }
                    }
                }
                else
                {
                    if (G.isNumeric(v[i]))
                    {
                        G.redimPreserve(ref n, n.Length + 1);
                        n[n.Length - 1] = int.Parse(v[i]);
                    }
                }
            }
            return n;
        }

        public void setReport(object rhs)
        {
            m_report = (CSReportDll.cReport)rhs;
        }

        private CSReportDll.cReportPageFields pGetLineAux(int indexLine, CSReportDll.cReportPageFields fields)
        {
            CSReportDll.cReportPageFields flds = new CSReportDll.cReportPageFields();

            for (int _i = 0; _i < fields.count(); _i++)
            {
                CSReportDll.cReportPageField fld = fields.item(_i);
                if (fld.getIndexLine() == indexLine)
                {
                    flds.add(fld);
                }
            }
            return flds;
        }

        private bool make()
        {
            float detailHeight = 0;
            float lineHeight = 0;

            CSReportDll.cReportPageFields fields = null;
            CSReportDll.cReportPageField field = null;
            CSReportDll.cReportPageFields detail = null;

            csRptGetLineResult rslt;
            csRptNewPageResult rsltNewPage;
            float top = 0;
            float topSection = 0;
            float heightSection = 0;
            int secLnIndex = 0;
            float[] offsetTop = null;
            float[] vdummy = null;

            cMouseWait mouse = new cMouseWait();

            printerSetSizeAndOrient(
                m_report.getLaunchInfo().getPrinter().getDeviceName(),
                m_report.getPaperInfo().getPaperSize(),
                m_report.getPaperInfo().getOrientation());

            m_currPage = 0;

            // we create the first page
            //
            rsltNewPage = m_report.newPage();

            // if it has failed
            //
            if (rsltNewPage == csRptNewPageResult.CSRPTNPERROR)
            {
                return false;
            }

            // if there is no data
            //
            if (rsltNewPage == csRptNewPageResult.CSRPTNPEND)
            {
                return m_report.endPage() != csRptEndPageResult.CSRPTEPERROR;
            }

            // we are goin to evaluate the detail's first line
            // or group header's first line only if there are not
            // groups
            //
            if (m_report.getGroups().count() == 0)
            {
                m_report.evalPreGroupHeader();
                m_report.evalPre();
            }

            // get details dimensions
            //
            detailHeight = getDetailHeight(m_report.getPages().item(m_report.getPages().count()), top);

            // add the height of the images for controls which can grow and are in the header
            //
            getLineHeight(m_report.getPages().item(m_report.getPages().count()).getHeader(), vdummy);

            do
            {
                // get the line
                //
                rslt = m_report.getLine(fields);

                // if we have finished
                //
                if (rslt == csRptGetLineResult.CSRPTGLEND)
                {
                    break;
                }

                // if the row is virtual we need to call the engine
                // to give it a chance to evalute formulas in the
                // header which are marked to be compiled before printing
                //
                if (rslt == csRptGetLineResult.CSRPTGLVIRTUALH)
                {

                    m_report.evalPreGroupHeader();

                    // idem for footers
                    //
                }
                else if (rslt == csRptGetLineResult.CSRPTGLVIRTUALF)
                {

                    m_report.evalPreGroupFooter();

                    // if the engine responded that we need to create a new page
                    //
                }
                else if (rslt == csRptGetLineResult.CSRPTGLNEWPAGE)
                {
                    // get the new page
                    //
                    if (!pNewPage(top, detailHeight))
                    {
                        return false;
                    }
                }
                else
                {
                    // get the line's height
                    //
                    lineHeight = getLineHeight(fields, offsetTop);

                    // if it can fit we create a new page
                    //
                    if (lineHeight > detailHeight)
                    {

                        // get the new page
                        //
                        if (!pNewPage(top, detailHeight))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        heightSection = 0;
                        topSection = 0;
                        secLnIndex = 0;

                        //---------------------------------------------------------------------------------
                        // this code is here and not in a function because we want to improve the
                        // speed when run the report
                        //
                        // add the line to the page
                        //
                        detail = m_report.getPages().item(m_report.getPages().count()).getDetail();

                        for (int _i = 0; _i < fields.count(); _i++)
                        {
                            field = fields.item(_i);

                            // get the field's top
                            //
                            CSReportDll.cReportSectionLine w_sectionLine = field.getInfo().getSectionLine();

                            // one time for section
                            //
                            if (secLnIndex != w_sectionLine.getIndex())
                            {
                                secLnIndex = w_sectionLine.getIndex();
                                CSReportDll.cReportAspect w_aspect = w_sectionLine.getAspect();
                                topSection = topSection + (w_aspect.getTop() - (topSection + heightSection));
                                heightSection = heightSection + w_aspect.getHeight();
                            }

                            field.setTop(top
                                            + offsetTop[secLnIndex]
                                            + (field.getInfo().getAspect().getTop()
                                            - topSection));

                            detail.add(field);
                        }
                        //---------------------------------------------------------------------------------

                        // get the detail's height
                        //
                        top = top + lineHeight;
                        detailHeight = detailHeight - lineHeight;

                        // notify the engine about the groups' staste
                        //
                        if (rslt == csRptGetLineResult.CSRPTGLGROUPHEADER)
                        {
                            m_report.markGroupHeaderPrinted();

                            // evaluate every function which are mark 
                            // to be printed after printing
                            //
                            m_report.evalPostGroupHeader();

                        }
                        else if (rslt == csRptGetLineResult.CSRPTGLGROUPFOOTER)
                        {
                            m_report.markGroupFooterPrinted();

                            // evaluate every function which are mark 
                            // to be printed after printing
                            //
                            m_report.evalPostGroupFooter();

                        }
                        else if (rslt == csRptGetLineResult.CSRPTGLDETAIL)
                        {
                            m_report.evalPost();
                            m_report.moveToNext();
                        }
                        if (m_report.getLineType() == csRptGetLineResult.CSRPTGLDETAIL)
                        {
                            m_report.evalPre();
                        }
                    }
                }
            } while (true);

            return m_report.endPage() != csRptEndPageResult.CSRPTEPERROR;
        }

        private void printerSetSizeAndOrient(string p, csReportPaperType csReportPaperType, int p_2)
        {
            throw new NotImplementedException();
        }

        private bool pNewPage(float top, float detailHeight)
        { // TODO: Use of ByRef founded Private Function pNewPage(ByRef Top As Single, ByRef DetailHeight As Single) As Boolean
            csRptNewPageResult rsltNewPage;
            csRptEndPageResult rsltEndPage;

            rsltEndPage = m_report.endPage();
            if (rsltEndPage == csRptEndPageResult.CSRPTEPERROR)
            {
                return false;
            }

            rsltNewPage = m_report.newPage();
            if (rsltNewPage == csRptNewPageResult.CSRPTNPERROR)
            {
                return false;
            }

            // get details' dimentions
            //
            detailHeight = getDetailHeight(m_report.getPages().item(m_report.getPages().count()), top);

            return true;
        }

        // returns details' height of this page
        //
        private float getDetailHeight(CSReportDll.cReportPage page, float top)
        { // TODO: Use of ByRef founded Private Function GetDetailHeight(ByRef Page As cReportPage, ByRef Top As Single) As Single
            top = page.getHeaderBottom();
            return page.getFooterTop() - top;
        }

        // returns the bigger control's height and set the height of every control
        //
        private float getLineHeight(CSReportDll.cReportPageFields fields, float[] offsetTop)
        { // TODO: Use of ByRef founded Private Function GetLineHeight(ByRef Fields As cReportPageFields, ByRef OffsetTop() As Single) As Single
            CSReportDll.cReportPageField field = null;
            float offBottom = 0;
            float aspectHeight = 0;
            float aspectWidth = 0;

            CSReportDll.cReportAspect aspect = null;
            CSReportDll.cReportAspect aspectLn = null;

            // used to get the offset to top
            //
            float lnHeight = 0;

            // used to increase the height of the line
            //
            float lnHeight2 = 0;
            float newLnHeight = 0;

            Font font = null;

            float topSection = 0;
            int indexSection = 0;
            float heightSection = 0;

            G.redim(ref offsetTop, 0);

            if (fields.count() > 0)
            {

                // search for the highest control
                //
                for (int _i = 0; _i < fields.count(); _i++)
                {
                    field = fields.item(_i);

                    // if it can grow we need to get its height to be
                    // able to print it
                    //
                    aspect = field.getInfo().getAspect();
                    aspectLn = field.getInfo().getSectionLine().getAspect();

                    // if the line has changed we need to get the height of the line
                    // and add it to heightSection
                    //
                    if (indexSection != field.getInfo().getSectionLine().getIndex())
                    {

                        // save a reference to this section
                        //
                        indexSection = field.getInfo().getSectionLine().getIndex();

                        if (offsetTop.Length < indexSection)
                        {
                            G.redimPreserve(ref offsetTop, indexSection);
                        }

                        // save this offset to add it to every control holded in the
                        // section lines which are under the current section line
                        //
                        offsetTop[indexSection] = offsetTop[indexSection] + newLnHeight - lnHeight;

                        // we get the top of the current line which includes only
                        // the height of visible lines
                        //
                        topSection = topSection + (aspectLn.getTop() - (topSection + heightSection));

                        // add to heightSection the height of this line
                        //
                        heightSection = heightSection + aspectLn.getHeight();

                        // save the height of the line to know if it has changed for canGrow
                        //
                        lnHeight = aspectLn.getHeight();

                        // the height of the original section line
                        //
                        lnHeight2 = lnHeight;

                        // save the height of the line to analize canGrow
                        //
                        newLnHeight = lnHeight;
                    }

                    // add to every control the offset produced by controls which
                    // can grow
                    //
                    if (aspect.getCanGrow())
                    {

                        aspectHeight = aspect.getHeight();
                        aspectWidth = aspect.getWidth();

                        // if there is an image we need to get its height
                        //
                        if (field.getImage() != null)
                        {

                            int imgWidth = 0;
                            int imgHeight = 0;

                            cGlobals.getBitmapSize(field.getImage(), out imgWidth, out imgHeight, true);

                            field.setHeight(imgHeight);
                            field.setWidth(imgWidth);

                            if (field.getHeight() < aspectHeight) { field.setHeight(aspectHeight); }
                            if (field.getWidth() < aspectWidth) { field.setWidth(aspectWidth); }
                        }
                        else
                        {
                            if (field.getValue() != "")
                            {
                                int flags = 0;

                                // TODO: flags to get height of text to be drawn
                                if (aspect.getWordWrap())
                                {
                                    flags = 0/*ECGTextAlignFlags.DT_WORDBREAK
                                            || ECGTextAlignFlags.DT_WORD_ELLIPSIS
                                            || ECGTextAlignFlags.DT_LEFT
                                            || ECGTextAlignFlags.DT_NOPREFIX
                                            || ECGTextAlignFlags.DT_EDITCONTROL*/;
                                }
                                else
                                {
                                    flags = 0/*ECGTextAlignFlags.DT_SINGLELINE
                                            || ECGTextAlignFlags.DT_WORD_ELLIPSIS
                                            || ECGTextAlignFlags.DT_LEFT
                                            || ECGTextAlignFlags.DT_NOPREFIX*/;
                                }

                                CSReportDll.cReportFont w_font = aspect.getFont();
                                FontStyle style = new FontStyle();
                                if (w_font.getBold())
                                {
                                    style = style | FontStyle.Bold;
                                }
                                if (w_font.getItalic())
                                {
                                    style = style | FontStyle.Italic;
                                }
                                if (w_font.getUnderline())
                                {
                                    style = style | FontStyle.Underline;
                                }
                                font = new Font(w_font.getName(), w_font.getSize(), style);

                                //TODO: evaluate height
                                //field.setHeight(mAux.evaluateTextHeight(field.getValue(), m_hFnt[mAux.addFontIfRequired(font, m_hDC, m_iFontCount, m_fnt, m_hFnt)], aspect.getWidth(), m_hDC, flags, m_scaleY, m_scaleX));
                                if (field.getHeight() < aspectHeight) { field.setHeight(aspectHeight); }
                            }
                        }

                        // if it doesn't fit in the line because is to much high
                        //
                        if (field.getHeight() + aspect.getTop() > topSection + newLnHeight)
                        {
                            offBottom = (topSection + newLnHeight) - (aspect.getTop() + aspectHeight);

                            // to separete a little
                            //
                            if (offBottom < 0) { offBottom = offBottom + 80; }

                            // new line's height 
                            //
                            newLnHeight = aspect.getTop() - topSection + field.getHeight() + offBottom;
                        }

                        // if the height of the previous line has changed because 
                        // some control has set to canGrow = true and its value
                        // makes the control's height to change, we need to add
                        // this height to heightSection
                        //
                        if (newLnHeight > lnHeight2)
                        {
                            //                                substract the original height
                            //                                |         add the hieght for canGrow
                            //                                |             |
                            heightSection = heightSection - lnHeight2 + newLnHeight;
                            lnHeight2 = newLnHeight;
                        }
                    }
                    else
                    {
                        field.setHeight(aspect.getHeight());
                    }
                }
            }

            // return the height of the section
            //
            return heightSection;
        }

        // if the caller hasn't assigned a preview object
        // we use the internal preview object
        //
        private void setPreviewForm()
        {
            if (m_rpwPrint == null)
            {
                if (m_fPreview == null)
                {
                    m_fPreview = new fPreview();
                }
                m_rpwPrint = m_fPreview.getRpwReport();
            }
            else
            {
                if (m_rpwPrint.Parent != null)
                {
                    if (!(m_rpwPrint.Parent.GetType() == typeof(Form)))
                    {
                        m_fPreview = new fPreview();
                        m_rpwPrint = m_fPreview.getRpwReport();
                    }
                }
                else
                {
                    m_fPreview = new fPreview();
                    m_rpwPrint = m_fPreview.getRpwReport();
                }
            }

            RectangleF tR;

            cPrinter w_printer = m_report.getLaunchInfo().getPrinter();
            tR = cGlobals.getRectFromPaperSize(w_printer.getPaperInfo(), w_printer.getPaperInfo().getPaperSize(), w_printer.getPaperInfo().getOrientation());

            m_realWidth = (int)tR.Width;
            m_realHeight = (int)tR.Height;

            m_rpwPrint.getBody().Width = (int)m_realWidth;
            m_rpwPrint.getBody().Height = (int)m_realHeight;

            if (!m_bModal)
            {
                if (!m_bHidePreviewWindow)
                {
                    var obj = m_rpwPrint.getParent();
                    if (obj.GetType() == typeof(Form)) 
                    {
                        Form f = obj as Form;
                        f.Show();
                    }
                }
            }
        }

        private void createPaintObjects(CSReportDll.cReportPageFields fields, int offset)
        { // TODO: Use of ByRef founded Private Sub CreatePaintObjects(ByRef Fields As cReportPageFields, ByVal Offset As Long)
            CSReportDll.cReportPageField field = null;

            CSReportDll.cReportAspect rptAspect = null;
            CSReportDll.cReportFont rptFont = null;

            int index = 0;

            for (int _i = 0; _i < fields.count(); _i++)
            {
                field = fields.item(_i);
                index = index + 1;

                if (field.getVisible())
                {

                    rptAspect = field.getInfo().getAspect();

                    cReportPaintObject w_add = m_paint.getPaintObjects().add(null, "");
                    CSReportDll.cReportAspect w_aspect = w_add.getAspect();
                    if (field.getTop() > 0)
                    {
                        w_aspect.setTop(field.getTop());
                    }
                    else
                    {
                        w_aspect.setTop(rptAspect.getTop());
                    }
                    if (field.getHeight() > 0)
                    {
                        w_aspect.setHeight(field.getHeight());
                    }
                    else
                    {
                        w_aspect.setHeight(rptAspect.getHeight());
                    }
                    if (field.getWidth() > 0)
                    {
                        w_aspect.setWidth(field.getWidth());
                    }
                    else
                    {
                        w_aspect.setWidth(rptAspect.getWidth());
                    }
                    w_aspect.setLeft(rptAspect.getLeft());
                    w_aspect.setBackColor(rptAspect.getBackColor());
                    w_aspect.setTransparent(rptAspect.getTransparent());
                    w_aspect.setAlign(rptAspect.getAlign());
                    w_aspect.setWordWrap(rptAspect.getWordWrap());

                    w_aspect.setBorderColor(rptAspect.getBorderColor());
                    w_aspect.setBorderColor3d(rptAspect.getBorderColor3d());
                    w_aspect.setBorderColor3dShadow(rptAspect.getBorderColor3dShadow());
                    w_aspect.setBorderRounded(rptAspect.getBorderRounded());
                    w_aspect.setBorderType(rptAspect.getBorderType());
                    w_aspect.setBorderWidth(rptAspect.getBorderWidth());

                    rptFont = rptAspect.getFont();
                    CSReportDll.cReportFont w_font = w_aspect.getFont();
                    w_font.setBold(rptFont.getBold());
                    w_font.setForeColor(rptFont.getForeColor());
                    w_font.setItalic(rptFont.getItalic());
                    w_font.setName(rptFont.getName());
                    w_font.setSize(rptFont.getSize() * m_scaleFont);
                    w_font.setStrike(rptFont.getStrike());
					w_font.setUnderline(rptFont.getUnderline());

                    w_add.setText(field.getValue());
                    w_add.setImage(field.getImage());

                    if (w_add.getImage() != null)
                    {
                        w_add.setPaintType(csRptPaintObjType.CSRPTPAINTOBJIMAGE);
                    }
                    w_add.setIndexField(index + offset);
                    if (field.getObjectID() != null)
                    {
                        w_add.setTag(field.getObjectID().getValue());
                    }
                }
            }
        }

        // TODO: see how to implement this functionality
        //
        private void m_fPreview_FormUnload()
        {
            m_rpwPrint = null;
            m_report.getLaunchInfo().getObjPaint().setReport(null);
            m_report.getLaunchInfo().setObjPaint(null);
        }

        //------------------------------------------------------------------
        // preview events
        //
        private void m_rpwPrint_BodyDblClick()
        {
            /*
            try {

                String sKey = "";

                if (m_paint == null) { return; }
                if (m_paint.pointIsInObject(m_x, m_y, sKey)) {
                    Iterator listeners = m_listeners.iterator();
                    while(listeners.hasNext()) {
                        ((cReportPrintEventI)listeners.next()).dblClickOnField(m_paint.getPaintObjects(sKey).IndexField);
                    }
                }

                //*TODO:** goto found: GoTo ExitProc;
            } catch (Exception ex) {
                cError.mngError(ex, "m_rpwPrint_BodyDblClick", C_MODULE, "");
                if (VBA.ex.Number) { /**TODO:** resume found: Resume(ExitProc)* / }
                //*TODO:** label found: ExitProc:;
        
            }
        */
        }

        private bool pGetFieldFromIndexAux(CSReportDll.cReportPageFields fields, int index, ref CSReportDll.cReportPageField rtn)
        { // TODO: Use of ByRef founded Private Function pGetFieldFromIndexAux(ByRef Fields As cReportPageFields, ByVal Index As Long, ByRef rtn As cReportPageField) As Boolean
            try
            {
                rtn = fields.item(index);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private void m_rpwPrint_BodyMouseDown(int button, int shift, float x, float y)
        { // TODO: Use of ByRef founded Private Sub m_rpwPrint_BodyMouseDown(ByRef Button As Integer, ByRef Shift As Integer, ByRef x As Single, ByRef y As Single)
            /*
            try {
                String sKey = "";

                if (m_paint == null) { return; }

                if (m_paint.pointIsInObject(x, y, sKey)) {
                    int index = 0;
                    index = m_paint.getPaintObjects(sKey).IndexField;

                    bool cancel = null;
                    Iterator listeners = m_listeners.iterator();
                    while(listeners.hasNext()) {
                        ((cReportPrintEventI)listeners.next()).mouseDownOnField(index, button, shift, cancel, x, y);
                    }

                    if (!cancel) {
                        Iterator listeners = m_listeners.iterator();
                        while(listeners.hasNext()) {
                            ((cReportPrintEventI)listeners.next()).clickOnField(index);
                        }
                    }
                }


                //*TODO:** goto found: GoTo ExitProc;
            } catch (Exception ex) {
                cError.mngError(ex, "m_rpwPrint_BodyMouseDown", C_MODULE, "");
                if (VBA.ex.Number) { /**TODO:** resume found: Resume(ExitProc)* / }
                //*TODO:** label found: ExitProc:;
        
            }
        */
        }

        private void m_rpwPrint_BodyMouseMove(int button, int shift, float x, float y)
        {
            /*
            try {

                String sKey = "";
                int indexField = 0;

                if (m_paint == null) { return; }

                if (m_paint.pointIsInObject(x, y, sKey)) {
                    indexField = m_paint.getPaintObjects(sKey).IndexField;
                    if (m_lastIndexField != indexField) {
                        Iterator listeners = m_listeners.iterator();
                        while(listeners.hasNext()) {
                            ((cReportPrintEventI)listeners.next()).mouseOnField(indexField);
                        }
                        m_lastIndexField = indexField;
                    }
                } 
                else {
                    if (m_lastIndexField) {
                        Iterator listeners = m_listeners.iterator();
                        while(listeners.hasNext()) {
                            ((cReportPrintEventI)listeners.next()).mouseOutField();
                        }
                        m_lastIndexField = 0;
                    }
                }

                m_x = x;
                m_y = y;

                //*TODO:** goto found: GoTo ExitProc;
            } catch (Exception ex) {
                cError.mngError(ex, "m_rpwPrint_BodyMouseMove", C_MODULE, "");
                if (VBA.ex.Number) { /**TODO:** resume found: Resume(ExitProc)* / }
                //*TODO:** label found: ExitProc:;
        
            }
        */
        }

        private void m_rpwPrint_BodyPaint()
        {
            if (m_paint == null) { return; }
            drawPage(m_rpwPrint.getBody());
        }

        private void m_rpwPrint_ChangeZoom(int zoom)
        {
            float nZoom = 0;
            float width = 0;
            float height = 0;

            switch (zoom)
            {
                case (int)csEZoom.csEZoomAllPage:

                    width = m_rpwPrint.Width / m_realWidth;
                    height = m_rpwPrint.Height / m_realHeight;

                    if (width < height)
                    {
                        nZoom = m_rpwPrint.Width / m_realWidth;
                    }
                    else
                    {
                        nZoom = m_rpwPrint.Height / m_realHeight;
                    }

                    break;
                case (int)csEZoom.csEZoomCustom:
                    nZoom = 1;
                    break;
                case (int)csEZoom.csEZoomWidth:
                    nZoom = m_rpwPrint.Width / m_realWidth;
                    break;
                default:
                    nZoom = zoom / 100;
                    break;
            }

            if (nZoom < 0.01) { nZoom = 0.01f; }

            PictureBox pic = m_rpwPrint.getBody();
            pic.Width = (int)(m_realWidth * nZoom);
            pic.Height = (int)(m_realHeight * nZoom);

            if (nZoom > 0.5)
            {
                m_paint.setZoom(100);
                m_paint.setScaleX(nZoom);
                m_paint.setScaleY(nZoom);
                m_scaleFont = nZoom;
                printPage(m_currPage);
            }
            else
            {
                m_paint.setZoom(zoom);
                m_rpwPrint.refresh();
            }
        }

        private void m_rpwPrint_DoPrint()
        {
            cIReportPrint_PrintReport();
        }

        /*TODO: we need to decide if it is useful
         * 
            private void m_rpwPrint_ExportExcel() {
                try {

                    cMouseWait mouse = new cMouseWait();

                    Application.DoEvents();

                    CSReportExport.cReportExcel expExcel = null;
                    expExcel = new CSReportExport.cReportExcel();

                    expExcel.export(m_report);

                } catch (Exception ex) {
                    cError.mngError(ex, "m_rpwPrint_ExportExcel", C_MODULE, "");
                }
            }
        */
        private void m_rpwPrint_ExportPDF()
        {
            exportPDF();
        }

        // Files is a list of file names separated by |
        //
        public object sendMail(String files, String emailAddress)
        {
            cReportPdf expPDF = null;
            expPDF = new cReportPdf();

            expPDF.setExportEmailAddress(emailAddress);
            return expPDF.sendMail(files);
        }

        public bool exportPDFEx(String outputFile, bool bShowPDFWindow)
        { // TODO: Use of ByRef founded Public Function ExportPDFEx(ByRef OutputFile As String, ByVal bShowPDFWindow As Boolean) As Boolean
            return pExportPDF(outputFile, bShowPDFWindow);
        }

        public bool exportPDF()
        {
            return pExportPDF("", true);
        }

        private String pGetExportFileName()
        {
            if (m_exportFileName != "")
            {
                return m_exportFileName;
            }
            else
            {
                return m_report.getName();
            }
        }

        private bool pExportPDF(String outputFile, bool bShowPDFWindow)
        { // TODO: Use of ByRef founded Private Function pExportPDF(ByRef OutputFile As String, ByVal bShowPDFWindow As Boolean) As Boolean
            try
            {
                cMouseWait mouse = new cMouseWait();

                Application.DoEvents();

                CSReportExport.cReportPdf expPDF = null;
                expPDF = new CSReportExport.cReportPdf();

                expPDF.setFileName(cUtil.getValidPath(System.Environment.GetEnvironmentVariable("TEMP")) + pGetExportFileName());
                expPDF.setExportEmailAddress(m_report.getExportEmailAddress());

                return expPDF.exportEx(m_report, this, outputFile, bShowPDFWindow);

            }
            catch (Exception ex)
            {
                cError.mngError(ex, "pExportPDF", C_MODULE, "");
                return false;
            }
        }

        /* We need to decide if it is useful
         * 
            private void m_rpwPrint_ExportWord() {
                try {
                    cMouseWait mouse = new cMouseWait();

                    CSReportExport.cReportWord expWord = null;
                    expWord = new CSReportExport.cReportWord();

                    expWord.export(m_report);

                } catch (Exception ex) {
                    cError.mngError(ex, "m_rpwPrint_ExportWord", C_MODULE, "");
                }
            }
        */
        private void m_rpwPrint_MoveFirst()
        {
            printPage((int)csEMoveTo.C_FIRSTPAGE);
        }

        private void m_rpwPrint_MoveLast()
        {
            printPage((int)csEMoveTo.C_LASTPAGE);
        }

        private void m_rpwPrint_MoveNext()
        {
            printPage((int)csEMoveTo.C_NEXTPAGE);
        }

        private void m_rpwPrint_MovePrevious()
        {
            printPage((int)csEMoveTo.C_PREVIOUSPAGE);
        }

        private void m_rpwPrint_MoveToPage(int page)
        {
            printPage(page);
        }

        private void m_rpwPrint_SaveDocument()
        {
            //If Not m_Report.SaveData(m_rpwPrint.cmFileSaveDialog) Then Exit Sub
        }

        private bool drawPage(object graph)
        {
            int i = 0;

            if (m_rePaintObject)
            {
                m_paint.clearPage(graph);

                if (graph.GetType() == typeof(cPrinter))
                {
                    for (i = 1; i <= m_paint.getPaintObjects().count(); i++)
                    {
                        if (!m_paint.drawObject(m_paint.getPaintObjects().getNextKeyForZOrder(i), (graph as cPrinter).getGraph())) { return false; }
                    }

                    for (i = 1; i <= m_paint.getPaintSections().count(); i++)
                    {
                        if (!m_paint.drawSection(m_paint.getPaintSections().getNextKeyForZOrder(i), (graph as cPrinter).getGraph())) { return false; }
                    }
                }
                else
                {
                    m_rePaintObject = false;
                }
            }
            else
            {
                m_paint.paintPicture(graph as Graphics);
            }
            return true;
        }

        private void pDestroyFonts()
        {
            /*
            int iFnt = 0;
            for (iFnt = 1; iFnt <= m_iFontCount; iFnt++)
            {
                DeleteObject(m_hFnt[iFnt]);
            }
            G.redim(m_fnt, 0);
            G.redim(m_hFnt, 0);
             */
        }

        public void Dispose()
        {
            m_report = null;
            m_paint = null;
            if (m_fPreview != null)
            {
                m_fPreview.Dispose();
            }
            m_rpwPrint = null;
        }

        public bool printReport()
        {
            return pDoPrint(null);
        }

    }
}
