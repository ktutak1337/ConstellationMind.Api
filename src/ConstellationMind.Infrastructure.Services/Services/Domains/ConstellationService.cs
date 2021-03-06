using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ConstellationMind.Core.Domain;
using ConstellationMind.Core.Repositories;
using ConstellationMind.Infrastructure.Services.DTO;
using ConstellationMind.Infrastructure.Services.Extensions;
using ConstellationMind.Infrastructure.Services.Services.Domains.Interfaces;
using ConstellationMind.Shared.Exceptions;
using ConstellationMind.Shared.Extensions;

namespace ConstellationMind.Infrastructure.Services.Services.Domains
{
    public class ConstellationService : IConstellationService
    {
        private readonly IConstellationRepository _constellationRepository;
        private readonly IMapper _mapper;

        public ConstellationService(IConstellationRepository constellationRepository, IMapper mapper)
        {
            _constellationRepository = constellationRepository;
            _mapper = mapper;
        }

        public async Task<ConstellationDto> GetAsync(Guid identity)
            => (await _constellationRepository.GetAsync(identity))
                .MapSingle<Constellation, ConstellationDto>(_mapper);

        public async Task<IEnumerable<ConstellationDto>> GetConstellationsAsync()
            => (await _constellationRepository.GetAllAsync())
                .MapCollection<Constellation, ConstellationDto>(_mapper);

        public async Task CreateAsync(Guid identity, string designation, string name)
        {
            var constellation = await _constellationRepository.GetOrFailAsync(identity);

            constellation = new Constellation(identity, designation, name);

            await _constellationRepository.AddAsync(constellation);
        }

        public async Task AddStarAsync(Guid constellationId, string designation, string name, string constellation, EquatorialCoordinates coordinates, double magnitude)
        {
            var @const = await _constellationRepository.GetAsync(constellationId);

            if(@const == null) throw new ConstellationMindException(ErrorCodes.ConstellationNotFound, $"Constellation with id: '{constellationId}' was not found.");

            var star = new Star(designation, name, constellation, coordinates, magnitude);

            @const.AddStar(star);

            await _constellationRepository.UpdateAsync(@const);
        }

        public async Task DeleteAsync(Guid identity)
            => await _constellationRepository.RemoveAsync(identity);
    }
}
