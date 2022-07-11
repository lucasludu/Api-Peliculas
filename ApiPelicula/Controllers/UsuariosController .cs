using ApiPelicula.Models;
using ApiPelicula.Models.Dtos;
using ApiPelicula.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiPelicula.Controllers
{
    [Authorize]
    [Route("api/Usuarios")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiPeliculasUsuarios")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UsuariosController(IUsuarioRepository userRepo, IMapper mapper, IConfiguration config)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _config = config;
        }

        /// <summary>
        /// Obtener todos los usuarios
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<UsuarioDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = _userRepo.GetUsuarios();
            var listaUsuariosDto = new List<UsuarioDto>();

            foreach (var lista in listaUsuarios)
            {
                listaUsuariosDto.Add(_mapper.Map<UsuarioDto>(lista));
            }
            return Ok(listaUsuariosDto);
        }

        /// <summary>
        /// Obtener un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns></returns>
        [HttpGet("{usuarioId:int}", Name = "GetUsuario")]
        [ProducesResponseType(200, Type = typeof(UsuarioDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetUsuario(int usuarioId)
        {
            var itemUsuario = _userRepo.GetUsuario(usuarioId);

            if (itemUsuario == null)
            {
                return NotFound();
            }

            var itemUsuarioDto = _mapper.Map<UsuarioDto>(itemUsuario);
            return Ok(itemUsuarioDto);
        }

        /// <summary>
        /// Crear un nuevo usuario
        /// </summary>
        /// <param name="usuarioAuthDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Registro")]
        [ProducesResponseType(201, Type = typeof(UsuarioAuthDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Registro(UsuarioAuthDto usuarioAuthDto)
        {
            usuarioAuthDto.Usuario = usuarioAuthDto.Usuario.ToLower();

            if (_userRepo.ExisteUsuario(usuarioAuthDto.Usuario))
            {
                return BadRequest("El usuario ya existe");
            }

            var usuarioACrear = new Usuario
            {
                UsuarioA = usuarioAuthDto.Usuario
            };

            var usuarioCreado = _userRepo.Registro(usuarioACrear, usuarioAuthDto.Password);
            return Ok(usuarioCreado);
        }

        /// <summary>
        /// Logueo de usuario
        /// </summary>
        /// <param name="usuarioAuthLoginDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(201, Type = typeof(UsuarioAuthLoginDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login(UsuarioAuthLoginDto usuarioAuthLoginDto)
        {
            //throw new Exception("Error Generado");
            var usuarioDesdeRepo = _userRepo.Login(usuarioAuthLoginDto.Usuario, usuarioAuthLoginDto.Password);

            if (usuarioDesdeRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioDesdeRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, usuarioDesdeRepo.UsuarioA.ToString())
            };

            //Generación de token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credenciales
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}