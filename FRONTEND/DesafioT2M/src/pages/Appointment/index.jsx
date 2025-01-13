import React, { useContext, useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import { useForm } from 'react-hook-form';
import * as styles from '../Appointment/Appointment.module.css';
import { AuthContext } from '../../context/AuthContext';

export default function Appointment() {
  const { medicoId } = useParams(); // Obtém o ID do médico da URL
  const { user } = useContext(AuthContext); // Obtém os dados do usuário autenticado
  const { register, handleSubmit, formState: { errors } } = useForm(); // Hook do React Hook Form
  const [pacienteId, setPacienteId] = useState(null); // Armazena o ID do paciente
  const [mensagem, setMensagem] = useState(null);
  const [loading, setLoading] = useState(true); // Controla o estado de carregamento

  // Função para buscar o pacienteId com base no nameid do JWT
  useEffect(() => {
    console.log('User:', user);

    if (!user || !user.nameid) {
      setMensagem('Usuário não autenticado.');
      setLoading(false);
      return;
    }

    const fetchPacienteId = async () => {
      try {
        const response = await axios.get('http://localhost:5026/api/paciente');
        const paciente = response.data.find(p => p.usuarioId === parseInt(user.nameid, 10));

        if (paciente) {
          setPacienteId(paciente.id);
        } else {
          setMensagem('Paciente não encontrado para o usuário atual.');
        }
      } catch (error) {
        console.error('Erro ao buscar paciente:', error);
        setMensagem('Erro ao buscar paciente. Tente novamente.');
      } finally {
        setLoading(false);
      }
    };

    fetchPacienteId();
  }, [user]);

  const onSubmit = async (data) => {
    if (!pacienteId) {
      setMensagem('Não foi possível identificar o paciente.');
      return;
    }

    const consultaData = {
      pacienteId, // ID do paciente obtido no useEffect
      medicoId: parseInt(medicoId, 10), // ID do médico vindo da URL
      dataHora: data.dataHora, // Data e hora do formulário
      status: 0, // Status fixo
      observacoes: data.observacoes, // Observações do formulário
    };

    try {
      const response = await axios.post('http://localhost:5026/api/consulta', consultaData);
      setMensagem('Consulta agendada com sucesso!');
      console.log('Consulta agendada:', response.data);
    } catch (error) {
      console.error('Erro ao agendar consulta:', error);
      setMensagem('Erro ao agendar consulta. Tente novamente.');
    }
  };

  // Exibe um carregamento enquanto os dados não estão disponíveis
  if (loading) {
    return <p>Carregando...</p>;
  }

  // Se o usuário não estiver autenticado ou pacienteId não for encontrado
  if (!user || !user.nameid || !pacienteId) {
    return <p>{mensagem || 'Erro ao carregar informações do usuário.'}</p>;
  }

  return (
    <main>
      <div className={styles.container}>
        <h1>Agendamentos</h1>
        <form onSubmit={handleSubmit(onSubmit)} className={styles.form}>
          <label>
            <h3>Data e Hora:</h3>
            <input
              type="datetime-local"
              {...register('dataHora', { required: 'A data e hora são obrigatórias' })}
            />
            {errors.dataHora && <p className={styles.error}>{errors.dataHora.message}</p>}
          </label>
          <label>
            <h3>Observações:</h3>
            <textarea
              {...register('observacoes')}
              placeholder="Digite observações (opcional)"
            />
          </label>
          <button type="submit" className={styles.button}>
            Confirmar Agendamento
          </button>
        </form>
        {mensagem && <p className={styles.message}>{mensagem}</p>}
      </div>
    </main>
  );
}
