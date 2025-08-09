using Xunit;
using FluentAssertions;
using FluentValidation;
using app.Models.Register;
using app.Validators;
using app.Services;
using app.Models; // para AppDbContext
using Microsoft.EntityFrameworkCore;

namespace Auth.Tests
{
    public class UserRegistrationTest
    {
        // Cria um contexto em memória para cada teste
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        // Instancia o validator
        private IValidator<UserRegistrationRequest> GetValidator()
        {
            return new UserRegistrationRequestValidator();
        }

        [Fact]
        public async Task Register_WithValidData_ShouldSucceed()
        {
            var context = GetInMemoryDbContext();
            var validator = GetValidator();
            var service = new UserService(context, validator);
            var request = new UserRegistrationRequest
            {
                Name = "Higor Freire",
                Email = "higor@email.com",
                Password = "MinhaSenhaForte123"
            };

            var result = await service.RegisterUserAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData("Hi")] // muito curto
        [InlineData("Haaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // muito longo
        public async Task Register_WithInvalidNameLength_ShouldFail(string name)
        {
            var context = GetInMemoryDbContext();
            var validator = GetValidator();
            var service = new UserService(context, validator);
            var request = new UserRegistrationRequest
            {
                Name = name,
                Email = "valid@email.com",
                Password = "MinhaSenhaForte123"
            };

            var result = await service.RegisterUserAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Nome");
        }

        [Theory]
        [InlineData("a@b.c")] // muito curto
        [InlineData("hbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb@email.com")] // muito longo
        [InlineData("emailinvalido")] // formato inválido
        public async Task Register_WithInvalidEmail_ShouldFail(string email)
        {
            var context = GetInMemoryDbContext();
            var validator = GetValidator();
            var service = new UserService(context, validator);
            var request = new UserRegistrationRequest
            {
                Name = "Nome Válido",
                Email = email,
                Password = "MinhaSenhaForte123"
            };

            var result = await service.RegisterUserAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Email");
        }

        [Theory]
        [InlineData("1234567")] // muito curta
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // muito longa
        public async Task Register_WithInvalidPasswordLength_ShouldFail(string password)
        {
            var context = GetInMemoryDbContext();
            var validator = GetValidator();
            var service = new UserService(context, validator);
            var request = new UserRegistrationRequest
            {
                Name = "Nome Válido",
                Email = "valid@email.com",
                Password = password
            };

            var result = await service.RegisterUserAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Senha");
        }

        [Fact]
        public async Task Register_WithMissingFields_ShouldFail()
        {
            var context = GetInMemoryDbContext();
            var validator = GetValidator();
            var service = new UserService(context, validator);
            var request = new UserRegistrationRequest
            {
                Name = "",
                Email = "",
                Password = ""
            };

            var result = await service.RegisterUserAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Contain("obrigatório");
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ShouldFail()
        {
            var context = GetInMemoryDbContext();
            var validator = GetValidator();
            var service = new UserService(context, validator);

            var request1 = new UserRegistrationRequest
            {
                Name = "Primeiro Usuário",
                Email = "duplicado@email.com",
                Password = "SenhaForte123"
            };
            
            var request2 = new UserRegistrationRequest
            {
                Name = "Segundo Usuário",
                Email = "duplicado@email.com",
                Password = "SenhaForte456"
            };

            // Registra o primeiro usuário (deve passar)
            var result1 = await service.RegisterUserAsync(request1);
            result1.IsSuccess.Should().BeTrue();

            // Tenta registrar o segundo usuário (deve falhar)
            var result2 = await service.RegisterUserAsync(request2);
            result2.IsSuccess.Should().BeFalse();
            result2.ErrorMessage.Should().Contain("email já está em uso");
        }
    }
}