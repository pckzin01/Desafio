import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import * as styles from "../HomeUser/HomeUser.module.css";
import Header from "../../components/Header";
import axios from "axios";

export default function HomeUser() {
  const [medicos, setMedicos] = useState([]); 
  const [usuarios, setUsuarios] = useState({});
  const navigate = useNavigate();

  useEffect(() => {
    const fetchMedicos = async () => {
      try {
        const medicoResponse = await axios.get("http://localhost:5026/api/medico");
        const medicosData = medicoResponse.data;

        const usuarioIds = medicosData.map((medico) => medico.usuarioId);

        const usuarioResponse = await axios.get("http://localhost:5026/api/usuario");
        const usuariosData = usuarioResponse.data;

        const usuarioMap = usuariosData.reduce((acc, usuario) => {
          acc[usuario.id] = usuario.nome;
          return acc;
        }, {});

        setMedicos(medicosData);
        setUsuarios(usuarioMap);
      } catch (error) {
        console.error("Erro ao buscar médicos ou usuários:", error);
      }
    };

    fetchMedicos();
  }, []);

  const handleAgendarConsulta = (medicoId) => {
    navigate(`/agendar-consulta/${medicoId}`);
  };

  const handleMinhasConsultas = () => {
    navigate("/minhas-consultas");
  };

  return (
    <main>
      <div className={styles.headerContainer}>
        <Header children={<h1>Agende uma consulta!</h1>} />
        <button 
          className={styles.AppointmentButton} 
          onClick={handleMinhasConsultas}
        >
          <h3>Minhas consultas</h3>
        </button>
      </div>
      <div className={styles.container}>
        <div className={styles.box}>
          <h2 className={styles.title}>Selecione um Médico:</h2>
          {medicos.map((medico) => (
            <div key={medico.id} className={styles.medicoCard}>
              <h3>Especialidade: {medico.especialidade}</h3>
              <p>Nome: {usuarios[medico.usuarioId] || "Carregando..."}</p>
              <button 
                className={styles.agendarButton} 
                onClick={() => handleAgendarConsulta(medico.id)}
              >
                Agendar Consulta
              </button>
            </div>
          ))}
        </div>
      </div>
    </main>
  );
}
