using APIs_Faundamentals.DTO;
using APIs_Faundamentals.Models;
using APIs_Faundamentals.Helper;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace APIs_Faundamentals.Services
{
    public class AuthoSirv: IAuthoServ
    {

        
        private readonly UserManager<ApplicationUser> userManager ;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly JWT _jwt;

        public AuthoSirv(UserManager<ApplicationUser> userManager , RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt )
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._jwt = jwt.Value;
        }

        public async Task<string> AssignRoleAsync(RoleModel roleModel)
        {
           var user = await userManager.FindByIdAsync(roleModel.Id);

            if (user == null || !await roleManager.RoleExistsAsync(roleModel.Role))
            {
                return "Invallied User ID or Role";
            }

            if (await userManager.IsInRoleAsync(user, roleModel.Role))
            {
                return "User already has this role";
            }

            var result = await userManager.AddToRoleAsync(user, roleModel.Role);

            return result.Succeeded ? "Role assigned successfully" : "Failed to assign role: " + string.Join(", ", result.Errors.Select(e => e.Description));

        }

     public async Task<AuthModel> RefreshTokenAsync(string Token)
        {
           var authmodel = new AuthModel();

            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == Token));
            // Check if user exists and token is valid
            if (user == null) {

                authmodel.IsAuthenticated = false;
                authmodel.Message = "Invalid token";
                return authmodel;
            
            }

            // Check if the token is active
            var refreshToken = user.RefreshTokens.Single(t => t.Token == Token);
            if (!refreshToken.IsAtive)
            {
                authmodel.IsAuthenticated = false;
                authmodel.Message = "Inactive token";
                return authmodel;
            }

            
            refreshToken.RevokeedOn = DateTime.UtcNow;
            var newRefreshToken = GenerateRefreshToken();
            
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            var jwtToken = await CraetejwtToken(user);
            authmodel.IsAuthenticated = true;
            authmodel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authmodel.UserName = user.UserName;
            authmodel.Email = user.Email;
            var Roles = await userManager.GetRolesAsync(user);
            authmodel.Roles = Roles.ToList();
            authmodel.RefreshToken = newRefreshToken.Token;
            authmodel.RefreshTokenExpired = newRefreshToken.ExpireOn;

            return authmodel;
        }

        public async Task<AuthModel> GetTokenAsync(TokenReqModel tokenReq)
        {
            var authmodel = new AuthModel();
            var user = await userManager.FindByEmailAsync(tokenReq.Email);


            // Check if user exists and password is correct

            if (user == null || !await userManager.CheckPasswordAsync(user, tokenReq.Password)) { 
            
                  authmodel.Message = "Invalid email or password";  
                authmodel.IsAuthenticated = false;
                return authmodel;
            }
            var token = await CraetejwtToken(user);
            var roles = await userManager.GetRolesAsync(user);

            authmodel.IsAuthenticated = true;
            authmodel.UserName = user.UserName;
            authmodel.Email = user.Email;
            authmodel.Roles = roles.ToList();
            authmodel.Token = new JwtSecurityTokenHandler().WriteToken(token);
           // authmodel.Expiration = token.ValidTo;
            authmodel.Message = "User logged in successfully";

            if (user.RefreshTokens.Any(t =>t.IsAtive)) {
            
              var activRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsAtive);
                authmodel.RefreshToken = activRefreshToken.Token;
                authmodel.RefreshTokenExpired = activRefreshToken.ExpireOn;
            }
            else
            {
                var newRefreshToken = GenerateRefreshToken();

                authmodel.RefreshToken = newRefreshToken.Token;
                authmodel.RefreshTokenExpired = newRefreshToken.ExpireOn;

                user.RefreshTokens.Add(newRefreshToken);
                await userManager.UpdateAsync(user);

              
            }


            return  authmodel;



        }

   

        public async Task<AuthModel> RegisterAsync(UserDataDTO userData)
        {
            if (await userManager.FindByEmailAsync(userData.Email) != null)
            {
                return new AuthModel { Message = "Email already exists", IsAuthenticated = false };
            }

            if (await userManager.FindByNameAsync(userData.UserName) != null)
            {
                return new AuthModel
                {
                    Message = "Username already exists",
                    IsAuthenticated = false
                };
            }

            var user = new ApplicationUser
            {
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                UserName = userData.UserName,
                Email = userData.Email
            };
            var result= await userManager.CreateAsync(user ,userData.Password );

            if (!result.Succeeded) { 

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                return new AuthModel
                {
                    Message = $"User registration failed: {errors}",
                    IsAuthenticated = false
                };
                   
            }
             await userManager.AddToRoleAsync(user, "User");

            var token = await CraetejwtToken(user);

            return new AuthModel
            {
                Message = "User registered successfully",
                IsAuthenticated = true,
                UserName = user.UserName,
                Email = user.Email,
                Roles = new List<string> { "User"},
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                //Expiration = token.ValidTo
            };

        }



        private async Task<JwtSecurityToken> CraetejwtToken(ApplicationUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var rolesClaim = new List<Claim>();


            foreach (var role in roles) {
                  rolesClaim.Add(new Claim("roles" , role));
            }

            var claims = new[]
            {

                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
 
            }
            .Union(rolesClaim)
            .Union(userClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials
            );

            return token;

        }


       

       
        private RefreshToken GenerateRefreshToken()
        {
            var bytes = new byte[32];

            RandomNumberGenerator.Fill(bytes);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(bytes),
                CreateOn = DateTime.UtcNow,
                ExpireOn = DateTime.UtcNow.AddDays(10),
                RevokeedOn = null
            };

        }

        public async Task<bool> RevokeTokenAsync(string Token)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == Token));
            if (user == null)
                return false; 

            var refreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == Token);

            if (refreshToken == null || !refreshToken.IsAtive)
                return false;
            
            refreshToken.RevokeedOn = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            return true;
        }
    }
}
