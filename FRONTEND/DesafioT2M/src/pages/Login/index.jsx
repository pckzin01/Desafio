import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { FaEye, FaEyeSlash } from "react-icons/fa"; 
import * as styles from "../Login/Login.module.css";
import Clinica from "../../assets/clinica.jpg";

export default function Login() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();
  const [showPassword, setShowPassword] = useState(false); 

  const onSubmit = (data) => {
    console.log("Dados enviados:", data);
  };

  return (
    <main>
      <div className={styles.container}>
        <div className={styles.box}>
          <img src={Clinica} alt="foto de atendimento medico" />
        </div>
        <div className={styles.box2}>
          <h1 className={styles.title}>Olá, Seja Bem-Vindo!</h1>
          <div className={styles.login}>
            <h3 className={styles.text}>Faça login para continuar</h3>

            <form onSubmit={handleSubmit(onSubmit)} className={styles.form}>
              <div className={styles.inputGroup}>
                <input
                  type="email"
                  id="email"
                  className={styles.input}
                  placeholder="Email"
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
                <div className={styles.passwordWrapper}>
                  <input
                    type={showPassword ? "text" : "password"}
                    id="password"
                    className={styles.input}
                    placeholder="Senha"
                    {...register("password", {
                      required: "A senha é obrigatória.",
                      minLength: {
                        value: 6,
                        message: "A senha deve ter no mínimo 6 caracteres.",
                      },
                    })}
                  />
                  <span
                    className={styles.eyeIcon}
                    onClick={() => setShowPassword((prev) => !prev)}
                  >
                    {showPassword ? <FaEyeSlash /> : <FaEye />}
                  </span>
                </div>
                {errors.password && <span className={styles.error}>{errors.password.message}</span>}
              </div>

              <button type="submit" className={styles.button}>
                Entrar
              </button>
            </form>
            <p className={styles.register}>
              Ainda não possui uma conta? <a href="#">Cadastre-se</a>
            </p>
          </div>
        </div>
      </div>
    </main>
  );
}
