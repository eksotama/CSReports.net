﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportDll
{
    public class cReportConnectsAux : NameObjectCollectionBase
    {

        private const String C_MODULE = "cReportConnectsAux";
        private const String C_RPTCONNECTSAUX = "RptConnectsAux";

        // Creates an empty collection.
        public cReportConnectsAux()
        {
        }

        // Adds elements from an IDictionary into the new collection.
        public cReportConnectsAux(IDictionary d, Boolean bReadOnly)
        {
            foreach (DictionaryEntry de in d)
            {
                this.BaseAdd((String)de.Key, de.Value);
            }
            this.IsReadOnly = bReadOnly;
        }

        // Gets a key-and-value pair (DictionaryEntry) using an index.
        public DictionaryEntry this[int index]
        {
            get
            {
                return (new DictionaryEntry(
                    this.BaseGetKey(index), this.BaseGet(index)));
            }
        }

        // Gets or sets the value associated with the specified key.
        public Object this[String key]
        {
            get
            {
                return (this.BaseGet(key));
            }
            set
            {
                this.BaseSet(key, value);
            }
        }

        // Gets a String array that contains all the keys in the collection.
        public String[] AllKeys
        {
            get
            {
                return (this.BaseGetAllKeys());
            }
        }

        // Gets an Object array that contains all the values in the collection.
        public Array AllValues
        {
            get
            {
                return (this.BaseGetAllValues());
            }
        }

        // Gets a String array that contains all the values in the collection.
        public String[] AllStringValues
        {
            get
            {
                return ((String[])this.BaseGetAllValues(typeof(String)));
            }
        }

        // Gets a value indicating if the collection contains keys that are not null.
        public Boolean HasKeys
        {
            get
            {
                return (this.BaseHasKeys());
            }
        }

        // Adds an entry to the collection.
        public void Add(String key, Object value)
        {
            this.BaseAdd(key, value);
        }

        // Removes an entry with the specified key from the collection.
        public void Remove(String key)
        {
            this.BaseRemove(key);
        }

        // Removes an entry in the specified index from the collection.
        public void Remove(int index)
        {
            this.BaseRemoveAt(index);
        }

        // Clears all the elements in the collection.
        public void Clear()
        {
            this.BaseClear();
        }

        // Removes an entry with the specified key from the collection.
        public void remove(String key)
        {
            this.BaseRemove(key);
        }

        // Removes an entry in the specified index from the collection.
        public void remove(int index)
        {
            this.BaseRemoveAt(index);
        }

        // Clears all the elements in the collection.
        public void clear()
        {
            this.BaseClear();
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            cReportConnect connect = null;

            if (nodeFather == null)
            {
                CSXml.cXmlProperty xProperty = null;
                xProperty = new CSXml.cXmlProperty();
                xProperty.setName(C_RPTCONNECTSAUX);
                nodeFather = xDoc.addNode(xProperty);
            }

            for (int _i = 0; _i < this.Count; _i++)
            {
                connect = item(_i);
                if (!connect.save(xDoc, nodeFather)) 
                { 
                    return false; 
                }
            }

            return true;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeFather)
        { 
            XmlNode nodeObj = null;
            clear();
            if (nodeFather != null)
            {
                if (xDoc.nodeHasChild(nodeFather))
                {
                    nodeObj = xDoc.getNodeChild(nodeFather);
                    while (nodeObj != null)
                    {
                        if (!add(null, "").load(xDoc, nodeObj)) 
                        { 
                            return false; 
                        }
                        nodeObj = xDoc.getNextNode(nodeObj);
                    }
                }
            }

            return true;
        }

		public cReportConnect add(cReportConnect c) 
		{
			return add (c, null);
		}

        public cReportConnect add(cReportConnect c, String key)
        {
            try
            {
                if (c == null) 
                { 
                    c = new cReportConnect(); 
                }

                if (key == null)
                {
                    this.BaseAdd(getDummyKey(), c);
                }
                else
                {
                    Add(cReportGlobals.getKey(key), c);
                }

                return c;
            }
            catch
            {
                return null;
            }
        }

        public int count()
        {
            return this.Count;
        }

        public cReportConnect item(String key)
        {
            try
            {
                return (cReportConnect)this.BaseGet(key);
            }
            catch
            {
                return null;
            }
        }

        public cReportConnect item(int index)
        {
            try
            {
                return (cReportConnect)this.BaseGet(index);
            }
            catch
            {
                return null;
            }
        }

        private String getDummyKey()
        {
            return "dummy_key_" + this.Count.ToString();
        }

    }

}
