using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using StudentManagement.Models;
using StudentManagement.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StudentManagement.Controllers
{
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
        public async Task<ActionResult<Student>> Post([FromForm] Student student, IFormFile photo)
        {
            if (photo != null)
            {
                var fileName = $"{student.Id}_{photo.FileName}";
                var filePath = Path.Combine("wwwroot", "photo", fileName); // Specify the directory where photos will be saved

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }

                var photoUrl = $"/wwwroot/photo/{fileName}"; // Constructing the URL where the photo will be accessible

                student.PhotoUrl = photoUrl; // Save the photo URL to the Student model
            }

            studentService.Create(student);

            return CreatedAtAction(nameof(Get), new { id = student.Id }, student);
        }


        // PUT api/<StudentsController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] Student updatedStudent)
        {
            var existingStudent = studentService.Get(id);

            if (existingStudent == null)
            {
                return NotFound($"Student with Id = {id} not found");
            }

            studentService.Update(id, updatedStudent);

            return Ok($"Student with Id = {id} updated");
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
}
