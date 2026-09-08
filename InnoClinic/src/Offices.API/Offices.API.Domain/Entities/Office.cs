using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InnoClinic.Offices.API.Domain.Entities
{
    public class Office
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("address")]
        public required string Address { get; set; }

        [BsonElement("photo_id")]
        [BsonRepresentation(BsonType.String)]
        public Guid PhotoId { get; set; }

        [BsonElement("registry_phone_number")]
        public required string RegistryPhoneNumber { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }
    }
}
