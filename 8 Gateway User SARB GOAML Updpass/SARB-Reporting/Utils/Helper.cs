using NecCore.models;
using SARB_Reporting.Models;
using SARB_Reporting.Models.Regular;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.Serialization;
using static SARB_Reporting.Models.SaReporting;

namespace Nec.Web.Utils
{
    public static class Helper
    {
        public static FileExtention FileSave<T>(SARBDEXEnvelopeCommon<T> sarb, string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filename = sarb.Header!.Reference;
                string fileSpec = Path.Combine(path, filename + ".xml");

                var serializer = new XmlSerializer(typeof(SARBDEXEnvelopeCommon<T>));
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("sarbdex", "x-schema:http://sarbdex.resbank.co.za/schemas/sarbdex_schema.xml");

                using (var stream = new FileStream(fileSpec, FileMode.Create, FileAccess.Write))
                {
                    serializer.Serialize(stream, sarb, namespaces);
                }

                return new FileExtention { FileName = filename + ".xml", FileLocation = fileSpec, IsFileSave = true };
            }
            catch (Exception ex)
            {
                return new FileExtention { FileName = ex.Message, IsFileSave = false };
            }
        }

        public static FileExtention FileGenerator<T>(T sarb, string path, string filename, XmlSerializerNamespaces namespaces)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileSpec = Path.Combine(path, filename);
                var serializer = new XmlSerializer(typeof(T));
                var xmlSettings = new System.Xml.XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true 
                };
                using (var stream = new FileStream(fileSpec, FileMode.Create, FileAccess.Write))
                using (var writer = System.Xml.XmlWriter.Create(stream, xmlSettings))
                {
                    serializer.Serialize(writer, sarb, namespaces);
                }
               return new FileExtention { FileName = filename, FileLocation = fileSpec };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
