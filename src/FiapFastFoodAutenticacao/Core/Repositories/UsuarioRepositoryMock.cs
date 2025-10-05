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
                Id = "1",
                Nome = "Cliente Teste",
                Cpf = "12345678901"
            };
            return Task.FromResult<Usuario?>(usuario);
        }

        return Task.FromResult<Usuario?>(null);
    }
}

