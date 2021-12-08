﻿using Login.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using EventBus.Core.Events;
using EventBus.Core.Interfaces;

namespace Login.Application.Extensions
{
    public static class EmailRequestExtensions
    {
        public static async Task Send(this EmailRequest email, IServiceProvider sp)
        {
            var bus = sp.GetRequiredService<IEventBus>();

            var emailRequested = new EmailRequestedEvent
            {
                DestinationEmail = email.DestinationEmail,
                Subject = email.Subject,
                Content = email.Content,
                Template = email.Template
            };
            await bus.PublishAsync(emailRequested);
        }
    }
}
