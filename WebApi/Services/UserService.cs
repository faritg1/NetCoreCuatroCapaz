using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WebApi.Dtos;
using WebApi.Helpers;

namespace WebApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JWT _jwt;

        public UserService(IUnitOfWork unitOfWork, IOptions<JWT> jwt, IPasswordHasher<User> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwt = jwt.Value;
        }

        public async Task<string> RegisterAsync(RegisterDto resgisterDto){
            var user = new User{
                Email = resgisterDto.Email,
                Username = resgisterDto.Username
            };

            user.Password = _passwordHasher.HashPassword(user, resgisterDto.Password);

            var existingUser = _unitOfWork.Users
                .Find(u => u.Username.ToLower() == resgisterDto.Username.ToLower())
                .FirstOrDefault();

            if(existingUser == null){
                var rolDefault = _unitOfWork.Roles
                    .Find(u => u.Nombre == Authorization.rol_default.ToString())
                    .First();
                try
                {
                    user.Rols.Add(rolDefault);
                    _unitOfWork.Users.Add(user);
                    await _unitOfWork.SaveAsync();

                    return $"User {resgisterDto.Username} has been registered successfully";
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    return $"Error: {message}";
                }
            }else{
                return $"User {resgisterDto.Username} already registered";
            }
        }

        public async Task<DataUserDto> GetTokenAsync(LoginDto model){
            DataUserDto dataUserDto = new DataUserDto();
            var user = await _unitOfWork.Users.GetByUsernameAsync(model.Username);

            if(user == null){
                dataUserDto.IsAuthenticated = false;
                dataUserDto.Message = $"User does not exist with username {model.Username}";
                return dataUserDto;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

            if(result == PasswordVerificationResult.Success){
                dataUserDto.IsAuthenticated = true;
                JwtSecurityToken jwtSecurityToken = CreateJwtToken(user);
                dataUserDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                dataUserDto.Email = user.Email;
                dataUserDto.UserName = user.Username;
                dataUserDto.Roles = user.Rols
                    .Select(u => u.Nombre)
                    .ToList();
                
                if(user.RefreshTokens.Any(a => a.IsActive)){
                    var activeRefreshToken = user.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();
                    dataUserDto.RefreshToken = activeRefreshToken.Token;
                    dataUserDto.RefreshTokenExpiration = activeRefreshToken.Expires;
                }else{
                    var refreshToken = CreateRefreshToken();
                    dataUserDto.RefreshToken = refreshToken.Token;
                    dataUserDto.RefreshTokenExpiration = refreshToken.Expires;
                    user.RefreshTokens.Add(refreshToken);
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveAsync();
                }
                return dataUserDto;
            }

            dataUserDto.IsAuthenticated = false;
            dataUserDto.Message = $"Credenciales incorrectas para el usuario {user.Username}";
            return dataUserDto;
        }

        public async Task<string> AddRoleAsync(AddRoleDto model){
            var user = await _unitOfWork.Users
                .GetByUsernameAsync(model.Username);
            if (user == null){
                return $"User {model.Username} does not exists";
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

            if(result == PasswordVerificationResult.Success){
                var rolExists = _unitOfWork.Roles
                    .Find(u => u.Nombre.ToLower() == model.Role.ToLower())
            }

        }
    }
}