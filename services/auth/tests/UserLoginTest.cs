using Xunit;
using FluentAssertions;
using FluentValidation;
using app.Models.Login;
using app.Models.Register;
using app.Validators;
using app.Services;
using app.Models;
using app.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Auth.Tests
{
    public class UserLoginTest
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private IValidator<UserLoginRequest> GetValidator()
        {
            return new UserLoginRequestValidator();
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

        [Fact]
        public async Task Login_WithValidData_ShouldSucceed()
        {
            InitJwtHelper();
            var context = GetInMemoryDbContext();
            var validator = GetValidator();
            var service = new AuthService(context, validator);

            var email = "teste@email.com";
            var password = "SenhaValida123";
            SeedUser(context, email, password);

            var request = new UserLoginRequest
            {
                Email = email,
                Password = password
            };

            var result = await service.LoginUserAsync(request);

            result.IsAuthenticated.Should().BeTrue();
            result.JwtToken.Should().NotBeNullOrEmpty();
            result.ErrorMessage.Should().BeNull();

            // Session deve ser gravada
            var userId = context.Users.FirstAsync().Result.Id;
            var session = await context.UserSessions.FirstOrDefaultAsync(s => s.UserId == userId);
            session.Should().NotBeNull();
            session.JwtToken.Should().Be(result.JwtToken);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ShouldFail()
        {
            InitJwtHelper();
            var context = GetInMemoryDbContext();
            var validator = GetValidator();
            var service = new AuthService(context, validator);

            var email = "teste@email.com";
            var password = "SenhaValida123";
            SeedUser(context, email, password);

            var request = new UserLoginRequest
            {
                Email = email,
                Password = "SenhaErrada"
            };

            var result = await service.LoginUserAsync(request);

            result.IsAuthenticated.Should().BeFalse();
            result.JwtToken.Should().BeNull();
            result.ErrorMessage.Should().Contain("Credenciais inválidas");
        }

        [Fact]
        public async Task Login_WithNonexistentUser_ShouldFail()
        {
            InitJwtHelper();
            var context = GetInMemoryDbContext();
            var validator = GetValidator();
            var service = new AuthService(context, validator);

            var request = new UserLoginRequest
            {
                Email = "naoexiste@email.com",
                Password = "SenhaQualquer123"
            };

            var result = await service.LoginUserAsync(request);

            result.IsAuthenticated.Should().BeFalse();
            result.JwtToken.Should().BeNull();
            result.ErrorMessage.Should().Contain("Credenciais inválidas");
        }

        [Theory]
        [InlineData("", "SenhaValida123")]
        [InlineData("teste@email.com", "")]
        [InlineData("", "")]
        public async Task Login_WithMissingFields_ShouldFail(string email, string password)
        {
            InitJwtHelper();
            var context = GetInMemoryDbContext();
            var validator = GetValidator();
            var service = new AuthService(context, validator);

            var request = new UserLoginRequest
            {
                Email = email,
                Password = password
            };

            var result = await service.LoginUserAsync(request);

            result.IsAuthenticated.Should().BeFalse();
            result.JwtToken.Should().BeNull();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("emailinvalido", "SenhaValida123")]
        [InlineData("a@b.c", "SenhaValida123")]
        public async Task Login_WithInvalidEmail_ShouldFail(string email, string password)
        {
            InitJwtHelper();
            var context = GetInMemoryDbContext();
            var validator = GetValidator();
            var service = new AuthService(context, validator);

            var request = new UserLoginRequest
            {
                Email = email,
                Password = password
            };

            var result = await service.LoginUserAsync(request);

            result.IsAuthenticated.Should().BeFalse();
            result.JwtToken.Should().BeNull();
            result.ErrorMessage.Should().Contain("Credenciais inválidas.");
        }

        [Theory]
        [InlineData("teste@email.com", "1234567")] // senha curta
        public async Task Login_WithInvalidPassword_ShouldFailValidation(string email, string password)
        {
            InitJwtHelper();
            var context = GetInMemoryDbContext();
            var validator = GetValidator();
            var service = new AuthService(context, validator);

            var request = new UserLoginRequest
            {
                Email = email,
                Password = password
            };

            var result = await service.LoginUserAsync(request);

            result.IsAuthenticated.Should().BeFalse();
            result.JwtToken.Should().BeNull();
            result.ErrorMessage.Should().Contain("Credenciais inválidas.");
        }
    }
}