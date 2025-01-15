import React, { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import axios from "axios";
import * as styles from "../Register/Register.module.css";
import { Navigate, useNavigate } from "react-router-dom";

export default function Register() {

  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
  } = useForm();

  const [userType, setUserType] = useState("");

  const selectedType = watch("tipo");

  useEffect(() => {
    setUserType(selectedType);
  }, [selectedType]);

  const onSubmit = async (data) => {
    try {
      const usuarioResponse = await axios.post(
        "http://localhost:5026/api/usuario",
        {
          nome: data.nome,
          email: data.email,
          senha: data.senha,
          tipoUsuario: data.tipo === "medico" ? 2 : 3,
          dataCadastro: new Date().toISOString(),
        }
      );

      if (usuarioResponse.status !== 201) {
        throw new Error("Erro ao criar usuário.");
      }

      const usuarioId = usuarioResponse.data.id;

      if (data.tipo === "paciente") {
        const pacienteResponse = await axios.post(
          "http://localhost:5026/api/paciente",
          {
            dataNascimento: data.dataNascimento,
            telefone: data.telefone,
            endereco: data.endereco,
            usuarioId: usuarioId,
          }
        );

        if (pacienteResponse.status !== 201) {
          throw new Error("Erro ao criar paciente.");
        }
      }

      if (data.tipo === "medico") {
        try {
          const medicoResponse = await axios.post(
            "http://localhost:5026/api/medico",
            {
              especialidade: data.especialidade,
              crm: data.crm,
              usuarioId: usuarioId,
            }
          );
        } catch (medicoError) {
          console.error(
            "Erro ao criar médico:",
            medicoError.response?.data || medicoError
          );
          throw medicoError;
        }
      }

      console.log("Cadastro realizado com sucesso!");
      alert("Cadastro realizado com sucesso!");

      navigate("/");
    } catch (error) {
      console.error("Erro ao cadastrar:", error);

      if (error.response?.data) {
        const errors = error.response.data.errors
          ? Object.values(error.response.data.errors).flat().join(", ")
          : JSON.stringify(error.response.data);
        alert(`Erro ao cadastrar: ${errors}`);
      } else {
        alert(`Erro ao cadastrar: ${error.message || error}`);
      }

      // try {
      //   if (usuarioResponse?.data?.id) {
      //     await axios.delete(
      //       `http://localhost:5026/api/usuario/${usuarioResponse.data.id}`
      //     );
      //     console.log(
      //       `Usuário ${usuarioResponse.data.id} deletado devido a erro.`
      //     );
      //   }
      // } catch (deleteError) {
      //   console.error(
      //     "Erro ao deletar usuário após falha:",
      //     deleteError.response?.data || deleteError
      //   );
      // }
    }
  };

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Cadastre-se</h1>
      <div className={styles.box}>
        <form onSubmit={handleSubmit(onSubmit)} className={styles.form}>
          <div className={styles.inputGroup}>
            <label htmlFor="nome">Nome</label>
            <input
              id="nome"
              type="text"
              placeholder="Nome"
              className={styles.input}
              {...register("nome", { required: "O nome é obrigatório." })}
            />
            {errors.nome && (
              <span className={styles.error}>{errors.nome.message}</span>
            )}
          </div>

          <div className={styles.inputGroup}>
            <label htmlFor="email">Email</label>
            <input
              id="email"
              type="email"
              placeholder="Email"
              className={styles.input}
              {...register("email", {
                required: "O email é obrigatório.",
                pattern: {
                  value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                  message: "Formato de email inválido.",
                },
              })}
            />
            {errors.email && (
              <span className={styles.error}>{errors.email.message}</span>
            )}
          </div>

          <div className={styles.inputGroup}>
            <label htmlFor="senha">Senha</label>
            <input
              id="senha"
              type="password"
              placeholder="Senha"
              className={styles.input}
              {...register("senha", {
                required: "A senha é obrigatória.",
                minLength: {
                  value: 6,
                  message: "A senha deve ter no mínimo 6 caracteres.",
                },
              })}
            />
            {errors.senha && (
              <span className={styles.error}>{errors.senha.message}</span>
            )}
          </div>

          <div className={styles.inputGroup}>
            <label htmlFor="confirmarSenha">Confirme a senha</label>
            <input
              id="confirmarSenha"
              type="password"
              placeholder="Confirme a senha"
              className={styles.input}
              {...register("confirmarSenha", {
                required: "A confirmação de senha é obrigatória.",
                validate: (value) =>
                  value === watch("senha") || "As senhas não coincidem.",
              })}
            />
            {errors.confirmarSenha && (
              <span className={styles.error}>
                {errors.confirmarSenha.message}
              </span>
            )}
          </div>

          <div className={styles.inputGroup}>
            <label htmlFor="tipo">Tipo de usuário</label>
            <select
              id="tipo"
              className={styles.input}
              {...register("tipo", {
                required: "Selecione o tipo de usuário.",
              })}
            >
              <option value="">Selecione o tipo</option>
              <option value="medico">Médico</option>
              <option value="paciente">Paciente</option>
            </select>
            {errors.tipo && (
              <span className={styles.error}>{errors.tipo.message}</span>
            )}
          </div>

          {userType === "medico" && (
            <>
              <div className={styles.inputGroup}>
                <label htmlFor="especialidade">Especialidade</label>
                <input
                  id="especialidade"
                  type="text"
                  placeholder="Especialidade"
                  className={styles.input}
                  {...register("especialidade", {
                    required: "A especialidade é obrigatória.",
                  })}
                />
                {errors.especialidade && (
                  <span className={styles.error}>
                    {errors.especialidade.message}
                  </span>
                )}
              </div>

              <div className={styles.inputGroup}>
                <label htmlFor="crm">CRM</label>
                <input
                  id="crm"
                  type="text"
                  placeholder="CRM"
                  className={styles.input}
                  {...register("crm", {
                    required: "O CRM é obrigatório.",
                    pattern: {
                      value: /^[0-9]{4,6}\/[A-Z]{2}$/,
                      message:
                        "O CRM deve estar no formato correto (Ex: 123456/SP).",
                    },
                  })}
                />
                {errors.crm && (
                  <span className={styles.error}>{errors.crm.message}</span>
                )}
              </div>
            </>
          )}

          {userType === "paciente" && (
            <>
              <div className={styles.inputGroup}>
                <label htmlFor="dataNascimento">Data de Nascimento</label>
                <input
                  id="dataNascimento"
                  type="date"
                  className={styles.input}
                  {...register("dataNascimento", {
                    required: "A data de nascimento é obrigatória.",
                  })}
                />
                {errors.dataNascimento && (
                  <span className={styles.error}>
                    {errors.dataNascimento.message}
                  </span>
                )}
              </div>

              <div className={styles.inputGroup}>
                <label htmlFor="telefone">Telefone</label>
                <input
                  id="telefone"
                  type="tel"
                  placeholder="Telefone"
                  className={styles.input}
                  {...register("telefone", {
                    required: "O telefone é obrigatório.",
                  })}
                />
                {errors.telefone && (
                  <span className={styles.error}>
                    {errors.telefone.message}
                  </span>
                )}
              </div>

              <div className={styles.inputGroup}>
                <label htmlFor="endereco">Endereço</label>
                <input
                  id="endereco"
                  type="text"
                  placeholder="Endereço"
                  className={styles.input}
                  {...register("endereco", {
                    required: "O endereço é obrigatório.",
                  })}
                />
                {errors.endereco && (
                  <span className={styles.error}>
                    {errors.endereco.message}
                  </span>
                )}
              </div>
            </>
          )}

          <button type="submit" className={styles.button}>
            Cadastrar
          </button>
        </form>
      </div>
    </div>
  );
}
