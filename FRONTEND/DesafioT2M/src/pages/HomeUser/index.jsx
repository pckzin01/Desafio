import React, { useEffect, useState } from "react";
import * as styles from "../HomeUser/HomeUser.module.css";
import Header from "../../components/Header";
import axios from "axios";

export default function HomeUser() {
  const [medicos, setMedicos] = useState([]); 
  const [usuarios, setUsuarios] = useState({});

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

  return (
    <main>
      <Header children={<h1>Agende uma consulta!</h1>} />
      <div className={styles.container}>
        <div className={styles.box}>
          {medicos.map((medico) => (
            <div key={medico.id} className={styles.medicoCard}>
              <h3>Especialidade: {medico.especialidade}</h3>
              <p>Nome: {usuarios[medico.usuarioId] || "Carregando..."}</p>
            </div>
          ))}
        </div>
      </div>
    </main>
  );
}
