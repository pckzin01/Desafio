using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GerenciamentoConsultas.Application.Validators
{
    public static class CrmValidator
    {
        // Lista de estados brasileiros válidos
        private static readonly HashSet<string> ValidStates = new HashSet<string>
        {
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG",
            "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        };

        /// <summary>
        /// Valida o formato e a sigla do estado do CRM.
        /// </summary>
        /// <param name="crm">O CRM a ser validado.</param>
        /// <returns>True se o CRM for válido; False caso contrário.</returns>
        public static bool IsValidCRM(string crm)
        {
            if (string.IsNullOrWhiteSpace(crm))
                return false;

            // Regex para verificar o formato: 4-6 dígitos, "/", seguido de 2 letras maiúsculas
            var regex = new Regex(@"^\d{4,6}\/[A-Z]{2}$");

            // Verificar se o formato geral do CRM está correto
            if (!regex.IsMatch(crm))
                return false;

            // Separar a sigla do estado para validação
            var state = crm.Split('/')[1];

            // Verificar se a sigla é um estado válido
            return ValidStates.Contains(state);
        }
    }
}
