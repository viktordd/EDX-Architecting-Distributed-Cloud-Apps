using System;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

namespace AggregatorService.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class DeviceData : IEquatable<DeviceData>
    { 
        /// <summary>
        /// Gets or Sets DeviceId
        /// </summary>
        [DataMember(Name="deviceId")]
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or Sets Timestamp
        /// </summary>
        [DataMember(Name="timestamp")]
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Gets or Sets Value
        /// </summary>
        [DataMember(Name="value")]
        public double? Value { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class DeviceData {\n");
            sb.Append("  DeviceId: ").Append(DeviceId).Append("\n");
            sb.Append("  Timestamp: ").Append(Timestamp).Append("\n");
            sb.Append("  Value: ").Append(Value).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((DeviceData)obj);
        }

        /// <summary>
        /// Returns true if DeviceData instances are equal
        /// </summary>
        /// <param name="other">Instance of DeviceData to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(DeviceData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    DeviceId == other.DeviceId ||
                    DeviceId != null &&
                    DeviceId.Equals(other.DeviceId)
                ) && 
                (
                    Timestamp == other.Timestamp ||
                    Timestamp != null &&
                    Timestamp.Equals(other.Timestamp)
                ) && 
                (
                    Value == other.Value ||
                    Value != null &&
                    Value.Equals(other.Value)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (DeviceId != null)
                    hashCode = hashCode * 59 + DeviceId.GetHashCode();
                    if (Timestamp != null)
                    hashCode = hashCode * 59 + Timestamp.GetHashCode();
                    if (Value != null)
                    hashCode = hashCode * 59 + Value.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(DeviceData left, DeviceData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DeviceData left, DeviceData right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
