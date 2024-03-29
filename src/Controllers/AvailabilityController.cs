﻿using System;
using System.Threading.Tasks;
using civica_service.Helpers.SessionProvider;
using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;

namespace civica_service.Controllers
{
    [Route("api/v1/[Controller]")]
    [Produces("application/json")]
    [ApiController]
    [TokenAuthentication]
    public class AvailabilityController : ControllerBase
    {
        private readonly ISessionProvider _sessionProvider;

        public AvailabilityController(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailability()
        {
            try
            {
                await _sessionProvider.GetSessionId();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(424);
            }
        }

        [HttpGet]
        [Route("get-anonymous-availability")]
        public async Task<IActionResult> GetAnonymousAvailability()
        {
            try
            {
                await _sessionProvider.GetAnonymousSessionId();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(424);
            }
        }
    }
}