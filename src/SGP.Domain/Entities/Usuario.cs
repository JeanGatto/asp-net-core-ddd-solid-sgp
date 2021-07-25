using SGP.Domain.ValueObjects;
using SGP.Shared.Entities;
using SGP.Shared.Interfaces;
using System;
using System.Collections.Generic;

namespace SGP.Domain.Entities
{
    public class Usuario : BaseEntity, IAggregateRoot
    {
        private readonly List<TokenAcesso> _tokens = new();

        public Usuario(string nome, Email email, string hashSenha)
        {
            Nome = nome;
            Email = email;
            HashSenha = hashSenha;
        }

        private Usuario() // ORM
        {
        }

        public string Nome { get; private set; }
        public Email Email { get; private set; }
        public string HashSenha { get; private set; }
        public DateTime? UltimoAcessoEm { get; private set; }
        public DateTime? BloqueioExpiraEm { get; private set; }
        public int NumeroFalhasAoAcessar { get; private set; }

        public IReadOnlyList<TokenAcesso> Tokens => _tokens.AsReadOnly();

        public void AdicionarToken(TokenAcesso tokenAcesso) => _tokens.Add(tokenAcesso);

        /// <summary>
        /// Indica se a conta do usuário está bloqueada.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>Verdadeiro se a conta estiver bloqueada; caso contrário, falso.</returns>
        public bool EstaBloqueado(IDateTime dateTime) => BloqueioExpiraEm > dateTime.Now;

        public void AtualizarNome(string nome) => Nome = nome;

        public void AtualizarEmail(Email email) => Email = email;

        public void DefinirHashSenha(string hashSenha) => HashSenha = hashSenha;

        public void DefinirUltimoAcesso(IDateTime dateTime) => UltimoAcessoEm = dateTime.Now;

        /// <summary>
        /// Incremenenta o número de acessos que falharam.
        /// Quando é atingido o limite de acessos a conta será bloqueada por um tempo.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="numeroTentativas">Número máximo de tentativas até a conta ser bloqueada.</param>
        /// <param name="lockedTimeSpan">Determinado tempo em que a conta ficará bloqueada.</param>
        public void IncrementarFalhas(IDateTime dateTime, short numeroTentativas, TimeSpan lockedTimeSpan)
        {
            if (EstaBloqueado(dateTime))
            {
                return;
            }

            NumeroFalhasAoAcessar++;

            if (NumeroFalhasAoAcessar == numeroTentativas)
            {
                NumeroFalhasAoAcessar = 0;
                BloqueioExpiraEm = dateTime.Now.Add(lockedTimeSpan);
            }
        }
    }
}
