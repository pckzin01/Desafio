import React, { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import * as styles from "../Register/Register.module.css";

export default function Register() {
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

  const onSubmit = (data) => {
    console.log("Dados enviados:", data);
  };

  return (
    <div className={styles.container}>
      <h1>Cadastre-se</h1>
      <div className={styles.box}>
        <form onSubmit={handleSubmit(onSubmit)} className={styles.form}>
          <div className={styles.inputGroup}>
            <input
              type="text"
              placeholder="Nome"
              className={styles.input}
              {...register("nome", { required: "O nome é obrigatório." })}
            />
            {errors.nome && <span className={styles.error}>{errors.nome.message}</span>}
          </div>

          <div className={styles.inputGroup}>
            <input
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
            {errors.email && <span className={styles.error}>{errors.email.message}</span>}
          </div>

          <div className={styles.inputGroup}>
            <input
              type="password"
              placeholder="Senha"
              className={styles.input}
              {...register("senha", {
                required: "A senha é obrigatória.",
                minLength: { value: 6, message: "A senha deve ter no mínimo 6 caracteres." },
              })}
            />
            {errors.senha && <span className={styles.error}>{errors.senha.message}</span>}
          </div>

          <div className={styles.inputGroup}>
            <input
              type="password"
              placeholder="Confirme a senha"
              className={styles.input}
              {...register("confirmarSenha", {
                required: "A confirmação de senha é obrigatória.",
                validate: (value) =>
                  value === watch("senha") || "As senhas não coincidem.",
              })}
            />
            {errors.confirmarSenha && <span className={styles.error}>{errors.confirmarSenha.message}</span>}
          </div>

          <div className={styles.inputGroup}>
            <select
              className={styles.input}
              {...register("tipo", { required: "Selecione o tipo de usuário." })}
            >
              <option value="">Selecione o tipo</option>
              <option value="medico">Médico</option>
              <option value="paciente">Paciente</option>
            </select>
            {errors.tipo && <span className={styles.error}>{errors.tipo.message}</span>}
          </div>

          {/* Renderiza os campos adicionais com base no tipo selecionado */}
          {userType === "medico" && (
            <>
              <div className={styles.inputGroup}>
                <input
                  type="text"
                  placeholder="Especialidade"
                  className={styles.input}
                  {...register("especialidade", { required: "A especialidade é obrigatória." })}
                />
                {errors.especialidade && (
                  <span className={styles.error}>{errors.especialidade.message}</span>
                )}
              </div>

              <div className={styles.inputGroup}>
                <input
                  type="text"
                  placeholder="CRM"
                  className={styles.input}
                  {...register("crm", { required: "O CRM é obrigatório." })}
                />
                {errors.crm && <span className={styles.error}>{errors.crm.message}</span>}
              </div>
            </>
          )}

          {userType === "paciente" && (
            <>
              <div className={styles.inputGroup}>
                <input
                  type="date"
                  placeholder="Data de Nascimento"
                  className={styles.input}
                  {...register("dataNascimento", {
                    required: "A data de nascimento é obrigatória.",
                  })}
                />
                {errors.dataNascimento && (
                  <span className={styles.error}>{errors.dataNascimento.message}</span>
                )}
              </div>

              <div className={styles.inputGroup}>
                <input
                  type="tel"
                  placeholder="Telefone"
                  className={styles.input}
                  {...register("telefone", { required: "O telefone é obrigatório." })}
                />
                {errors.telefone && <span className={styles.error}>{errors.telefone.message}</span>}
              </div>

              <div className={styles.inputGroup}>
                <input
                  type="text"
                  placeholder="Endereço"
                  className={styles.input}
                  {...register("endereco", { required: "O endereço é obrigatório." })}
                />
                {errors.endereco && <span className={styles.error}>{errors.endereco.message}</span>}
              </div>
            </>
          )}

          <button type="submit" className={styles.button}>Cadastrar</button>
        </form>
      </div>
    </div>
  );
}
