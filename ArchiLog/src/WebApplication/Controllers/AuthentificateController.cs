using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using APILibrary.Core.IdentityUserModel;
using APILibrary.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Data;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AuthentificateController : ControllerBase
    {

        private readonly UserManager<User> Usermanager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<Role> Rolemanager;

        
        public AuthentificateController(UserManager<User> Usermanager, SignInManager<User> signInManager, RoleManager<Role> Rolemanager)
        {
            this.Usermanager = Usermanager;
            this.signInManager = signInManager;
            this.Rolemanager = Rolemanager;
        }


        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> AuthenticateUser(UserModel userI)
        {

            var resultat = await this.Usermanager.CreateAsync(
                new User
                {
                    UserName = userI.Email,
                    Email = userI.Email,
                }, userI.Password
                );

            if (!resultat.Succeeded)
            {return BadRequest();}

            return Ok(resultat);
        }


       
        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Login(UserModel userI)
        {

            IActionResult response = Unauthorized();
            var result = await this.signInManager.PasswordSignInAsync(
                userI.Email,
                userI.Password,
                true,// rendre l'authentification persistante
                false);//vérrouiller l'utulisateur si l'authentification ne fonctionne pas
            if (!ModelState.IsValid)
            {
                return (IActionResult)userI;
            }

            if (result.Succeeded)
            {
                var tokenString = this.GenerateToken(userI.Email);
                response = Ok(new { Token = tokenString, Message = "Success" });
            }

            return response;
        }


        //methode pour générer le Token de connexion 
        private Object GenerateToken(string email)
        {

            var key = Encoding.ASCII.GetBytes("ICImachainesecreteTreslongue2020");
            
            var token = new JwtSecurityToken(

                claims: new Claim[] { new Claim(ClaimTypes.Name, email) },
                notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                expires:new DateTimeOffset(DateTime.Now.AddMinutes(60)).DateTime,
                signingCredentials :new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
