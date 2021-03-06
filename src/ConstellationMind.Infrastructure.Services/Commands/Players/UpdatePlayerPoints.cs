using System;
using ConstellationMind.Shared.Types;

namespace ConstellationMind.Infrastructure.Services.Commands.Players
{
    public class UpdatePlayerPoints : ICommand
    {
        public Guid Id { get; set; }
        public int Points { get; set; }
    }
}
