using System;
using System.Threading.Tasks;
using ConstellationMind.Core.Domain;
using ConstellationMind.Core.Repositories;
using ConstellationMind.Infrastructure.Services.Authentication.Interfaces;
using ConstellationMind.Infrastructure.Services.Services;
using ConstellationMind.Infrastructure.Services.Services.Domains.Interfaces;
using ConstellationMind.Infrastructure.Services.Services.Interfaces;
using Moq;
using NUnit.Framework;

namespace ConstellationMind.UnitTests.Services
{
    [TestFixture]
    public class AccountServiceTests
    {
        private Mock<IPlayerRepository> _playerRepositoryMock;
        private Mock<IScoreboardRepository> _scoreboardRepositoryMock;
        private Mock<IPasswordService> _passwordServiceMock;
        private Mock<IJwtProvider> _jwtProviderMock;
        private Mock<IRefreshTokenService> _refreshTokenServiceMock;

        [SetUp]
        public void BeforeEach()
        {
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _scoreboardRepositoryMock = new Mock<IScoreboardRepository>();
            _passwordServiceMock = new Mock<IPasswordService>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        }

        [Test]
        public async Task SignUpAsync_ShouldCreateANewAccount_ShouldInvokeAddAsyncOnPlayerAndScoreboardRepository()
        {
            // ------------==== Arrange ====------------

            var accountService = new AccountService(_playerRepositoryMock.Object,
                                                    _scoreboardRepositoryMock.Object,
                                                    _passwordServiceMock.Object,
                                                    _jwtProviderMock.Object,
                                                    _refreshTokenServiceMock.Object);                                      

            // ------------====   Act   ====------------
            
            await accountService.SignUpAsync(Guid.NewGuid(), "player@email.com", "Secret123!", "Player", "Player", "Player");

            // ------------====  Assert ====------------
            _playerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Player>()), Times.Once);
            _scoreboardRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PlayerScore>()), Times.Once);
        }
    }
}
