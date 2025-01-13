using GerenciamentoConsultas.Application.DTOs;
using GerenciamentoConsultas.Domain.Entities;
using GerenciamentoConsultas.Domain.Interfaces;

namespace GerenciamentoConsultas.Application.Services
{
    public class UsuarioAppService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioAppService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UsuarioDTO> GetUsuarioByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario
            };
        }

        public async Task<IEnumerable<UsuarioDTO>> GetAllUsuariosAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return usuarios.Select(u => new UsuarioDTO
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                TipoUsuario = u.TipoUsuario
            });
        }

        public async Task<int> AddUsuarioAsync(UsuarioInserirDTO usuarioInserirDTO)
        {
            if (await _usuarioRepository.EmailExistsAsync(usuarioInserirDTO.Email))
                throw new Exception("O e-mail informado já está em uso.");

            if (usuarioInserirDTO.TipoUsuario == TipoUsuario.Medico && usuarioInserirDTO.TipoUsuario == TipoUsuario.Paciente)
                throw new Exception("Um usuário não pode ser médico e paciente ao mesmo tempo.");

            usuarioInserirDTO.HashSenha();

            var usuario = new Usuario
            {
                Nome = usuarioInserirDTO.Nome,
                Email = usuarioInserirDTO.Email,
                Senha = usuarioInserirDTO.Senha,
                TipoUsuario = usuarioInserirDTO.TipoUsuario,
                DataCadastro = usuarioInserirDTO.DataCadastro
            };

            // Adiciona o usuário e captura o ID gerado
            var usuarioId = await _usuarioRepository.AddAsync(usuario);

            // Retorna o ID do usuário criado
            return usuarioId;
        }


        public async Task UpdateUsuarioAsync(UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO.Id <= 0)
                throw new ArgumentException("ID inválido para atualização.");

            var usuario = await _usuarioRepository.GetByIdAsync(usuarioDTO.Id);

            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado para atualização.");

            usuario.Nome = usuarioDTO.Nome;
            usuario.Email = usuarioDTO.Email;
            usuario.TipoUsuario = usuarioDTO.TipoUsuario;

            await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task DeleteUsuarioAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado para exclusão.");

            await _usuarioRepository.DeleteAsync(id);
        }

        public async Task<UsuarioDTO> AuthenticateAsync(string email, string senha)
        {
            var usuario = (await _usuarioRepository.GetAllAsync())
                .FirstOrDefault(u => u.Email == email);

            if (usuario == null)
                return null;

            if (!VerificarSenha(senha, usuario.Senha))
                return null;

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario
            };
        }

        public static bool VerificarSenha(string senha, string senhaCriptografada)
        {
            return BCrypt.Net.BCrypt.Verify(senha, senhaCriptografada);
        }
    }
}
