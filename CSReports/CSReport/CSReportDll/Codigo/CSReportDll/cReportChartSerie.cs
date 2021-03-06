﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportDll
{
    public class cReportChartSerie
    {

        private const String C_MODULE = "cReportChartSerie";

        private String m_valueFieldName = "";
        private String m_labelFieldName = "";
        private csColors m_color = csColors.ALICEBLUE;
        private int m_valueIndex = 0;
        private int m_labelIndex = 0;

        public String getValueFieldName()
        {
            return m_valueFieldName;
        }

        public void setValueFieldName(String rhs)
        {
            m_valueFieldName = rhs;
        }

        public String getLabelFieldName()
        {
            return m_labelFieldName;
        }

        public void setLabelFieldName(String rhs)
        {
            m_labelFieldName = rhs;
        }

        public csColors getColor()
        {
            return m_color;
        }

        public void setColor(csColors value)
        {
            m_color = value;
        }

        public int getValueIndex()
        {
            return m_valueIndex;
        }

        public void setValueIndex(int rhs)
        {
            m_valueIndex = rhs;
        }

        public int getLabelIndex()
        {
            return m_labelIndex;
        }

        public void setLabelIndex(int rhs)
        {
            m_labelIndex = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj, int index)
        {
            try { m_valueFieldName = xDoc.getNodeProperty(nodeObj, "ValueFieldName").getValueString(eTypes.eText); }
            catch { }
            try { m_labelFieldName = xDoc.getNodeProperty(nodeObj, "LabelFieldName").getValueString(eTypes.eText); }
            catch { }
            try { m_color = (csColors)xDoc.getNodeProperty(nodeObj, "Color").getValueInt(eTypes.eLong); }
            catch { }

            return true;
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather, int index)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("Serie_" + index.ToString());
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("ValueFieldName");
            xProperty.setValue(eTypes.eText, m_valueFieldName);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("LabelFieldName");
            xProperty.setValue(eTypes.eText, m_labelFieldName);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Color");
            xProperty.setValue(eTypes.eLong, m_color);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            return true;
        }

    }

}
