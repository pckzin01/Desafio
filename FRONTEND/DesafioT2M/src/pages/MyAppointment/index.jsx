import React, { useContext, useState, useEffect } from "react";
import axios from "axios";
import * as styles from "../MyAppointment/MyAppointment.module.css";
import Header from "../../components/Header/index";
import { AuthContext } from "../../context/AuthContext";

export default function MyAppointment() {
  const { user } = useContext(AuthContext);
  const [consultas, setConsultas] = useState([]);
  const [mensagem, setMensagem] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchConsultas = async () => {
      if (!user || !user.nameid) {
        setMensagem("Erro ao identificar o usuário.");
        setLoading(false);
        return;
      }

      try {
        const pacienteResponse = await axios.get(
          "http://localhost:5026/api/paciente"
        );
        const pacientes = pacienteResponse.data;

        const paciente = pacientes.find(
          (p) => p.usuarioId === parseInt(user.nameid, 10)
        );

        if (paciente && paciente.id) {
          const consultaResponse = await axios.get(
            "http://localhost:5026/api/consulta"
          );
          const todasConsultas = consultaResponse.data;

          const consultasPaciente = todasConsultas.filter(
            (consulta) => consulta.pacienteId === paciente.id
          );

          const medicoResponse = await axios.get(
            "http://localhost:5026/api/medico"
          );
          const medicos = medicoResponse.data;

          const usuarioResponse = await axios.get(
            "http://localhost:5026/api/usuario"
          );
          const usuarios = usuarioResponse.data;

          const consultasComMedico = consultasPaciente.map((consulta) => {
            const medico = medicos.find((m) => m.id === consulta.medicoId);
            const usuarioMedico = medico
              ? usuarios.find((u) => u.id === medico.usuarioId)
              : null;

            return {
              ...consulta,
              medicoNome: usuarioMedico ? usuarioMedico.nome : "Desconhecido",
              medicoEspecialidade: medico
                ? medico.especialidade
                : "Desconhecida",
            };
          });

          setConsultas(consultasComMedico);
        } else {
          setMensagem("Paciente não encontrado.");
        }
      } catch (error) {
        console.error("Erro ao buscar dados:", error);
        setMensagem("Erro ao carregar dados. Tente novamente.");
      } finally {
        setLoading(false);
      }
    };

    fetchConsultas();
  }, [user]);

  const cancelarConsulta = async (consultaId) => {
    try {
      const consulta = consultas.find((c) => c.id === consultaId);

      if (!consulta) {
        alert("Consulta não encontrada.");
        return;
      }

      await axios.put(`http://localhost:5026/api/consulta/${consultaId}`, {
        id: consulta.id,
        pacienteId: consulta.pacienteId,
        medicoId: consulta.medicoId,
        dataHora: consulta.dataHora,
        status: 3,
        observacoes: consulta.observacoes,
      });

      setConsultas((prevConsultas) =>
        prevConsultas.map((consulta) =>
          consulta.id === consultaId ? { ...consulta, status: 3 } : consulta
        )
      );
      alert("Consulta cancelada com sucesso!");
    } catch (error) {
      console.error("Erro ao cancelar consulta:", error);
      alert("Erro ao cancelar consulta. Tente novamente.");
    }
  };

  if (loading) {
    return <p>Carregando...</p>;
  }

  if (mensagem) {
    return <p>{mensagem}</p>;
  }

  return (
    <main>
      <Header children={<h1>Minhas Consultas</h1>} />
      <div className={styles.container}>
        <div className={styles.card}>
          <h2>Consultas Agendadas</h2>
          {consultas.length > 0 ? (
            <ul>
              {consultas.map((consulta) => (
                <li key={consulta.id} className={styles.itemCard}>
                  <h3>
                    Data: {new Date(consulta.dataHora).toLocaleDateString()} -
                    Hora:{" "}
                    {new Date(consulta.dataHora).toLocaleTimeString([], {
                      hour: "2-digit",
                      minute: "2-digit",
                    })}
                  </h3>
                  <p>Médico: {consulta.medicoNome}</p>
                  <p>Especialidade: {consulta.medicoEspecialidade}</p>
                  <p className={`${consulta.status === 1 ? styles.ativa : consulta.status === 2 ? styles.realizada : styles.cancelada}`}>
                    Status:{" "}
                    {consulta.status === 1
                      ? "Ativa"
                      : consulta.status === 2
                      ? "Realizada"
                      : "Cancelada"}
                  </p>
                  <p>
                    Observações: {consulta.observacoes || "Nenhuma observação."}
                  </p>
                  {consulta.status === 1 && (
                    <button
                      className={styles.actionButton}
                      onClick={() => cancelarConsulta(consulta.id)}
                    >
                      Cancelar Consulta
                    </button>
                  )}
                </li>
              ))}
            </ul>
          ) : (
            <p>Nenhuma consulta encontrada.</p>
          )}
        </div>
      </div>
    </main>
  );
}
