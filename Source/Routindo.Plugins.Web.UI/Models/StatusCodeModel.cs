using System.Net;

namespace Routindo.Plugins.Web.UI.Models
{
    public class StatusCodeModel
    {
        public StatusCodeModel(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
        }
        public HttpStatusCode StatusCode { get; }

        public int StatusCodeValue => (int) this.StatusCode;

        public string StatusCodeName => this.StatusCode.ToString("G");

        public override string ToString()
        {
            return $"{this.StatusCodeValue} - {this.StatusCodeName}";
        }

        public override bool Equals(object obj)
        {
            return obj is StatusCodeModel model && model.StatusCode == this.StatusCode;
        }

        public override int GetHashCode()
        {
            return StatusCodeValue;
        }
    }
}
