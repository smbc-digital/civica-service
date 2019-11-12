using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace civica_service.Services.Models
{
    [XmlRoot(ElementName = "PersonName")]
    public class PersonName
    {
        [XmlElement(ElementName = "PersonReference")]
        public string PersonReference { get; set; }
        [XmlElement(ElementName = "PersonTitle")]
        public string PersonTitle { get; set; }
        [XmlElement(ElementName = "Initials")]
        public string Initials { get; set; }
        [XmlElement(ElementName = "Forenames")]
        public string Forenames { get; set; }
        [XmlElement(ElementName = "Surname")]
        public string Surname { get; set; }
        [XmlElement(ElementName = "NINo")]
        public string NINo { get; set; }
    }

    [XmlRoot(ElementName = "TelephoneNumber")]
    public class TelephoneNumber
    {
        [XmlAttribute(AttributeName = "Priority")]
        public string Priority { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "MobileNumber")]
    public class MobileNumber
    {
        [XmlAttribute(AttributeName = "Priority")]
        public string Priority { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "contact_type")]
    public class Contact_type
    {
        [XmlAttribute(AttributeName = "contact_type")]
        public string _contact_type { get; set; }
        [XmlAttribute(AttributeName = "details")]
        public string Details { get; set; }
        [XmlAttribute(AttributeName = "priority")]
        public string Priority { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "contact_types")]
    public class Contact_types
    {
        [XmlElement(ElementName = "contact_type")]
        public List<Contact_type> Contact_type { get; set; }
    }

    [XmlRoot(ElementName = "PersonalDetails")]
    public class PersonalDetails
    {
        [XmlElement(ElementName = "NINo")]
        public string NINo { get; set; }
        [XmlElement(ElementName = "TelephoneNumber")]
        public List<TelephoneNumber> TelephoneNumber { get; set; }
        [XmlElement(ElementName = "MobileNumber")]
        public MobileNumber MobileNumber { get; set; }
        [XmlElement(ElementName = "contact_types")]
        public Contact_types Contact_types { get; set; }
    }

    [XmlRoot(ElementName = "HTTPReferer")]
    public class HTTPReferer
    {
        [XmlAttribute(AttributeName = "Docid")]
        public string Docid { get; set; }
    }

    [XmlRoot(ElementName = "CurrentQueryString")]
    public class CurrentQueryString
    {
        [XmlElement(ElementName = "viewset")]
        public string Viewset { get; set; }
        [XmlElement(ElementName = "outputtype")]
        public string Outputtype { get; set; }
        [XmlElement(ElementName = "docid")]
        public string Docid { get; set; }
        [XmlElement(ElementName = "sessionId")]
        public string SessionId { get; set; }
    }

    [XmlRoot(ElementName = "TimeOut")]
    public class TimeOut
    {
        [XmlAttribute(AttributeName = "TimeOutValue")]
        public string TimeOutValue { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "AuthorityDefaults")]
    public class AuthorityDefaults
    {
        [XmlElement(ElementName = "AuthorityName")]
        public string AuthorityName { get; set; }
        [XmlElement(ElementName = "AuthorityLogo")]
        public string AuthorityLogo { get; set; }
        [XmlElement(ElementName = "TimeOut")]
        public TimeOut TimeOut { get; set; }
    }

    [XmlRoot(ElementName = "Today")]
    public class Today
    {
        [XmlAttribute(AttributeName = "Date")]
        public string Date { get; set; }
        [XmlAttribute(AttributeName = "DateSort")]
        public string DateSort { get; set; }
        [XmlAttribute(AttributeName = "Time")]
        public string Time { get; set; }
    }

    [XmlRoot(ElementName = "StandardInfo")]
    public class StandardInfo
    {
        [XmlElement(ElementName = "SessionID")]
        public string SessionID { get; set; }
        [XmlElement(ElementName = "LoggedIn")]
        public string LoggedIn { get; set; }
        [XmlElement(ElementName = "SIDString")]
        public string SIDString { get; set; }
        [XmlElement(ElementName = "BaseURL")]
        public string BaseURL { get; set; }
        [XmlElement(ElementName = "PageName")]
        public string PageName { get; set; }
        [XmlElement(ElementName = "HTTPReferer")]
        public HTTPReferer HTTPReferer { get; set; }
        [XmlElement(ElementName = "QueryString")]
        public string QueryString { get; set; }
        [XmlElement(ElementName = "CurrentQueryString")]
        public CurrentQueryString CurrentQueryString { get; set; }
        [XmlElement(ElementName = "AuthorityDefaults")]
        public AuthorityDefaults AuthorityDefaults { get; set; }
        [XmlElement(ElementName = "Today")]
        public Today Today { get; set; }
    }
}
