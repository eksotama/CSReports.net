﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSKernelClient;

namespace CSReportPaint
{
    public partial class fPreview : Form
    {
        public fPreview()
        {
            InitializeComponent();
        }

        public CSReportPreview.cReportPreview getRpwReport() 
        {
            return rpwReport;
        }

        private void fPreview_Load(object sender, EventArgs e)
        {
            this.Height = 950;
            this.Width = 850;
            cWindow.centerForm(this);
        }
    }
}
