import React, { useContext } from "react";
import * as styles from "./Header.module.css";
import { AuthContext } from "../../context/AuthContext";
import { FaSignOutAlt } from "react-icons/fa";

export default function Header({ children }) {
  const { logout } = useContext(AuthContext);

  const handleLogout = () => {
    logout();
    window.location.href = "/";
  };

  return (
    <header className={styles.header}>
      {children}
      <button onClick={handleLogout} className={styles.logoutButton}>
        <FaSignOutAlt size={35} color="#fff" />
      </button>
    </header>
  );
}
