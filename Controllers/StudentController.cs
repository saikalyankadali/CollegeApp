using CollegeApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.JsonPatch;
using CollegeApp.Logger;
using CollegeApp.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CollegeApp.Data.Repository;
using Microsoft.AspNetCore.Authorization;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _myLogger;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;

        public StudentController(ILogger<StudentController> myLogger, IMapper mapper, IStudentRepository studentRepository)
        { 
            this._myLogger = myLogger;
            this._mapper = mapper;
            this._studentRepository = studentRepository;
        }

        [HttpGet("LogMessage", Name = "StudentLogMessage")]
        public ActionResult LogMessage(string message)
        {
            _myLogger.LogTrace("This is from Log Trace");
            _myLogger.LogInformation("This is from Log Information");
            _myLogger.LogDebug("This is from Log Debug");
            _myLogger.LogWarning("This is from Log Warning");
            _myLogger.LogError("This is from Log Error");
            _myLogger.LogCritical("This is from Log Critical");
            return Ok("Message logged successfully");
        }

        [HttpGet]
        [Route("All", Name = "GetAllStudents")]
        public async Task<ActionResult<StudentDTO>> GetStudentsAsync()
        {
            //var students = await _dbContext.Students.Select(stu => new StudentDTO
            //{
            //    Id = stu.Id,
            //    Name = stu.Name,
            //    Email = stu.Email,
            //    Phone = stu.Phone,
            //    DOB = stu.DOB
            //}).ToListAsync();

            var students = await _studentRepository.GetAllStudentsAsync();
            var studentsDTO = _mapper.Map<List<StudentDTO>>(students);
            return Ok(studentsDTO);
        }

        [HttpGet("Get/{id}", Name = "GetStudentById")]
        [AllowAnonymous] // allow any anonymous person can access
        public async Task<ActionResult> GetStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(student => student.Id == id);
            //StudentDTO studentDTO = new StudentDTO
            //{
            //    Id = student.Id,
            //    Name = student.Name,
            //    Email = student.Email,
            //    Phone = student.Phone,
            //    DOB = student.DOB
            //};
            var studentDTO = _mapper.Map<StudentDTO>(student);
            return Ok(studentDTO);
        }

        [HttpPost("createstudent", Name = "CreateStudent")]
        public async Task<ActionResult<StudentDTO>> CreateStudentAsync([FromBody] StudentDTO studentDto)
        {
            if (studentDto == null)
            {
                return BadRequest();
            }
            //Student student = new Student
            //{
            //    Name = studentDto.Name,
            //    Email = studentDto.Email,
            //    Phone = studentDto.Phone,
            //    DOB = studentDto.DOB
            //};
            Student student = _mapper.Map<Student>(studentDto);
            var stu = await _studentRepository.CreateStudentAsync(student);
            studentDto.Id = stu.Id;
            return CreatedAtRoute("GetStudentById", new { id = studentDto.Id }, studentDto);
        }

        [HttpPut("updaterecord")]
        public async Task<ActionResult> UpdateStudentDetailsAsync([FromBody] StudentDTO model)
        {
            if (model == null || model.Id <= 0)
                return BadRequest();
            var existingRecord = await _studentRepository.GetStudentByIdAsync(student => student.Id == model.Id, true);
            if (existingRecord == null)
                return NotFound();

            //var student = new Student()
            //{
            //    Id = existingRecord.Id,
            //    Name = model.Name,
            //    Email = model.Email,
            //    Phone = model.Phone,
            //    DOB = model.DOB
            //};
            var student = _mapper.Map<Student>(existingRecord);
            await _studentRepository.UpdateStudentAsync(student);
            //existingRecord.Name = model.Name;
            //existingRecord.Email = model.Email;
            //existingRecord.Phone = model.Phone;
            //existingRecord.DOB = model.DOB;
            return NoContent();
        }

        [HttpPatch("updaterecordpartial/{id:int}")]
        public async Task<ActionResult> UpdateStudentDetailsPartialAsync(int id, [FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        {
            if (patchDocument == null || id <= 0)
                return BadRequest();
            var existingRecord = await _studentRepository.GetStudentByIdAsync(student => student.Id == id, true);
            if (existingRecord == null)
                return NotFound();
            //var studentDTO = new StudentDTO
            //{
            //    Id = existingRecord.Id,
            //    Name = existingRecord.Name,
            //    Email = existingRecord.Email,
            //    Phone = existingRecord.Phone,
            //    DOB = existingRecord.DOB
            //};
            var studentDTO = _mapper.Map<StudentDTO>(existingRecord);
            patchDocument.ApplyTo(studentDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //existingRecord.Name = studentDTO.Name;
            //existingRecord.Email = studentDTO.Email;
            //existingRecord.Phone = studentDTO.Phone;
            //existingRecord.DOB = studentDTO.DOB;
            existingRecord = _mapper.Map<Student>(studentDTO);
            await _studentRepository.UpdateStudentAsync(existingRecord);
            return NoContent();
        }

        [HttpDelete("deletestudent/{id:int}", Name = "DeleteStudentById")]
        public async Task<bool> DeleteStudentByIdAsync(int id)
        {
            Student student = await _studentRepository.GetStudentByIdAsync(student => student.Id == id);
            if (student == null)
            {
                throw new ArgumentNullException($"No Student found with id {student.Id}");
            }
            return await _studentRepository.DeleteStudentAsync(student);
        }

        [HttpGet("GetId/{id}", Name = "GetStudentByIdAction")]
        public async Task<ActionResult<Student>> GetStudentByIdActionAsync(int id)
        {
            if (id <= 0)
                return BadRequest();
            Student student = await _studentRepository.GetStudentByIdAsync(student => student.Id == id);
            if (student == null)
                return NotFound($"The Student with id {id} is not found");
            return Ok(student);
        }

        //[HttpGet("VerifyEmail", Name = "VerifyEmail")]
        //[HttpGet("/Student/VerifyEmail")]
        //public IActionResult VerifyEmail([FromQuery] string email, [FromQuery] int id = 0)
        //{
        //    // Remote validation expects a JSON true when valid, or a string error message when invalid.
        //    if (string.IsNullOrWhiteSpace(email))
        //        return new JsonResult("Email is required");

        //    bool exists = _dbContext.Students
        //        .Any(s => !string.IsNullOrEmpty(s.Email)
        //                  && s.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
        //                  && s.Id != id); // allow the same record to keep its email

        //    if (exists)
        //        return new JsonResult($"The email '{email}' is already in use.");

        //    return new JsonResult(true);
        //}
    }
}
