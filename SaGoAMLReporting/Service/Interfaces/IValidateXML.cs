using Microsoft.AspNetCore.Components.Forms;
using System.Xml.Schema;
using System.Xml;

namespace SaGoAMLReporting.Service.Interfaces
{
    public interface IValidateXML
    {
        string Validate(string xmlFile);
        string Validate(byte[] xmlBytes);
    }
}
