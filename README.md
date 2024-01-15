
-This isASP.NET  web API for student managment system , you can download and test it 
-feel free to contact me if you got in to any problem during set up
###Here is some Explanation 

#Map MongoDB to C# objects
Add Models folder and place the following Student.cs class file in it. When student data is retrieved from Mongo, the JSON data is mapped to this Student class in .NET and vice-versa.

[BsonIgnoreExtraElements]
public class Student
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = String.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = String.Empty;

    [BsonElement("graduated")]
    public bool IsGraduated { get; set; }

    [BsonElement("courses")]
    public string[]? Courses { get; set; }

    [BsonElement("gender")]
    public string Gender { get; set; } = String.Empty;

    [BsonElement("age")]
    public int Age { get; set; }
}
[BsonId] attribute specifies that this is the Id field or property. In this example, the property Id maps to _id field in Mongo document.
[BsonRepresentation] attribute automatically converts Mongo data type to a .Net data type and vice-versa. In this example, Mongo data type ObjectId is automatically converted to string datatype and vice-versa.
Name property is decorated with the [BsonElement] attribute. So this means, Name property corresponds to name field in Mongo document. So, [BsonElement] attribute specifies the field in the Mongo document the decorated property corresponds to.
The property names (Name, Courses, Gender, Age) have the same name as the fields in the Mongo document (name, courses, gender, age). However the casing is different. In C# the properties start with an uppercase letter whereas in Mongo the field starts with lowercase. There are several approaches to handle this case sensitive mapping. One of the easiest and cleanest approaches is to use [BsonElement] attribute.
What to do if the JSON document in MongoDB contains more fields than the properties in the corresponding C# class? Well, we can use [BsonIgnoreExtraElements] attribute and instruct the serializer to ignore the extra elements.

-DM me via habtamu.argeta-ug@aau.edu.et<br/>
If you got this repo helpful , give one star to it !
