using System;
using System.Xml.Serialization;

namespace NSoft.NFramework.Xml {
    /// <summary>
    /// XML Serialize 테스트용 Class
    /// </summary>
    [Serializable]
    [XmlRoot("User")]
    public class User : ValueObjectBase {
        [XmlAttribute("id")]
        public Guid Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("desc")]
        public string Description { get; set; }

        [XmlElement("age")]
        public int Age { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Id, Name);
        }

        public override string ToString() {
            return string.Format("User# Id={0}, Name={1}, Age={2}, Description={3}", Id, Name, Age, Description);
        }
    }
}