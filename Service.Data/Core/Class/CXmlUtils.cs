using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace Service.Data.Core.Class
{
    public class CXmlUtils
    {
        public static string RemoveDuplicateAttribute(string xml, string Node, string Attribute)
        {
            int beginNodeIndex = xml.IndexOf("<"+ Node);
            int endNodeIndex = xml.IndexOf("/>", beginNodeIndex + 1);
            //Neu khong ton tai node can tim
            if (beginNodeIndex < 0 || endNodeIndex < 0 || endNodeIndex <= beginNodeIndex) return xml;
            
            //xu ly dong bo xml
            xml = xml.Replace("'","\"");
            xml = xml.Replace(" =\"","=\"");

            Attribute = Attribute +"=\"";
            //Tim Atribute trong node
            int firstAttrIndex = xml.IndexOf(Attribute, beginNodeIndex);
            //Neu khong co attr 
            if (firstAttrIndex <= beginNodeIndex || firstAttrIndex >= endNodeIndex) return xml;
            //tim attr thu 2
            int secondAttrIndex = xml.IndexOf(Attribute, firstAttrIndex + 1);
            //Neu khong ton tai attr thu 2 
            if (secondAttrIndex <= beginNodeIndex || secondAttrIndex >= endNodeIndex) return xml;

            int lenght = xml.IndexOf("\"", secondAttrIndex + Attribute.Length + 1);

            xml = xml.Remove(secondAttrIndex, lenght - secondAttrIndex + 1);

            return xml;
        }

        public static string GetXmlNodeValue(string xmlString, string xpath)
        {

            if (string.IsNullOrEmpty(xmlString))
            {
                return null;
            }
            XmlDocument doc = new XmlDocument();

            try
            {
                string localXml = "<Root>" + xmlString + "</Root>";
                xpath = "Root/" + xpath;
                doc.LoadXml(localXml);
                return doc.SelectSingleNode(xpath).Value;

            }
            catch
            {
                return null;
            }
            finally
            {
                doc = null;
            }
        }

        public static string AddXmlAttribute(string xmlString, string xpath, string attrName, string value)
        {

            if (string.IsNullOrEmpty(xmlString))
            {
                return null;
            }
            XmlDocument doc = new XmlDocument();

            try
            {
                string localXml = "<Root>" + xmlString + "</Root>";
                xpath = "Root/" + xpath;
                doc.LoadXml(localXml);
                XmlNodeList nodes = doc.SelectNodes(xpath);
                if (nodes.Count > 0)
                {
                    XmlElement element = (XmlElement)nodes[0];
                    element.SetAttribute(attrName, value);
                }
                XmlElement root = doc.DocumentElement;

                return root.InnerXml;

            }
            catch
            {
                return null;
            }
            finally
            {
                doc = null;
            }
        }
        public static string Query(string xmlString, string xpath)
        {

            if (string.IsNullOrEmpty(xmlString))
            {
                return null;
            }
            XmlDocument doc = new XmlDocument();

            try
            {
                string localXml = "<Root>" + xmlString + "</Root>";
                //xpath = "Root/" + xpath;
                doc.LoadXml(localXml);
                string result = "";
                string[] xpathSubs = xpath.Split(',');
                foreach (string sub in xpathSubs)
                {
                    string xpathSub = "Root/" + sub;
                    XmlNodeList nodes = doc.SelectNodes(xpathSub);

                    foreach (XmlNode node in nodes)
                    {
                        result += node.OuterXml;
                    }
                }
                return result;
            }
            catch
            {
                return "";
            }
            finally
            {
                doc = null;
            }
        }

        public static List<XmlNode> QueryXmlNodes(string xmlString, string xpath)
        {

            if (string.IsNullOrEmpty(xmlString))
            {
                return null;
            }
            XmlDocument doc = new XmlDocument();

            try
            {
                string localXml = "<Root>" + xmlString + "</Root>";
                //xpath = "Root/" + xpath;
                doc.LoadXml(localXml);
                List<XmlNode> result = new List<XmlNode>();
                string[] xpathSubs = xpath.Split(',');
                foreach (string sub in xpathSubs)
                {
                    string xpathSub = "Root/" + sub;
                    XmlNodeList nodes = doc.SelectNodes(xpathSub);

                    for (int i = 0; i < nodes.Count; i++)
                    {
                        result.Add(nodes[i]);
                    }
                }
                return result;
            }
            catch
            {
                return null;
            }
            finally
            {
                doc = null;
            }
        }

        public static List<T> MakeObjectFromXMLNode<T>(string XMlString, T oSample, string xpath = "")
        {
            List<T> result = new List<T>();
            try
            {
                List<XmlNode> lXMLNode = QueryXmlNodes(XMlString, xpath);
                foreach (XmlNode xNode in lXMLNode)
                {
                    if (xNode != null)
                    {
                        Type typeT = oSample.GetType();
                        oSample = (T)Activator.CreateInstance(typeT);
                        System.Reflection.PropertyInfo[] propertiesT = typeT.GetProperties();
                        for (int i = 0; i < propertiesT.Length; i++)
                        {
                            if (propertiesT[i].Name != "InnerXml")
                            {
                                XmlNode xNodeRef = xNode.Attributes.GetNamedItem(propertiesT[i].Name);
                                if(xNodeRef != null)
                                    propertiesT[i].SetValue(oSample, xNodeRef.Value, null);
                            }
                            else
                                propertiesT[i].SetValue(oSample, xNode.InnerXml, null);
                        }
                        result.Add(oSample);
                    }
                }
            }
            catch
            {
            //    return oSample;
            }
            return result;
        }

       
    }
}