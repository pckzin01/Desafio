import React, { useEffect, useState } from 'react';
import Header from '../../components/Header/index';
import * as styles from "../Admin/Admin.module.css";

export default function Admin() {
  const [usuarios, setUsuarios] = useState([]);
  const [pacientes, setPacientes] = useState([]);
  const [medicos, setMedicos] = useState([]);

  useEffect(() => {
    const fetchUsuarios = async () => {
      try {
        const response = await fetch('http://localhost:5026/api/usuario');
        const data = await response.json();
        setUsuarios(data);
      } catch (error) {
        console.error("Erro ao buscar usuários:", error);
      }
    };

    fetchUsuarios();
  }, []);

  useEffect(() => {
    const fetchPacientes = async () => {
      try {
        const response = await fetch('http://localhost:5026/api/paciente');
        const data = await response.json();
        setPacientes(data);
      } catch (error) {
        console.error("Erro ao buscar pacientes:", error);
      }
    };

    if (usuarios.length > 0) {
      fetchPacientes();
    }
  }, [usuarios]);

  useEffect(() => {
    const fetchMedicos = async () => {
      try {
        const response = await fetch('http://localhost:5026/api/medico');
        const data = await response.json();
        setMedicos(data);
      } catch (error) {
        console.error("Erro ao buscar médicos:", error);
      }
    };

    if (usuarios.length > 0) {
      fetchMedicos();
    }
  }, [usuarios]);

  const getPacienteDetails = (usuarioId) => {
    const paciente = pacientes.find(p => p.usuarioId === usuarioId);
    return paciente ? paciente : null;
  };

  const getMedicoDetails = (usuarioId) => {
    const medico = medicos.find(m => m.usuarioId === usuarioId);
    return medico ? medico : null;
  };

  return (
    <main>
      <Header children={<h1>Pessoas Cadastradas</h1>} />
      <div className={styles.container}>
        <div className={styles.box}>
          <h1>Médicos</h1>
            {usuarios.filter(usuario => usuario.tipoUsuario === 2).map(usuario => {
              const medico = getMedicoDetails(usuario.id);
              return (
                <div key={usuario.id} className={styles.card}>
                  <h3>{usuario.nome}</h3>
                  <p><strong>CRM:</strong> {medico ? medico.crm : 'Não disponível'}</p>
                  <p><strong>Especialidade:</strong> {medico ? medico.especialidade : 'Não disponível'}</p>
                </div>
              );
            })}
        </div>

        <div className={styles.box}>
          <h1>Pacientes</h1>
            {usuarios.filter(usuario => usuario.tipoUsuario === 3).map(usuario => {
              const paciente = getPacienteDetails(usuario.id);
              return (
                <div key={usuario.id} className={styles.card}>
                  <h3>{usuario.nome}</h3>
                  <p><strong>Telefone:</strong> {paciente ? paciente.telefone : 'Não disponível'}</p>
                  <p><strong>Data de Nascimento:</strong> {paciente ? new Date(paciente.dataNascimento).toLocaleDateString() : 'Não disponível'}</p>
                  <p><strong>Endereço:</strong> {paciente ? paciente.endereco : 'Não disponível'}</p>
                </div>
              );
            })}
        </div>
      </div>
    </main>
  );
}
