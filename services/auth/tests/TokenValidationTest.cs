using Xunit;
using FluentAssertions;
using app.Models.TokenValidation;
using app.Helpers;
using app.Services;
using app.Models.Register;
using app.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Auth.Tests
{
    public class ValidationTokenTest
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private void InitJwtHelper()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "JWT:SecretKey", "minha-chave-jwt-super-secreta-123" },
                    { "JWT:Issuer", "TestIssuer" },
                    { "JWT:Audience", "TestAudience" }
                } as IEnumerable<KeyValuePair<string, string?>>)
                .Build();
            JwtHelper.Initialize(config);
        }

        private void SeedUser(AppDbContext context, string email, string password)
        {
            var user = new User
            {
                Name = "Usuário Teste",
                Email = email,
                PasswordHash = PasswordHasher.Hash(password)
            };
            context.Users.Add(user);
            context.SaveChanges();
        }

        private string GetValidToken(AppDbContext context)
        {
            var user = context.Users.First();
            var sessionId = Guid.NewGuid();
            return JwtHelper.GenerateJwtToken(user, sessionId);
        }

        [Fact]
        public void Access_WithValidToken_ShouldBeAuthorized()
        {
            InitJwtHelper();
            var context = GetInMemoryDbContext();
            SeedUser(context, "valid@email.com", "SenhaValida123");

            var token = GetValidToken(context);

            var service = new AuthService(context, null);
            var request = new TokenValidationRequest { JwtToken = token };

            var result = service.ValidateJwtToken(request);

            result.IsValid.Should().BeTrue();
            result.Claims.Should().NotBeNull();
            result.ErrorMessage.Should().BeNullOrEmpty();
        }

        // [Fact]
        // public void Access_WithExpiredToken_ShouldBeDenied()
        // {
        //     InitJwtHelper();
        //     var context = GetInMemoryDbContext();
        //     SeedUser(context, "expired@email.com", "SenhaValida123");

        //     var user = context.Users.First();
        //     var sessionId = Guid.NewGuid();
        //     var expiredToken = JwtHelper.GenerateJwtToken(user, sessionId, expireHours: 0);

        //     var service = new AuthService(context, null);
        //     var request = new TokenValidationRequest { JwtToken = expiredToken };

        //     var result = service.ValidateJwtToken(request);

        //     result.IsValid.Should().BeFalse();
        //     result.Claims.Should().BeNull();
        //     result.ErrorMessage.Should().Contain("Token inválido");
        // }

        [Fact]
        public void Access_WithInvalidToken_ShouldBeDenied()
        {
            InitJwtHelper();
            var context = GetInMemoryDbContext();

            var service = new AuthService(context, null);
            var request = new TokenValidationRequest { JwtToken = "token-invalido-123456" };

            var result = service.ValidateJwtToken(request);

            result.IsValid.Should().BeFalse();
            result.Claims.Should().BeNull();
            result.ErrorMessage.Should().Contain("Token inválido");
        }

        [Fact]
        public void Access_WithoutToken_ShouldBeDenied()
        {
            InitJwtHelper();
            var context = GetInMemoryDbContext();

            var service = new AuthService(context, null);
            var request = new TokenValidationRequest { JwtToken = "" };

            var result = service.ValidateJwtToken(request);

            result.IsValid.Should().BeFalse();
            result.Claims.Should().BeNull();
            result.ErrorMessage.Should().Contain("Token inválido");
        }
    }
}