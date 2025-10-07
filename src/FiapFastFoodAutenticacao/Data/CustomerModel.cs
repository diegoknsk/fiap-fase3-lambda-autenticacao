using System;

namespace FiapFastFoodAutenticacao.Data
{
    public class CustomerModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Cpf { get; set; } // Pode ser null para usuários anônimos
        public int CustomerType { get; set; } = 1; // 1 = Registered, 2 = Anonymous
    }
}
