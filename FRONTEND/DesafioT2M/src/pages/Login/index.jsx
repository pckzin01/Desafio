import React, { useState, useContext } from "react";
import { useForm } from "react-hook-form";
import { FaEye, FaEyeSlash } from "react-icons/fa";
import { AuthContext } from "../../context/AuthContext";
import * as styles from "../Login/Login.module.css";
import Clinica from "../../assets/clinica.jpg";
import axios from "axios";
import { jwtDecode } from "jwt-decode";
import { Navigate, useNavigate } from "react-router-dom";

export default function Login() {
  const { login } = useContext(AuthContext);
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();
  const [showPassword, setShowPassword] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const onSubmit = async (data) => {
    try {
      const response = await axios.post(
        "http://localhost:5026/api/auth/login",
        data
      );

      if (response.data && response.data.token) {
        login(response.data.token);
        setErrorMessage("");
        alert("Login bem-sucedido!");
        
        const decodedToken = jwtDecode(response.data.token);

        const role = decodedToken.role;

        if (role === "Paciente") {
          navigate("/home");
        } else if (role === "Medico") {
          navigate("/home-medico");
        } else if (role === "Administrador") {
          navigate("/admin");
        } else {
          alert("Role não reconhecida.");
        }
      } else {
        throw new Error("Resposta inválida do servidor.");
      }
    } catch (error) {
      setErrorMessage("Falha ao fazer login. Verifique suas credenciais.");
      console.error("Erro ao fazer login:", error);
    }
  };

  return (
    <main>
      <div className={styles.container}>
        <div className={styles.box}>
          <img src={Clinica} alt="foto de atendimento médico" />
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
                {errors.email && (
                  <span className={styles.error}>{errors.email.message}</span>
                )}
              </div>

              <div className={styles.inputGroup}>
                <div className={styles.passwordWrapper}>
                  <input
                    type={showPassword ? "text" : "password"}
                    id="password"
                    className={styles.input}
                    placeholder="Senha"
                    {...register("senha", {
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
                {errors.senha && (
                  <span className={styles.error}>{errors.senha.message}</span>
                )}
              </div>

              {errorMessage && (
                <p className={styles.errorMessage}>{errorMessage}</p>
              )}

              <button type="submit" className={styles.button}>
                Entrar
              </button>
            </form>
            <p className={styles.register}>
              Ainda não possui uma conta? <a href="/cadastrar">Cadastre-se</a>
            </p>
          </div>
        </div>
      </div>
    </main>
  );
}
