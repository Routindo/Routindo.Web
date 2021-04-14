namespace Routindo.Plugins.Web.UI.Models
{
    public class PingStatusCodeModel
    {
        public PingStatusCodeModel(bool status)
        {
            this.Status = status;
        }
        public bool Status { get; }

        public bool StatusCodeValue => Status;

        public string StatusCodeName => Status ? "Reachable" : "Unreachable";

        public override string ToString()
        {
            return $"{this.StatusCodeValue} - {this.StatusCodeName}";
        }

        public override bool Equals(object obj)
        {
            return obj is PingStatusCodeModel model && model.Status == this.Status;
        }

        public override int GetHashCode()
        {
            return StatusCodeValue ? 1 : 0;
        }
    }
}