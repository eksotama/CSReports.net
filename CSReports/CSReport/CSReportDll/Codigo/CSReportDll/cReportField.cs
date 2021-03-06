﻿using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSKernelClient;

namespace CSReportDll
{
    public class cReportField
    {

        private String m_name = "";
        private int m_index = 0;
        private int m_fieldType = 0;

        public String getName()
        {
            return m_name;
        }

        public void setName(String rhs)
        {
            m_name = rhs;
        }

        public int getIndex()
        {
            return m_index;
        }

        public void setIndex(int rhs)
        {
            m_index = rhs;
        }

        public int getFieldType()
        {
            return m_fieldType;
        }

        public void setFieldType(int rhs)
        {
            m_fieldType = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            nodeObj = xDoc.getNodeFromNode(nodeObj, "Field");
            m_index = xDoc.getNodeProperty(nodeObj, "Index").getValueInt(eTypes.eInteger);
            m_name = xDoc.getNodeProperty(nodeObj, "Name").getValueString(eTypes.eText);
            m_fieldType = xDoc.getNodeProperty(nodeObj, "FieldType").getValueInt(eTypes.eInteger);
            return true;
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;

            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("Field");
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("Index");
            xProperty.setValue(eTypes.eInteger, m_index);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Name");
            xProperty.setValue(eTypes.eText, m_name);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("FieldType");
            xProperty.setValue(eTypes.eInteger, m_fieldType);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            return true;
        }

    }

}
