﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using exSales.API.DTO;
using exSales.Domain.Interfaces.Services;
using exSales.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using exSales.Domain.Impl.Models;
using exSales.Domain.Interfaces.Models;
using exSales.DTO.Domain;
using exSales.Domain.Impl.Services;
using System.Runtime.CompilerServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace exSales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        private UserInfo ModelToInfo(IUserModel md)
        {
            var user = new UserInfo { 
                Id = md.Id,
                Hash = md.Hash,
                CreateAt = md.CreateAt,
                UpdateAt = md.UpdateAt,
                Name = md.Name,
                Email = md.Email
            };
            return user;
        }

        [HttpGet("checkUserRegister/{chainId}/{address}")]
        public ActionResult<UserResult> CheckUserRegister(int chainId, string address)
        {
            try
            {
                //Console.WriteLine("Chegou aqui");
                var user = _userService.GetUserByAddress((ChainEnum)chainId, address);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = true, Mensagem = "BTC Address Not Found" };
                }
                return new UserResult()
                {
                    User = ModelToInfo(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
