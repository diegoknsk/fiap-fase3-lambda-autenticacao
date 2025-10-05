using FiapFastFoodAutenticacao.Core.Models;

namespace FiapFastFoodAutenticacao.Core.Repositories;

public class UsuarioRepositoryMock : IUsuarioRepository
{
    public Task<Usuario?> ObterPorCpfAsync(string cpf)
    {
        // Mock: retorna usuário fixo para CPF específico
        if (cpf == "12345678901")
        {
            var usuario = new Usuario
            {
                Id = Guid.Parse("12345678-1234-1234-1234-123456789012"),
                Nome = "Cliente Teste",
                Email = "cliente@teste.com",
                Cpf = "12345678901",
                Senha = "1234"
            };
            return Task.FromResult<Usuario?>(usuario);
        }

        return Task.FromResult<Usuario?>(null);
    }
}

