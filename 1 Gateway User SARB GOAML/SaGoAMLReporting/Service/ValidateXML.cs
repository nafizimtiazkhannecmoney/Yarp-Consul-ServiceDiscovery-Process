using System.Xml.Schema;
using System.Xml;
using System.Xml.Linq;
using SaGoAMLReporting.Service.Interfaces;
using SaGoAMLReporting.Constants;

namespace SaGoAMLReporting.Service
{   
    public class ValidateXML : IValidateXML
    {
        public string validationMessage;
        public readonly string xsdFilePath;
        public ValidateXML()
        {
            validationMessage = string.Empty;
            xsdFilePath = Path.GetFullPath(GoAmlConstants.xsdFilePath);
        }
        public string Validate(string xmlFile)
        {
            // Set the XML and XSD file paths
            try
            {
                // Load the XML document
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);

                // Set up the schema
                xmlDoc.Schemas.Add(null, xsdFilePath);

                // Validate the XML document
                xmlDoc.Validate(ValidationEventHandler);

                if (string.IsNullOrEmpty(validationMessage))
                {
                    return  "XML is valid.";
                }
                else
                {
                    return $"Validation message: {validationMessage}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception caught: {ex.Message}";
            }
        }
        public string Validate(byte[] xmlBytes)
        {
            try
            {
                // Load the XML document from the byte array
                XmlDocument xmlDoc = new XmlDocument();
                using (MemoryStream stream = new MemoryStream(xmlBytes))
                {
                    xmlDoc.Load(stream);
                }
                // Set up the schema
                xmlDoc?.Schemas.Add(null, xsdFilePath);

                // Validate the XML document
                xmlDoc?.Validate(ValidationEventHandler);

                if (string.IsNullOrEmpty(validationMessage))
                {
                    return "XML is valid.";
                }
                else
                {
                    return $"Validation message: {validationMessage}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception caught: {ex.Message}";
            }
        }
        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            XmlSeverityType type = XmlSeverityType.Warning;
            if (Enum.TryParse<XmlSeverityType>("Error", out type))
            {
                if (type == XmlSeverityType.Error)
                    validationMessage = e.Message;
                    //throw new Exception(e.Message);
            }
        }
    }
}
