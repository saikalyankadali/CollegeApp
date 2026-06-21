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
using System.Net;

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
        private APIResponse _apiResponse;

        public StudentController(ILogger<StudentController> myLogger, IMapper mapper, IStudentRepository studentRepository)
        {
            this._myLogger = myLogger;
            this._mapper = mapper;
            this._studentRepository = studentRepository;
            this._apiResponse = new APIResponse();
        }

        [HttpGet("LogMessage", Name = "StudentLogMessage")]
        public Task<APIResponse> LogMessage(string message)
        {
            var apiResponse = new APIResponse();
            _myLogger.LogTrace("This is from Log Trace");
            _myLogger.LogInformation("This is from Log Information");
            _myLogger.LogDebug("This is from Log Debug");
            _myLogger.LogWarning("This is from Log Warning");
            _myLogger.LogError("This is from Log Error");
            _myLogger.LogCritical("This is from Log Critical");
            apiResponse.Status = true;
            apiResponse.StatusCode = HttpStatusCode.OK;
            apiResponse.Data = "Message logged successfully";
            return Task.FromResult(apiResponse);
        }

        [HttpGet]
        [Route("All", Name = "GetAllStudents")]
        public async Task<APIResponse> GetStudentsAsync()
        {
            try
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
                _apiResponse.Data = _mapper.Map<List<StudentDTO>>(students);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Errors.Add(ex.Message);
                return _apiResponse;
            }
        }

        [HttpGet("Get/{id}", Name = "GetStudentById")]
        [AllowAnonymous] // allow any anonymous person can access
        public async Task<APIResponse> GetStudentByIdAsync(int id)
        {
            try
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
                _apiResponse.Data = _mapper.Map<StudentDTO>(student);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Errors.Add(ex.Message);
                return _apiResponse;
            }
        }

        [HttpPost("createstudent", Name = "CreateStudent")]
        public async Task<APIResponse> CreateStudentAsync([FromBody] StudentDTO studentDto)
        {
            try
            {
                if (studentDto == null)
                {
                    _apiResponse.Status = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.Errors.Add("Request body is null");
                    return _apiResponse;
                }

                Student student = _mapper.Map<Student>(studentDto);
                var stu = await _studentRepository.CreateStudentAsync(student);
                studentDto.Id = stu.Id;
                _apiResponse.Data = studentDto;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Errors.Add(ex.Message);
                return _apiResponse;
            }
        }

        [HttpPut("updaterecord")]
        public async Task<APIResponse> UpdateStudentDetailsAsync([FromBody] StudentDTO model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                {
                    _apiResponse.Status = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.Errors.Add("Invalid payload or missing Id");
                    return _apiResponse;
                }

                var existingRecord = await _studentRepository.GetStudentByIdAsync(student => student.Id == model.Id, true);
                if (existingRecord == null)
                {
                    _apiResponse.Status = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Errors.Add($"Student with id {model.Id} not found");
                    return _apiResponse;
                }

                var student = _mapper.Map<Student>(model);
                await _studentRepository.UpdateStudentAsync(student);

                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.Data = null;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Errors.Add(ex.Message);
                return _apiResponse;
            }
        }

        [HttpPatch("updaterecordpartial/{id:int}")]
        public async Task<APIResponse> UpdateStudentDetailsPartialAsync(int id, [FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Errors.Add("Invalid request");
                return _apiResponse;
            }

            var existingRecord = await _studentRepository.GetStudentByIdAsync(student => student.Id == id, true);
            if (existingRecord == null)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.Errors.Add($"Student with id {id} not found");
                return _apiResponse;
            }

            var studentDTO = _mapper.Map<StudentDTO>(existingRecord);
            patchDocument.ApplyTo(studentDTO, ModelState);

            if (!ModelState.IsValid)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Errors.Add("Invalid model state");
                return _apiResponse;
            }

            existingRecord = _mapper.Map<Student>(studentDTO);
            await _studentRepository.UpdateStudentAsync(existingRecord);

            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            return _apiResponse;
        }

        [HttpDelete("deletestudent/{id:int}", Name = "DeleteStudentById")]
        public async Task<APIResponse> DeleteStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(student => student.Id == id);
            if (student == null)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.Errors.Add($"No Student found with id {id}");
                return _apiResponse;
            }

            var deleted = await _studentRepository.DeleteStudentAsync(student);
            _apiResponse.Status = deleted;
            _apiResponse.StatusCode = deleted ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            _apiResponse.Data = deleted;
            return _apiResponse;
        }

        [HttpGet("GetId/{id}", Name = "GetStudentByIdAction")]
        public async Task<APIResponse> GetStudentByIdActionAsync(int id)
        {
            if (id <= 0)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Errors.Add("Invalid id");
                return _apiResponse;
            }

            var student = await _studentRepository.GetStudentByIdAsync(student => student.Id == id);
            if (student == null)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.Errors.Add($"The Student with id {id} is not found");
                return _apiResponse;
            }

            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Data = student;
            return _apiResponse;
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
