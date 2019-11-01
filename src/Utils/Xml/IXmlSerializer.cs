using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace civica_service.Utils.Xml
{
    public interface IXmlSerializer
    {
         XmlSchema GetSchema();

         void ReadXml(XmlReader reader);

         void WriteXml(XmlWriter writer);
    }
}
