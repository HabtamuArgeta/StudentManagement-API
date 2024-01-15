
-This is ASP.NET  web API for student managment system , you can download and test it 
<br/><br/>
###Here is some Explanation 
<br/><br/>
#Map MongoDB to C# objects<br/>
Add Models folder and place the following Student.cs class file in it. When student data is retrieved from Mongo, the JSON <br/>data is mapped to this Student class in .NET and vice-versa.

<br/>[BsonIgnoreExtraElements]
<br/>public class Student
<br/>{
    <br/>[BsonId]
    <br/>[BsonRepresentation(BsonType.ObjectId)]
  <br/>  public string Id { get; set; } = String.Empty;

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
<br/><br/>[BsonId] attribute specifies that this is the Id field or property. In this example, the property Id maps to _id field in Mongo document.
<br/><br/>[BsonRepresentation] attribute automatically converts Mongo data type to a .Net data type and vice-versa. In this <br/>example, Mongo data type ObjectId is automatically converted to string datatype and vice-versa.
<br/><br/>Name property is decorated with the [BsonElement] attribute. So this means, Name property corresponds to name field in Mongo document. <br/>So, [BsonElement] attribute specifies the field in the Mongo document the decorated property corresponds to.
<br/><br/>The property names (Name, Courses, Gender, Age) have the same name as the fields in the Mongo document (name, courses, gender, age). However the casing is different. In C# the properties start with an uppercase letter whereas in Mongo the field starts with lowercase.<br/><br/> There are several approaches to handle this case sensitive mapping. One of the easiest and cleanest approaches is to use [BsonElement] attribute.
<br/><br/>What to do if the JSON document in MongoDB contains more fields than the properties in the corresponding C# class? Well, we can use [BsonIgnoreExtraElements] attribute and instruct the serializer to ignore the extra elements.


<br/><br/>All the following attributes are present in MongoDB.Bson.Serialization.Attributes

-BsonId<br/><br/>
-BsonElement<br/><br/>
-BsonRepresentation<br/><br/>
-BsonIgnoreExtraElements<br/><br/><br/><br/>
MongoDB connection string in ASP.NET Core
Store MongoDB connection information in appsettings.json file.

<br/>{
 <br/> "StudentStoreDatabaseSettings": {
   <br/> "StudentCoursesCollectionName": "studentcourses",
    <br/>"ConnectionString": "Your_MongoDB_ConnectionString",
   <br/> "DatabaseName": "myFirstDatabase"
 <br/> },
<br/>  "Logging": {
  <br/>  "LogLevel": {
   <br/>   "Default": "Information",
    <br/>  "Microsoft.AspNetCore": "Warning"
   <br/> }
 <br/> },
 <br/> "AllowedHosts": "*"
<br/>}

<br/><br/>Add the following 2 files in the Models folder. This interface and the class provide strongly typed access to MongoDB connection information.

<br/>1IStudentStoreDatabaseSettings.cs
<br/>2StudentStoreDatabaseSettings.cs

<br/>IStudentStoreDatabaseSettings.cs
<br/>public interface IStudentStoreDatabaseSettings
<br/>{
  <br/>  string StudentCoursesCollectionName { get; set; }
   <br/> string ConnectionString { get; set; }
   <br/> string DatabaseName { get; set; }
<br/>}
<br/>StudentStoreDatabaseSettings.cs
<br/>public class StudentStoreDatabaseSettings : IStudentStoreDatabaseSettings
<br/>{
  <br/>  public string StudentCoursesCollectionName { get; set; } = String.Empty;
  <br/>  public string ConnectionString { get; set; } = String.Empty;
   <br/> public string DatabaseName { get; set; } = String.Empty;
<br/>}

<br/><br/>How to call MongoDB API from C#<br/>
<br/>-For separation of concerns we will keep the code that calls Mongo API in a separate service layer.
<br/>-ASP.NET Web API Controller calls this service. 
<br/>-Add Services folder and place the 2 files in it (IStudentService.cs and StudentService.cs)
<br/><br/>IStudentService.cs
<br/>public interface IStudentService
<br/>{
  <br/>  List<Student> Get();
   <br/> Student Get(string id);
   <br/> Student Create(Student student);
   <br/> void Update(string id, Student student);
   <br/> void Remove(string id);
<br/>}
<br/>StudentService.cs<br/>
public class StudentService : IStudentService
{
    private readonly IMongoCollection<Student> _students;

    public StudentService(IStudentStoreDatabaseSettings settings, IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        _students = database.GetCollection<Student>(settings.StudentCoursesCollectionName);
    }

    public Student Create(Student student)
    {
        _students.InsertOne(student);
        return student;
    }

    public List<Student> Get()
    {
        return _students.Find(student => true).ToList();
    }

    public Student Get(string id)
    {
        return _students.Find(student => student.Id == id).FirstOrDefault();
    }

    public void Remove(string id)
    {
        _students.DeleteOne(student => student.Id == id);
    }

    public void Update(string id, Student student)
    {
        _students.ReplaceOne(student => student.Id == id, student);
    }
}

<br/><br/>ASP.NET Core REST API Controller<br/>
<br/>Add the following StudentsController.cs file in the Controllers folder.
[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IStudentService studentService;

    public StudentsController(IStudentService studentService)
    {
        this.studentService = studentService;
    }
    // GET: api/<StudentsController>
    [HttpGet]
    public ActionResult<List<Student>> Get()
    {
        return studentService.Get();
    }

    // GET api/<StudentsController>/5
    [HttpGet("{id}")]
    public ActionResult<Student> Get(string id)
    {
        var student = studentService.Get(id);

        if (student == null)
        {
            return NotFound($"Student with Id = {id} not found");
        }

        return student;
    }

    // POST api/<StudentsController>
    [HttpPost]
    public ActionResult<Student> Post([FromBody] Student student)
    {
        studentService.Create(student);

        return CreatedAtAction(nameof(Get), new { id = student.Id }, student);
    }

    // PUT api/<StudentsController>/5
    [HttpPut("{id}")]
    public ActionResult Put(string id, [FromBody] Student student)
    {
        var existingStudent = studentService.Get(id);

        if (existingStudent == null)
        {
            return NotFound($"Student with Id = {id} not found");
        }

        studentService.Update(id, student);

        return NoContent();
    }

    // DELETE api/<StudentsController>/5
    [HttpDelete("{id}")]
    public ActionResult Delete(string id)
    {
        var student = studentService.Get(id);

        if (student == null)
        {
            return NotFound($"Student with Id = {id} not found");
        }

        studentService.Remove(student.Id);

        return Ok($"Student with Id = {id} deleted");
    }
}

<br/><br/>Configure Services 
<br/>In Program.cs file, configure the required services.
<br/>
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StudentStoreDatabaseSettings>(
                builder.Configuration.GetSection(nameof(StudentStoreDatabaseSettings)));

builder.Services.AddSingleton<IStudentStoreDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<StudentStoreDatabaseSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(s =>
        new MongoClient(builder.Configuration.GetValue<string>("StudentStoreDatabaseSettings:ConnectionString")));

builder.Services.AddScoped<IStudentService, StudentService>();

<br/><br/>Test REST API using Swagger
<br/>With Swagger it's easy to test the API calls directly in the browser.
<br/>

<br/><br/>-feel free to contact me if you got in to any problem during set up
-DM me via habtamu.argeta-ug@aau.edu.et<br/>
<br/>If you got this repo helpful , give one star to it !
