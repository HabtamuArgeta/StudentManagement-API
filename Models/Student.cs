using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Models
{
    [BsonIgnoreExtraElements]
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } 

        [BsonElement("name")]
        public string Name { get; set; } = String.Empty;

        [BsonElement("photoUrl")]
        public string? PhotoUrl { get; set; }

        [BsonElement("graduated")]
        public bool IsGraduated { get; set; }


        [BsonElement("gender")]
        public string Gender { get; set; } = String.Empty;

        [BsonElement("age")]
        public int Age { get; set; }
    }
}
