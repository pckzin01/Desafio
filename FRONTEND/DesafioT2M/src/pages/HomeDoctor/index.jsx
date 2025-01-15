import React, { useContext, useState, useEffect } from "react";
import axios from "axios";
import * as styles from "../HomeDoctor/HomeDoctor.module.css";
import Header from "../../components/Header";
import { AuthContext } from "../../context/AuthContext";

export default function HomeDoctor() {
  const { user } = useContext(AuthContext);
  const [consultasAgendadas, setConsultasAgendadas] = useState([]);
  const [consultasRealizadas, setConsultasRealizadas] = useState([]);
  const [consultasCanceladas, setConsultasCanceladas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [mensagem, setMensagem] = useState(null);

  useEffect(() => {
    const fetchMedicoEConsultas = async () => {
      if (!user || !user.nameid) {
        setMensagem("Erro ao identificar o usuário.");
        setLoading(false);
        return;
      }

      try {
        const medicoResponse = await axios.get(
          "http://localhost:5026/api/medico"
        );
        const medicos = medicoResponse.data;
        const medico = medicos.find(
          (med) => med.usuarioId === parseInt(user.nameid, 10)
        );

        if (!medico) {
          setMensagem("Médico não encontrado.");
          setLoading(false);
          return;
        }

        const consultaResponse = await axios.get(
          "http://localhost:5026/api/consulta"
        );
        const todasConsultas = consultaResponse.data;

        const consultasMedico = todasConsultas.filter(
          (consulta) => consulta.medicoId === medico.id
        );

        const consultasComPacientes = await Promise.all(
          consultasMedico.map(async (consulta) => {
            try {
              const pacienteResponse = await axios.get(
                `http://localhost:5026/api/paciente/${consulta.pacienteId}`
              );
              const paciente = pacienteResponse.data;

              const usuarioResponse = await axios.get(
                `http://localhost:5026/api/usuario/${paciente.usuarioId}`
              );
              const usuario = usuarioResponse.data;

              return { ...consulta, pacienteNome: usuario.nome };
            } catch (error) {
              return { ...consulta, pacienteNome: "Nome não encontrado" };
            }
          })
        );

        setConsultasAgendadas(
          consultasComPacientes.filter((consulta) => consulta.status === 1)
        );
        setConsultasRealizadas(
          consultasComPacientes.filter((consulta) => consulta.status === 2)
        );
        setConsultasCanceladas(
          consultasComPacientes.filter((consulta) => consulta.status === 3)
        );
      } catch (error) {
        setMensagem("Erro ao carregar dados. Tente novamente.");
      } finally {
        setLoading(false);
      }
    };

    fetchMedicoEConsultas();
  }, [user]);

  const cancelarConsulta = async (consultaId) => {
    try {
      console.log(`Cancelando consulta com ID: ${consultaId}`);

      const consulta = consultasAgendadas.find((c) => c.id === consultaId);

      if (!consulta) {
        console.error("Consulta não encontrada na lista.");
        return;
      }

      await axios.put(`http://localhost:5026/api/consulta/${consultaId}`, {
        pacienteId: consulta.pacienteId,
        medicoId: consulta.medicoId,
        dataHora: consulta.dataHora,
        status: 3,
        observacoes: consulta.observacoes || "Cancelada pelo médico.",
      });

      setConsultasAgendadas((prev) => prev.filter((c) => c.id !== consultaId));
      setConsultasCanceladas((prev) => [
        ...prev,
        { ...consulta, status: 3 },
      ]);

      console.log("Consulta cancelada com sucesso.");
    } catch (error) {
      console.error("Erro ao cancelar consulta:", error);
    }
  };

  const marcarComoRealizada = async (consultaId) => {
    try {
      console.log(`Marcando consulta como realizada, ID: ${consultaId}`);

      const consulta = consultasAgendadas.find((c) => c.id === consultaId);

      if (!consulta) {
        console.error("Consulta não encontrada na lista.");
        return;
      }

      await axios.put(`http://localhost:5026/api/consulta/${consultaId}`, {
        pacienteId: consulta.pacienteId,
        medicoId: consulta.medicoId,
        dataHora: consulta.dataHora,
        status: 2,
        observacoes: consulta.observacoes || "Consulta realizada com sucesso.",
      });

      setConsultasAgendadas((prev) => prev.filter((c) => c.id !== consultaId));
      setConsultasRealizadas((prev) => [
        ...prev,
        { ...consulta, status: 2 },
      ]);

      console.log("Consulta marcada como realizada com sucesso.");
    } catch (error) {
      console.error("Erro ao marcar consulta como realizada:", error);
    }
  };

  const deletarConsulta = async (consultaId, tipo) => {
    try {
      console.log(`Deletando consulta com ID: ${consultaId}`);
  
      await axios.delete(`http://localhost:5026/api/consulta/${consultaId}`);
  
      if (tipo === "realizada") {
        setConsultasRealizadas((prev) => prev.filter((c) => c.id !== consultaId));
      } else if (tipo === "cancelada") {
        setConsultasCanceladas((prev) => prev.filter((c) => c.id !== consultaId));
      } else {
        console.error(`Tipo de consulta inválido: ${tipo}`);
        return;
      }
  
      console.log("Consulta deletada com sucesso.");
      alert("Consulta deletada com sucesso!");
    } catch (error) {
      console.error("Erro ao deletar consulta:", error.response || error.message);
      alert("Erro ao deletar consulta. Tente novamente.");
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
      <Header children={<h1>Gerenciar Consultas</h1>} />
      <div className={styles.container}>
        <div className={styles.content}>
          <h3>Agendadas</h3>
          <div className={styles.box}>
            {consultasAgendadas.length > 0 ? (
              consultasAgendadas.map((consulta) => (
                <div key={consulta.id} className={styles.consultaCardAgen}>
                  <p>
                    Data: {new Date(consulta.dataHora).toLocaleDateString()}
                  </p>
                  <p>
                    Hora:{" "}
                    {new Date(consulta.dataHora).toLocaleTimeString([], {
                      hour: "2-digit",
                      minute: "2-digit",
                    })}
                  </p>
                  <p>Paciente: {consulta.pacienteNome}</p>
                  <p>
                    Observações: {consulta.observacoes || "Nenhuma observação."}
                  </p>
                  <div className={styles.positionButton}>
                    <button
                      onClick={() => cancelarConsulta(consulta.id)}
                      className={styles.buttonRed}
                    >
                      Cancelar Consulta
                    </button>

                    <button
                      onClick={() => marcarComoRealizada(consulta.id)}
                      className={styles.buttonGreen}
                    >
                      Marcar como Realizada
                    </button>
                  </div>
                </div>
              ))
            ) : (
              <p>Nenhuma consulta agendada.</p>
            )}
          </div>

          <h3>Histórico</h3>
          <div className={styles.historicoContainer}>
            <div className={styles.column}>
              <h4 style={{ color: "green", marginTop: "20px" }}>Realizadas</h4>
              <div className={styles.historicoColunaRealizada}>
                {consultasRealizadas.length > 0 ? (
                  consultasRealizadas.map((consulta) => (
                    <div key={consulta.id} className={styles.consultaCard}>
                      <p>
                        Data: {new Date(consulta.dataHora).toLocaleDateString()}
                      </p>
                      <p>
                        Hora:{" "}
                        {new Date(consulta.dataHora).toLocaleTimeString([], {
                          hour: "2-digit",
                          minute: "2-digit",
                        })}
                      </p>
                      <p>Paciente: {consulta.pacienteNome}</p>
                      <p>
                        Observações:{" "}
                        {consulta.observacoes || "Nenhuma observação."}
                      </p>
                      <button
                        onClick={() =>
                          deletarConsulta(consulta.id, "realizada")
                        }
                        className={styles.buttonRed}
                      >
                        Deletar
                      </button>
                    </div>
                  ))
                ) : (
                  <p>Nenhuma consulta realizada.</p>
                )}
              </div>
            </div>

            <div className={styles.column}>
              <h4 style={{ color: "red", marginTop: "20px" }}>Canceladas</h4>
              <div className={styles.historicoColunaCancelada}>
                {consultasCanceladas.length > 0 ? (
                  consultasCanceladas.map((consulta) => (
                    <div key={consulta.id} className={styles.consultaCard}>
                      <p>
                        Data: {new Date(consulta.dataHora).toLocaleDateString()}
                      </p>
                      <p>
                        Hora:{" "}
                        {new Date(consulta.dataHora).toLocaleTimeString([], {
                          hour: "2-digit",
                          minute: "2-digit",
                        })}
                      </p>
                      <p>Paciente: {consulta.pacienteNome}</p>
                      <p>
                        Observações:{" "}
                        {consulta.observacoes || "Nenhuma observação."}
                      </p>
                      <button
                        onClick={() =>
                          deletarConsulta(consulta.id, "realizada")
                        }
                        className={styles.buttonRed}
                      >
                        Deletar
                      </button>
                    </div>
                  ))
                ) : (
                  <p>Nenhuma consulta cancelada.</p>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  );
}
