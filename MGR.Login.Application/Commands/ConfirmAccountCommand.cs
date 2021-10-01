﻿using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MGR.Login.Application.Commands
{
    public class ConfirmAccountCommand : IRequest
    {
        [EmailAddress]
        [Required] public string Email { get; set; }

        [Required] public string Token { get; set; }
    }
}
