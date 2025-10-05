using FiapFastFoodAutenticacao.Core.Models;

namespace FiapFastFoodAutenticacao.Core.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorCpfAsync(string cpf);
}

