﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSReportPaint
{
    class cReportPaintError
    {
        public static String errGetDescript(csRptPaintErrors rptErrCode)
        {
            switch (rptErrCode)
            {
                case csRptPaintErrors.CSRPT_PAINT_ERR_OBJ_CLIENT:
                    return "The ObjectClient property of the object cReportPaint is not defined when calling to DrawObject method.";
                case csRptPaintErrors.CSRPT_PAINT_ERR_OBJ_CLIENT_INVALID:
                    return "The ObjectClient property of the object cReportPaint is invalid (it references an object which is neither a Printer nor a PictureBox) when calling DrawObject method.";
                default:
                    return "There is not information for this error";
            }
        }
    }

    public enum csRptPaintErrors
    {
        CSRPT_PAINT_ERR_OBJ_CLIENT = 2001,
        CSRPT_PAINT_ERR_OBJ_CLIENT_INVALID,
        CSRPT_PAINT_ERR_PRINTING
    }
}
