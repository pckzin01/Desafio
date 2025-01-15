import React, { useContext, useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import axios from 'axios';
import { useForm } from 'react-hook-form';
import * as styles from '../Appointment/Appointment.module.css';
import { AuthContext } from '../../context/AuthContext';

export default function Appointment() {
  const navigate = useNavigate();
  const { medicoId } = useParams();
  const { user } = useContext(AuthContext);
  const { register, handleSubmit, formState: { errors } } = useForm();
  const [pacienteId, setPacienteId] = useState(null);
  const [mensagem, setMensagem] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user || !user.nameid) {
      setMensagem('Usuário não autenticado.');
      setLoading(false);
      return;
    }
  
    const fetchPacienteId = async () => {
      try {
        const response = await axios.get('http://localhost:5026/api/paciente');
        console.log('Pacientes encontrados:', response.data);
        const paciente = response.data.find(p => p.usuarioId === parseInt(user.nameid, 10));
  
        if (paciente) {
          console.log('Paciente encontrado:', paciente);
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
      pacienteId,
      medicoId: parseInt(medicoId, 10),
      dataHora: data.dataHora,
      status: 1,
      observacoes: data.observacoes,
    };
  
    try {
      const response = await axios.post('http://localhost:5026/api/consulta', consultaData);
      setMensagem('Consulta agendada com sucesso!');  
      navigate('/minhas-consultas');
    } catch (error) {
      console.error('Erro ao agendar consulta:', error);
      setMensagem('Erro ao agendar consulta. Tente novamente.');
    }
  };
  

  if (loading) {
    return <p>Carregando...</p>;
  }

  if (!user || !user.nameid || !pacienteId) {
    return <p>{mensagem || 'Erro ao carregar informações do usuário.'}</p>;
  }

  return (
    <main>
      <div className={styles.container}>
        <h1 className={styles.title}>Agendamento</h1>
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
