using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConstellationMind.Core.Domain;
using ConstellationMind.Core.Repositories;
using ConstellationMind.Infrastructure.Services.Authentication;
using ConstellationMind.Infrastructure.Services.Authentication.Interfaces;
using ConstellationMind.Infrastructure.Services.Services.Domains.Interfaces;
using ConstellationMind.Shared.Exceptions;

namespace ConstellationMind.Infrastructure.Services.Services.Domains
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPlayerRepository _playerRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IJwtProvider jwtProvider, IPlayerRepository playerRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtProvider = jwtProvider;
            _playerRepository = playerRepository;
        }
        public async Task<string> CreateAsync(Guid playerId)
        {
            var token = GenerateRefreshToken();
            var refreshToken = new RefreshToken(playerId, token);
            await _refreshTokenRepository.AddAsync(refreshToken);
            return token;
        }

        public async Task<Jwt> RefreshAccessTokenAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetAsync(refreshToken);

            if(token == null)
                throw new ConstellationMindException(ErrorCodes.InvalidRefreshToken, $"Given refresh token is invalid.");

            if (token.IsInvalidate)
                throw new ConstellationMindException(ErrorCodes.InvalidatedRefreshToken, $"Refresh token with Id: '{token.Identity}' was already invalidated.");

            var player = await _playerRepository.GetAsync(token.PlayerId);
            
            if (player == null)
                throw new ConstellationMindException(ErrorCodes.PlayerNotFound, $"Player with ID: '{token.PlayerId}' was not found.");

            var accessToken = _jwtProvider.CreateToken(player.Identity.ToString("N"), player.Role);
            accessToken.RefreshToken = refreshToken;

            return accessToken;
        }

        public async Task InvalidateAsync(string refreshToken)
        {
            // TODO
            await Task.CompletedTask;
        }

        private string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
            
            using var randomNumberGenerator = RandomNumberGenerator.Create();			
            randomNumberGenerator.GetBytes(randomNumber);
            var result = Convert.ToBase64String(randomNumber);
            
            return Regex.Replace(result, @"[^0-9a-zA-Z]+", string.Empty);
		}
    }
}
