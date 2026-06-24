using AutoMapper;
using CollegeApp.Data;
using CollegeApp.Data.Repository;
using CollegeApp.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CollegeApp.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ICollegeRepository<User> _userRepository;
        private readonly ICollegeRepository<UserRoleMapping> _userRoleMappingRepository;
        private readonly ICollegeRepository<Role> _roleRepository;

        public UserService(
            IMapper mapper,
            ICollegeRepository<User> userRepository,
            ICollegeRepository<UserRoleMapping> userRoleMappingRepository,
            ICollegeRepository<Role> roleRepository)
        {
            this._mapper = mapper;
            this._userRepository = userRepository;
            this._userRoleMappingRepository = userRoleMappingRepository;
            this._roleRepository = roleRepository;
        }

        public async Task<bool> CreateUserAsync(UserDTO dto)
        {
            //Old way
            //if(dto == null)
            //    throw new ArgumentNullException(nameof(dto));

            //New way
            ArgumentNullException.ThrowIfNull(dto, $"the argument {nameof(dto)} is null");

            var existingUser = await _userRepository.GetByIdAsync(user => user.Username.Equals(dto.Username));

            if (existingUser != null)
            {
                throw new Exception("The username already taken");
            }

            User user = _mapper.Map<User>(dto);
            user.IsDeleted = false;
            user.CreatedDate = DateTime.Now;
            user.ModifiedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                var passwordHash = CreatePasswordHashWithSalt(dto.Password);
                user.Password = passwordHash.PasswordHash;
                user.PasswordSalt = passwordHash.Salt;
            }

            await _userRepository.CreateAsync(user);

            return true;
        }

        public async Task<List<UserReadonlyDTO>> GetUsersAsync()
        {
            var users = await _userRepository.GetAllByFilterAsync(u => !u.IsDeleted);

            return _mapper.Map<List<UserReadonlyDTO>>(users);
        }

        public async Task<UserReadonlyDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(user => !user.IsDeleted && user.Id == id);

            return _mapper.Map<UserReadonlyDTO>(user);
        }

        public async Task<UserReadonlyDTO> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByIdAsync(user => !user.IsDeleted && user.Username.Equals(username));

            return _mapper.Map<UserReadonlyDTO>(user);
        }

        public async Task<LoginUserResult?> GetUserForLoginAsync(string username)
        {
            // Fetch full entity (need Password + PasswordSalt, which the read-only DTO strips)
            var user = await _userRepository.GetByIdAsync(
                u => !u.IsDeleted && u.Username.Equals(username), true);

            if (user == null)
                return null;

            // Resolve role name via UserRoleMapping -> Role
            string roleName = "Admin"; // fallback if no mapping exists
            var mapping = await _userRoleMappingRepository.GetByIdAsync(
                m => m.UserId == user.Id, true);

            if (mapping != null)
            {
                var role = await _roleRepository.GetByIdAsync(
                    r => r.Id == mapping.RoleId, true);
                if (role != null)
                    roleName = role.RoleName;
            }

            return new LoginUserResult
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                PasswordSalt = user.PasswordSalt,
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted,
                RoleName = roleName
            };
        }

        public async Task<bool> UpdateUserAsync(UserDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, nameof(dto));

            var existingUser = await _userRepository.GetByIdAsync(user => !user.IsDeleted && user.Id == dto.Id, true);
            if (existingUser == null)
            {
                throw new Exception($"User not found with the Id: {dto.Id}");
            }

            var userToUpdate = _mapper.Map<User>(dto);
            userToUpdate.ModifiedDate = DateTime.Now;

            //1. We will update only the user information
            //2. We need to provide separate method to update the password
            //for the demo purpose I am updating the password also
            if (!string.IsNullOrEmpty(dto.Password))
            {
                var passwordHash = CreatePasswordHashWithSalt(dto.Password);
                userToUpdate.Password = passwordHash.PasswordHash;
                userToUpdate.PasswordSalt = passwordHash.Salt;
            }

            await _userRepository.UpdateAsync(userToUpdate);

            return true;
        }

        public async Task<bool> DeleteUser(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException(nameof(userId));

            var existingUser = await _userRepository.GetByIdAsync(user => !user.IsDeleted && user.Id == userId, true);
            if (existingUser == null)
            {
                throw new Exception($"User not found with the Id: {userId}");
            }

            //1. Hard delete -- you can try this
            //2. Soft delete -- we will do this now

            existingUser.IsDeleted = true;

            await _userRepository.UpdateAsync(existingUser);

            return true;
        }

        public (string PasswordHash, string Salt) CreatePasswordHashWithSalt(string password)
        {
            //Create the salt
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            //Create Password Hash
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                ));

            return (hash, Convert.ToBase64String(salt));
        }

        // Re-derives the hash using the stored salt and compares to the stored hash.
        // Must mirror CreatePasswordHashWithSalt exactly (same PRF, iterations, byte length).
        public bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            if (string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            var saltBytes = Convert.FromBase64String(storedSalt);

            var computedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                ));

            return computedHash == storedHash;
        }
    }
}