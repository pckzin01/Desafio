import React from "react";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import HomeUser from "../pages/HomeUser";
import Register from "../pages/Register";
import Login from "../pages/Login";
import HomeDoctor from "../pages/HomeDoctor";
import Appointment from "../pages/Appointment";
import MyAppointment from "../pages/MyAppointment";
import Admin from "../pages/Admin";

function AppRoutes() {
  return (
      <Routes>
        <Route path="/home" element={<HomeUser />} />
        <Route path="/home-medico" element={<HomeDoctor />} />
        <Route path="/" element={<Login />} />
        <Route path="/cadastrar" element={<Register />} />
        <Route path="/agendar-consulta/:medicoId" element={<Appointment />} />
        <Route path="/minhas-consultas" element={<MyAppointment />} />
        <Route path="/admin" element={<Admin />} />
      </Routes>
  );
}

export default AppRoutes;
