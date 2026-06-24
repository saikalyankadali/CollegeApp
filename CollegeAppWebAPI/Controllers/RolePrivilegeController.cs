using AutoMapper;
using CollegeApp.Data;
using CollegeApp.Data.Repository;
using CollegeApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePrivilegeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICollegeRepository<RolePrivilege> _rolePrivilegeRepository;
        private APIResponse _apiResponse;

        public RolePrivilegeController(IMapper mapper, ICollegeRepository<RolePrivilege> rolePrivilegeRepository)
        {
            this._mapper = mapper;
            this._rolePrivilegeRepository = rolePrivilegeRepository;
            this._apiResponse = new();
        }

        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<APIResponse>> CreateRolePrivilegeAsync(RolePrivilegeDTO rolePrivilegeDTO)
        {
            try
            {
                if (rolePrivilegeDTO == null)
                {
                    return BadRequest();
                }

                RolePrivilege rolePrivilege = _mapper.Map<RolePrivilege>(rolePrivilegeDTO);
                rolePrivilege.IsDeleted = false;
                rolePrivilege.CreatedDate = DateTime.Now;
                rolePrivilege.ModifiedDate = DateTime.Now;

                var result = await _rolePrivilegeRepository.CreateAsync(rolePrivilege);
                rolePrivilegeDTO.Id = result.Id;
                _apiResponse.Data = rolePrivilegeDTO;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
                //return CreatedAtRoute("GetRolePrivilegeById", new { id = rolePrivilegeDTO.Id }, _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Errors.Add(ex.Message);
                return _apiResponse;
            }
        }

        [HttpGet]
        [Route("All", Name = "GetAllRolePrivileges")]
        public async Task<ActionResult<APIResponse>> GetRolePrivilegesAsync()
        {
            try
            {
                var rolePrivileges = await _rolePrivilegeRepository.GetAllAsync();
                _apiResponse.Data = _mapper.Map<List<RolePrivilegeDTO>>(rolePrivileges);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Errors.Add(ex.Message);
                return _apiResponse;
            }
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetRolePrivilegeById")]
        public async Task<ActionResult<APIResponse>> GetRolePrivilegeByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }
                var rolePrivilege = await _rolePrivilegeRepository.GetByIdAsync(rolePrivilege => rolePrivilege.Id == id);
                if (rolePrivilege == null)
                {
                    return NotFound($"The RolePrivilege not found with id {id}");
                }
                _apiResponse.Data = _mapper.Map<RolePrivilegeDTO>(rolePrivilege);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Errors.Add(ex.Message);
                return _apiResponse;
            }
        }

        [HttpGet]
        [Route("ByRole/{roleId:int}", Name = "GetRolePrivilegeByRoleId")]
        public async Task<ActionResult<APIResponse>> GetRolePrivilegeByRoleIdAsync(int roleId)
        {
            try
            {
                if (roleId <= 0)
                {
                    return BadRequest();
                }
                var rolePrivileges = await _rolePrivilegeRepository.GetAllByFilterAsync(rolePrivilege => rolePrivilege.RoleId == roleId);
                if (rolePrivileges == null || !rolePrivileges.Any())
                {
                    return NotFound($"No RolePrivileges found for role id {roleId}");
                }
                _apiResponse.Data = _mapper.Map<List<RolePrivilegeDTO>>(rolePrivileges);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Errors.Add(ex.Message);
                return _apiResponse;
            }
        }

        [HttpPut("updaterecord")]
        public async Task<APIResponse> UpdateRolePrivilegeAsync([FromBody] RolePrivilegeDTO rolePrivilegeDTO)
        {
            try
            {
                if (rolePrivilegeDTO == null || rolePrivilegeDTO.Id <= 0)
                {
                    _apiResponse.Status = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.Errors.Add("Invalid payload or missing Id");
                    return _apiResponse;
                }

                var existingRecord = await _rolePrivilegeRepository.GetByIdAsync(rolePrivilege => rolePrivilege.Id == rolePrivilegeDTO.Id, true);
                if (existingRecord == null)
                {
                    _apiResponse.Status = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Errors.Add($"RolePrivilege with id {rolePrivilegeDTO.Id} not found");
                    return _apiResponse;
                }

                var rolePrivilege = _mapper.Map<RolePrivilege>(rolePrivilegeDTO);
                await _rolePrivilegeRepository.UpdateAsync(rolePrivilege);

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
        public async Task<APIResponse> UpdateRolePrivilegePartialAsync(int id, [FromBody] JsonPatchDocument<RolePrivilegeDTO> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Errors.Add("Invalid request");
                return _apiResponse;
            }

            var existingRecord = await _rolePrivilegeRepository.GetByIdAsync(rolePrivilege => rolePrivilege.Id == id, true);
            if (existingRecord == null)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.Errors.Add($"RolePrivilege with id {id} not found");
                return _apiResponse;
            }

            var rolePrivilegeDTO = _mapper.Map<RolePrivilegeDTO>(existingRecord);
            patchDocument.ApplyTo(rolePrivilegeDTO, ModelState);

            if (!ModelState.IsValid)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Errors.Add("Invalid model state");
                return _apiResponse;
            }

            existingRecord = _mapper.Map<RolePrivilege>(rolePrivilegeDTO);
            await _rolePrivilegeRepository.UpdateAsync(existingRecord);

            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            return _apiResponse;
        }

        [HttpDelete("deleterolePrivilege/{id:int}", Name = "DeleteRolePrivilegeById")]
        public async Task<APIResponse> DeleteRolePrivilegeByIdAsync(int id)
        {
            var rolePrivilege = await _rolePrivilegeRepository.GetByIdAsync(rolePrivilege => rolePrivilege.Id == id);
            if (rolePrivilege == null)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.Errors.Add($"No RolePrivilege found with id {id}");
                return _apiResponse;
            }

            var deleted = await _rolePrivilegeRepository.DeleteAsync(rolePrivilege);
            _apiResponse.Status = deleted;
            _apiResponse.StatusCode = deleted ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            _apiResponse.Data = deleted;
            return _apiResponse;
        }
    }
}