using System.Runtime.Serialization;

namespace Hacked.DataLayer.Models
{
    [DataContract]
    public class CategoricalChartData
    {
        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public double Value { get; set; }
    }
}
