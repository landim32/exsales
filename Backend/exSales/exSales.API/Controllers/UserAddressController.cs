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
    public class UserAddressController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserAddressController(IUserService userService)
        {
            _userService = userService;
        }

        private UserAddressInfo ModelToInfo(IUserAddressModel md)
        {
            var userAddr = new UserAddressInfo { 
                Id = md.Id,
                ChainId = (int)md.Chain,
                CreateAt = md.CreateAt,
                UpdateAt = md.UpdateAt,
                Address = md.Address
            };
            return userAddr;
        }

        [Authorize]
        [HttpGet("listaddressbyuser")]
        public ActionResult<UserAddressListResult> ListAddressByUser()
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }

                var userAddrs = _userService.ListAddressByUser(userSession.Id);
                if (userAddrs == null)
                {
                    return new UserAddressListResult() { UserAddresses = null, Sucesso = false, Mensagem = "User Addresses Not Found" };
                }
                return new UserAddressListResult()
                {
                    UserAddresses = userAddrs.Select(x => ModelToInfo(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("getaddressbychain/{chainId}")]
        public ActionResult<UserAddressResult> ListAddressByUser(int chainId)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }

                var userAddr = _userService.GetAddressByChain(userSession.Id, (ChainEnum)chainId);
                if (userAddr == null)
                {
                    return new UserAddressResult() { UserAddress = null, Sucesso = false, Mensagem = "User Addresses Not Found" };
                }
                return new UserAddressResult()
                {
                    UserAddress = ModelToInfo(userAddr)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("addorchangeaddress")]
        public ActionResult<StatusResult> AddOrChangeAddress([FromBody] UserAddressParam param)
        {
            try
            {
                _userService.AddOrChangeAddress(param.UserId, (ChainEnum)param.ChainId, param.Address);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Address add successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("removeaddress/{chainId}")]
        public ActionResult<StatusResult> RemoveAddress(int chainId)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }

                _userService.RemoveAddress(userSession.Id, (ChainEnum)chainId);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Address remove successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
