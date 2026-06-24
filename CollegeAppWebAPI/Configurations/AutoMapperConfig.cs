using AutoMapper;
using CollegeApp.Data;
using CollegeApp.Models;

namespace CollegeApp.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig() 
        {
            CreateMap<Student, StudentDTO>().ForMember(n => n.StudentName, opt => opt.MapFrom(x => x.Name)).ReverseMap();
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<RolePrivilege, RolePrivilegeDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserReadonlyDTO>().ReverseMap();
        }
    }
}
